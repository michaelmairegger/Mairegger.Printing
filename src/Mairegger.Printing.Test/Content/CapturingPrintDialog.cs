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

using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Mairegger.Printing.PrintProcessor;

namespace Mairegger.Printing.Tests.Content
{
    /// <summary>
    ///     An <see cref="IPrintDialog" /> that does not print but keeps the printed <see cref="FixedDocument" /> for inspection.
    /// </summary>
    internal sealed class CapturingPrintDialog : IPrintDialog
    {
        public FixedDocument? Document { get; private set; }

        public bool CurrentPageEnabled { get; set; }

        public int MaxPage { get; set; }

        public int MinPage { get; set; }

        public System.Windows.Controls.PageRange PageRange { get; set; }

        public PageRangeSelection PageRangeSelection { get; set; }

        public double PrintableAreaHeight { get; set; } = 500;

        public double PrintableAreaWidth { get; set; } = 500;

        public PrintQueue PrintQueue { get; set; } = null!;

        public PrintTicket PrintTicket { get; set; } = new PrintTicket();

        public bool SelectedPagesEnabled { get; set; }

        public bool UserPageRangeEnabled { get; set; }

        public void PrintDocument(DocumentPaginator documentPaginator, string description)
        {
            Document = (FixedDocument)documentPaginator.Source;
        }

        public void PrintVisual(Visual visual, string description)
        {
        }

        public bool? ShowDialog()
        {
            return true;
        }
    }

    internal static class PageContentExtensions
    {
        /// <summary>
        ///     Returns the elements that are placed directly on the page, unwrapping the <see cref="ContentControl" /> used for
        ///     header, footer, summary and page numbers.
        /// </summary>
        public static IList<UIElement> PlacedElements(this PageContent pageContent)
        {
            return pageContent.Child.Children
                              .OfType<UIElement>()
                              .Select(e => e is ContentControl { Content: UIElement content } ? content : e)
                              .ToList();
        }

        /// <summary>
        ///     Returns the content of the line items of the body grid of the page.
        /// </summary>
        public static IList<object> BodyItems(this PageContent pageContent)
        {
            return pageContent.Child.Children
                              .OfType<Grid>()
                              .SelectMany(g => g.Children.OfType<ItemsControl>())
                              .SelectMany(i => i.Items.OfType<ContentControl>())
                              .Select(c => c.Content)
                              .ToList();
        }

        /// <summary>
        ///     Returns the texts of all <see cref="TextBlock" />s that are placed directly on the page.
        /// </summary>
        public static IList<string> PlacedTexts(this PageContent pageContent)
        {
            return pageContent.PlacedElements().OfType<TextBlock>().Select(t => t.Text).ToList();
        }
    }
}
