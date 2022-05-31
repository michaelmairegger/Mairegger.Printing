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

namespace Mairegger.Printing.Tests.Content
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;

    [PrintOnAllPages(PrintAppendixes.All)]
    public class PrintEverything : PrintProcessor
    {
        public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
        {
            reserveHeightOf = 0;
            borderBrush = Brushes.Transparent;
            return new UIElement();
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            yield return PrintContent.TextLine("Test", 10);
        }

        public override PrintDocumentBackground GetBackground()
        {
            return new PrintDocumentBackground(new Grid());
        }

        public override UIElement GetFooter()
        {
            return new TextBlock();
        }

        public override UIElement GetHeader()
        {
            return new TextBlock();
        }

        public override UIElement GetHeaderDescription()
        {
            return new TextBlock();
        }

        public override UIElement GetSummary()
        {
            return new TextBlock();
        }

        protected override void PreparePrint()
        {
            base.PreparePrint();
            PrintDimension.PageSize = new Size(500, 100);
        }
    }
    
    [PrintOnPage(PrintAppendixes.PageNumbers, 2)]
    public class TestPrintProcessor : PrintProcessor
    {
        public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
        {
            reserveHeightOf = 0;
            borderBrush = Brushes.Transparent;
            return new UIElement();
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            yield return PrintContent.TextLine("Test", 10);
            yield return PrintContent.BlankLine(50);
            yield return PrintContent.PageBreak();
            yield return PrintContent.BlankLine(50);
            yield return PrintContent.BlankLine(90);
            yield return new DirectPrintContent { Content = new TextBlock { Background = Brushes.Red } };
            yield return PrintContent.BlankLine(100);
        }

        protected override void PreparePrint()
        {
            base.PreparePrint();
            PrintDimension.PageSize = new Size(500, 100);
        }

        public override IEnumerable<IDirectPrintContent> GetCustomPageContent(int pageNumber)
        {
            yield return new DirectPrintContent { Content = new TextBlock() };
        }
    }

    public class NoLineItemsTestPrintProcessor : PrintProcessor
    {
        public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
        {
            reserveHeightOf = 0;
            borderBrush = Brushes.Transparent;
            return new UIElement();
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            return Enumerable.Empty<IPrintContent>();
        }
    }
}