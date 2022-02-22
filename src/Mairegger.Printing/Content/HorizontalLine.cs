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
    using System.Windows.Shapes;

    public sealed class HorizontalLine : IPrintContent
    {
        private readonly double? _lineHeight;
        private readonly Thickness _margin;
        private readonly Brush? _lineColor;

        internal HorizontalLine(double? lineHeight = 1, Thickness margin = new Thickness(), Brush? lineColor = null)
        {
            _lineHeight = lineHeight;
            _margin = margin;
            _lineColor = lineColor;
        }

        public UIElement Content
        {
            get
            {
                Grid g = new Grid { Margin = _margin };

                g.Children.Add(new Rectangle
                               {
                                   Fill = _lineColor ?? Brushes.Black,
                                   Height = _lineHeight.GetValueOrDefault(1),
                                   HorizontalAlignment = HorizontalAlignment.Stretch,
                               });
                return g;
            }
        }
    }
}