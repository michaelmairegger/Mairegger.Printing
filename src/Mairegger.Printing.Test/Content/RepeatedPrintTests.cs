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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mairegger.Printing.Content;
using Mairegger.Printing.Definition;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class RepeatedPrintTests
    {
        [Test, STAThreadExecutor]
        public async Task PrintTwice_CurrentPageStartsFromScratch()
        {
            var printProcessor = new HeaderPrintProcessor { PrintDialog = new CapturingPrintDialog() };

            printProcessor.PrintDocument();
            var currentPageAfterFirstPrint = printProcessor.CurrentPage;

            printProcessor.PrintDocument();

            await Assert.That(printProcessor.CurrentPage).IsEqualTo(currentPageAfterFirstPrint);
        }

        [Test, STAThreadExecutor]
        public async Task PrintTwice_HeaderHeightIsMeasuredAgain()
        {
            var printProcessor = new HeaderPrintProcessor { PrintDialog = new CapturingPrintDialog() };

            printProcessor.PrintDocument();
            var heightAfterFirstPrint = printProcessor.PrintDimension.GetHeightFor(PrintAppendixes.Header, 1, false);

            printProcessor.HeaderLines = 5;
            printProcessor.PrintDocument();
            var heightAfterSecondPrint = printProcessor.PrintDimension.GetHeightFor(PrintAppendixes.Header, 1, false);

            await Assert.That(heightAfterSecondPrint).IsGreaterThan(heightAfterFirstPrint);
        }

        [Test, STAThreadExecutor]
        public async Task PrintTwice_HeightSetBySetHeightValueIsKept()
        {
            var printProcessor = new HeaderPrintProcessor { PrintDialog = new CapturingPrintDialog() };
            printProcessor.PrintDimension.SetHeightValue(PrintAppendixes.Header, 42);

            printProcessor.PrintDocument();
            printProcessor.PrintDocument();

            await Assert.That(printProcessor.PrintDimension.GetHeightFor(PrintAppendixes.Header, 1, false)).IsEqualTo(42);
        }

        [PrintOnAllPages(PrintAppendixes.Header)]
        private sealed class HeaderPrintProcessor : PrintProcessor.PrintProcessor
        {
            public int HeaderLines { get; set; } = 1;

            public override UIElement GetHeader()
            {
                return new TextBlock { Text = string.Join("\n", Enumerable.Range(1, HeaderLines).Select(i => $"Header {i}")) };
            }

            public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
            {
                reserveHeightOf = 0;
                borderBrush = Brushes.Transparent;
                return new UIElement();
            }

            public override IEnumerable<IPrintContent> ItemCollection()
            {
                yield return PrintContent.BlankLine(10);
                yield return PrintContent.PageBreak();
                yield return PrintContent.BlankLine(10);
            }

            protected override void PreparePrint()
            {
                base.PreparePrint();
                PrintDimension.PageSize = new Size(500, 500);
            }
        }
    }
}
