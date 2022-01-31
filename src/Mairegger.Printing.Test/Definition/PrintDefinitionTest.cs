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
    using Mairegger.Printing.Definition;
    using NUnit.Framework;

    [TestFixture]
    public class PrintDefinitionTest
    {
        [Test]
        public void IsToPrint_CheckLastPage()
        {
            var pd = new PrintDefinition();
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, true), Is.False);

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, true), Is.True);
        }

        [Test]
        public void IsToPrint_CheckSinglePage()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, PrintPartDefinitionAttribute.LastPage));

            // do not print on page #1
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);

            // but print on last page
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, true), Is.True);
        }

        [Test]
        public void IsToPrint_ExcludeIsStrongerThanInclude()
        {
            var pd = new PrintDefinition();
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.False);

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.True);

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.False);
        }

        [Test]
        public void IsToPrint_ExcludePage()
        {
            var pd = new PrintDefinition();
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.False);

            pd.SetPrintAttribute(new ExcludeFromPageAttribute(PrintAppendixes.Footer, 2));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.False);

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.False);
        }

        [Test]
        public void IsToPrint_IncludePage()
        {
            var pd = new PrintDefinition();
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 2));

            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 1, false), Is.False);
            Assert.That(pd.IsToPrint(PrintAppendixes.Footer, 2, false), Is.True);
        }

        [Test]
        public void IsToPrint_InvalidArgumentException()
        {
            var pd = new PrintDefinition();
            Assert.That(() => pd.IsToPrint((PrintAppendixes)(-1), 1, false), Throws.ArgumentException);
        }

        [Test]
        public void IsToPrint_NegativePage_ThrowsException()
        {
            var pd = new PrintDefinition();
            pd.SetPrintAttribute(new PrintOnAllPagesAttribute(PrintAppendixes.Footer));
            Assert.That(() => pd.IsToPrint(PrintAppendixes.Footer, -1, false), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void SetPrintAttribute()
        {
            var pd = new PrintDefinition();

            pd.SetPrintAttribute(new PrintOnPageAttribute(PrintAppendixes.Footer, 1));

            Assert.That(pd.IsDefined(PrintAppendixes.Footer), Is.True);
        }
    }
}