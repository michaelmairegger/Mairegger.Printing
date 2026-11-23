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

using Mairegger.Printing.Definition;

namespace Mairegger.Printing.Tests.Definition
{
    public class PrintOnPageTests
    {
        [Test]
        public async Task Ctor()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, new PageRange(2, 5));

            using (Assert.Multiple())
            {

                await Assert.That(attribute.GetPrintDefinition(1)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(2)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(3)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(4)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(5)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(6)).IsEqualTo(PrintPartStatus.NotDefined);
            }
        }

        [Test]
        public async Task Ctor1()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 1, 3, 5);

            using (Assert.Multiple())
            {

                await Assert.That(attribute.GetPrintDefinition(1)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(2)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(3)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(4)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(5)).IsEqualTo(PrintPartStatus.Include);
            }
        }

        [Test]
        public async Task Ctor2()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 3);

            using (Assert.Multiple())
            {

                await Assert.That(attribute.GetPrintDefinition(1)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(2)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(3)).IsEqualTo(PrintPartStatus.Include);
                await Assert.That(attribute.GetPrintDefinition(4)).IsEqualTo(PrintPartStatus.NotDefined);
                await Assert.That(attribute.GetPrintDefinition(5)).IsEqualTo(PrintPartStatus.NotDefined);
            }
        }
    }
}
