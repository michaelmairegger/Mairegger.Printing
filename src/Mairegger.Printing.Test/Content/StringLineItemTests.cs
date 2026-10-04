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
using Mairegger.Printing.Content;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class StringLineItemTests
    {
        private static readonly string MultiLineText = string.Join("\n", Enumerable.Range(1, 50).Select(i => $"Line {i}"));

        [Test, STAThreadExecutor]
        [Arguments(0d)]
        [Arguments(5d)]
        [Arguments(-10d)]
        public async Task PageContents_NoLineFitsOnCurrentPage_TextIsStillSplitOnFollowingPages(double currentPageHeight)
        {
            var stringLineItem = PrintContent.TextLine(MultiLineText, 12);

            var pageContents = stringLineItem.PageContents(currentPageHeight, new Size(500, 100)).ToList();

            using (Assert.Multiple())
            {
                await Assert.That(pageContents.Count).IsGreaterThan(1);
                await Assert.That(string.Concat(pageContents.Select(GetText))).IsEqualTo(MultiLineText);
            }
        }

        [Test, STAThreadExecutor]
        public async Task PageContents_LinesFitOnCurrentPage_TextIsSplit()
        {
            var stringLineItem = PrintContent.TextLine(MultiLineText, 12);

            var pageContents = stringLineItem.PageContents(100, new Size(500, 100)).ToList();

            using (Assert.Multiple())
            {
                await Assert.That(pageContents.Count).IsGreaterThan(1);
                await Assert.That(string.Concat(pageContents.Select(GetText))).IsEqualTo(MultiLineText);
            }
        }

        private static string GetText(UIElement element)
        {
            return ((Grid)element).Children.OfType<TextBlock>().Single().Text;
        }
    }
}
