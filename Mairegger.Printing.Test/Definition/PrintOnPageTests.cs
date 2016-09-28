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
    using Mairegger.Printing.Definition;
    using NUnit.Framework;

    [TestFixture]
    public class PrintOnPageTests
    {
        [Test]
        public void Ctor()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, new Range<int>(2, 5));

            Assert.That(attribute.GetPrintDefinition(1), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(2), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(3), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(4), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(5), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(6), Is.EqualTo(PrintPartStatus.NotDefined));
        }

        [Test]
        public void Ctor1()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 1, 3, 5);

            Assert.That(attribute.GetPrintDefinition(1), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(2), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(3), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(4), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(5), Is.EqualTo(PrintPartStatus.Include));
        }

        [Test]
        public void Ctor2()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 3);

            Assert.That(attribute.GetPrintDefinition(1), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(2), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(3), Is.EqualTo(PrintPartStatus.Include));
            Assert.That(attribute.GetPrintDefinition(4), Is.EqualTo(PrintPartStatus.NotDefined));
            Assert.That(attribute.GetPrintDefinition(5), Is.EqualTo(PrintPartStatus.NotDefined));
        }
    }
}