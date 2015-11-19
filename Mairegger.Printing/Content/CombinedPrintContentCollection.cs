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

namespace Mairegger.Printing.Content
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     Represents a list of <see cref="IPrintContent" /> for combining more than one <see cref="IPrintContent" /> into one
    ///     in order to ensure that each item of the collection gets printed without page break.
    /// </summary>
    public class CombinedPrintContentCollection : List<IPrintContent>, IPrintContent
    {
        public CombinedPrintContentCollection()
        {
        }

        public CombinedPrintContentCollection(params IPrintContent[] content)
        {
            if (content != null)
            {
                AddRange(content);
            }
        }

        public UIElement Content
        {
            get
            {
                var contentPanel = new StackPanel();

                foreach (var lineItem in this)
                {
                    contentPanel.Children.Add(lineItem.Content);
                }

                return contentPanel;
            }
        }
    }
}