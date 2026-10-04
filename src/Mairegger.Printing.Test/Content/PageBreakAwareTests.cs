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
    public class PageBreakAwareTests
    {
        [Test, STAThreadExecutor]
        public async Task PageContents_FollowingPageSize_UsesLayoutOfFollowingPage()
        {
            var pageBreakAware = new RecordingPageBreakAware([new Grid { Height = 10 }]);
            var printProcessor = new HeaderExcludedOnFirstPagePrintProcessor(pageBreakAware, PrintContent.BlankLine(10))
            {
                PrintDialog = new CapturingPrintDialog()
            };

            printProcessor.PrintDocument();

            // page size - header (not printed on page 1, but on the following pages) - reserved space of the table
            await Assert.That(pageBreakAware.PrintablePageSize.Height).IsEqualTo(500 - 100 - 20);
        }

        [Test, STAThreadExecutor]
        [Arguments(true)]
        [Arguments(false)]
        public async Task PageContents_Empty_DoesNotThrow(bool isLastItem)
        {
            var printDialog = new CapturingPrintDialog();
            IPrintContent[] items = isLastItem
                ? [PrintContent.BlankLine(10), new RecordingPageBreakAware([])]
                : [new RecordingPageBreakAware([]), PrintContent.BlankLine(10)];
            var printProcessor = new HeaderExcludedOnFirstPagePrintProcessor(items) { PrintDialog = printDialog };

            printProcessor.PrintDocument();

            await Assert.That(printDialog.Document!.Pages.Count).IsEqualTo(1);
        }

        private sealed class RecordingPageBreakAware(IEnumerable<UIElement> pageContents) : IPageBreakAware
        {
            public UIElement Content => throw new InvalidOperationException();

            public Size PrintablePageSize { get; private set; }

            public IEnumerable<UIElement> PageContents(double currentPageHeight, Size printablePageSize)
            {
                PrintablePageSize = printablePageSize;
                return pageContents;
            }
        }

        [PrintOnAllPages(PrintAppendixes.Header)]
        [ExcludeFromPage(PrintAppendixes.Header, 1)]
        private sealed class HeaderExcludedOnFirstPagePrintProcessor(params IPrintContent[] items) : PrintProcessor.PrintProcessor
        {
            public override UIElement GetHeader()
            {
                return new Grid { Height = 100 };
            }

            public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
            {
                reserveHeightOf = 20;
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
