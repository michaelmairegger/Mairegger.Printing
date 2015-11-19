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
    using Mairegger.Printing.Internal;

    public class PrintDefinition
    {
        private readonly PrinOnAttributeHelper _prinOnAttributeHelper = new PrinOnAttributeHelper();

        public bool IsDefined(PrintAppendixes value)
        {
            return _prinOnAttributeHelper.IsDefined(value);
        }

        public bool IsToPrint(PrintAppendixes pa, int pageNumber, bool isLastPage)
        {
            return GetPrintPrintDefinitionForPage(pa, pageNumber, isLastPage) == PrintPartStatus.Include;
        }

        public void SetPrintAttribute(IPrintPartDefinition definition)
        {
            _prinOnAttributeHelper.AddAttribute(definition);
        }

        public void SetPrintOnAttribute(IPrintPartDefinition printOnPageAttribute)
        {
            if (printOnPageAttribute == null)
            {
                throw new ArgumentNullException(nameof(printOnPageAttribute));
            }

            _prinOnAttributeHelper.AddAttribute(printOnPageAttribute);
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
                throw new ArgumentException(nameof(pa));
            }

            if (IsDefined(pa))
            {
                return _prinOnAttributeHelper.IsPrintOnPage(pa, pageNumber);
            }

            return PrintPartStatus.NotDefined;
        }

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

            Debug.WriteLineIf(result == PrintPartStatus.NotDefined, pa + " not defined on page " + pageNumber + " (lastpage=" + isLastPage + ")");
            Debug.WriteLineIf(result == PrintPartStatus.Include, pa + " included on page " + pageNumber + " (lastpage=" + isLastPage + ")");
            Debug.WriteLineIf(result == PrintPartStatus.Exclude, pa + " excluded on page " + pageNumber + " (lastpage=" + isLastPage + ")");

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