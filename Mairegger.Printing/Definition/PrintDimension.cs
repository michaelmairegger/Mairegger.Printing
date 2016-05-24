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
    using System.Reflection;
    using System.Windows;
    using Mairegger.Printing.PrintProcessor;

    /// <summary>
    ///     Provides a class that contains several Printing Dimensions
    /// </summary>
    public class PrintDimension
    {
        private const double DefaultPageNumberHeight = 25;
        private readonly Dictionary<PrintAppendixes, double?> _printPartDimensions = new Dictionary<PrintAppendixes, double?>();

        private readonly Dictionary<PrintAppendixes, Func<IPrintProcessor, UIElement>> _printPartDimensionsRetrievalDictionary = new Dictionary<PrintAppendixes, Func<IPrintProcessor, UIElement>>
                                                                                                                                 {
                                                                                                                                     { PrintAppendixes.Header, p => p.GetHeader() },
                                                                                                                                     { PrintAppendixes.HeaderDescription, p => p.GetHeaderDescription() },
                                                                                                                                     { PrintAppendixes.Summary, p => p.GetSummary() },
                                                                                                                                     { PrintAppendixes.Footer, p => p.GetFooter() }
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

            var values = Enum.GetValues(typeof(PrintAppendixes)).Cast<PrintAppendixes>();

            foreach (var printAppendixese in values)
            {
                _printPartDimensions.Add(printAppendixese, null);
            }
            _printPartDimensions[PrintAppendixes.PageNumbers] = DefaultPageNumberHeight;
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

        internal PrintDefinition InternalPrintDefinition { get; set; }

        internal IPrintProcessor PrintProcessor { get; set; }

        protected bool UseRelativeColumnPosition { get; set; }

        public void SetHeightValue(PrintAppendixes printAppendix, double value)
        {
            if (_printPartDimensions.ContainsKey(printAppendix))
            {
                _printPartDimensions[printAppendix] = value;
            }
        }

        internal double GetHeightFor(PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
        {
            if (!InternalPrintDefinition.IsToPrint(printAppendix, pageNumber, isLastPage))
            {
                return 0;
            }
            if (!InternalPrintDefinition.IsDefined(printAppendix))
            {
                return 0;
            }

            double? value;
            if (_printPartDimensions.TryGetValue(printAppendix, out value))
            {
                if (!value.HasValue)
                {
                    var uiElement = _printPartDimensionsRetrievalDictionary[printAppendix](PrintProcessor);
                    if (uiElement == null)
                    {
                        throw new ArgumentNullException($"${nameof(PrintProcessor)} must return a value for \"Get{printAppendix}\" if \"{printAppendix}\" is set.");
                    }
                    uiElement.Measure(new Size(double.MaxValue, double.MaxValue));
                    value = uiElement.DesiredSize.Height;
                    _printPartDimensions[printAppendix] = value;
                }
                return value.Value;
            }
            throw new ArgumentOutOfRangeException(nameof(printAppendix), "Does not exists as height value");
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
        internal Range<double> GetRangeFor(PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
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
                    throw new ArgumentException(nameof(printAppendix));
            }
            return new Range<double>(fromValue, fromValue + height);
        }

        internal Range<double> GetRangeForBodyGrid(int pageNumber, bool isLastPage)
        {
            var top = GetRangeFor(PrintAppendixes.HeaderDescription, pageNumber, isLastPage).To;
            var bottom = top + GetHeightForBodyGrid(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        internal void PositionizeRelative()
        {
            if (!UseRelativeColumnPosition)
            {
                return;
            }

            var properties = GetPropertiesWithColumnDimensionAttribute();

            var absoluteWidhts = properties.Where(s => s.Value.DimensionType == ColumnDimensionType.Pixels);
            var relativeWidhts = properties.Where(s => s.Value.DimensionType == ColumnDimensionType.Star);

            var reservedForAbsolute = absoluteWidhts.Sum(w => w.Value.ColumnWidth);
            var totalPiecesForRelative = relativeWidhts.Sum(w => w.Value.ColumnWidth);
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
            Trace.WriteLine("Following print dimensions have been set:");
            Trace.Indent();
            Trace.WriteLine("┌──────────────────────────────────────────┬───────────────┐");
            Trace.WriteLine("│              PROPERTY NAME               │     VALUE     │");
            Trace.WriteLine("├──────────────────────────────────────────┼───────────────┤");
            foreach (var propertyInfo in propertyInfos)
            {
                Trace.WriteLine($"│ {propertyInfo.Name,-40} │ {propertyInfo.GetValue(this),10:N3} px │");
            }
            Trace.WriteLine("└──────────────────────────────────────────┴───────────────┘");
            Trace.Unindent();
        }
    }
}