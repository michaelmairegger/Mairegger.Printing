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

namespace Mairegger.Printing.Internal
{
    using System.Printing;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Mairegger.Printing.PrintProcessor;

    internal class PrintDialogWrapper : IPrintDialog
    {
        private readonly PrintDialog _printDialog = new PrintDialog();

        public bool CurrentPageEnabled
        {
            get => _printDialog.CurrentPageEnabled;
            set => _printDialog.CurrentPageEnabled = value;
        }

        public int MaxPage
        {
            get => (int)_printDialog.MaxPage;
            set => _printDialog.MaxPage = (uint)value;
        }

        public int MinPage
        {
            get => (int)_printDialog.MinPage;
            set => _printDialog.MinPage = (uint)value;
        }

        public PageRange PageRange
        {
            get => _printDialog.PageRange;
            set => _printDialog.PageRange = value;
        }

        public PageRangeSelection PageRangeSelection
        {
            get => _printDialog.PageRangeSelection;
            set => _printDialog.PageRangeSelection = value;
        }

        public double PrintableAreaHeight => _printDialog.PrintableAreaHeight;

        public double PrintableAreaWidth => _printDialog.PrintableAreaWidth;

        public PrintQueue PrintQueue
        {
            get => _printDialog.PrintQueue;
            set => _printDialog.PrintQueue = value;
        }

        public PrintTicket PrintTicket
        {
            get => _printDialog.PrintTicket;
            set => _printDialog.PrintTicket = value;
        }

        public bool SelectedPagesEnabled
        {
            get => _printDialog.SelectedPagesEnabled;
            set => _printDialog.SelectedPagesEnabled = value;
        }

        public bool UserPageRangeEnabled
        {
            get => _printDialog.UserPageRangeEnabled;
            set => _printDialog.UserPageRangeEnabled = value;
        }

        public void PrintDocument(DocumentPaginator documentPaginator, string description)
        {
            _printDialog.PrintDocument(documentPaginator, description);
        }

        public void PrintVisual(Visual visual, string description)
        {
            _printDialog.PrintVisual(visual, description);
        }

        public bool? ShowDialog()
        {
            return _printDialog.ShowDialog();
        }
    }
}