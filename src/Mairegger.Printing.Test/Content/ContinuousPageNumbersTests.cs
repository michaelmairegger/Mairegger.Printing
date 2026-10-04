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
using Mairegger.Printing.PrintProcessor;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class ContinuousPageNumbersTests
    {
        [Test, STAThreadExecutor]
        public async Task IndividualPageNumbers_False_PageNumbersContinueOverAllPrintProcessors()
        {
            var printDialog = new CapturingPrintDialog();
            var collection = new PrintProcessorCollection(
                [new TwoPagesPrintProcessor { PrintDialog = printDialog }, new TwoPagesPrintProcessor()])
            {
                IndividualPageNumbers = false
            };

            collection.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(4);
                await Assert.That(document.Pages[0].PlacedTexts()).Contains("1 | 4");
                await Assert.That(document.Pages[1].PlacedTexts()).Contains("2 | 4");
                await Assert.That(document.Pages[2].PlacedTexts()).Contains("3 | 4");
                await Assert.That(document.Pages[3].PlacedTexts()).Contains("4 | 4");
            }
        }

        [Test, STAThreadExecutor]
        public async Task IndividualPageNumbers_True_PageNumbersRestartForEachPrintProcessor()
        {
            var printDialog = new CapturingPrintDialog();
            var collection = new PrintProcessorCollection(
                [new TwoPagesPrintProcessor { PrintDialog = printDialog }, new TwoPagesPrintProcessor()])
            {
                IndividualPageNumbers = true
            };

            collection.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(4);
                await Assert.That(document.Pages[0].PlacedTexts()).Contains("1 | 2");
                await Assert.That(document.Pages[1].PlacedTexts()).Contains("2 | 2");
                await Assert.That(document.Pages[2].PlacedTexts()).Contains("1 | 2");
                await Assert.That(document.Pages[3].PlacedTexts()).Contains("2 | 2");
            }
        }

        [PrintOnAllPages(PrintAppendixes.PageNumbers)]
        private sealed class TwoPagesPrintProcessor : PrintProcessor.PrintProcessor
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
