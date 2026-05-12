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

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Mairegger.Printing.Definition;
using Mairegger.Printing.PrintProcessor;
using Mairegger.Printing.Tests.Content;
using PageRange = Mairegger.Printing.Definition.PageRange;
using System.Threading.Tasks;
using Bogus;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Definition
{
    public class PrintingDimensionsTests
    {
        [Test, STAThreadExecutor]
        public async Task GetHeightFor()
        {
            Mock<IPrintProcessor> mock = IPrintProcessor.Mock();
            mock.GetHeader().Returns(new Grid { Height = 1 });
            mock.GetFooter().Returns(new Grid { Height = 2 });
            mock.GetHeaderDescription().Returns(new Grid { Height = 3 });

            PrintDimension pd = new PrintDimension
            {
                PrintProcessor = mock.Object,
                InternalPrintDefinition = new PrintDefinition()
            };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Header | PrintAppendixes.Footer | PrintAppendixes.Summary));

            using (Assert.Multiple())
            {
                await Assert.That(pd.GetHeightFor(PrintAppendixes.Header, 1, false)).IsEqualTo(1);
                await Assert.That(pd.GetHeightFor(PrintAppendixes.Footer, 1, false)).IsEqualTo(2);
                await Assert.That(pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false)).IsEqualTo(0);
                Assert.Throws<ArgumentNullException>(() => pd.GetHeightFor(PrintAppendixes.Summary, 1, false));
            }

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.HeaderDescription));
            await Assert.That(pd.GetHeightFor(PrintAppendixes.HeaderDescription, 1, false)).IsEqualTo(3);
        }

        [Test]
        public async Task GetHeightForBody_IsTotalPageHeight_IfNoAdditionalPrintParts()
        {
            PrintDimension pd = new PrintDimension { InternalPrintDefinition = new PrintDefinition() };

            await Assert.That(pd.GetHeightForBodyGrid(1, false)).IsEqualTo(0);

            pd.PageSize = new Size(100, 300);

            await Assert.That(pd.GetHeightForBodyGrid(1, false)).IsEqualTo(300);

            pd.Margin = new Thickness(10);

            await Assert.That(pd.GetHeightForBodyGrid(1, false)).IsEqualTo(280);
        }

        [Test, STAThreadExecutor]
        public async Task GetHeightForBody_IsTotalPageHeightMinusPrintParts()
        {
            Mock<IPrintProcessor> mock = IPrintProcessor.Mock();
            mock.GetHeader().Returns(new Grid { Height = 10 });
            mock.GetFooter().Returns(new Grid { Height = 20 });
            mock.GetHeaderDescription().Returns(new Grid { Height = 30 });
            mock.GetSummary().Returns(new Grid { Height = 40 });
            mock.GetPageNumbers(Any<int>(), Any<int>()).Returns(new Grid { Height = 25 });

            PrintDimension pd = new PrintDimension
            {
                PrintProcessor = mock.Object,
                InternalPrintDefinition = new PrintDefinition()
            };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            pd.PageSize = new Size(100, 300);

            // 300 - Header - Footer - HeaderDescritpion - Summary - PageNumbers
            await Assert.That(pd.GetHeightForBodyGrid(1, false)).IsEqualTo(175);
        }

        [Test, STAThreadExecutor]
        public async Task GetRangeFor()
        {
            Mock<IPrintProcessor> mock = IPrintProcessor.Mock();
            mock.GetHeader().Returns(new Grid { Height = 10 });
            mock.GetFooter().Returns(new Grid { Height = 20 });
            mock.GetHeaderDescription().Returns(new Grid { Height = 30 });
            mock.GetSummary().Returns(new Grid { Height = 40 });
            mock.GetPageNumbers(Any<int>(), Any<int>()).Returns(new Grid { Height = 25 });

            PrintDimension pd = new PrintDimension
            {
                PrintProcessor = mock.Object,
                Margin = new Thickness(10),
                PageSize = new Size(100, 1000),
                InternalPrintDefinition = new PrintDefinition()
            };
            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            using (Assert.Multiple())
            {
                await Assert.That(pd.GetRangeFor(PrintAppendixes.Header, 1, false)).IsEqualTo(new PageRange(10, 20));
                await Assert.That(pd.GetRangeFor(PrintAppendixes.HeaderDescription, 1, false)).IsEqualTo(new PageRange(20, 50));
                await Assert.That(pd.GetRangeForBodyGrid(1, false)).IsEqualTo(new PageRange(50, 905));
                await Assert.That(pd.GetRangeFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(new PageRange(905, 945));
                await Assert.That(pd.GetRangeFor(PrintAppendixes.Footer, 1, false)).IsEqualTo(new PageRange(945, 965));
                await Assert.That(pd.GetRangeFor(PrintAppendixes.PageNumbers, 1, false)).IsEqualTo(new PageRange(965, 990));
            }
        }

        [Test]
        public void GetRangeFor_InvalidPrintAppendix()
        {
            PrintDimension pd = new PrintDimension();
            Assert.Throws<ArgumentException>(() => pd.GetRangeFor(PrintAppendixes.All, 1, false));
        }

        [Test]
        public async Task PageSize_Test()
        {
            Thickness margin = new Thickness(10, 20, 30, 40);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            await Assert.That(printingDimensions.PageSize).IsEqualTo(pageSize);
        }

        [Test]
        [Arguments(1, 4, 7, 10)]
        [Arguments(2, 5, 8, 11)]
        public async Task PrintablePageSize_Test(double left, double top, double right, double bottom)
        {
            Thickness margin = new Thickness(left, top, right, bottom);
            var printingDimensions = new PrintDimension(margin);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(printingDimensions, pageSize);

            Size expected = new Size(pageSize.Width - margin.Left - margin.Right, pageSize.Height - margin.Top - margin.Bottom);

            await Assert.That(printingDimensions.PrintablePageSize).IsEqualTo(expected);
        }

        [Test]
        public async Task PrintDimensionTest()
        {
            var thickness = new Thickness(10, 10, 10, 10);
            TestPrintDimension tpd = new TestPrintDimension(thickness);

            Size pageSize = new Size(500, 1000);
            SetPageSizeToPrintDimension(tpd, pageSize);

            double totalWidth = pageSize.Width - thickness.Left - thickness.Right;

            await Assert.That(tpd.PrintablePageSize.Width).IsEqualTo(totalWidth);

            double pieces = 5; // sum of TestPrintDimensions
            double widthPerPiece = (totalWidth - 100) / pieces;

            using (Assert.Multiple())
            {
                await Assert.That(tpd.Column1).IsEqualTo(1 * widthPerPiece);
                await Assert.That(tpd.Column2).IsEqualTo(3 * widthPerPiece);
                await Assert.That(tpd.Column3).IsEqualTo(100);
                await Assert.That(tpd.Column4).IsEqualTo(1 * widthPerPiece);
            }
        }

        [Test]
        public void SetColumnDimensionToPropertyWithWrongType_ThrowsException()
        {
            var invalidPrintDimension = new InvalidPrintDimension();
            invalidPrintDimension.Column1 = new InvalidPrintDimension().Column1;
            Assert.Throws<InvalidOperationException>(() => SetPageSizeToPrintDimension(invalidPrintDimension, new Size(500, 1000)));
        }

        [Test, STAThreadExecutor]
        public async Task SetColumnDimensionToReadOnlyProperty()
        {
            var cannotWritePrintDimension = new CannotWritePrintDimension();

            await Assert.That(cannotWritePrintDimension.Column1).IsEqualTo(1);

            SetPageSizeToPrintDimension(cannotWritePrintDimension, new Size(500, 1000));

            await Assert.That(cannotWritePrintDimension.Column1).IsEqualTo(500);
        }

        [Test]
        public async Task SetHeightValue()
        {
            PrintDimension pd = new PrintDimension
            {
                InternalPrintDefinition = new PrintDefinition()
            };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            pd.SetHeightValue(PrintAppendixes.Summary, 5);
            await Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(5);
            pd.SetHeightValue(PrintAppendixes.Summary, 6);
            await Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(6);

            Assert.Throws<ArgumentOutOfRangeException>(() => pd.SetHeightValue(PrintAppendixes.Summary, -1));
        }

        private static readonly Faker s_faker = new();
        public static IEnumerable<int> GetRandomList()
        {
            for (int i = 1; i <= 1; i++)
            {
                yield return s_faker.Random.Int(100);
            }
        }

        [Test, STAThreadExecutor]
        [MethodDataSource(nameof(GetRandomList))]
        public async Task RecalculateHeightValueWhen(int initialHeight)
        {
            PrintDimension pd = new PrintDimension
            {
                InternalPrintDefinition = new PrintDefinition()
            };

            pd.InternalPrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.All));

            Mock<IPrintProcessor> printProcessor = IPrintProcessor.Mock();
            printProcessor.GetSummary().Returns(() => new Grid() { Height = initialHeight });
            pd.PrintProcessor = printProcessor.Object;

            await Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(initialHeight);
            pd.SetHeightValue(PrintAppendixes.Summary, 5);

            pd.RecalculateHeightValueWhen(() => false, PrintAppendixes.Summary);

            await Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(5);

            pd.RecalculateHeightValueWhen(() => true, PrintAppendixes.Summary);

            await Assert.That(pd.GetHeightFor(PrintAppendixes.Summary, 1, false)).IsEqualTo(initialHeight);
        }

        [Test]
        public void RecalculateHeightValueWhen1()
        {
            Assert.Throws<ArgumentNullException>(() => new PrintDimension().RecalculateHeightValueWhen(null!, PrintAppendixes.None));
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

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
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
