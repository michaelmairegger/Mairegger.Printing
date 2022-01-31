// Copyright 2017 Michael Mairegger
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Mairegger.Printing.Definition;

    internal class PageHelper
    {
        private const double Threshold = 0;

        public PageHelper(ItemsControl bodyGrid, Brush borderBrush, PageContent pageContent, PrintDimension printingDimension, double usedSpace)
        {
            BodyGrid = bodyGrid;
            BorderBrush = borderBrush;
            PageContent = pageContent;
            PrintingDimension = printingDimension;
            
            BodyGrid.Measure(new Size(double.MaxValue, double.MaxValue));
            RemoveRemainingSpace(usedSpace);
        }

        public ItemsControl BodyGrid { get; }

        public Brush BorderBrush { get; }

        public PageContent PageContent { get; }

        public PrintDimension PrintingDimension { get; }

        private double UsedSpace { get; set; }

        public double GetRemainingSpace(int pageCount, bool supposeLastPage)
        {
            return PrintingDimension.GetHeightForBodyGrid(pageCount, supposeLastPage) - UsedSpace - Threshold;
        }

        public bool HasSpace(double space, int pageCount, bool supposeLastPage)
        {
            var maxGridHeight = PrintingDimension.GetHeightForBodyGrid(pageCount, supposeLastPage);
            return maxGridHeight - UsedSpace - space >= 0;
        }

        public void RemoveRemainingSpace(double space)
        {
            UsedSpace += space + Threshold;
        }
    }
}