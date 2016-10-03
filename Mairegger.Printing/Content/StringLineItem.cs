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

namespace Mairegger.Printing.Content
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public interface IPageBreakAware
    {
        IEnumerable<UIElement> PageContents(double currentPageHeight, Size printablePageSize);
    }

    public class StringLineItem : IPrintContent, IPageBreakAware
    {
        internal StringLineItem(string text, double? fontSize = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        {
            Text = text;
            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;

            Margin = new Thickness(0, 10, 0, 0);
            Padding = new Thickness(5, 0, 5, 0);
        }

        public Brush Background { get; set; }

        public string Text { get; set; }

        public double? FontSize { get; set; }

        public FontWeight FontWeight { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public Thickness Margin { get; set; }

        public Thickness Padding { get; set; }

        UIElement IPrintContent.Content
        {
            get { return ConstructContent(Text); }
        }

        private UIElement ConstructContent(string text)
        {
           Grid g = new Grid();
            if (Background != null)
            {
                g.Background = Background;
            }
            g.Margin = Margin;

            var constructTextBlock = ConstructTextBlock(text);
            constructTextBlock.Padding = Padding;
            g.Children.Add(constructTextBlock);
            return g;
        }

        private TextBlock ConstructTextBlock(string text)
        {
            var textBlock = new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment,
                TextWrapping = TextWrapping.Wrap
            };

            if (FontSize.HasValue)
            {
                textBlock.FontSize = FontSize.Value;
            }
            if (Background != null)
            {
                textBlock.Background = Background;
            }
            textBlock.FontWeight = FontWeight;

            return textBlock;
        }

        public IEnumerable<UIElement> PageContents(double currentPageHeight, Size printablePageSize)
        {
            var rLineCount = typeof(TextBlock).GetProperty("LineCount", BindingFlags.Instance | BindingFlags.NonPublic);
            var rGetLine = typeof(TextBlock).GetMethod("GetLine", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo rLineLength = null;


            var lineHeight = GetLineHeight();
            var printablePageHeight = currentPageHeight;

            var textBlock = ConstructTextBlock(Text);
            textBlock.Measure(new Size(printablePageSize.Width - Margin.Left - Margin.Right - Padding.Left - Padding.Right, printablePageSize.Height));


            var totalLines = (int)rLineCount.GetValue(textBlock);

            var currentLine = 0;
            var currentLineOnPage = 0;
            var currentPosition = 0;

            var stringBuilder = new StringBuilder();

            while (currentLine < totalLines)
            {
                int linesThatHaveSpace = (int)(printablePageHeight / lineHeight * .95); //remove 5% of the page height

                var line = rGetLine.Invoke(textBlock, new object[] { currentLine });

                if (rLineLength == null)
                {
                    rLineLength = line.GetType().GetProperty("Length", BindingFlags.Instance | BindingFlags.NonPublic);
                }

                var lenght = (int)rLineLength.GetValue(line);

                var substring = Text.Substring(currentPosition, lenght);
                stringBuilder.Append(substring);


                currentPosition += lenght;
                currentLineOnPage++;
                currentLine++;

                if (currentLineOnPage == linesThatHaveSpace || currentLine == totalLines)
                {
                    yield return ConstructContent(stringBuilder.ToString());
                    stringBuilder.Clear();

                    currentLineOnPage = 0;

                    printablePageHeight = printablePageSize.Height;
                }
            }
        }

        private double GetLineHeight()
        {
            var textBlock = ConstructTextBlock(string.Empty);
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            return textBlock.DesiredSize.Height;
        }
    }
}