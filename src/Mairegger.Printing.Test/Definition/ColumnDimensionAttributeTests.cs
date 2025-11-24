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

namespace Mairegger.Printing.Tests.Definition
{
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK")]
    public class ColumnDimensionAttributeTests
    {
        public static readonly IEnumerable<TheoryDataRow<double>> Values = [2, 3, 4, 5, 6, 7, 8, 9, 10, 11];

        [Theory]
        [MemberData(nameof(Values))]
        public void AbsoluteWidth_Test(double param)
        {
            double width = param * 100;
            var v = new ColumnDimensionAttribute(width, ColumnDimensionType.Pixels);
            Assert.Multiple(
                () => Assert.Equal(width, v.ColumnWidth),
                () => Assert.Equal(ColumnDimensionType.Pixels, v.DimensionType));
        }

        [Fact]
        public void RelativeWidth_Px()
        {
            var v = new ColumnDimensionAttribute("2px");
            Assert.Multiple(
                () => Assert.Equal(2, v.ColumnWidth),
                () => Assert.Equal(ColumnDimensionType.Pixels, v.DimensionType));
        }

        [Fact]
        public void RelativeWidth_Start()
        {
            var v = new ColumnDimensionAttribute("3*");
            Assert.Multiple(
                () => Assert.Equal(3, v.ColumnWidth),
                () => Assert.Equal(ColumnDimensionType.Star, v.DimensionType));
        }

        [Fact]
        public void InvalidPrintDimension()
        {
            Assert.Throws<ArgumentException>(() => new ColumnDimensionAttribute(string.Empty));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void PercentageOfPage_NegativeValues_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(-param * 100, ColumnDimensionType.Pixels));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void RelativeWidth_OutOfRange_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(param));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void RelativeWidth_Test2(double param)
        {
            double width = 1 / param;
            var v = new ColumnDimensionAttribute(width);
            Assert.Multiple(
                () => Assert.Equal(width, v.ColumnWidth),
                () => Assert.Equal(ColumnDimensionType.Star, v.DimensionType));
        }
    }
}
