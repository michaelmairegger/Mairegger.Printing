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

using System.Diagnostics.CodeAnalysis;
using Mairegger.Printing.Definition;
using Mairegger.Printing.Tests.Content;
using System.Threading.Tasks;
using Bogus;

namespace Mairegger.Printing.Tests.Definition
{
    public class RangeTests
    {
        [Test]
        public async Task IsInRange_InRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            await Assert.That(r1.IsInRange(r2)).IsTrue();
        }

        [Test]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(4)]
        [Arguments(5)]
        public async Task IsInRange_InRange_True(double value)
        {
            var r = new PageRange(1, 100);
            await Assert.That(r.IsInRange(value)).IsTrue();
        }

        [Test]
        public async Task IsInRange_NotInRange_ReturnsFalse()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            await Assert.That(r2.IsInRange(r1)).IsFalse();
        }

        [Test]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(4)]
        [Arguments(5)]
        public async Task IsInRange_OutOfRange_False(double value)
        {
            var r = new PageRange(6, 10);
            await Assert.That(r.IsInRange(value)).IsFalse();
        }

        [Test]
        public async Task IsInRange_SameRange_ReturnsTrue()
        {
            var r1 = new PageRange(5, 10);

            await Assert.That(r1.IsInRange(r1)).IsTrue();
        }

        [Test]
        [Arguments(1)]
        [Arguments(2)]
        [Arguments(3)]
        [Arguments(4)]
        [Arguments(5)]
        public async Task Length(double length)
        {
            var r = new PageRange(0, length);
            await Assert.That(r.Length).EqualTo(length);
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
        public async Task Parse_ValidRange()
        {
            var r = PageRange.Parse("4-6");

            using (Assert.Multiple())
            {
                await Assert.That(r.From).EqualTo(4);
                await Assert.That(r.To).EqualTo(6);
            }
        }

        [Test]
        public async Task ParseRanges()
        {
            var ranges = PageRange.ParseRanges("1-2,5,10-11").ToList();

            await Assert.That(ranges).Contains(PageRange.Parse("1-2"));
            await Assert.That(ranges).Contains(PageRange.Parse("5"));
            await Assert.That(ranges).Contains(PageRange.Parse("10-11"));
        }

        [Test]
        public async Task Range_Equals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(5, 10);

            await Assert.That(r2).EqualTo(r1);
            await Assert.That(r2.GetHashCode()).EqualTo(r1.GetHashCode());
        }

        [Test]
        public async Task Range_FromPoint_ToString()
        {
            var r = PageRange.FromPoint(5);
            await Assert.That("5-5").EqualTo(r.ToString());
        }

        [Test]
        [Arguments(1, 1)]
        [Arguments(2, 2)]
        [Arguments(3, 3)]
        [Arguments(4, 4)]
        [Arguments(5, 5)]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public async Task Range_MinEqualsMax_Valie(double min, double max)
        {
            await Assert.That(() => new PageRange(min, max)).ThrowsNothing();
        }


        private static readonly Faker s_faker = new();
        public static IEnumerable<(int, int)> Range_MinGreatherMax_ValieRandomList()
        {
            for (int i = 1; i <= 5; i++)
            {
                yield return (s_faker.Random.Int(10, 20), s_faker.Random.Int(0,10));
            }
        }

        public static IEnumerable<(int, int)> Range_MinLessMax_ValieRandomList()
        {
            for (int i = 1; i <= 5; i++)
            {
                yield return (s_faker.Random.Int(0, 10), s_faker.Random.Int(10,20));
            }
        }

        [Test]
        [MethodDataSource(nameof(Range_MinGreatherMax_ValieRandomList))]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public async Task Range_MinGreatherMax_Valie(double min, double max)
        {
            await Assert.That(() => new PageRange(min, max)).Throws<ArgumentOutOfRangeException>();
        }

        [Test]
        [MethodDataSource(nameof(Range_MinLessMax_ValieRandomList))]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK for UnitTests")]
        public async Task Range_MinLessMax_Valie(double min, double max)
        {
            await Assert.That(() => new PageRange(min, max)).ThrowsNothing();
        }

        [Test]
        public async Task Range_NotEquals()
        {
            var r1 = new PageRange(5, 10);
            var r2 = new PageRange(6, 10);

            await Assert.That(r2.GetHashCode()).IsNotEqualTo(r1.GetHashCode());
        }

        [Test]
        public async Task Range_SinglePoint()
        {
            var r = PageRange.FromPoint(5);

            await Assert.That(r.To).EqualTo(r.From);
        }

        [Test]
        public async Task Range_ToString()
        {
            var r = new PageRange(7, 11);
            await Assert.That(r.ToString()).EqualTo("7-11");
        }
    }
}
