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

using System.Windows;
using System.Windows.Controls;
using Mairegger.Printing.Definition;
using Mairegger.Printing.PrintProcessor;
using Mairegger.Printing.Tests.Content;
using PageRange = Mairegger.Printing.Definition.PageRange;

namespace Mairegger.Printing.Tests.Definition
{
    public class PrintingDimensionsTests
    {
        [StaFact]
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

            Assert.Multiple(
                ()=> Assert.Equal(1, pd.GetHeightFor(PrintAppendixes.Header, 1, false)),
                ()=> Assert.Equal(2, pd.GetHeightFor(PrintAppendixes.Footer, 1, false)),

                ()=> Assert.Equal(0, pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false)),

                ()=> Assert.Throws<ArgumentNullException>(() => pd.GetHeightFor(PrintAppendixes.Summary, 1, false)));

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.HeaderDescription));
            Assert.Equal(3, pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false));
        }

        [Fact]
        public void GetHeightForBody_IsTotalPageHeight_IfNoAdditionalPrintParts()
        {
            PrintDimension pd = new PrintDimension { InternalPrintDefinition = new PrintDefinition() };

            Assert.Equal(0, pd.GetHeightForBodyGrid(1, false));

            pd.PageSize = new Size(100, 300);

            Assert.Equal(300, pd.GetHeightForBodyGrid(1, false));

            pd.Margin = new Thickness(10);

            Assert.Equal(280, pd.GetHeightForBodyGrid(1, false));
        }

        [StaFact]
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
            Assert.Equal(175, pd.GetHeightForBodyGrid(1, false));
        }

        [StaFact]
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

            Assert.Multiple(
                ()=> Assert.Equal(new PageRange(10, 20), pd.GetRangeFor(PrintAppendixes.Header, 1, false)),
                ()=> Assert.Equal(new PageRange(20, 50), pd.GetRangeFor(PrintAppendixes.HeaderDescription, 1, false)),
                ()=> Assert.Equal(new PageRange(50, 905), pd.GetRangeForBodyGrid(1, false)),
                ()=> Assert.Equal(new PageRange(905, 945), pd.GetRangeFor(PrintAppendixes.Summary, 1, false)),
                ()=> Assert.Equal(new PageRange(945, 965), pd.GetRangeFor(PrintAppendixes.Footer, 1, false)),
                ()=> Assert.Equal(new PageRange(965, 990), pd.GetRangeFor(PrintAppendixes.PageNumbers, 1, false)));
        }

        [Fact]
        public void GetRangeFor_InvalidPrintAppendix()
        {
            PrintDimension pd = new PrintDimension();
            Assert.Throws<ArgumentException>(() => pd.GetRangeFor(PrintAppendixes.All, 1, false));
        }

        [Fact]
        public void PageSize_Test()
        {
            Thickness margin = new Thickness(10, 20, 30, 40);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            Assert.Equal(pageSize, printingDimensions.PageSize);
        }

        [Theory]
        [InlineData(1,4,7,10)]
        [InlineData(2,5,8,11)]
        public void PrintablePageSize_Test(double left, double top, double right, double bottom)
        {
            Thickness margin = new Thickness(left, top, right, bottom);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            Size expected = new Size(pageSize.Width - margin.Left - margin.Right, pageSize.Height - margin.Top - margin.Bottom);

            Assert.Equal(expected, printingDimensions.PrintablePageSize);
        }

        [Fact]
        public void PrintDimensionTest()
        {
            var thickness = new Thickness(10, 10, 10, 10);
            TestPrintDimension tpd = new TestPrintDimension(thickness);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(tpd, pageSize);

            double totalWidth = pageSize.Width - thickness.Left - thickness.Right;

            Assert.Equal(totalWidth, tpd.PrintablePageSize.Width);

            double pieces = 5; // sum of TestPrintDimensions
            double widthPerPiece = (totalWidth - 100) / pieces;

            Assert.Multiple(
                ()=> Assert.Equal(1 * widthPerPiece, tpd.Column1),
                ()=> Assert.Equal(3 * widthPerPiece, tpd.Column2),
                ()=> Assert.Equal(100, tpd.Column3),
                ()=> Assert.Equal(1 * widthPerPiece, tpd.Column4));
        }

        [Fact]
        public void SetColumnDimensionToPropertyWithWrongType_ThrowsException()
        {
            var invalidPrintDimension = new InvalidPrintDimension();
            invalidPrintDimension.Column1 = new InvalidPrintDimension().Column1;
            Assert.Throws<InvalidOperationException>(() => SetPageSizeToPrintDimension(invalidPrintDimension, new Size(500, 1000)));
        }

        [Fact]
        public void SetColumnDimensionToReadOnlyProperty()
        {
            var cannotWritePrintDimension = new CannotWritePrintDimension();

            Assert.Equal(1, cannotWritePrintDimension.Column1);

            SetPageSizeToPrintDimension(cannotWritePrintDimension, new Size(500, 1000));

            Assert.Equal(500, cannotWritePrintDimension.Column1);
        }

        [Fact]
        public void SetHeightValue()
        {
            PrintDimension pd = new PrintDimension
                                {
                                    InternalPrintDefinition = new PrintDefinition()
                                };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            pd.SetHeightValue(PrintAppendixes.Summary, 5);
            Assert.Equal(5, pd.GetHeightFor(PrintAppendixes.Summary, 1, false));
            pd.SetHeightValue(PrintAppendixes.Summary, 6);
            Assert.Equal(6, pd.GetHeightFor(PrintAppendixes.Summary, 1, false));

            Assert.Throws<ArgumentOutOfRangeException>(()=> pd.SetHeightValue(PrintAppendixes.Summary, -1));
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList), 1, 100, 1, MemberType = typeof(RandomTest))]
        public void RecalculateHeightValueWhen(int initialHeight)
        {
            PrintDimension pd = new PrintDimension
                                {
                                    InternalPrintDefinition = new PrintDefinition()
                                };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            Mock<IPrintProcessor> printProcessor = new Mock<IPrintProcessor>();
            printProcessor.Setup(i => i.GetSummary()).Returns(() => new Grid() { Height = initialHeight });
            pd.PrintProcessor = printProcessor.Object;

            Assert.Equal(initialHeight, pd.GetHeightFor(PrintAppendixes.Summary, 1, false));
            pd.SetHeightValue(PrintAppendixes.Summary, 5);

            pd.RecalculateHeightValueWhen(() => false, PrintAppendixes.Summary);

            Assert.Equal(5, pd.GetHeightFor(PrintAppendixes.Summary, 1, false));

            pd.RecalculateHeightValueWhen(() => true, PrintAppendixes.Summary);

            Assert.Equal(initialHeight, pd.GetHeightFor(PrintAppendixes.Summary, 1, false));
        }

        [Fact]
        public void RecalculateHeightValueWhen1()
        {
            Assert.Throws<ArgumentNullException>(()=> new PrintDimension().RecalculateHeightValueWhen(null!, PrintAppendixes.None));
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
