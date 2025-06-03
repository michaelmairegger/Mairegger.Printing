// Copyright 2016 Michael Mairegger
//
// Licensed under the Apache License, Version 2.0 (the "License");
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

namespace Mairegger.Printing.Tests.Content
{
    public class PrintProcessorCollectionTests
    {
        [Fact]
        public void Ctor()
        {
            Mock<PrintProcessor.PrintProcessor>[] m1 = [new(), new(), new(), new()];
            PrintProcessorCollection pp = new PrintProcessorCollection(m1.Select(i => i.Object), "FileName");
            Assert.Equal(m1.Select(i => i.Object), pp);

            Assert.Equal("FileName", pp.FileName);
        }

        [Fact]
        public void Ctor_SingleElement()
        {
            var p = new Mock<PrintProcessor.PrintProcessor>();
            PrintProcessorCollection pp = new PrintProcessorCollection(p.Object);

            Assert.Multiple(
                () => Assert.Equal(p.Object.FileName, pp.FileName),
                () => Assert.Contains(p.Object, pp),
                () => Assert.Single(pp));
        }

        [Fact]
        public void FileName_Default_IsStringEmpty()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            Assert.Empty(ppcoll.FileName);
        }

        [Fact]
        public void FileName_InvalidCharacters_GetsRemoved()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            var formattableString = $"Hello{Path.GetInvalidFileNameChars()[0]}Hello{Path.GetInvalidFileNameChars()[1]}";

            Assert.Empty(ppcoll.FileName);

            ppcoll.FileName = formattableString;
            Assert.DoesNotContain(ppcoll.FileName, Path.GetInvalidFileNameChars(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Fact]
        public void PreviewDocument()
        {
            var printProcessor = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            var windowProvider = new Mock<IWindowProvider>();

            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Verify(i => i.Show(It.IsNotNull<string>(), It.IsNotNull<DocumentViewer>()), Times.Never);
        }

        [StaFact]
        public void PrintEverything()
        {
            var printDialog = new Mock<IPrintDialog>();
            var printProcessor = new PrintEverything() { PrintDialog = printDialog.Object };

            var windowProvider = new Mock<IWindowProvider>();
            Assert.True(printProcessor.PrintDocument());
            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Verify(i => i.Show(It.IsNotNull<string>(), It.IsNotNull<DocumentViewer>()), Times.Once);
        }

        [StaFact]
        public void PreviewDocument1()
        {
            var printDialog = new Mock<IPrintDialog>();
            var testPrintProcessor = new TestPrintProcessor { PrintDialog = printDialog.Object };
            var printProcessor = new PrintProcessorCollection(testPrintProcessor);
            var windowProvider = new Mock<IWindowProvider>();
            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.Verify(i => i.Show(It.IsNotNull<string>(), It.IsNotNull<DocumentViewer>()), Times.Once);
        }

        [Fact]
        public void PrintDocument_NoPrintProcessor_DoesNotPrint()
        {
            var ppcoll = new PrintProcessorCollection(Enumerable.Empty<PrintProcessor.PrintProcessor>());
            Assert.Multiple(
                () => Assert.False(ppcoll.PrintDocument()),
                () => Assert.False(ppcoll.PrintDocument(string.Empty))
                );
        }

        [Fact]
        public void PrintDoucment_CloseDialog_ReturnsFalse()
        {
            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            var testPrintProcessor = new TestPrintProcessor
                                     {
                                         PrintDialog = printDialog.Object
                                     };

            var printProcessor = new PrintProcessorCollection(testPrintProcessor);

            Assert.False(printProcessor.PrintDocument());
        }

        [StaFact]
        public void PrintDoucment_Direct_ReturnsTrue()
        {
            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            var testPrintProcessor = new TestPrintProcessor();
            var printProcessor = new PrintProcessorCollection(testPrintProcessor);

            testPrintProcessor.PrintDialog = printDialog.Object;

            Assert.Multiple(
                () => Assert.True(printProcessor.PrintDocument()),
                () => Assert.True(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new LocalPrintServer())),
                () => Assert.True(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0])));
        }
    }
}
