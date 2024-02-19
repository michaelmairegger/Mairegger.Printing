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

namespace Mairegger.Printing.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Mairegger.Printing.Definition;

    internal class PrintOnAttributeHelper
    {
        private readonly List<IPrintPartDefinition> _printOnAttributes = new List<IPrintPartDefinition>();

        public void AddAttribute(IPrintPartDefinition printPartDefinition)
        {
            if (printPartDefinition == null)
            {
                throw new ArgumentNullException(nameof(printPartDefinition));
            }
            Debug.WriteLine("PRINTING: Found {0} for {1}", printPartDefinition.GetType().Name, printPartDefinition.PrintAppendixes);
            _printOnAttributes.Add(printPartDefinition);
        }

        public bool IsDefined(PrintAppendixes printAppendixes)
        {
            return GetPossiblePrintDefinitionAttributes(printAppendixes).Any();
        }

        public PrintPartStatus IsPrintOnPage(PrintAppendixes printA, int page)
        {
            if (page <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }

            var printOnPageAttributes = GetPossiblePrintDefinitionAttributes(printA).ToList();
            if (printOnPageAttributes.Count == 0)
            {
                return PrintPartStatus.NotDefined;
            }

            var list = printOnPageAttributes.Select(printOnAttribute => printOnAttribute.GetPrintDefinition(page)).ToList();

            var excludedOnAny = list.Any(i => i == PrintPartStatus.Exclude);

            if (excludedOnAny)
            {
                return PrintPartStatus.Exclude;
            }

            var includedOnAny = list.Any(i => i == PrintPartStatus.Include);
            if (includedOnAny)
            {
                return PrintPartStatus.Include;
            }

            return PrintPartStatus.NotDefined;
        }

        private IEnumerable<IPrintPartDefinition> GetPossiblePrintDefinitionAttributes(PrintAppendixes printAppendixes)
        {
            return _printOnAttributes.Where(v => v.PrintAppendixes.HasFlag(printAppendixes));
        }
    }
}
