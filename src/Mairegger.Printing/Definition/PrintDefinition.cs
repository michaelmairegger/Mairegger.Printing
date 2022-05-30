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
    using Mairegger.Printing.Internal;

    public class PrintDefinition
    {
        private readonly PrintOnAttributeHelper _printOnAttributeHelper = new PrintOnAttributeHelper();

        public bool IsDefined(PrintAppendixes value)
        {
            return _printOnAttributeHelper.IsDefined(value);
        }

        public bool IsToPrint(PrintAppendixes pa, int pageNumber, bool isLastPage)
        {
            return GetPrintPrintDefinitionForPage(pa, pageNumber, isLastPage) == PrintPartStatus.Include;
        }

        public void SetPrintAttribute(IPrintPartDefinition printPartDefinition)
        {
            _printOnAttributeHelper.AddAttribute(printPartDefinition);
        }

        /// <summary>
        ///     Checks whether the passed <see cref="PrintAppendixes" /> value is set as valid
        ///     <see
        ///         cref="PrintAppendixes" />
        ///     for printing
        /// </summary>
        /// <returns>
        ///     True if the passed <see cref="PrintAppendixes" /> is set, false otherwise.
        /// </returns>
        private PrintPartStatus GetPrintPrintDefinitionForPage(PrintAppendixes pa, int pageNumber)
        {
            if (!Enum.IsDefined(typeof(PrintAppendixes), pa))
            {
                throw new ArgumentException($"Enum value {pa} is not defined on enum {typeof(PrintAppendixes).FullName}", nameof(pa));
            }
            return _printOnAttributeHelper.IsPrintOnPage(pa, pageNumber);
        }

        [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags", Justification = "This is OK, because PrintPartStatus should not have a flag-attribute and the line with & should be a bit-flag")]
        private PrintPartStatus GetPrintPrintDefinitionForPage(PrintAppendixes pa, int pageNumber, bool isLastPage)
        {
            PrintPartStatus result;

            if (isLastPage && (pageNumber == PrintPartDefinitionAttribute.FirstPage))
            {
                result = GetPrintPrintDefinitionForPage(pa, pageNumber) & IsPrintPartToPrintOnLastPage(pa, pageNumber);
            }
            else if (isLastPage)
            {
                result = IsPrintPartToPrintOnLastPage(pa, pageNumber);
            }
            else
            {
                result = GetPrintPrintDefinitionForPage(pa, pageNumber);
            }

            Debug.WriteLine($"{pa,-20}| {result,-12}| {(isLastPage ? "last page" : $"page {pageNumber}")}");

            return result;
        }

        private PrintPartStatus IsPrintPartToPrintOnLastPage(PrintAppendixes pa, int page)
        {
            var printOnLastPage = GetPrintPrintDefinitionForPage(pa, PrintPartDefinitionAttribute.LastPage);

            if (printOnLastPage == PrintPartStatus.NotDefined)
            {
                return GetPrintPrintDefinitionForPage(pa, page);
            }

            return printOnLastPage;
        }
    }
}