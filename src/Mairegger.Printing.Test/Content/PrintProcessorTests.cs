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
using System.Windows.Media;
using Bogus;
using Mairegger.Printing.Content;
using Mairegger.Printing.Definition;
using Mairegger.Printing.PrintProcessor;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintProcessorTests
    {
        private static readonly Faker faker = new Faker();

        [StaFact]
        public void CheckPrintDimensions_HasPrintDimensionsSet()
        {
            var print = new PrintProcessorWithPrintOnAllPages();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            print.PrintDialog = printDialog.Object;
            print.PrintDocument();

            Assert.True(print.PrintDefinition.IsDefined(PrintAppendixes.Footer));
        }

        [Fact]
        public void ColorPrintPartsForDebug_IsDefault_False()
        {
            PrintProcessor.PrintProcessor tp = new TestPrintProcessor();
            Assert.False(tp.ColorPrintPartsForDebug);
            tp.ColorPrintPartsForDebug = true;
            Assert.True(tp.ColorPrintPartsForDebug);
        }

        [Fact]
        public void Ctor()
        {
            var printProcessor = new Mock<PrintProcessor.PrintProcessor>();

            Assert.Multiple(
                ()=> Assert.NotNull(printProcessor.Object.PrintDialog),
                ()=> Assert.Equal(PageOrientation.Portrait, printProcessor.Object.PageOrientation));
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList2), 10, 20, 3, 7,1, MemberType = typeof(RandomTest))]
        public void CustomAlternatingRowColors(int itemCount, int differentColors)
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

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            for (int i = 0; i < itemCount; i++)
            {
                Assert.Equal(colorList[i % differentColors], retrievedContent[i].Content.GetValue(Panel.BackgroundProperty));
            }
        }

        [Fact]
        public void FileName_Default_IsStringEmpty()
        {
            Mock<PrintProcessor.PrintProcessor> mock = new Mock<PrintProcessor.PrintProcessor>();
            Assert.Empty(mock.Object.FileName);
        }

        [Fact]
        public void FileName_InvalidCharacters_GetsRemoved()
        {
            Mock<PrintProcessor.PrintProcessor> mock = new Mock<PrintProcessor.PrintProcessor>();
            var formattableString = $"Hello{Path.GetInvalidFileNameChars()[0]}Hello{Path.GetInvalidFileNameChars()[1]}";

            mock.Object.FileName = formattableString;

            char[] invalid = Path.GetInvalidFileNameChars();
            #if NETFRAMEWORK
            Assert.DoesNotContain(mock.Object.FileName, v => invalid.Contains(v));
            #else
            Assert.DoesNotContain(mock.Object.FileName, Path.GetInvalidFileNameChars(), StringComparison.InvariantCultureIgnoreCase);
            #endif
        }

        [Fact]
        public void GetBackgound_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetBackground());
        }

        [Fact]
        public void GetFooter_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetFooter());
        }

        [Fact]
        public void GetHeader_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetHeader());
        }

        [Fact]
        public void GetHeaderDescription_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(()=> new TestPrintProcessor().GetHeaderDescription());
        }

        [Fact]
        public void GetSummary_Throws_IfNotImplemented()
        {
            Assert.Throws<NotImplementedException>(() => new TestPrintProcessor().GetSummary());
        }

        [StaFact]
        public void IsAlternatingRowColor_False_NotColoring()
        {
            var retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAllPages(retrievedContent)
                     {
                         ItemCount = 3,
                         IsAlternatingRowColor = false
                     };

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            Assert.Multiple(
                ()=> Assert.Null(retrievedContent[0].Content.GetValue(Panel.BackgroundProperty)),
                ()=> Assert.Null(retrievedContent[1].Content.GetValue(Panel.BackgroundProperty)),
                ()=> Assert.Null(retrievedContent[2].Content.GetValue(Panel.BackgroundProperty)));
        }

        [StaFact]
        public void IsAlternatingRowColor_True_Coloring()
        {
            var retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAllPages(retrievedContent)
                     {
                         ItemCount = 10,
                         IsAlternatingRowColor = true
                     };

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();


            for (int j = 0; j < retrievedContent.Count; j++)
            {
                Assert.Equal(pp.AlternatingRowColors[j % 2], retrievedContent[j].Content.GetValue(Panel.BackgroundProperty));
            }
        }

        [StaFact]
        public void NoItemsOnPrintout()
        {
            var printDialog = new Mock<IPrintDialog>();
            var printProcessor = new NoLineItemsTestPrintProcessor
                                 {
                                     PrintDialog = printDialog.Object
                                 };

            Assert.True(printProcessor.PrintDocument());
        }

        [StaTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void PreviewDocument(bool colorPrintPartsForDebug)
        {
            var printDialog = new Mock<IPrintDialog>();
            var printProcessor = new TestPrintProcessor
                                 {
                                     PrintDialog = printDialog.Object,
                                     ColorPrintPartsForDebug = colorPrintPartsForDebug
            };

            var windowProvider = new Mock<IWindowProvider>();
            windowProvider.Setup(i => i.Show(It.IsNotNull<string>(), It.IsNotNull<DocumentViewer>()));

            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.VerifyAll();
        }

        [Fact]
        public void PrintDimension()
        {
            var pp = new PrintProcessorWithPrintOnAllPages();
            var pd = new PrintDimension();
            pp.PrintDimension = pd;
            Assert.Equal(pd, pp.PrintDimension);
        }

        [Fact]
        public void PrintDoucment_CloseDialog_ReturnsFalse()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.False(printProcessor.PrintDocument());
        }

        [StaFact]
        public void PrintDoucment_CloseDialog_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.True(printProcessor.PrintDocument());
        }

        [StaFact]
        public void PrintDoucment_Direct_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.Multiple(
                ()=> Assert.True(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new LocalPrintServer())),
                ()=> Assert.True(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0])));
        }

        [StaFact]
        public void SaveToXps_FileIsFilled()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            string file = Path.GetTempFileName();
            Assert.Equal(0, new FileInfo(file).Length);
            printProcessor.SaveToXps(file);

            Assert.True(new FileInfo(file).Length > 0);

            File.Delete(file);
        }
    }
}
