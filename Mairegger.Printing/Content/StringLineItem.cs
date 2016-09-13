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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class StringLineItem : IPrintContent
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
            get
            {
                Grid g = new Grid();
                if (Background != null)
                {
                    g.Background = Background;
                }
                g.Margin = Margin;

                var textBlock = new TextBlock
                                {
                                    Text = Text,
                                    HorizontalAlignment = HorizontalAlignment,
                                    Padding = Padding,
                                    TextWrapping = TextWrapping.Wrap
                                };

                if (FontSize.HasValue)
                {
                    textBlock.FontSize = FontSize.Value;
                }
                textBlock.FontWeight = FontWeight;

                g.Children.Add(textBlock);
                return g;
            }
        }
    }
}