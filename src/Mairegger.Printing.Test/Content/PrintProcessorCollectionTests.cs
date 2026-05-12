// Copyright 2017-2025 Michael Mairegger
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows.Controls;
using Mairegger.Printing.PrintProcessor;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintProcessorCollectionTests
    {
        // [Test]
        // public async Task Ctor()
        // {
        //     Mock<TestPrintProcessor>[] m1 = [TestPrintProcessor.Mock(), TestPrintProcessor.Mock(), TestPrintProcessor.Mock(), TestPrintProcessor.Mock()];
        //     PrintProcessorCollection pp = new PrintProcessorCollection(m1.Select(i => i.Object), "FileName");
        //     await Assert.That(pp).IsEquivalentTo(m1.Select(i => i.Object));
        //
        //     await Assert.That(pp.FileName).IsEqualTo("FileName");
        // }

        [Test]
        public async Task Ctor_SingleElement()
        {
            var p = TestPrintProcessor.Mock();
            PrintProcessorCollection pp = new PrintProcessorCollection(p.Object);

            using (Assert.Multiple())
            {
                await Assert.That(pp.FileName).IsEqualTo(p.Object.FileName);
                await Assert.That(pp).Contains(p.Object);
                await Assert.That(pp).HasSingleItem();
            }
        }

        [Test]
        public async Task FileName_Default_IsStringEmpty()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            await Assert.That(ppcoll.FileName).IsEmpty();
        }

        [Test]
        public async Task FileName_InvalidCharacters_GetsRemoved()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            var formattableString = $"Hello{Path.GetInvalidFileNameChars()[0]}Hello{Path.GetInvalidFileNameChars()[1]}";

            await Assert.That(ppcoll.FileName).IsEmpty();

            ppcoll.FileName = formattableString;

            char[] invalid = Path.GetInvalidFileNameChars();
            await Assert.That(ppcoll.FileName.ToCharArray()).DoesNotContain(e => invalid.Contains(e));
        }

        [Test]
        public void PreviewDocument()
        {
            var printProcessor = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            var windowProvider = IWindowProvider.Mock();

            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Show(Any<string>(), Any<DocumentViewer>()).WasNeverCalled();
            ((IMock)windowProvider).VerifyNoOtherCalls();
        }

        [Test, STAThreadExecutor]
        public async Task PrintEverything()
        {
            var printDialog = IPrintDialog.Mock();
            var printProcessor = new PrintEverything() { PrintDialog = printDialog.Object };

            var windowProvider = IWindowProvider.Mock();
            await Assert.That(printProcessor.PrintDocument()).IsTrue();
            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Show(Any<string>(), Any<DocumentViewer>()).WasCalled();
            ((IMock)windowProvider).VerifyNoOtherCalls();
        }

        [Test, STAThreadExecutor]
        public void PreviewDocument1()
        {
            var printDialog = IPrintDialog.Mock();
            var testPrintProcessor = new TestPrintProcessor { PrintDialog = printDialog.Object };
            var printProcessor = new PrintProcessorCollection(testPrintProcessor);
            var windowProvider = IWindowProvider.Mock();
            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Show(Any<string>(), Any<DocumentViewer>()).WasCalled();
            ((IMock)windowProvider).VerifyNoOtherCalls();
        }

        [Test]
        public async Task PrintDocument_NoPrintProcessor_DoesNotPrint()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());

            using (Assert.Multiple())
            {
                await Assert.That(ppcoll.PrintDocument()).IsFalse();
                await Assert.That(ppcoll.PrintDocument(string.Empty)).IsFalse();
            }

        }

        [Test]
        public async Task PrintDoucment_CloseDialog_ReturnsFalse()
        {
            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(false);

            var testPrintProcessor = new TestPrintProcessor
            {
                PrintDialog = printDialog.Object
            };

            var printProcessor = new PrintProcessorCollection(testPrintProcessor);

            await Assert.That(printProcessor.PrintDocument()).IsFalse();
        }

        [Test, STAThreadExecutor]
        public async Task PrintDoucment_Direct_ReturnsTrue()
        {
            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            var testPrintProcessor = new TestPrintProcessor();
            var printProcessor = new PrintProcessorCollection(testPrintProcessor);

            testPrintProcessor.PrintDialog = printDialog.Object;

            await Assert.That(printProcessor.PrintDocument()).IsTrue();
            await Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new LocalPrintServer())).IsTrue();
            await Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0])).IsTrue();
        }
    }
}
