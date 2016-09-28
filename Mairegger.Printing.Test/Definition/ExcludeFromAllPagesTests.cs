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
    #region Using Directives

    using Mairegger.Printing.Definition;
    using NUnit.Framework;

    #endregion

    [TestFixture]
    public class ExcludeFromAllPagesTests
    {
        [Test]
        public void Ctor([Random(1, int.MaxValue, 10)] int page)
        {
            var attribute = new ExcludeFromAllPagesAttribute(PrintAppendixes.All);

            Assert.That(attribute.GetPrintDefinition(page), Is.EqualTo(PrintPartStatus.Exclude));
        }
    }
}