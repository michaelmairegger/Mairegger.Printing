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

namespace Mairegger.Printing.Tests.Content
{
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using Mairegger.Printing.Content;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class CombinedPrintContentCollectionTests
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Content()
        {
            Mock<IPrintContent>[] m1 =
            {
                new Mock<IPrintContent>(),
                new Mock<IPrintContent>(),
                new Mock<IPrintContent>(),
                new Mock<IPrintContent>()
            };

            foreach (var mock in m1)
            {
                mock.SetupGet(i => i.Content).Returns(new UIElement());
            }

            CombinedPrintContentCollection collection = new CombinedPrintContentCollection(m1.Select(i => i.Object).ToArray());

            Assert.That(collection.Content, Is.Not.Null);
            CollectionAssert.AreEqual(m1.Select(i => i.Object), collection);

            foreach (var mock in m1)
            {
                mock.VerifyAll();
            }
        }

        [Test]
        public void Ctor_Null_ThrowsArgumentNullException()
        {
            Assert.That(() => new CombinedPrintContentCollection(null), Throws.ArgumentNullException);
        }
    }
}