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

namespace Mairegger.Printing.Tests.Definition
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class PrintingDimensionsTests
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetHeightFor()
        {
            Mock<IPrintProcessor> mock = new Mock<IPrintProcessor>();
            mock.Setup(i => i.GetHeader()).Returns(new Grid { Height = 1 });
            mock.Setup(i => i.GetFooter()).Returns(new Grid { Height = 2 });
            mock.Setup(i => i.GetHeaderDescription()).Returns(new Grid { Height = 3 });

            PrintDimension pd = new PrintDimension
                                {
                                    PrintProcessor = mock.Object,
                                    InternalPrintDefinition = new PrintDefinition()
                                };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Header | PrintAppendixes.Footer | PrintAppendixes.Summary));

            Assert.That(pd.GetHeightFor(PrintAppendixes.Header, 1, false), Is.EqualTo(1));
            Assert.That(pd.GetHeightFor(PrintAppendixes.Footer, 1, false), Is.EqualTo(2));

            Assert.That(pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false), Is.EqualTo(0));

            Assert.That(() => pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Throws.ArgumentNullException);

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.HeaderDescription));
            Assert.That(pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false), Is.EqualTo(3));
        }

        [Test]
        public void GetHeightForBody_IsTotalPageHeight_IfNoAdditionalPrintParts()
        {
            PrintDimension pd = new PrintDimension { InternalPrintDefinition = new PrintDefinition() };

            Assert.That(pd.GetHeightForBodyGrid(1, false), Is.EqualTo(0));

            pd.PageSize = new Size(100, 300);

            Assert.That(pd.GetHeightForBodyGrid(1, false), Is.EqualTo(300));

            pd.Margin = new Thickness(10);

            Assert.That(pd.GetHeightForBodyGrid(1, false), Is.EqualTo(280));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetHeightForBody_IsTotalPageHeightMinusPrintParts()
        {
            Mock<IPrintProcessor> mock = new Mock<IPrintProcessor>();
            mock.Setup(i => i.GetHeader()).Returns(new Grid { Height = 10 });
            mock.Setup(i => i.GetFooter()).Returns(new Grid { Height = 20 });
            mock.Setup(i => i.GetHeaderDescription()).Returns(new Grid { Height = 30 });
            mock.Setup(i => i.GetSummary()).Returns(new Grid { Height = 40 });
            mock.Setup(i => i.GetPageNumbers(It.IsAny<int>(), It.IsAny<int>())).Returns(new Grid { Height = 25 });

            PrintDimension pd = new PrintDimension
                                {
                                    PrintProcessor = mock.Object,
                                    InternalPrintDefinition = new PrintDefinition()
                                };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            pd.PageSize = new Size(100, 300);

            // 300 - Header - Footer - HeaderDescritpion - Summary - PageNumbers
            Assert.That(pd.GetHeightForBodyGrid(1, false), Is.EqualTo(175));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GetRangeFor()
        {
            Mock<IPrintProcessor> mock = new Mock<IPrintProcessor>();
            mock.Setup(i => i.GetHeader()).Returns(new Grid { Height = 10 });
            mock.Setup(i => i.GetFooter()).Returns(new Grid { Height = 20 });
            mock.Setup(i => i.GetHeaderDescription()).Returns(new Grid { Height = 30 });
            mock.Setup(i => i.GetSummary()).Returns(new Grid { Height = 40 });
            mock.Setup(i => i.GetPageNumbers(It.IsAny<int>(), It.IsAny<int>())).Returns(new Grid { Height = 25 });

            PrintDimension pd = new PrintDimension
                                {
                                    PrintProcessor = mock.Object,
                                    Margin = new Thickness(10),
                                    PageSize = new Size(100, 1000),
                                    InternalPrintDefinition = new PrintDefinition()
                                };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            Assert.That(pd.GetRangeFor(PrintAppendixes.Header, 1, false), Is.EqualTo(new Printing.Definition.PageRange(10, 20)));
            Assert.That(pd.GetRangeFor(PrintAppendixes.HeaderDescription, 1, false), Is.EqualTo(new Printing.Definition.PageRange(20, 50)));
            Assert.That(pd.GetRangeForBodyGrid(1, false), Is.EqualTo(new Printing.Definition.PageRange(50, 905)));
            Assert.That(pd.GetRangeFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(new Printing.Definition.PageRange(905, 945)));
            Assert.That(pd.GetRangeFor(PrintAppendixes.Footer, 1, false), Is.EqualTo(new Printing.Definition.PageRange(945, 965)));
            Assert.That(pd.GetRangeFor(PrintAppendixes.PageNumbers, 1, false), Is.EqualTo(new Printing.Definition.PageRange(965, 990)));
        }

        [Test]
        public void GetRangeFor_InvalidPrintAppendix()
        {
            PrintDimension pd = new PrintDimension();
            Assert.That(() => pd.GetRangeFor(PrintAppendixes.All, 1, false), Throws.ArgumentException);
        }

        [Test]
        public void PageSize_Test()
        {
            Thickness margin = new Thickness(10, 20, 30, 40);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            Assert.AreEqual(pageSize, printingDimensions.PageSize);
        }

        [Test]
        public void PrintablePageSize_Test(
            [Values(1, 2)] double left,
            [Values(4, 5)] double top,
            [Values(7, 8)] double right,
            [Values(10, 11)] double bottom)
        {
            Thickness margin = new Thickness(left, top, right, bottom);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            Size expected = new Size(pageSize.Width - margin.Left - margin.Right, pageSize.Height - margin.Top - margin.Bottom);

            Assert.AreEqual(expected, printingDimensions.PrintablePageSize);
        }

        [Test]
        public void PrintDimensionTest()
        {
            var thickness = new Thickness(10, 10, 10, 10);
            TestPrintDimension tpd = new TestPrintDimension(thickness);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(tpd, pageSize);

            double totalWidth = pageSize.Width - thickness.Left - thickness.Right;

            Assert.That(tpd.PrintablePageSize.Width, Is.EqualTo(totalWidth));

            double pieces = 5; // sum of TestPrintDimensions
            double widthPerPiece = (totalWidth - 100) / pieces;

            Assert.That(tpd.Column1, Is.EqualTo(1 * widthPerPiece), $"{nameof(tpd.Column1)}");
            Assert.That(tpd.Column2, Is.EqualTo(3 * widthPerPiece), $"{nameof(tpd.Column2)}");
            Assert.That(tpd.Column3, Is.EqualTo(100), $"{nameof(tpd.Column3)}");
            Assert.That(tpd.Column4, Is.EqualTo(1 * widthPerPiece), $"{nameof(tpd.Column4)}");
        }

        [Test]
        public void SetColumnDimensionToPropertyWithWrongType_ThrowsException()
        {
            var invalidPrintDimension = new InvalidPrintDimension();
            invalidPrintDimension.Column1 = new InvalidPrintDimension().Column1;
            Assert.That(() => SetPageSizeToPrintDimension(invalidPrintDimension, new Size(500, 1000)), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void SetColumnDimensionToReadOnlyProperty()
        {
            var cannotWritePrintDimension = new CannotWritePrintDimension();

            Assert.That(cannotWritePrintDimension.Column1, Is.EqualTo(1));

            SetPageSizeToPrintDimension(cannotWritePrintDimension, new Size(500, 1000));

            Assert.That(cannotWritePrintDimension.Column1, Is.EqualTo(500));
        }

        [Test]
        public void SetHeightValue()
        {
            PrintDimension pd = new PrintDimension
                                {
                                    InternalPrintDefinition = new PrintDefinition()
                                };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            pd.SetHeightValue(PrintAppendixes.Summary, 5);
            Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(5));
            pd.SetHeightValue(PrintAppendixes.Summary, 6);
            Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(6));

            Assert.That(()=> pd.SetHeightValue(PrintAppendixes.Summary, -1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void RecalculateHeightValueWhen([Random(10, 100, 1)] int initialHeight)
        {
            PrintDimension pd = new PrintDimension
                                {
                                    InternalPrintDefinition = new PrintDefinition()
                                };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            Mock<IPrintProcessor> printProcessor = new Mock<IPrintProcessor>();
            printProcessor.Setup(i => i.GetSummary()).Returns(() => new Grid() { Height = initialHeight });
            pd.PrintProcessor = printProcessor.Object;

            Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(initialHeight));
            pd.SetHeightValue(PrintAppendixes.Summary, 5);

            pd.RecalculateHeightValueWhen(() => false, PrintAppendixes.Summary);

            Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(5));

            pd.RecalculateHeightValueWhen(() => true, PrintAppendixes.Summary);

            Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false), Is.EqualTo(initialHeight));
        }

        private static void SetPageSizeToPrintDimension(PrintDimension printingDimension, Size pageSize)
        {
            printingDimension.PageSize = pageSize;
            printingDimension.PositionRelative();
        }

        private class InvalidPrintDimension : PrintDimension
        {
            public InvalidPrintDimension()
            {
                UseRelativeColumnPosition = true;
            }

            [ColumnDimension("1*")]
            public int Column1 { get; set; }
        }

        private class CannotWritePrintDimension : PrintDimension
        {
            public CannotWritePrintDimension()
            {
                UseRelativeColumnPosition = true;
            }

            [ColumnDimension(".1*")]
            public double Column1 { get; } = 1;
        }

        private class TestPrintDimension : PrintDimension
        {
            public TestPrintDimension(Thickness margin)
                : base(margin)
            {
                UseRelativeColumnPosition = true;
            }

            [ColumnDimension("1*")]
            public double Column1 { get; private set; }

            [ColumnDimension("3*")]
            public double Column2 { get; private set; }

            [ColumnDimension("100px")]
            public double Column3 { get; private set; }

            [ColumnDimension("1*")]
            public double Column4 { get; private set; }
        }
    }
}