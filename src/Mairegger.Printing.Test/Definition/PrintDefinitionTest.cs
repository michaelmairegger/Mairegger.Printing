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
        [Fact]
        public void IsToPrint_CheckLastPage()
        {
            var pd = new PrintDefinition();
            Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, true));

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            Assert.True(pd.IsToPrint(PrintAppendixes.Footer, 2, true));
        }

        [Fact]
        public void SetPrintAttributeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new PrintDefinition().SetPrintAttribute(null!));
        }

        [Fact]
        public void IsToPrint_CheckSinglePage()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            Assert.Multiple(
                // do not print on page #1
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false)),

                // but print on last page
                () => Assert.True(pd.IsToPrint(PrintAppendixes.Footer, 1, true))
                );
        }

        [Fact]
        public void IsToPrint_ExcludeIsStrongerThanInclude()
        {
            var pd = new PrintDefinition();
            Assert.Multiple(
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false)),
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, false)));

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.Multiple(
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false)),
                () => Assert.True(pd.IsToPrint(PrintAppendixes.Footer, 2, false)));

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            Assert.Multiple(
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false)),
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, false)));
        }

        [Fact]
        public void IsToPrint_ExcludePage()
        {
            var pd = new PrintDefinition();
            Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, false));

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, false));

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 2, false));
        }

        [Fact]
        public void IsToPrint_IncludePage()
        {
            var pd = new PrintDefinition();
            Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false));

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.Multiple(
                () => Assert.False(pd.IsToPrint(PrintAppendixes.Footer, 1, false)),
                () => Assert.True(pd.IsToPrint(PrintAppendixes.Footer, 2, false)));
        }

        [Fact]
        public void IsToPrint_InvalidArgumentException()
        {
            var pd = new PrintDefinition();
            Assert.Throws<ArgumentException>(() => pd.IsToPrint((PrintAppendixes)(-1), 1, false));
        }

        [Fact]
        public void IsToPrint_NegativePage_ThrowsException()
        {
            var pd = new PrintDefinition();
            pd.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            Assert.Throws<ArgumentOutOfRangeException>(() => pd.IsToPrint(PrintAppendixes.Footer, -1, false));
        }

        [Fact]
        public void SetPrintAttribute()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 1));

            Assert.True(pd.IsDefined(PrintAppendixes.Footer));
        }
    }
}
