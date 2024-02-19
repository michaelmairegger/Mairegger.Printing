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

using System.Text;

namespace Mairegger.Printing.Definition
{
    using System;
    using System.Globalization;
    using JetBrains.Annotations;
    using Mairegger.Printing.PrintProcessor;
    using Mairegger.Printing.Properties;

    /// <summary>
    ///     Provides an attribute to define the columns in the Table of the <see cref="PrintProcessor" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    public sealed class ColumnDimensionAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDimensionAttribute" /> class width the column width defined by
        ///     the passed percentage.
        /// </summary>
        /// <param name="columnWidth"> The relative width of the column width respect to the total width. </param>
        /// <example>
        ///     The following defines an attribute on a property that is used for a column that takes 20% of the total width.
        ///     <code>
        /// [ColumnDimension(0.2d)]
        /// public double ColumnWidth { get; private set; }
        ///      </code>
        /// </example>
        public ColumnDimensionAttribute(double columnWidth)
            : this(columnWidth, ColumnDimensionType.Star)
        {
            if ((columnWidth <= 0) || (columnWidth > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(columnWidth), l10n.ColumnDimensionAttribute_ColumnDimensionAttribute_The_value_must_be_between_0_and_1_);
            }
        }

        public ColumnDimensionAttribute(string value)
        {
            #if NETFRAMEWORK
            if (value.EndsWith("*", StringComparison.Ordinal))
            #else
            if (value.EndsWith('*'))
            #endif
            {
                DimensionType = ColumnDimensionType.Star;

                ColumnWidth = 1;

                if (value.Length > 1)
                {
                    #if NET7_0_OR_GREATER
                    ColumnWidth = double.Parse(value.AsSpan(0, value.Length - 1), CultureInfo.InvariantCulture);
                    #else
                    ColumnWidth = double.Parse(value.Substring(0, value.Length - 1), CultureInfo.InvariantCulture);
                    #endif
                }
            }
            else if (value.EndsWith("px", StringComparison.Ordinal))
            {
                DimensionType = ColumnDimensionType.Pixels;
                #if NET7_0_OR_GREATER
                ColumnWidth = double.Parse(value.AsSpan(0, value.Length - 2), CultureInfo.InvariantCulture);
                #else
                ColumnWidth = double.Parse(value.Substring(0, value.Length - 2), CultureInfo.InvariantCulture);
                #endif
            }
            else
            {
                #if NET8_0_OR_GREATER
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, l10nComposite.ColumnDimensionAttribute_ColumnDimensionAttribute__0__is_no_valid_column_dimension, value), nameof(value));
                #else
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, l10n.ColumnDimensionAttribute_ColumnDimensionAttribute__0__is_no_valid_column_dimension, value), nameof(value));
                #endif
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDimensionAttribute" /> class  defining the column width width
        ///     the passed value. The column width is either Relative or Absolute.
        /// </summary>
        /// <param name="columnWidth"> The width of the column. </param>
        /// <param name="dimensionType"> The type of the width (Relative or Absolute). </param>
        public ColumnDimensionAttribute(double columnWidth, ColumnDimensionType dimensionType)
        {
            if (columnWidth <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnWidth));
            }

            ColumnWidth = columnWidth;
            DimensionType = dimensionType;
        }

        public double ColumnWidth { get; private set; }

        public ColumnDimensionType DimensionType { get; private set; }
    }
}
