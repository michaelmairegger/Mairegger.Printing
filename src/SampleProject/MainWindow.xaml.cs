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
    using System.Collections.Generic;
    using System.Printing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Definition;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel _mcv = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _mcv;
        }

        private void AlternateRowCount1(object sender, RoutedEventArgs e)
        {
            var pa = PrintAppendixes();

            var printer = new Printer(pa, _mcv.CollectionToPrint);
            printer.IsAlternatingRowColor = true;
            printer.PageOrientation = PageOrientation.Landscape;
            printer.PreviewDocument();
        }

        private void AlternateRowCount2(object sender, RoutedEventArgs e)
        {
            var pa = PrintAppendixes();

            var printer = new Printer(pa, _mcv.CollectionToPrint);
            printer.IsAlternatingRowColor = true;
            printer.AlternatingRowColors = new List<SolidColorBrush> { Brushes.Red, Brushes.Orange, Brushes.Green };
            printer.PreviewDocument();
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            var pa = PrintAppendixes();

            new Printer(pa, _mcv.CollectionToPrint).PreviewDocument();
        }

        private void ButtonClick1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtBox.Text))
            {
                _mcv.CollectionToPrint.Add(new MyShownObject(TxtBox.Text));
            }
        }

        private void ButtonClick2(object sender, RoutedEventArgs e)
        {
            var pa = PrintAppendixes();

            new ExludeHeaderOnPageTwo(pa, _mcv.CollectionToPrint).PreviewDocument();
        }

        private void MenuItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem? mi = sender as MenuItem;
            if (mi == null)
            {
                return;
            }
            MyShownObject? mso = mi.Tag as MyShownObject;
            if (mso == null)
            {
                return;
            }
            _mcv.CollectionToPrint.Remove(mso);
        }

        private PrintAppendixes PrintAppendixes()
        {
            PrintAppendixes pa = Definition.PrintAppendixes.PageNumbers;
            if (_mcv.PrintParts.PrintFooter)
            {
                pa = pa | Definition.PrintAppendixes.Footer;
            }
            if (_mcv.PrintParts.PrintHeader)
            {
                pa = pa | Definition.PrintAppendixes.Header;
            }
            if (_mcv.PrintParts.PrintHeaderDesription)
            {
                pa = pa | Definition.PrintAppendixes.HeaderDescription;
            }
            if (_mcv.PrintParts.PrintSummary)
            {
                pa = pa | Definition.PrintAppendixes.Summary;
            }
            return pa;
        }
    }
}