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

namespace Mairegger.Printing.Test.Content
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Printing;
    using System.IO;
    using System.Printing;
    using System.Threading;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class PrintProcessorTests
    {
        [Test]
        public void CheckPrintDimensions_HasPrintDimensionsSet()
        {
            var print = new PrintProcessorWithPrintOnAttribute();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            print.PrintDialog = printDialog.Object;
            print.PrintDocument();

            Assert.That(print.PrintDefinition.IsDefined(PrintAppendixes.Footer));
        }

        [Test]
        public void ColorPrintPartsForDebug_IsDefault_False()
        {
            Printing.PrintProcessor.PrintProcessor tp = new TestPrintProcessor();
            Assert.That(tp.ColorPrintPartsForDebug, Is.False);
            tp.ColorPrintPartsForDebug = true;
            Assert.That(tp.ColorPrintPartsForDebug, Is.True);
        }

        [Test]
        public void Ctor()
        {
            var printProcessor = new Mock<Printing.PrintProcessor.PrintProcessor>();

            Assert.That(printProcessor.Object.PrintDialog, Is.Not.Null);
            Assert.That(printProcessor.Object.PageOrientation, Is.EqualTo(PageOrientation.Portrait));
        }

        [Test]
        public void CustomAlternatingRowColors([Random(10, 20, 1)] int itemCount, [Random(3, 7, 1)] int differentColors)
        {
            IList<IPrintContent> retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAttribute(retrievedContent);
            pp.ItemCount = itemCount;
            pp.IsAlternatingRowColor = true;

            var colorList = new List<SolidColorBrush>();
            var r = new Random();
            for (int i = 0; i < differentColors; i++)
            {
                colorList.Add(new SolidColorBrush(Color.FromRgb((byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255))));
            }
            pp.AlternatingRowColors = colorList;

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            for (int i = 0; i < itemCount; i++)
            {
                Assert.That(retrievedContent[i].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(colorList[i % differentColors]));
            }
        }

        [Test]
        public void FileName_Default_IsStringEmpty()
        {
            Mock<Printing.PrintProcessor.PrintProcessor> mock = new Mock<Printing.PrintProcessor.PrintProcessor>();
            Assert.That(mock.Object.FileName, Is.Empty);
        }

        [Test]
        public void FileName_InvalidCharacters_GetsRemoved()
        {
            Mock<Printing.PrintProcessor.PrintProcessor> mock = new Mock<Printing.PrintProcessor.PrintProcessor>();
            var formattableString = $"Hello{Path.GetInvalidFileNameChars()[0]}Hello{Path.GetInvalidFileNameChars()[1]}";

            mock.Object.FileName = formattableString;

            CollectionAssert.DoesNotContain(mock.Object.FileName, Path.GetInvalidFileNameChars());
        }

        [Test]
        public void FileName_Null_IsStringEmpty()
        {
            Mock<Printing.PrintProcessor.PrintProcessor> mock = new Mock<Printing.PrintProcessor.PrintProcessor>();
            mock.Object.FileName = null;

            Assert.That(mock.Object.FileName, Is.Empty);
        }

        [Test]
        public void GetBackgound_Default_Null()
        {
            Assert.That(new TestPrintProcessor().GetBackgound(), Is.Null);
        }

        [Test]
        public void GetFooter_Default_Null()
        {
            Assert.That(new TestPrintProcessor().GetFooter(), Is.Null);
        }

        [Test]
        public void GetHeader_Default_Null()
        {
            Assert.That(new TestPrintProcessor().GetHeader(), Is.Null);
        }

        [Test]
        public void GetHeaderDescription_Default_Null()
        {
            Assert.That(new TestPrintProcessor().GetHeaderDescription(), Is.Null);
        }

        [Test]
        public void GetSummary_Default_Null()
        {
            Assert.That(new TestPrintProcessor().GetSummary(), Is.Null);
        }

        [Test]
        public void IsAlternatingRowColor_False_NotColoring()
        {
            IList<IPrintContent> retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAttribute(retrievedContent);
            pp.ItemCount = 3;
            pp.IsAlternatingRowColor = false;

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            Assert.That(retrievedContent[0].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(null));
            Assert.That(retrievedContent[1].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(null));
            Assert.That(retrievedContent[2].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(null));
        }

        [Test]
        public void IsAlternatingRowColor_True_Coloring()
        {
            IList<IPrintContent> retrievedContent = new List<IPrintContent>();
            var pp = new PrintProcessorWithPrintOnAttribute(retrievedContent);
            pp.ItemCount = 3;
            pp.IsAlternatingRowColor = true;

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            pp.PrintDialog = printDialog.Object;
            pp.PrintDocument();

            Assert.That(retrievedContent[0].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(pp.AlternatingRowColors[0]));
            Assert.That(retrievedContent[1].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(pp.AlternatingRowColors[1 % 2]));
            Assert.That(retrievedContent[2].Content.GetValue(Panel.BackgroundProperty), Is.EqualTo(pp.AlternatingRowColors[2 % 2]));
        }

        [Test]
        public void PreviewDocument()
        {
            var printProcessor = new TestPrintProcessor();
            var windowProvider = new Mock<IWindowProvider>();
            windowProvider.Setup(i => i.Show(It.IsNotNull<string>(), It.IsNotNull<DocumentViewer>()));

            printProcessor.PreviewDocument(windowProvider.Object);

            windowProvider.VerifyAll();
        }

        [Test]
        public void PrintDialog_Null_ThrowsArgumentNullException()
        {
            var printProcessor = new Mock<Printing.PrintProcessor.PrintProcessor>();

            Assert.That(() => printProcessor.Object.PrintDialog = null, Throws.ArgumentNullException);
        }

        [Test]
        public void PrintDimension()
        {
            var pp = new PrintProcessorWithPrintOnAttribute();
            var pd = new PrintDimension();
            pp.PrintDimension = pd;
            Assert.That(pp.PrintDimension, Is.EqualTo(pd));
        }

        [Test]
        public void PrintDimension_SetNull_ThrowsArgumentNullException()
        {
            var pp = new PrintProcessorWithPrintOnAttribute();
            Assert.That(pp.PrintDimension, Is.Not.Null);
            Assert.That(() => pp.PrintDimension = null, Throws.ArgumentNullException);
        }

        [Test]
        public void PrintDoucment_CloseDialog_ReturnsFalse()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.That(printProcessor.PrintDocument(), Is.False);
        }

        [Test]
        public void PrintDoucment_CloseDialog_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(true);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.That(printProcessor.PrintDocument(), Is.True);
            //Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new System.Printing.LocalPrintServer()), Is.False);
            //Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0]), Is.False);
        }

        [Test]
        public void PrintDoucment_Direct_ReturnsTrue()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0], new LocalPrintServer()), Is.True);
            Assert.That(printProcessor.PrintDocument(PrinterSettings.InstalledPrinters[0]), Is.True);
        }

        [Test]
        public void SaveToXps_FileIsFilled()
        {
            var printProcessor = new TestPrintProcessor();

            var printDialog = new Mock<IPrintDialog>();
            printDialog.Setup(i => i.ShowDialog()).Returns(false);

            printProcessor.PrintDialog = printDialog.Object;

            string file = Path.GetTempFileName();
            Assert.That(new FileInfo(file).Length, Is.Zero);
            printProcessor.SaveToXps(file);

            Assert.That(new FileInfo(file).Length, Is.Positive);

            File.Delete(file);
        }
    }
}