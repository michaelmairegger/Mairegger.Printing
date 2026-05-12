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

using Bogus;
using Mairegger.Printing.Definition;

namespace Mairegger.Printing.Tests.Definition
{
    public class PrintOnAllPagesTests
    {
        private static readonly Faker s_faker = new();
        public static IEnumerable<int> GetRandomList()
        {
            for (int i = 1; i <= 10; i++)
            {
                yield return s_faker.Random.Int(1);
            }
        }

        [Test]
        [MethodDataSource(nameof(GetRandomList))]
        public async Task Ctor(int page)
        {
            var attribute = new PrintOnAllPagesAttribute(PrintAppendixes.All);

            await Assert.That(attribute.GetPrintDefinition(page)).IsEqualTo(PrintPartStatus.Include);
        }
    }
}
