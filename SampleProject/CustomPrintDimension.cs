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
    using Mairegger.Printing.Definition;

    public class CustomPrintDimension : PrintDimension
    {
        public CustomPrintDimension()
        {
            UseRelativeColumnPosition = true;
        }

        [ColumnDimension(.50)]
        public double WidthColumn1 { get; set; }

        [ColumnDimension(.15)]
        public double WidthColumn2 { get; set; }

        [ColumnDimension(.35)]
        public double WidthColumn3 { get; set; }
    }
}