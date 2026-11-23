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
using TUnit.Assertions;
using TUnit.Core;

namespace Mairegger.Printing.Tests.Definition
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK")]
    public class ColumnDimensionAttributeTests
    {
        public static readonly IEnumerable<double> Values = [2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

        [Test]
        [MethodDataSource(nameof(Values))]
        public async Task AbsoluteWidth_Test(double param)
        {
            double width = param * 100;
            var v = new ColumnDimensionAttribute(width, ColumnDimensionType.Pixels);
            using (Assert.Multiple())
            {
                await Assert.That(v.ColumnWidth).IsEqualTo(width);
                await Assert.That(v.DimensionType).IsEqualTo(ColumnDimensionType.Pixels);
            }
        }

        [Test]
        public async Task RelativeWidth_Px()
        {
            var v = new ColumnDimensionAttribute("2px");

            using (Assert.Multiple())
            {
                await Assert.That(v.ColumnWidth).IsEqualTo(2);
                await Assert.That(v.DimensionType).IsEqualTo(ColumnDimensionType.Pixels);
            }
        }

        [Test]
        public async Task RelativeWidth_Start()
        {
            var v = new ColumnDimensionAttribute("3*");

            using (Assert.Multiple())
            {
                await Assert.That(v.ColumnWidth).IsEqualTo(3);
                await Assert.That(v.DimensionType).IsEqualTo(ColumnDimensionType.Star);
            }
        }

        [Test]
        public void InvalidPrintDimension()
        {
            Assert.Throws<ArgumentException>(() => new ColumnDimensionAttribute(string.Empty));
        }

        [Test]
        [MethodDataSource(nameof(Values))]
        public void PercentageOfPage_NegativeValues_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(-param * 100, ColumnDimensionType.Pixels));
        }

        [Test]
        [MethodDataSource(nameof(Values))]
        public void RelativeWidth_OutOfRange_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(param));
        }

        [Test]
        [MethodDataSource(nameof(Values))]
        public async Task RelativeWidth_Test2(double param)
        {
            double width = 1 / param;
            var v = new ColumnDimensionAttribute(width);

            using (Assert.Multiple())
            {
                await Assert.That(v.ColumnWidth).IsEqualTo(width);
                await Assert.That(v.DimensionType).IsEqualTo(ColumnDimensionType.Star);
            }
        }
    }
}
