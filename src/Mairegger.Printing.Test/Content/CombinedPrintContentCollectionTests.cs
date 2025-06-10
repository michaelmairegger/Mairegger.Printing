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


using System.Windows;
using Mairegger.Printing.Content;

namespace Mairegger.Printing.Tests.Content
{
    public class CombinedPrintContentCollectionTests
    {
        [WpfFact]
        public void Content()
        {
            Mock<IPrintContent>[] m1 = [new (), new (), new (), new ()];

            foreach (var mock in m1)
            {
                mock.SetupGet(i => i.Content).Returns(new UIElement());
            }

            var collection = new CombinedPrintContentCollection(m1.Select(i => i.Object).ToArray());

            Assert.Multiple(
                ()=> Assert.NotNull(collection.Content),
                ()=> Assert.Equal(m1.Select(i => i.Object), collection));

            foreach (var mock in m1)
            {
                mock.VerifyAll();
            }
        }
    }
}
