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
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    ///     Represents a contiguous area between two numeric
    /// </summary>
    public struct Range<T> : IEquatable<Range<T>>
        where T : IComparable<T>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Range{T}" /> struct.
        /// </summary>
        /// <param name="fromValue"> the minimum value </param>
        /// <param name="toValue"> the maximum value </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="fromValue" /> is greater than
        ///     <paramref name="toValue" />.
        /// </exception>
        public Range(T fromValue, T toValue)
            : this()
        {
            if (fromValue.CompareTo(toValue) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fromValue), $"{nameof(fromValue)} must be lower or equal than {nameof(toValue)}");
            }
            From = fromValue;
            To = toValue;
        }

        /// <summary>
        ///     Gets the minimum bound of the range
        /// </summary>
        public T From { get; }

        /// <summary>
        ///     Gets the difference between <see cref="To" /> and <see cref="From" />.
        /// </summary>
        /// <exception cref="RuntimeBinderException"><typeparamref name="T" /> does not overloads the --operator.</exception>
        public T Length => (To as dynamic) - (From as dynamic);

        /// <summary>
        ///     Gets the maximum bound of the range
        /// </summary>
        public T To { get; }

        public static bool operator ==(Range<T> left, Range<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Range<T>))
            {
                return false;
            }
            return Equals((Range<T>)obj);
        }

        public bool Equals(Range<T> other)
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

        public bool IsInRange(T value)
        {
            return (From.CompareTo(value) <= 0) && (To.CompareTo(value) >= 0);
        }

        public bool IsInRange(Range<T> other)
        {
            return IsInRange(other.From) && IsInRange(other.To);
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }
    }

    public static class Range
    {
        public static Range<T> FromPoint<T>(T value) where T : IComparable<T>
        {
            return new Range<T>(value, value);
        }

        /// <summary>
        ///     Parses the input towards the corresponding <see cref="Range{T}" /> value
        /// </summary>
        /// <param name="input"> the string representation of the range to convert to </param>
        /// <returns> the range representation </returns>
        /// <exception cref="ArgumentNullException">If the passed argument is null</exception>
        /// <exception cref="ArgumentException">If the passed argument is in a invalid format</exception>
        /// <exception cref="FormatException">If the passed argument does not contains the correct number</exception>
        /// <example>
        ///     <code>Range r = Range.Parse("4-6");</code>
        /// </example>
        public static Range<double> Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (input.Contains(',')) // TODO Remove this check in a future releases
            {
                throw new ArgumentException("',' is not supported anymore change range to '-'");
            }

            var range = input.Split('-');

            if ((range.Length != 2 && range.Length != 1) || range.Any(c => c.Length == 0))
            {
                throw new ArgumentException("invalid format", nameof(input));
            }

            var fromString = range.First();
            var toString = range.Last();

            if (!double.TryParse(fromString, out double min))
            {
                throw new FormatException($"Cannot convert '{fromString}' from '{input}'");
            }

            if (!double.TryParse(toString, out double max))
            {
                throw new FormatException($"Cannot convert '{toString}' from '{input}'");
            }

            return new Range<double>(min, max);
        }

        public static IList<Range<double>> ParseRanges(string input)
        {
            var ranges = input.Split(',');

            return ranges.Select(Parse).ToList();
        }
    }
}