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
    using Mairegger.Printing.Definition;
    using NUnit.Framework;

    [TestFixture]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "OK")]
    public class ColumnDimensionAttributeTests
    {
        private static readonly double[] Values =
        {
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11
        };

        [Test]
        [TestCaseSource(nameof(Values))]
        public void AbsoluteWidth_Test(double param)
        {
            double width = param * 100;
            var v = new ColumnDimensionAttribute(width, ColumnDimensionType.Pixels);
            Assert.Multiple(() =>
            {
                Assert.That(v.ColumnWidth, Is.EqualTo(width));
                Assert.That(v.DimensionType, Is.EqualTo(ColumnDimensionType.Pixels));
            });
        }

        [Test]
        public void RelativeWidth_Px()
        {
            var v = new ColumnDimensionAttribute("2px");
            Assert.Multiple(() =>
            {
                Assert.That(v.DimensionType, Is.EqualTo(ColumnDimensionType.Pixels));
                Assert.That(v.ColumnWidth, Is.EqualTo(2));
            });
        }

        [Test]
        public void RelativeWidth_Start()
        {
            var v = new ColumnDimensionAttribute("3*");
            Assert.Multiple(() =>
            {
                Assert.That(v.DimensionType, Is.EqualTo(ColumnDimensionType.Star));
                Assert.That(v.ColumnWidth, Is.EqualTo(3));
            });
        }

        [Test]
        public void InvalidPrintDimension()
        {
            Assert.Throws<ArgumentException>(() => new ColumnDimensionAttribute(string.Empty));
        }

        [Test]
        [TestCaseSource(nameof(Values))]
        public void PercentageOfPage_NegativeValues_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(-param * 100, ColumnDimensionType.Pixels));
        }

        [Test]
        [TestCaseSource(nameof(Values))]
        public void RelativeWidth_OutOfRange_Fail(double param)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ColumnDimensionAttribute(param));
        }

        [Test]
        [TestCaseSource(nameof(Values))]
        public void RelativeWidth_Test2(double param)
        {
            double width = 1 / param;
            var v = new ColumnDimensionAttribute(width);
            Assert.Multiple(() =>
            {
                Assert.That(v.ColumnWidth, Is.EqualTo(width));
                Assert.That(v.DimensionType, Is.EqualTo(ColumnDimensionType.Star));
            });
        }
    }
}
