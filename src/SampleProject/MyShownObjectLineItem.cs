// Copyright 2015 Michael Mairegger
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

namespace Mairegger.Printing.Sample
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;

    internal class MyShownObjectLineItem : IPrintContent
    {
        private readonly MyShownObject _myshownObject;
        private readonly CustomPrintDimension _printDimensions;

        public MyShownObjectLineItem(MyShownObject myShownObject, CustomPrintDimension printDimensions)
        {
            if (myShownObject == null)
            {
                throw new ArgumentNullException("myShownObject");
            }
            _myshownObject = myShownObject;
            _printDimensions = printDimensions;
        }

        public UIElement Content
        {
            get
            {
                StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };

                TextBlock tb1 = new TextBlock { Text = _myshownObject.LenghtOfText.ToString("#,##0") };

                LinearGradientBrush lgb = new LinearGradientBrush(Colors.Red, Colors.Red, new Point(0, 0), new Point(1, 0));
                Pen p = new Pen { Brush = lgb, Thickness = 1.5d };

                TextDecoration td = new TextDecoration(TextDecorationLocation.Baseline, p, 0,
                    TextDecorationUnit.FontRecommended,
                    TextDecorationUnit.FontRecommended) { Pen = p };

                TextBlock tb = new TextBlock { Text = _myshownObject.Text };
                tb.TextDecorations.Add(td);
                TextBlock tb2 = new TextBlock { Text = _myshownObject.NumberOfLines.ToString("#,##0") };
                tb.Width = _printDimensions.WidthColumn1;
                tb1.Width = _printDimensions.WidthColumn2;
                tb2.Width = _printDimensions.WidthColumn3;

                sp.Children.Add(tb);
                sp.Children.Add(tb1);
                sp.Children.Add(tb2);
                return sp;
            }
        }
    }
}