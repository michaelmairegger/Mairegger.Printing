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

using System.Globalization;
using EnumsNET;

namespace Mairegger.Printing.Definition
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Mairegger.Printing.PrintProcessor;
    using Mairegger.Printing.Properties;

    /// <summary>
    ///     Provides a class that contains several Printing Dimensions
    /// </summary>
    public class PrintDimension
    {
        private readonly Dictionary<PrintAppendixes, double?> _printPartDimensions = new Dictionary<PrintAppendixes, double?>();

        private readonly Dictionary<PrintAppendixes, Func<IPrintProcessor, UIElement>> _printPartDimensionsRetrievalDictionary = new Dictionary<PrintAppendixes, Func<IPrintProcessor, UIElement>>
                                                                                                                                 {
                                                                                                                                     { PrintAppendixes.Header, p => p.GetHeader() },
                                                                                                                                     { PrintAppendixes.HeaderDescription, p => p.GetHeaderDescription() },
                                                                                                                                     { PrintAppendixes.Summary, p => p.GetSummary() },
                                                                                                                                     { PrintAppendixes.Footer, p => p.GetFooter() },
                                                                                                                                     { PrintAppendixes.PageNumbers, p => p.GetPageNumbers(1, 1) }
                                                                                                                                 };

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrintDimension" /> class and sets the margin to 0,0,0,0.
        /// </summary>
        public PrintDimension()
            : this(new Thickness(0))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PrintDimension" /> class.
        /// </summary>
        public PrintDimension(Thickness margin)
        {
            Margin = margin;

            var printAppendixes = Enums.GetValues<PrintAppendixes>();

            foreach (var item in printAppendixes)
            {
                _printPartDimensions.Add(item, null);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating the margin for the print
        /// </summary>
        public Thickness Margin { get; set; }

        /// <summary>
        ///     Gets the size of the page on witch the print has to be placed
        /// </summary>
        public Size PageSize { get; internal set; }

        /// <summary>
        ///     Gets the <see cref="PrintAppendixes" /> for each part that should be used for printing.
        /// </summary>
        /// <summary>
        ///     Gets the size of the printable area of the page.
        /// </summary>
        /// <value>
        ///     The <see cref="Size" /> of the printable area of the page.
        /// </value>
        public Size PrintablePageSize
        {
            get
            {
                var printableWidth = PageSize.Width - Margin.Left - Margin.Right;
                var printableHeight = PageSize.Height - Margin.Bottom - Margin.Top;
                return new Size(printableWidth, printableHeight);
            }
        }

        internal PrintDefinition? InternalPrintDefinition { get; set; }

        internal IPrintProcessor? PrintProcessor { get; set; }

        protected bool UseRelativeColumnPosition { get; set; }

        /// <summary>
        /// Sets the height for <paramref name="printAppendix"/> to a specific value
        /// </summary>
        /// <param name="printAppendix">The <see cref="PrintAppendixes"/> which the height is set.</param>
        /// <param name="value">The height for the item.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is negative.</exception>
        public void SetHeightValue(PrintAppendixes printAppendix, double? value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, l10n.PrintDimension_SetHeightValue_Must_be_a_positive_number_or_null);
            }
            if (_printPartDimensions.ContainsKey(printAppendix))
            {
                _printPartDimensions[printAppendix] = value;
            }
        }

        /// <summary>
        /// Recalculates the height for <paramref name="printAppendix"/> when the specific <paramref name="condition"/> is met.
        /// </summary>
        /// <param name="condition">The condition when to reset the height for <paramref name="printAppendix"/></param>
        /// <param name="printAppendix">The <see cref="PrintAppendixes"/> which value is reset.</param>
        /// <exception cref="ArgumentNullException"><paramref name="condition"/> is null.</exception>
        public void RecalculateHeightValueWhen(Func<bool> condition, PrintAppendixes printAppendix)
        {
            #if NETFRAMEWORK
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            #else
            ArgumentNullException.ThrowIfNull(condition);
            #endif
            if (condition())
            {
                SetHeightValue(printAppendix, null);
            }
        }

        internal double GetHeightFor(PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
        {
            if (InternalPrintDefinition?.IsToPrint(printAppendix, pageNumber, isLastPage) == true)
            {
                double? value = _printPartDimensions[printAppendix];
                if (!value.HasValue && PrintProcessor != null)
                {
                    var uiElement = _printPartDimensionsRetrievalDictionary[printAppendix](PrintProcessor);
                    if (uiElement == null)
                    {
                        #if NETFRAMEWORK
                        throw new ArgumentNullException($"{nameof(PrintProcessor)}.Get{printAppendix}()", string.Format(CultureInfo.CurrentCulture, l10n.PrintDimension_GetHeightFor__0__must_return_a_value_for__Get_1_____if___2___is_set_, typeof(PrintProcessor), printAppendix, printAppendix));
                        #else
                        throw new ArgumentNullException($"{nameof(PrintProcessor)}.Get{printAppendix}()", string.Format(CultureInfo.CurrentCulture, l10nComposite.PrintDimension_GetHeightFor__0__must_return_a_value_for__Get_1_____if___2___is_set_, typeof(PrintProcessor), printAppendix, printAppendix));
                        #endif
                    }
                    uiElement.Measure(new Size(double.MaxValue, double.MaxValue));
                    value = uiElement.DesiredSize.Height;
                    _printPartDimensions[printAppendix] = value;
                }
                return value ?? 0;
            }
            return 0;
        }

        internal double GetHeightForBodyGrid(int pageNumber, bool isLastPage)
        {
            var reservedHeight = GetHeightFor(PrintAppendixes.Summary, pageNumber, isLastPage) +
                                 GetHeightFor(PrintAppendixes.HeaderDescription, pageNumber, isLastPage) +
                                 GetHeightFor(PrintAppendixes.Header, pageNumber, isLastPage) +
                                 GetHeightFor(PrintAppendixes.Footer, pageNumber, isLastPage) +
                                 GetHeightFor(PrintAppendixes.PageNumbers, pageNumber, isLastPage);

            var maxGridHeight = PrintablePageSize.Height - reservedHeight;
            return maxGridHeight;
        }

        /// <summary>
        ///     Gets the range from which point the <paramref name="printAppendix" /> starts and where it ends.
        ///     The layout of the page is constructed as follows:
        ///     <list type="bullet">
        ///         <item>
        ///             <see cref="IPrintProcessor.GetHeader">Header</see>
        ///         </item>
        ///         <item>
        ///             <see cref="IPrintProcessor.GetHeaderDescription">HeaderDescription</see>
        ///         </item>
        ///         <item>
        ///             <see cref="IPrintProcessor.ItemCollection">Body Grid</see>
        ///         </item>
        ///         <item>
        ///             <see cref="IPrintProcessor.GetSummary">Summary</see>
        ///         </item>
        ///         <item>
        ///             <see cref="IPrintProcessor.GetFooter">Footer</see>
        ///         </item>
        ///         <item>Page numbers</item>
        ///     </list>
        /// </summary>
        /// <param name="printAppendix"></param>
        /// <param name="pageNumber"></param>
        /// <param name="isLastPage"></param>
        /// <returns></returns>
        internal PageRange GetRangeFor(PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
        {
            double fromValue;
            double height;

            switch (printAppendix)
            {
                case PrintAppendixes.Header:
                    fromValue = Margin.Top;
                    height = GetHeightFor(printAppendix, pageNumber, isLastPage);
                    break;
                case PrintAppendixes.HeaderDescription:
                    fromValue = GetRangeFor(PrintAppendixes.Header, pageNumber, isLastPage).To;
                    height = GetHeightFor(printAppendix, pageNumber, isLastPage);
                    break;
                case PrintAppendixes.Summary:
                    fromValue = GetRangeFor(PrintAppendixes.HeaderDescription, pageNumber, isLastPage).To + GetHeightForBodyGrid(pageNumber, isLastPage);
                    height = GetHeightFor(printAppendix, pageNumber, isLastPage);
                    break;
                case PrintAppendixes.Footer:
                    fromValue = GetRangeFor(PrintAppendixes.Summary, pageNumber, isLastPage).To;
                    height = GetHeightFor(printAppendix, pageNumber, isLastPage);
                    break;
                case PrintAppendixes.PageNumbers:
                    fromValue = GetRangeFor(PrintAppendixes.Footer, pageNumber, isLastPage).To;
                    height = GetHeightFor(printAppendix, pageNumber, isLastPage);
                    break;
                default:
                    throw new ArgumentException($"Enum value {printAppendix} is not defined on enum {typeof(PrintAppendixes).FullName}", nameof(printAppendix));
            }
            return new PageRange(fromValue, fromValue + height);
        }

        internal PageRange GetRangeForBodyGrid(int pageNumber, bool isLastPage)
        {
            var top = GetRangeFor(PrintAppendixes.HeaderDescription, pageNumber, isLastPage).To;
            var bottom = top + GetHeightForBodyGrid(pageNumber, isLastPage);

            return new PageRange(top, bottom);
        }

        internal void PositionRelative()
        {
            if (!UseRelativeColumnPosition)
            {
                return;
            }

            var properties = GetPropertiesWithColumnDimensionAttribute();

            var absoluteWidths = properties.Where(s => s.Value.DimensionType == ColumnDimensionType.Pixels);
            var relativeWidths = properties.Where(s => s.Value.DimensionType == ColumnDimensionType.Star);

            var reservedForAbsolute = absoluteWidths.Sum(w => w.Value.ColumnWidth);
            var totalPiecesForRelative = relativeWidths.Sum(w => w.Value.ColumnWidth);
            var remainingForRelative = PrintablePageSize.Width - reservedForAbsolute;
            var widthPerPiece = remainingForRelative / totalPiecesForRelative;

            foreach (var columnDimension in properties)
            {
                var columnWidth = columnDimension.Value.ColumnWidth;
                var property = columnDimension.Key;

                if (property.PropertyType != typeof(double))
                {
                    throw new InvalidOperationException($"The property {property.ReflectedType?.Name}.{property.Name} must be of type double. The current type is {property.PropertyType}");
                }

                var effectiveWidth = columnWidth;

                if (columnDimension.Value.DimensionType == ColumnDimensionType.Star)
                {
                    effectiveWidth *= widthPerPiece;
                }

                if (!property.CanWrite)
                {
                    var field = GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

                    field?.SetValue(this, effectiveWidth);
                }
                else
                {
                    property.SetValue(this, effectiveWidth, null);
                }
            }

            WritePropertyInformation(properties.Select(i => i.Key));
        }

        private Dictionary<PropertyInfo, ColumnDimensionAttribute> GetPropertiesWithColumnDimensionAttribute()
        {
            var dict = new Dictionary<PropertyInfo, ColumnDimensionAttribute>();

            foreach (var propertyInfo in GetType().GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(ColumnDimensionAttribute), true).SingleOrDefault(a => a is ColumnDimensionAttribute);

                if (attribute != null)
                {
                    var pageDimensionAttribute = (ColumnDimensionAttribute)attribute;

                    dict.Add(propertyInfo, pageDimensionAttribute);
                }
            }
            return dict;
        }

        private void WritePropertyInformation(IEnumerable<PropertyInfo> propertyInfos)
        {
            Debug.WriteLine("PRINTING: Following print dimensions have been set:");
            Debug.Indent();
            Debug.WriteLine("PRINTING: ┌──────────────────────────────────────────┬───────────────┐");
            Debug.WriteLine("PRINTING: │              PROPERTY NAME               │     VALUE     │");
            Debug.WriteLine("PRINTING: ├──────────────────────────────────────────┼───────────────┤");
            foreach (var propertyInfo in propertyInfos)
            {
                Debug.WriteLine($"PRINTING: │ {propertyInfo.Name,-40} │ {propertyInfo.GetValue(this),10:N3} px │");
            }
            Debug.WriteLine("PRINTING: └──────────────────────────────────────────┴───────────────┘");
            Debug.Unindent();
        }
    }
}
