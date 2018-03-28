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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;

    public class Printer : PrintProcessor
    {
        private readonly IEnumerable<MyShownObject> _collToPrint;
        private readonly CustomPrintDimension _printDimensions = new CustomPrintDimension();
        private readonly PrintAppendixes _printingAppendix;

        public Printer(PrintAppendixes printingAppendix, IEnumerable<MyShownObject> collToPrint)
        {
            _printingAppendix = printingAppendix;
            _collToPrint = collToPrint;

            FileName = "FileName";
            PrintDimension = _printDimensions;
        }

        public override UIElement GetFooter()
        {
            return new Label { Content = "This is the footer" };
        }

        public override UIElement GetHeader()
        {
            return new Label { Content = "This is the header" };
        }

        public override UIElement GetHeaderDescription()
        {
            return new Label { Content = "This the description of the header, left of the destinator" };
        }

        public override UIElement GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Summary" + Environment.NewLine);
            sb.Append("No elements: " + _collToPrint.Count() + Environment.NewLine);
            sb.Append("No of chars:" + _collToPrint.Sum(o => o.LenghtOfText) + Environment.NewLine);
            sb.Append("No of total lines: " + _collToPrint.Sum(o => o.NumberOfLines));

            Label l = new Label
                      {
                          Content = sb.ToString(),
                          Width = 600,
                          HorizontalContentAlignment = HorizontalAlignment.Right
                      };

            return l;
        }

        public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
        {
            Grid g = new Grid();
            borderBrush = Brushes.Gray;
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_printDimensions.WidthColumn1) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_printDimensions.WidthColumn2) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_printDimensions.WidthColumn3) });

            TextBox tb = new TextBox { Text = "Line Content" };
            TextBox t1 = new TextBox { Text = "Char Count" };
            TextBox t2 = new TextBox { Text = "Line Count" };

            Grid.SetColumn(tb, 0);
            Grid.SetColumn(t1, 1);
            Grid.SetColumn(t2, 2);

            g.Children.Add(tb);
            g.Children.Add(t1);
            g.Children.Add(t2);
            reserveHeightOf = 30;
            return g;
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            return _collToPrint.Select(obj => PrintContent.TextLine(obj.Text));
        }

        protected override void PreparePrint()
        {
            base.PreparePrint();
            PrintDimension.Margin = new Thickness(50);

            if (_printingAppendix.HasFlag(PrintAppendixes.Summary))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Summary));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Footer))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            }
            if (_printingAppendix.HasFlag(PrintAppendixes.Header))
            {
                PrintDefinition.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Header));
            }
        }
    }

    public static class UIElementExtensions
    {

        private static readonly Size MaxSize = new Size(double.MaxValue, double.MaxValue);

        public static Size ComputeDesiredSize(this UIElement uiElement)
        {
            uiElement.Measure(MaxSize);
            return uiElement.DesiredSize;
        }
    }
}