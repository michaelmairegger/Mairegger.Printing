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

namespace Mairegger.Printing.Definition
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Defines on which pages the specific <see cref="PrintAppendixes" /> should not be printed.
    /// </summary>
    /// <remarks>
    ///     <see cref="ExcludeFromPageAttribute" /> is prioritized against <see cref="PrintOnPageAttribute" />
    /// </remarks>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Inheriting should be allowed")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [DebuggerDisplay("Exclude on page: {PrintAppendixes}")]
    public class ExcludeFromPageAttribute : PrintPartDefinitionAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExcludeFromPageAttribute" /> class regarding the specified range and
        ///     print parts
        /// </summary>
        /// <param name="printAppendixes">The print parts that are defined by this attribute.</param>
        /// <param name="pages">The page range where the print parts are not defined</param>
        public ExcludeFromPageAttribute(PrintAppendixes printAppendixes, Range<int> pages)
            : base(printAppendixes, pages)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExcludeFromPageAttribute" /> class regarding the specified single page
        ///     and print parts
        /// </summary>
        /// <param name="printAppendixes">The print parts that are defined by this attribute.</param>
        /// <param name="singlePage">The page where the print parts is not defined.</param>
        public ExcludeFromPageAttribute(PrintAppendixes printAppendixes, int singlePage)
            : base(printAppendixes, singlePage)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExcludeFromPageAttribute" /> class regarding the specified pages and
        ///     print parts
        /// </summary>
        /// <param name="printAppendixes">The print parts that are defined by this attribute.</param>
        /// <param name="definedPages">The pages where the print parts are not defined.</param>
        public ExcludeFromPageAttribute(PrintAppendixes printAppendixes, params int[] definedPages)
            : base(printAppendixes, definedPages)
        {
        }

        public sealed override PrintPartStatus GetPrintDefinition(int page)
        {
            if (IsPageInAnyRange(page))
            {
                return PrintPartStatus.Exclude;
            }

            return PrintPartStatus.NotDefined;
        }
    }
}