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
using System.Windows.Media;
using Mairegger.Printing.Content;
using Mairegger.Printing.Definition;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PageNumbersTests
    {
        [Test, STAThreadExecutor]
        public async Task PageNumbers_PrintOnLastPage_PrintedOnLastPageOnly()
        {
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new PageNumbersOnLastPagePrintProcessor { PrintDialog = printDialog };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(2);
                await Assert.That(document.Pages[0].PlacedTexts()).DoesNotContain("1 | 2");
                await Assert.That(document.Pages[1].PlacedTexts()).Contains("2 | 2");
            }
        }

        [Test, STAThreadExecutor]
        public async Task PageNumbers_ExcludedFromLastPage_NotPrintedOnLastPage()
        {
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new PageNumbersExcludedFromLastPagePrintProcessor { PrintDialog = printDialog };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(2);
                await Assert.That(document.Pages[0].PlacedTexts()).Contains("1 | 2");
                await Assert.That(document.Pages[1].PlacedTexts()).DoesNotContain("2 | 2");
            }
        }

        [PrintOnPage(PrintAppendixes.PageNumbers, PrintPartDefinitionAttribute.LastPage)]
        private sealed class PageNumbersOnLastPagePrintProcessor : TwoPagesPrintProcessor;

        [PrintOnAllPages(PrintAppendixes.PageNumbers)]
        [ExcludeFromPage(PrintAppendixes.PageNumbers, PrintPartDefinitionAttribute.LastPage)]
        private sealed class PageNumbersExcludedFromLastPagePrintProcessor : TwoPagesPrintProcessor;

        private abstract class TwoPagesPrintProcessor : PrintProcessor.PrintProcessor
        {
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
