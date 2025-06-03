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

using Mairegger.Printing.Definition;

namespace Mairegger.Printing.Tests.Definition
{
    public class PrintOnPageTests
    {
        [Fact]
        public void Ctor()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, new PageRange(2, 5));

            Assert.Multiple(
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(1)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(2)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(3)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(4)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(5)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(6)));
        }

        [Fact]
        public void Ctor1()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 1, 3, 5);

            Assert.Multiple(
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(1)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(2)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(3)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(4)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(5))
                );
        }

        [Fact]
        public void Ctor2()
        {
            var attribute = new PrintOnPageAttribute(PrintAppendixes.All, 3);

            Assert.Multiple(
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(1)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(2)),
                ()=> Assert.Equal(PrintPartStatus.Include, attribute.GetPrintDefinition(3)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(4)),
                ()=> Assert.Equal(PrintPartStatus.NotDefined, attribute.GetPrintDefinition(5)));
        }
    }
}
