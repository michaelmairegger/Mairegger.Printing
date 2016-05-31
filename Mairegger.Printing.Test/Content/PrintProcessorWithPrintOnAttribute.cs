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

namespace Mairegger.Printing.Test.Content
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Moq;

    [PrintOnAllPages(PrintAppendixes.Footer)]
    public class PrintProcessorWithPrintOnAttribute : TestPrintProcessor
    {
        private readonly IList<IPrintContent> _retrievedContent;

        public PrintProcessorWithPrintOnAttribute()
        {
        }

        public PrintProcessorWithPrintOnAttribute(IList<IPrintContent> retrievedContent)
        {
            _retrievedContent = retrievedContent;
        }

        public int ItemCount { get; set; }

        public override UIElement GetFooter()
        {
            return new Grid();
        }

        public override IEnumerable<IPrintContent> ItemCollection()
        {
            for (int i = 0; i < ItemCount; i++)
            {
                var itemCollection = new Mock<IPrintContent>();
                itemCollection.SetupGet(x => x.Content).Returns(new Grid());

                _retrievedContent?.Add(itemCollection.Object);

                yield return itemCollection.Object;
            }
        }
    }
}