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

namespace Mairegger.Printing.Tests.Definition
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Mairegger.Printing.Definition;
    using NUnit.Framework;

    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void IsInRange_InRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.True(r1.IsInRange(r2));
        }

        [Test]
        public void IsInRange_InRange_True([Values(1, 2, 3, 4, 5)] double value)
        {
            var r = new PageRange(1, 100);
            Assert.IsTrue(r.IsInRange(value));
        }

        [Test]
        public void IsInRange_NotInRange_ReturnsFalse()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.False(r2.IsInRange(r1));
        }

        [Test]
        public void IsInRange_OutOfRange_False([Values(1, 2, 3, 4, 5)] double value)
        {
            var r = new PageRange(6, 10);
            Assert.IsFalse(r.IsInRange(value));
        }

        [Test]
        public void IsInRange_SameRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);

            Assert.True(r1.IsInRange(r1));
        }

        [Test]
        public void Length([Values(1, 2, 3, 4, 5)] double length)
        {
            var r = new PageRange(0, length);
            Assert.AreEqual(r.Length, length);
        }

        [Test]
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

        [Test]
        public void Parse_ValidRange()
        {
            var r = PageRange.Parse("4-6");
            Assert.That(r.From, Is.EqualTo(4));
            Assert.That(r.To, Is.EqualTo(6));
        }

        [Test]
        public void ParseRanges()
        {
            var ranges = PageRange.ParseRanges("1-2,5,10-11").ToList();

            CollectionAssert.Contains(ranges, PageRange.Parse("1-2"));
            CollectionAssert.Contains(ranges, PageRange.Parse("5"));
            CollectionAssert.Contains(ranges, PageRange.Parse("10-11"));
        }

        [Test]
        public void Range_Equals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(5, 10);

            Assert.That(r1, Is.EqualTo(r2));
            Assert.True(r1 == r2);
            Assert.True(Equals(r1, r2));
            Assert.That(r1.GetHashCode(), Is.EqualTo(r2.GetHashCode()));
        }

        [Test]
        public void Range_FromPoint_ToString()
        {
            var r = PageRange.FromPoint(5);
            Assert.That(r.ToString(), Is.EqualTo("5-5"));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinEqualsMax_Valie(double min, double max)
        {
            Assert.DoesNotThrow(() => new PageRange(min, max));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinGreatherMax_Valie(
            [Values(6, 7, 8, 9, 10)] double min,
            [Values(1, 2, 3, 4, 5)] double max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PageRange(min, max));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public void Range_MinLessMax_Valie(
            [Values(1, 2, 3, 4, 5)] double min,
            [Values(6, 7, 8, 9, 10)] double max)
        {
            Assert.DoesNotThrow(() => new PageRange(min, max));
        }

        [Test]
        public void Range_NotEquals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            Assert.That(r1, Is.Not.EqualTo(r2));

            Assert.True(r1 != r2);

            Assert.False(Equals(r1, r2));
            Assert.False(r1.Equals(null));
            Assert.That(r1, Is.Not.EqualTo(null));
            // ReSharper disable once SuspiciousTypeConversion.Global
            Assert.False(r1.Equals(5));
            Assert.That(r1.GetHashCode(), Is.Not.EqualTo(r2.GetHashCode()));
        }

        [Test]
        public void Range_SinglePoint()
        {
            var r = PageRange.FromPoint(5);

            Assert.That(r.From, Is.EqualTo(r.To));
        }

        [Test]
        public void Range_ToString()
        {
            var r = new PageRange(7, 11);
            Assert.That(r.ToString(), Is.EqualTo("7-11"));
        }
    }
}