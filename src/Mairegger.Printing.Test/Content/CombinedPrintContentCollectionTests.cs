﻿// Copyright 2016 Michael Mairegger
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
            Mock<IPrintContent>[] m1 = [new (), new (), new (), new ()];

            foreach (var mock in m1)
            {
                mock.SetupGet(i => i.Content).Returns(new UIElement());
            }

            var collection = new CombinedPrintContentCollection(m1.Select(i => i.Object).ToArray());

            Assert.Multiple(() =>
            {
                Assert.That(collection.Content, Is.Not.Null);
                Assert.That(collection, Is.EqualTo(m1.Select(i => i.Object)).AsCollection);
            });

            foreach (var mock in m1)
            {
                mock.VerifyAll();
            }
        }
    }
}
