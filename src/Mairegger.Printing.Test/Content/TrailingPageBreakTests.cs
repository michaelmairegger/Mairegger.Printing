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
    public class TrailingPageBreakTests
    {
        [Test, STAThreadExecutor]
        public async Task PageBreak_AsLastItem_PrintsLastPageAppendixes()
        {
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new SummaryOnLastPagePrintProcessor(PrintContent.BlankLine(10), PrintContent.PageBreak())
            {
                PrintDialog = printDialog
            };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(1);
                await Assert.That(document.Pages[0].PlacedElements()).Contains(printProcessor.Summary);
            }
        }

        [Test, STAThreadExecutor]
        public async Task PageBreak_NotLastItem_StartsNewPage()
        {
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new SummaryOnLastPagePrintProcessor(PrintContent.BlankLine(10), PrintContent.PageBreak(), PrintContent.BlankLine(10))
            {
                PrintDialog = printDialog
            };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(2);
                await Assert.That(document.Pages[0].PlacedElements()).DoesNotContain(printProcessor.Summary);
                await Assert.That(document.Pages[1].PlacedElements()).Contains(printProcessor.Summary);
            }
        }

        [PrintOnPage(PrintAppendixes.Summary, PrintPartDefinitionAttribute.LastPage)]
        private sealed class SummaryOnLastPagePrintProcessor(params IPrintContent[] items) : PrintProcessor.PrintProcessor
        {
            public TextBlock Summary { get; } = new TextBlock { Text = "Summary" };

            public override UIElement GetSummary()
            {
                return Summary;
            }

            public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
            {
                reserveHeightOf = 0;
                borderBrush = Brushes.Transparent;
                return new UIElement();
            }

            public override IEnumerable<IPrintContent> ItemCollection()
            {
                return items;
            }

            protected override void PreparePrint()
            {
                base.PreparePrint();
                PrintDimension.PageSize = new Size(500, 500);
            }
        }
    }
}
