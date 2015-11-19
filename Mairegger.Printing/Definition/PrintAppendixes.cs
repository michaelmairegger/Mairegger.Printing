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

    /// <summary>
    ///     A Enumeration that contains the field that are printed
    /// </summary>
    [Flags]
    public enum PrintAppendixes
    {
        /// <summary>
        ///     No additional field is printed
        /// </summary>
        None = 0x0,
        /// <summary>
        ///     The header field is printed
        /// </summary>
        Header = 0x1,
        /// <summary>
        ///     The footer field is printed
        /// </summary>
        Footer = 0x2,
        /// <summary>
        ///     The summary field is printed
        /// </summary>
        Summary = 0x4,
        /// <summary>
        ///     The background field is printed
        /// </summary>
        Background = 0x8,
        /// <summary>
        ///     The header description field is printed
        /// </summary>
        HeaderDescription = 0x10,
        /// <summary>
        ///     The page numbers are printed on every page
        /// </summary>
        PageNumbers = 0x20,
        /// <summary>
        ///     The footer and the summary field are printed
        /// </summary>
        FooterSummary = Footer | Summary,
        /// <summary>
        ///     The recipient summary and the header field are printed
        /// </summary>
        SummaryHeader = Summary | Header,
        /// <summary>
        ///     All additional fields are printed
        /// </summary>
        AllWithoutBackground = All & ~Background,
        /// <summary>
        ///     All additional fields are printed
        /// </summary>
        /// <remarks>
        ///     The Background is not flagged
        /// </remarks>
        All = SummaryHeader | Footer | Summary | Header | HeaderDescription | PageNumbers | Background
    }
}