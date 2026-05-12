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
using System.Windows.Media;
using Bogus;
using Mairegger.Printing.Content;
using Mairegger.Printing.Definition;
using Mairegger.Printing.PrintProcessor;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintProcessorTests
    {
        private static readonly Faker faker = new Faker();

        [Test, STAThreadExecutor]
        public async Task CheckPrintDimensions_HasPrintDimensionsSet()
        {
            var print = new PrintProcessorWithPrintOnAllPages();

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            print.PrintDialog = printDialog.Object;
            print.PrintDocument();

            await Assert.That(print.PrintDefinition.IsDefined(PrintAppendixes.Footer)).IsTrue();
        }

        [Test]
        public async Task ColorPrintPartsForDebug_IsDefault_False()
        {
            PrintProcessor.PrintProcessor tp = new TestPrintProcessor();
            await Assert.That(tp.ColorPrintPartsForDebug).IsFalse();
            tp.ColorPrintPartsForDebug = true;
            await Assert.That(tp.ColorPrintPartsForDebug).IsTrue();
        }

        [Test]
        public async Task Ctor()
        {
            var printProcessor = TestPrintProcessor.Mock();

            using (Assert.Multiple())
            {
                await Assert.That(printProcessor.Object.PrintDialog).IsNotNull();
                await Assert.That(printProcessor.Object.PageOrientation).IsEqualTo(PageOrientation.Portrait);
            }
        }


        private static readonly Faker s_faker = new();
        public static IEnumerable<(int, int)> CustomAlternatingRowColorsRandomList()
        {
            for (int i = 1; i <= 1; i++)
            {
                yield return (s_faker.Random.Int(10,20),s_faker.Random.Int(3,7));
            }
        }


        [Test, STAThreadExecutor]
        [MethodDataSource(nameof(CustomAlternatingRowColorsRandomList))]
        public async Task CustomAlternatingRowColors(int itemCount, int differentColors)
        {
            var retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAllPages(retrievedContent)
            {
                ItemCount = itemCount,
                IsAlternatingRowColor = true
            };

            var colorList = new List<SolidColorBrush>();
            for (int i = 0; i < differentColors; i++)
            {
                colorList.Add(new SolidColorBrush(Color.FromRgb(faker.Random.Byte(), faker.Random.Byte(), faker.Random.Byte())));
            }
            pp.AlternatingRowColors = colorList;

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            for (int i = 0; i < itemCount; i++)
            {
                await Assert.That(retrievedContent[i].Content.GetValue(Panel.BackgroundProperty)).IsEqualTo(colorList[i % differentColors]);
            }
        }

        [Test]
        public async Task FileName_Default_IsStringEmpty()
        {
            Mock<TestPrintProcessor> mock = TestPrintProcessor.Mock();
            await Assert.That(mock.Object.FileName).IsEmpty();
        }

        [Test]
        public async Task FileName_InvalidCharacters_GetsRemoved()
        {
            Mock<TestPrintProcessor> mock = TestPrintProcessor.Mock();
            var formattableString = $"Hello{Path.GetInvalidFileNameChars()[0]}Hello{Path.GetInvalidFileNameChars()[1]}";

            mock.Object.FileName = formattableString;

            char[] invalid = Path.GetInvalidFileNameChars();
            await Assert.That(mock.Object.FileName.ToCharArray()).DoesNotContain(e => invalid.Contains(e));
        }

        [Test]
        public void GetBackgound_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetBackground());
        }

        [Test]
        public void GetFooter_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetFooter());
        }

        [Test]
        public void GetHeader_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetHeader());
        }

        [Test]
        public void GetHeaderDescription_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetHeaderDescription());
        }

        [Test]
        public void GetSummary_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetSummary());
        }

        [Test, STAThreadExecutor]
        public async Task IsAlternatingRowColor_False_NotColoring()
        {
            var retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAllPages(retrievedContent)
            {
                ItemCount = 3,
                IsAlternatingRowColor = false
            };

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            using (Assert.Multiple())
            {
                await Assert.That(retrievedContent[0].Content.GetValue(Panel.BackgroundProperty)).IsNull();
                await Assert.That(retrievedContent[1].Content.GetValue(Panel.BackgroundProperty)).IsNull();
                await Assert.That(retrievedContent[2].Content.GetValue(Panel.BackgroundProperty)).IsNull();
            }
        }

        [Test, STAThreadExecutor]
        public async Task IsAlternatingRowColor_True_Coloring()
        {
            var retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAllPages(retrievedContent)
            {
                ItemCount = 10,
                IsAlternatingRowColor = true
            };

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();


            for (int j = 0; j < retrievedContent.Count; j++)
            {
                await Assert.That(retrievedContent[j].Content.GetValue(Panel.BackgroundProperty)).IsEqualTo(pp.AlternatingRowColors[j % 2]);
            }
        }

        [Test, STAThreadExecutor]
        public async Task NoItemsOnPrintout()
        {
            var printDialog = IPrintDialog.Mock();
            var printProcessor = new NoLineItemsTestPrintProcessor
            {
                PrintDialog = printDialog.Object
            };

            await Assert.That(printProcessor.PrintDocument()).IsTrue();
        }

        [Test, STAThreadExecutor]
        [Arguments(true)]
        [Arguments(false)]
        public void PreviewDocument(bool colorPrintPartsForDebug)
        {
            var printDialog = IPrintDialog.Mock();
            var printProcessor = new TestPrintProcessor
            {
                PrintDialog = printDialog.Object,
                ColorPrintPartsForDebug = colorPrintPartsForDebug
            };

            var windowProvider = IWindowProvider.Mock();
            windowProvider.Show(Any<string>(), Any<DocumentViewer>());

            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.VerifyAll();
        }

        [Test]
        public async Task PrintDimension()
        {
            var pp = new PrintProcessorWithPrintOnAllPages();
            var pd = new PrintDimension();
            pp.PrintDimension = pd;
            await Assert.That(pp.PrintDimension).IsEqualTo(pd);
        }

        [Test]
        public async Task PrintDoucment_CloseDialog_ReturnsFalse()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            await Assert.That(printProcessor.PrintDocument()).IsFalse();
        }

        [Test, STAThreadExecutor]
        public async Task PrintDoucment_CloseDialog_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(true);

            printProcessor.PrintDialog = printDialog.Object;

            await Assert.That(printProcessor.PrintDocument()).IsTrue();
        }

        [Test, STAThreadExecutor]
        public async Task PrintDoucment_Direct_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            using (Assert.Multiple())
            {
                await Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new LocalPrintServer())).IsTrue();
                await Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0])).IsTrue();
            }
        }

        [Test, STAThreadExecutor]
        public async Task SaveToXps_FileIsFilled()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = IPrintDialog.Mock();
            printDialog.ShowDialog().Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            string file = Path.GetTempFileName();
            await Assert.That(new FileInfo(file).Length).IsEqualTo(0);
            printProcessor.SaveToXps(file);

            await Assert.That(new FileInfo(file).Length > 0).IsTrue();

            File.Delete(file);
        }
    }
}
