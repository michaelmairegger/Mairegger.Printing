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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    ///     Provides a class that contains several Printing Dimensions
    /// </summary>
    public class PrintDimension
    {
        private double? _footerHeight;
        private double? _headerHeight;
        private double? _recipientHeight;
        private double? _summaryHeight;

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
        }

        /// <summary>
        ///     Gets or sets the height of the footer.
        /// </summary>
        /// <value> The height of the footer. </value>
        public double FooterHeight
        {
            get
            {
                if (GetPrintDefinitionForPage(PrintAppendixes.Footer))
                {
                    if (!_footerHeight.HasValue)
                    {
                        throw new InvalidOperationException("You have to set the height of the footer prior printing if the PrintAppendixes flag has set to \"Footer\".");
                    }

                    return _footerHeight.Value;
                }

                return 0;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _footerHeight = value;
            }
        }

        /// <summary>
        ///     Gets or sets the height of the recipient.
        /// </summary>
        /// <value> The height of the recipient. </value>
        public double HeaderDescriptionHeight
        {
            get
            {
                if (GetPrintDefinitionForPage(PrintAppendixes.HeaderDescription))
                {
                    if (!_recipientHeight.HasValue)
                    {
                        throw new InvalidOperationException("You have to set the descriptional header prior printing if the PrintAppendixes flag has set to \"HeaderDescription\".");
                    }

                    return _recipientHeight.Value;
                }

                return 0;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _recipientHeight = value;
            }
        }

        /// <summary>
        ///     Gets or sets the height of the header.
        /// </summary>
        /// <value> The height of the header. </value>
        public double HeaderHeight
        {
            get
            {
                if (GetPrintDefinitionForPage(PrintAppendixes.Header))
                {
                    if (!_headerHeight.HasValue)
                    {
                        throw new InvalidOperationException("You have to set the height of the header prior printing if the PrintAppendixes flag has set to \"Header\".");
                    }

                    return _headerHeight.Value;
                }

                return 0;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _headerHeight = value;
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

        /// <summary>
        ///     Gets or sets the height of the summary.
        /// </summary>
        /// <value> The height of the summary. </value>
        public double SummaryHeight
        {
            get
            {
                if (GetPrintDefinitionForPage(PrintAppendixes.Summary))
                {
                    if (!_summaryHeight.HasValue)
                    {
                        throw new InvalidOperationException("You have to set the height of the summary before printing if the PrintAppendixes flag has set to \"Summary\".");
                    }

                    return _summaryHeight.Value;
                }

                return 0;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _summaryHeight = value;
            }
        }

        protected bool UseRelativeColumnPosition { get; set; }

        internal PrintDefinition InternalPrintDefinition { get; set; }

        internal Range<double> GetBodyGridRange(int pageNumber, bool isLastPage)
        {
            var top = GetHeaderDescriptionRange(pageNumber, isLastPage).To;
            var bottom = top + GetMaxGridHeight(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        internal double GetMaxGridHeight(int pageNumber, bool isLastPage)
        {
            var reservedHeight = GetReservedHeight(pageNumber, isLastPage);
            var maxGridHeight = PrintablePageSize.Height - reservedHeight;
            return maxGridHeight;
        }

        internal double GetPageNumberHeight(int pageNumber)
        {
            return InternalPrintDefinition.IsToPrint(PrintAppendixes.PageNumbers, pageNumber, false) ? 25 : 0;
        }

        internal Range<double> GetRange(PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
        {
            switch (printAppendix)
            {
                case PrintAppendixes.Header:
                    return GetHeaderRange(pageNumber, isLastPage);
                case PrintAppendixes.Footer:
                    return GetFooterRange(pageNumber, isLastPage);
                case PrintAppendixes.Summary:
                    return GetSummaryRange(pageNumber, isLastPage);
                case PrintAppendixes.HeaderDescription:
                    return GetHeaderDescriptionRange(pageNumber, isLastPage);
                case PrintAppendixes.PageNumbers:
                    return GetPageNumberRange(pageNumber, isLastPage);
                default:
                    throw new ArgumentOutOfRangeException(nameof(printAppendix));
            }
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

        private double GetFooterHeight(int pageNumber, bool isLastPage)
        {
            return InternalPrintDefinition.IsToPrint(PrintAppendixes.Footer, pageNumber, isLastPage) ? FooterHeight : 0;
        }

        private Range<double> GetFooterRange(int pageNumber, bool isLastPage)
        {
            var top = GetSummaryRange(pageNumber, isLastPage).To;
            var bottom = top + GetFooterHeight(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        private double GetHeaderDescriptionHeight(int pageNumber, bool isLastPage)
        {
            return InternalPrintDefinition.IsToPrint(PrintAppendixes.HeaderDescription, pageNumber, isLastPage) ? HeaderDescriptionHeight : 0;
        }

        private Range<double> GetHeaderDescriptionRange(int pageNumber, bool isLastPage)
        {
            var top = GetHeaderRange(pageNumber, isLastPage).To;
            var bottom = top + GetHeaderDescriptionHeight(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        private double GetHeaderHeight(int pageNumber, bool isLastPage)
        {
            return InternalPrintDefinition.IsToPrint(PrintAppendixes.Header, pageNumber, isLastPage) ? HeaderHeight : 0;
        }

        private Range<double> GetHeaderRange(int pageNumber, bool isLastPage)
        {
            var top = Margin.Top;
            var bottom = top + GetHeaderHeight(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        private Range<double> GetPageNumberRange(int pageNumber, bool isLastPage)
        {
            var top = GetFooterRange(pageNumber, isLastPage).To;
            var bottom = top + GetPageNumberHeight(pageNumber);

            return new Range<double>(top, bottom);
        }

        private bool GetPrintDefinitionForPage(PrintAppendixes printAppendixes)
        {
            return InternalPrintDefinition.IsDefined(printAppendixes);
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

        private double GetReservedHeight(int pageNumber, bool isLastPage)
        {
            return GetSummaryHeight(pageNumber, isLastPage) + GetHeaderDescriptionHeight(pageNumber, isLastPage) + GetHeaderHeight(pageNumber, isLastPage) + GetFooterHeight(pageNumber, isLastPage) + GetPageNumberHeight(pageNumber);
        }

        private double GetSummaryHeight(int pageNumber, bool isLastPage)
        {
            return InternalPrintDefinition.IsToPrint(PrintAppendixes.Summary, pageNumber, isLastPage) ? SummaryHeight : 0;
        }

        private Range<double> GetSummaryRange(int pageNumber, bool isLastPage)
        {
            var top = GetBodyGridRange(pageNumber, isLastPage).To;
            var bottom = top + GetSummaryHeight(pageNumber, isLastPage);

            return new Range<double>(top, bottom);
        }

        private void WritePropertyInformation(IEnumerable<PropertyInfo> propertyInfos)
        {
            Trace.WriteLine("Following print dimensions have been set:");
            Trace.Indent();
            Trace.WriteLine("┌───────────────────────────┬─────────────┐");
            Trace.WriteLine("│       PROPERTY NAME       │    VALUE    │");
            Trace.WriteLine("├───────────────────────────┼─────────────┤");
            foreach (var propertyInfo in propertyInfos)
            {
                Trace.WriteLine($"│ {propertyInfo.Name,-25} │ {propertyInfo.GetValue(this),8:N3} px │");
            }
            Trace.WriteLine("└───────────────────────────┴─────────────┘");
            Trace.Unindent();
        }
    }
}