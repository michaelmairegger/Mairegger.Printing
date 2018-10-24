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

namespace Mairegger.Printing.PrintProcessor
{
    using System.Printing;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public interface IPrintDialog
    {
        bool CurrentPageEnabled { get; set; }

        int MaxPage { get; set; }

        int MinPage { get; set; }

        PageRange PageRange { get; set; }

        PageRangeSelection PageRangeSelection { get; set; }

        double PrintableAreaHeight { get; }

        double PrintableAreaWidth { get; }

        PrintQueue PrintQueue { get; set; }

        PrintTicket PrintTicket { get; set; }

        bool SelectedPagesEnabled { get; set; }

        bool UserPageRangeEnabled { get; set; }

        void PrintDocument(DocumentPaginator documentPaginator, string description);

        void PrintVisual(Visual visual, string description);

        bool? ShowDialog();
    }
}