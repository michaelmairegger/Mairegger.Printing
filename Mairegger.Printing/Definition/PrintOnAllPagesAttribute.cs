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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Defines that the specific <see cref="PrintAppendixes" /> should be printed on all pages.
    /// </summary>
    /// <remarks>
    ///     <see cref="ExcludeFromPageAttribute" /> is prioritized against <see cref="PrintOnPageAttribute" />
    /// </remarks>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Inheriting should be allowed")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [DebuggerDisplay("Print on all pages: {PrintAppendixes}")]
    public sealed class PrintOnAllPagesAttribute : PrintOnPageAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PrintOnAllPagesAttribute" /> class regarding the specified range and
        ///     print parts
        /// </summary>
        /// <param name="printAppendixes">The print parts that are defined by this attribute.</param>
        public PrintOnAllPagesAttribute(PrintAppendixes printAppendixes)
            : base(printAppendixes, new Range<int>(FirstPage, LastPage))
        {
        }
    }
}