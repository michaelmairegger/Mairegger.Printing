// Copyright 2017-2025 Michael Mairegger
//
// Licensed under the Apache License, Version 2.0 (the "License")
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
using Mairegger.Printing.Properties;

namespace Mairegger.Printing.Definition
{
    /// <summary>
    ///     Represents a contiguous area between two numeric
    /// </summary>
    public struct PageRange : IEquatable<PageRange>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PageRange" /> struct.
        /// </summary>
        /// <param name="fromValue"> the minimum value </param>
        /// <param name="toValue"> the maximum value </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="fromValue" /> is greater than
        ///     <paramref name="toValue" />.
        /// </exception>
        public PageRange(double fromValue, double toValue)
            : this()
        {
            if (fromValue.CompareTo(toValue) > 0)
            {
#if NETFRAMEWORK
                throw new ArgumentOutOfRangeException(nameof(fromValue), string.Format(CultureInfo.CurrentCulture, l10n.PageRange_PageRange__0__must_be_lower_or_equal_than__1_, nameof(fromValue), nameof(toValue)));
#else
                throw new ArgumentOutOfRangeException(nameof(fromValue), string.Format(CultureInfo.CurrentCulture, l10nComposite.PageRange_PageRange__0__must_be_lower_or_equal_than__1_, nameof(fromValue), nameof(toValue)));
#endif
            }

            From = fromValue;
            To = toValue;
        }

        /// <summary>
        ///     Gets the minimum bound of the range
        /// </summary>
        public double From { get; }

        /// <summary>
        ///     Gets the difference between <see cref="To" /> and <see cref="From" />.
        /// </summary>
        public double Length => To - From;

        /// <summary>
        ///     Gets the maximum bound of the range
        /// </summary>
        public double To { get; }

        public static bool operator ==(PageRange left, PageRange right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PageRange left, PageRange right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is PageRange))
            {
                return false;
            }

            return Equals((PageRange)obj);
        }

        public bool Equals(PageRange other)
        {
            return To.Equals(other.To) && From.Equals(other.From);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ To.GetHashCode();
            }
        }

        public bool IsInRange(double value)
        {
            return (From.CompareTo(value) <= 0) && (To.CompareTo(value) >= 0);
        }

        public bool IsInRange(PageRange other)
        {
            return IsInRange(other.From) && IsInRange(other.To);
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }

        public static PageRange FromPoint(double value)
        {
            return new PageRange(value, value);
        }

        /// <summary>
        ///     Parses the input towards the corresponding <see cref="PageRange" /> value
        /// </summary>
        /// <param name="input"> the string representation of the range to convert to </param>
        /// <returns> the range representation </returns>
        /// <exception cref="ArgumentNullException">If the passed argument is null</exception>
        /// <exception cref="ArgumentException">If the passed argument is in a invalid format</exception>
        /// <exception cref="FormatException">If the passed argument does not contains the correct number</exception>
        /// <example>
        ///     <code>Range r = Range.Parse("4-6");</code>
        /// </example>
        public static PageRange Parse(string input)
        {
#if NETFRAMEWORK
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
#else
            ArgumentNullException.ThrowIfNull(input);
#endif

#if NETFRAMEWORK
            if (input.Contains(','))
#else
            if (input.Contains(',', StringComparison.Ordinal))
#endif
            {
                throw new ArgumentException($"Use {nameof(ParseRanges)} for parsing multi-range values");
            }

            var range = input.Split('-');

            if (((range.Length != 2) && (range.Length != 1)) || range.Any(c => c.Length == 0))
            {
                throw new ArgumentException(l10n.PageRange_Parse_Invalid_format, nameof(input));
            }

            var fromString = range.First();
            var toString = range.Last();

            if (!double.TryParse(fromString, out var min))
            {
                throw new FormatException($"Cannot convert '{fromString}' from '{input}'");
            }

            if (!double.TryParse(toString, out var max))
            {
                throw new FormatException($"Cannot convert '{toString}' from '{input}'");
            }

            return new PageRange(min, max);
        }

        public static IList<PageRange> ParseRanges(string input)
        {
            var ranges = input.Split(',');

            return ranges.Select(Parse).ToList();
        }
    }
}
