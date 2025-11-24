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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mairegger.Printing.Content
{
    file static class TextBlockAccessors
    {
        private static readonly Type TextBlockType = typeof(TextBlock);
        private const string TextBlock_GetLine = "GetLine";
        private const string LineMetrics = "MS.Internal.Text.LineMetrics, PresentationFramework";
        private const string LineMetrics_Lenght = "Length";
        private const string TextBlock_getLineCount = "get_LineCount";

        private static readonly MethodInfo _reflectionGetLine = TextBlockType.GetMethod(TextBlock_GetLine, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"BLABLA {TextBlock_GetLine}\" on type {TextBlockType}");
        private static readonly PropertyInfo _reflectionLineLength = Type.GetType(LineMetrics, true)!.GetProperty(LineMetrics_Lenght, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException($"Exception in reflecting property \"{LineMetrics_Lenght}\" on type \"{LineMetrics}\"");

        #if NET11_0_OR_GREATER
        #error Check if GetCurrentLineLength can be used with UnsafeAccessor. Up to now this is not possible since LineMetrics is a struct and UnsafeAccessor does not support structs.
        #endif
        public static int GetCurrentLineLength(TextBlock textBlock, int currentLine)
        {
            object line = _reflectionGetLine.Invoke(textBlock, [currentLine]) ?? throw new InvalidOperationException("Reflection exception");
            return _reflectionLineLength.GetValue(line) as int? ?? 0;
        }

        #if NETFRAMEWORK

        private static readonly MethodInfo _lineCountProperty = typeof(TextBlock).GetMethod(TextBlock_getLineCount, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new InvalidOperationException("Exception in reflecting LineCount on object of type TextBlock");
        public static int GetTextBoxLineCount(TextBlock textBlock)
        {
            return _lineCountProperty.Invoke(textBlock, null) as int? ?? 0;
        }

        #else

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = TextBlock_getLineCount)]
        public static extern int GetTextBoxLineCount(TextBlock textBlock);

        #endif
    }

    public class StringLineItem : IPageBreakAware
    {
        internal StringLineItem(string text, StringLineItemConfiguration configuration)
            : this(text, configuration.FontSize, configuration.HorizontalAlignment)
        {
            FontFamily = configuration.FontFamily;
        }

        internal StringLineItem(string text, double? fontSize = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
        {
            Text = text;
            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;

            Margin = new Thickness(0, 10, 0, 0);
            Padding = new Thickness(5, 0, 5, 0);
        }

        public Brush? Background { get; set; }

        public double? FontSize { get; set; }

        public FontWeight FontWeight { get; set; }

        public FontFamily? FontFamily { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; }

        public Thickness Margin { get; set; }

        public Thickness Padding { get; set; }

        public string Text { get; set; }

        UIElement IPrintContent.Content => ConstructContent(Text);

        public IEnumerable<UIElement> PageContents(double currentPageHeight, Size printablePageSize)
        {
            var lineHeight = GetLineHeight();
            var printablePageHeight = currentPageHeight;

            var textBlock = ConstructTextBlock(Text);
            textBlock.Measure(new Size(printablePageSize.Width - Margin.Left - Margin.Right - Padding.Left - Padding.Right, printablePageSize.Height));

            var totalLines = TextBlockAccessors.GetTextBoxLineCount(textBlock);

            var currentLine = 0;
            var currentLineOnPage = 0;
            var currentPosition = 0;

            var stringBuilder = new StringBuilder();

            while (currentLine < totalLines)
            {
                var linesThatHaveSpace = (int)(printablePageHeight / lineHeight * .95); // remove 5% of the page height
                var currentLineLength = TextBlockAccessors.GetCurrentLineLength(textBlock, currentLine);

                var substring = Text.Substring(currentPosition, currentLineLength);
                stringBuilder.Append(substring);

                currentPosition += currentLineLength;
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

        private Grid ConstructContent(string text)
        {
            var g = new Grid();
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
            if (FontFamily != null)
            {
                textBlock.FontFamily = FontFamily;
            }

            return textBlock;
        }

        private double GetLineHeight()
        {
            var textBlock = ConstructTextBlock(string.Empty);
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            return textBlock.DesiredSize.Height;
        }
    }
}
