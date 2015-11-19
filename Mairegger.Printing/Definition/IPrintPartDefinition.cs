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
    /// <summary>
    ///     Defines methods for defining print parts.
    /// </summary>
    public interface IPrintPartDefinition
    {
        /// <summary>
        ///     Gets the <see cref="PrintAppendixes" /> for the print part that defines the attribute.
        /// </summary>
        PrintAppendixes PrintAppendixes { get; }

        /// <summary>
        ///     Gets whether the current <see cref="PrintAppendixes" /> is valid on the passed page.
        /// </summary>
        /// <param name="page"> The page number of the current page. </param>
        /// <returns> True if the PrintAppendix is valid on page with the corresponding number, false otherwise. </returns>
        PrintPartStatus GetPrintDefinition(int page);
    }
}