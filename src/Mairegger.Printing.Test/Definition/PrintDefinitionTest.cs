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
    public class PrintDefinitionTest
    {
        [Test]
        public async Task IsToPrint_CheckLastPage()
        {
            var pd = new PrintDefinition();
            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, true)).IsFalse();

            pd.SetPrintAttribute(
                new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, true)).IsTrue();
        }

        [Test]
        public void SetPrintAttributeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PrintDefinition().SetPrintAttribute(null!));
        }

        [Test]
        public async Task IsToPrint_CheckSinglePage()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(
                new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            using (Assert.Multiple())
            {
                // do not print on page #1
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();

                // but print on last page
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, true)).IsTrue();
            }
        }

        [Test]
        public async Task IsToPrint_ExcludeIsStrongerThanInclude()
        {
            var pd = new PrintDefinition();
            using (Assert.Multiple())
            {
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsFalse();
            }

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            using (Assert.Multiple())
            {
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsTrue();
            }

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            using (Assert.Multiple())
            {
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsFalse();
            }
        }

        [Test]
        public async Task IsToPrint_ExcludePage()
        {
            var pd = new PrintDefinition();
            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsFalse();

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsFalse();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsFalse();
        }

        [Test]
        public async Task IsToPrint_IncludePage()
        {
            var pd = new PrintDefinition();
            await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            using (Assert.Multiple())
            {
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false)).IsFalse();
                await Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false)).IsTrue();
            }
        }

        [Test]
        public void IsToPrint_InvalidArgumentException()
        {
            var pd = new PrintDefinition();
            Assert.Throws<ArgumentException>(() => pd.IsToPrint((PrintAppendixes)(-1), 1, false));
        }

        [Test]
        public void IsToPrint_NegativePage_ThrowsException()
        {
            var pd = new PrintDefinition();
            pd.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            Assert.Throws<ArgumentOutOfRangeException>(() => pd.IsToPrint(PrintAppendixes.Footer, -1, false));
        }

        [Test]
        public async Task SetPrintAttribute()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 1));

            await Assert.That(pd.IsDefined(PrintAppendixes.Footer)).IsTrue();
        }
    }
}
