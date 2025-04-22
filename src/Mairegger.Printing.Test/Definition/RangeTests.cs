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

using Mairegger.Printing.Tests.Content;

namespace Mairegger.Printing.Tests.Definition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mairegger.Printing.Definition;

    public class RangeTests
    {
        [Fact]
        public void IsInRange_InRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.True(r1.IsInRange(r2));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void IsInRange_InRange_True(double value)
        {
            var r = new PageRange(1, 100);
            Assert.True(r.IsInRange(value));
        }

        [Fact]
        public void IsInRange_NotInRange_ReturnsFalse()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.False(r2.IsInRange(r1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void IsInRange_OutOfRange_False(double value)
        {
            var r = new PageRange(6, 10);
            Assert.False(r.IsInRange(value));
        }

        [Fact]
        public void IsInRange_SameRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);

            Assert.True(r1.IsInRange(r1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Length(double length)
        {
            var r = new PageRange(0, length);
            Assert.Equal(r.Length, length);
        }

        [Fact]
        public void Parse_Invalid()
        {
            var input = string.Empty;
            Assert.Throws<ArgumentNullException>(() => PageRange.Parse(null!));
            Assert.Throws<ArgumentException>(() => PageRange.Parse(input));
            Assert.Throws<ArgumentException>(() => PageRange.Parse("-"));
            Assert.Throws<FormatException>(() => PageRange.Parse("InvalidNumber-6"));
            Assert.Throws<FormatException>(() => PageRange.Parse("6-InvalidNumber"));

            Assert.Throws<ArgumentOutOfRangeException>(() => PageRange.Parse("6-4"));

            Assert.Throws<ArgumentException>(() => PageRange.Parse("4,6"));
        }

        [Fact]
        public void Parse_ValidRange()
        {
            var r = PageRange.Parse("4-6");
            Assert.Multiple(() =>
            {
                Assert.Equal(4, r.From);
                Assert.Equal(6, r.To);
            });
        }

        [Fact]
        public void ParseRanges()
        {
            var ranges = PageRange.ParseRanges("1-2,5,10-11").ToList();

            Assert.Contains(PageRange.Parse("1-2"), ranges);
            Assert.Contains(PageRange.Parse("5"), ranges);
            Assert.Contains(PageRange.Parse("10-11"), ranges);
        }

        [Fact]
        public void Range_Equals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(5, 10);

            Assert.Equal(r2, r1);
            Assert.Equal(r2.GetHashCode(), r1.GetHashCode());
        }

        [Fact]
        public void Range_FromPoint_ToString()
        {
            var r = PageRange.FromPoint(5);
            Assert.Equal("5-5", r.ToString());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 4)]
        [InlineData(5, 5)]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinEqualsMax_Valie(double min, double max)
        {
            var exception = Record.Exception(() => new PageRange(min, max));
            Assert.Null(exception);
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList2Double), 10d, 20d, 0d, 10d,5, MemberType = typeof(RandomTest))]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinGreatherMax_Valie(double min, double max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PageRange(min, max));
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList2Double), 0d, 10d, 10d, 20d,5, MemberType = typeof(RandomTest))]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinLessMax_Valie(double min, double max)
        {
            var exception = Record.Exception(() => new PageRange(min, max));
            Assert.Null(exception);
        }

        [Fact]
        public void Range_NotEquals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.NotEqual(r2.GetHashCode(), r1.GetHashCode());
        }

        [Fact]
        public void Range_SinglePoint()
        {
            var r = PageRange.FromPoint(5);

            Assert.Equal(r.To, r.From);
        }

        [Fact]
        public void Range_ToString()
        {
            var r = new PageRange(7, 11);
            Assert.Equal("7-11", r.ToString());
        }
    }
}
