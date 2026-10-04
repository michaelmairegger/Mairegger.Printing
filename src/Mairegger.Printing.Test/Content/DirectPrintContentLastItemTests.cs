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
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class DirectPrintContentLastItemTests
    {
        [Test, STAThreadExecutor]
        public async Task DirectPrintContent_AsLastItem_PageIsAddedToDocument()
        {
            var directContent = new TextBlock { Text = "direct" };
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new ItemsPrintProcessor(
                PrintContent.BlankLine(10),
                new DirectPrintContent { Content = directContent })
            {
                PrintDialog = printDialog
            };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(1);
                await Assert.That(document.Pages[0].PlacedElements()).Contains(directContent);
            }
        }

        [Test, STAThreadExecutor]
        public async Task DirectPrintContent_AsOnlyItem_PageIsAddedToDocument()
        {
            var directContent = new TextBlock { Text = "direct" };
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new ItemsPrintProcessor(new DirectPrintContent { Content = directContent })
            {
                PrintDialog = printDialog
            };

            printProcessor.PrintDocument();

            var document = printDialog.Document!;
            using (Assert.Multiple())
            {
                await Assert.That(document.Pages.Count).IsEqualTo(1);
                await Assert.That(document.Pages[0].PlacedElements()).Contains(directContent);
            }
        }

        private sealed class ItemsPrintProcessor(params IPrintContent[] items) : PrintProcessor.PrintProcessor
        {
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
