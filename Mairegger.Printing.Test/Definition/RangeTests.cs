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

namespace Mairegger.Printing.Test.Definition
{
    #region Using Directives

    using System;
    using Mairegger.Printing.Definition;
    using Microsoft.CSharp.RuntimeBinder;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void IsInRange_InRange_ReturnsTrue()
        {
            var r1 = new Range<double>(5, 10);
            var r2 = new Range<double>(6, 10);

            Assert.True(r1.IsInRange(r2));
        }

        [Test]
        public void IsInRange_InRange_True([Values(1, 2, 3, 4, 5)] double value)
        {
            var r = new Range<double>(1, 100);
            Assert.IsTrue(r.IsInRange(value));
        }

        [Test]
        public void IsInRange_NotInRange_ReturnsFalse()
        {
            var r1 = new Range<double>(5, 10);
            var r2 = new Range<double>(6, 10);

            Assert.False(r2.IsInRange(r1));
        }

        [Test]
        public void IsInRange_OutOfRange_False([Values(1, 2, 3, 4, 5)] double value)
        {
            var r = new Range<double>(6, 10);
            Assert.IsFalse(r.IsInRange(value));
        }

        [Test]
        public void IsInRange_SameRange_ReturnsTrue()
        {
            var r1 = new Range<double>(5, 10);

            Assert.True(r1.IsInRange(r1));
        }

        [Test]
        public void Length([Values(1, 2, 3, 4, 5)] double length)
        {
            var r = new Range<double>(0, length);
            Assert.AreEqual(r.Length, length);
        }

        [Test]
        public void Parse_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Range.Parse(null));
            var input = string.Empty;
            Assert.Throws<ArgumentException>(() => Range.Parse(input));
            Assert.Throws<ArgumentException>(() => Range.Parse("4"));
            Assert.Throws<ArgumentException>(() => Range.Parse(","));
            Assert.Throws<FormatException>(() => Range.Parse("InvalidNumber,6"));
            Assert.Throws<FormatException>(() => Range.Parse("6,InvalidNumber"));

            Assert.Throws<ArgumentOutOfRangeException>(() => Range.Parse("6,4"));
        }

        [Test]
        public void Parse_ValidRange()
        {
            var r = Range.Parse("4,6");
            Assert.That(r.From, Is.EqualTo(4));
            Assert.That(r.To, Is.EqualTo(6));
        }

        [Test]
        public void Range_Equals()
        {
            var r1 = new Range<double>(5, 10);
            var r2 = new Range<double>(5, 10);

            Assert.That(r1, Is.EqualTo(r2));
            Assert.True(r1 == r2);
            Assert.True(Equals(r1, r2));
            Assert.That(r1.GetHashCode(), Is.EqualTo(r2.GetHashCode()));
        }

        [Test]
        public void Range_FromPoint_ToString()
        {
            var r = Range.FromPoint<double>(5);
            Assert.That(r.ToString(), Is.EqualTo("5-5"));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        public void Range_MinEqualsMax_Valie(double min, double max)
        {
            Assert.DoesNotThrow(() => new Range<double>(min, max));
        }

        [Test]
        public void Range_MinGreatherMax_Valie(
            [Values(6, 7, 8, 9, 10)] double min,
            [Values(1, 2, 3, 4, 5)] double max)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Range<double>(min, max));
        }

        [Test]
        public void Range_MinLessMax_Valie(
            [Values(1, 2, 3, 4, 5)] double min,
            [Values(6, 7, 8, 9, 10)] double max)
        {
            Assert.DoesNotThrow(() => new Range<double>(min, max));
        }

        [Test]
        public void Range_Minus_ClassImplementingMinus()
        {
            var r = Range.FromPoint(new ClassWithMinusOperatorOverloading());

            Assert.DoesNotThrow(() =>
                                {
                                    var i = r.Length;
                                });
        }

        [Test]
        public void Range_Minus_ClassNotImplementingMinus_ThrowsRuntimeBinderException()
        {
            var r = Range.FromPoint(new ClassWithoutMinusOperatorOverloading());

            Assert.That(() => r.Length, Throws.TypeOf<RuntimeBinderException>());
        }

        [Test]
        public void Range_NotEquals()
        {
            var r1 = new Range<double>(5, 10);
            var r2 = new Range<double>(6, 10);

            Assert.That(r1, Is.Not.EqualTo(r2));

            Assert.True(r1 != r2);

            Assert.False(Equals(r1, r2));
            Assert.False(r1.Equals(null));
            Assert.That(r1, Is.Not.EqualTo(null));
            Assert.False(r1.Equals(5));
            Assert.That(r1.GetHashCode(), Is.Not.EqualTo(r2.GetHashCode()));
        }

        [Test]
        public void Range_SinglePoint()
        {
            var r = Range.FromPoint(5);

            Assert.That(r.From, Is.EqualTo(r.To));
        }

        [Test]
        public void Range_ToString()
        {
            var r = new Range<double>(7, 11);
            Assert.That(r.ToString(), Is.EqualTo("7-11"));
        }

        public class ClassWithMinusOperatorOverloading : ClassWithoutMinusOperatorOverloading
        {
            public static ClassWithMinusOperatorOverloading operator -(ClassWithMinusOperatorOverloading left, ClassWithMinusOperatorOverloading right)
            {
                return new ClassWithMinusOperatorOverloading();
            }
        }

        public class ClassWithoutMinusOperatorOverloading : IComparable<ClassWithoutMinusOperatorOverloading>
        {
            public int CompareTo(ClassWithoutMinusOperatorOverloading other)
            {
                return 0;
            }
        }
    }
}