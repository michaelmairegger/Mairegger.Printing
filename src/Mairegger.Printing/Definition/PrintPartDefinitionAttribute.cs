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

namespace Mairegger.Printing.Definition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public abstract class PrintPartDefinitionAttribute : Attribute, IPrintPartDefinition
    {
        /// <summary>
        ///     Defines the first page.
        /// </summary>
        public const int FirstPage = 1;

        /// <summary>
        ///     Defines the last page.
        /// </summary>
        public const int LastPage = int.MaxValue;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly List<PageRange> _validPrintRanges = [];

        protected PrintPartDefinitionAttribute(PrintAppendixes printAppendixes, PageRange pages)
            : this(printAppendixes)
        {
            AddRange(pages);
        }

        protected PrintPartDefinitionAttribute(PrintAppendixes printAppendixes, int singlePage)
            : this(printAppendixes, singlePage, singlePage)
        {
        }

        protected PrintPartDefinitionAttribute(PrintAppendixes printAppendixes, params int[] definedPages)
        {
            PrintAppendixes = printAppendixes;

            var ranges = definedPages.Select(e => PageRange.FromPoint(e)).ToArray();
            AddRange(ranges);
        }

        private PrintPartDefinitionAttribute(PrintAppendixes printAppendixes, int firstPage, int lastPage)
            : this(printAppendixes, new PageRange(firstPage, lastPage))
        {
        }

        public PrintAppendixes PrintAppendixes { get; }

        public virtual PrintPartStatus GetPrintDefinition(int page)
        {
            return IsPageInAnyRange(page) ? PrintPartStatus.Include : PrintPartStatus.NotDefined;
        }

        protected bool IsPageInAnyRange(int page)
        {
            return _validPrintRanges.Any(r => r.IsInRange(page));
        }

        private void AddRange(params PageRange[] pages)
        {
            _validPrintRanges.AddRange(pages);
        }
    }
}
