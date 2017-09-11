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
    using System;
    using System.Collections.Generic;
    using System.Printing;
    using System.Windows;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;

    /// <summary>
    ///     An interface that provides methods for printing in WPF
    /// </summary>
    public interface IPrintProcessor : IPrintProcessorPrinter
    {
        /// <summary>
        ///     Gets or sets a list of all alternating background colors that are used in printing if you set
        ///     <see
        ///         cref="IsAlternatingRowColor" />
        ///     to true;
        /// </summary>
        IList<SolidColorBrush> AlternatingRowColors { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the different <see cref="PrintAppendixes" /> should be highlighted in order
        ///     to have a better overview of the print areas of the
        ///     <see
        ///         cref="PrintAppendixes" />
        /// </summary>
        bool ColorPrintPartsForDebug { get; set; }

        /// <summary>
        ///     Gets or sets the CurrentPage
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        ///     Gets the file name for the document that has to be printed
        /// </summary>
        /// <value> The file name suggestion. </value>
        string FileName { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the lines have alternating background colors. The default is false.
        /// </summary>
        bool IsAlternatingRowColor { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating how the page content is oriented for printing.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Calling code has attempted to set the property to a value that is not in the
        ///     <see cref="PageOrientation" />
        ///     enumeration.
        /// </exception>
        PageOrientation PageOrientation { get; set; }

        PrintDefinition PrintDefinition { get; }

        /// <summary>
        ///     Gets the <see cref="PrintDimension" /> to paginate the print.
        ///     <remarks>
        ///         This property cannot return null
        ///     </remarks>
        /// </summary>
        PrintDimension PrintDimension { get; }

        /// <summary>
        ///     Gets the background for the print.
        /// </summary>
        /// <returns>UIElement containing the content.</returns>
        PrintDocumentBackground GetBackgound();

        /// <summary>
        ///     Gets the footer.
        /// </summary>
        /// <returns>UIElement containing the content.</returns>
        UIElement GetFooter();

        /// <summary>
        ///     Gets the header.
        /// </summary>
        /// <returns>UIElement containing the content.</returns>
        UIElement GetHeader();

        /// <summary>
        ///     Gets the header description.
        /// </summary>
        /// <returns>UIElement containing the content.</returns>
        UIElement GetHeaderDescription();

        /// <summary>
        ///     Gets the summary.
        /// </summary>
        /// <returns>UIElement containing the content.</returns>
        UIElement GetSummary();

        /// <summary>
        ///     Gets the table.
        /// </summary>
        /// <param name="reserveHeightOf">
        ///     The height that is reserved as header.
        /// </param>
        /// <param name="borderBrush">
        ///     The border Brush.
        /// </param>
        /// <returns>
        ///     UIElement containing the content.
        /// </returns>
        UIElement GetTable(out double reserveHeightOf, out Brush borderBrush);

        /// <summary>
        ///     Returns a collection that is printed onto the print.
        /// </summary>
        /// <returns> A collection containing the print contents </returns>
        IEnumerable<IPrintContent> ItemCollection();


        /// <summary>
        /// Returns a list of <see cref="IDirectPrintContent"/> to allow custom positionizing element on the page.
        /// </summary>
        /// <param name="pageNumber">The current page number. The numbering starts with 1.</param>
        /// <returns></returns>
        IEnumerable<IDirectPrintContent> GetCustomPageContent(int pageNumber);
    }
}