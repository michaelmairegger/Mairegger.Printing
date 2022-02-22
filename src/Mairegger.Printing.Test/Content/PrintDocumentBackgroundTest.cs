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
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using Mairegger.Printing.Content;
    using NUnit.Framework;

    [TestFixture]
    public class PrintDocumentBackgroundTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void Ctor_Element()
        {
            Panel p = new StackPanel();
            var printDocumentBackground = new PrintDocumentBackground(p, Rect.Empty);
            Assert.That(printDocumentBackground.Element, Is.EqualTo(p));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Ctor_Size()
        {
            var size = new Rect(new Point(4, 4), new Size(10, 10));
            var printDocumentBackground = new PrintDocumentBackground(new StackPanel(), size);
            Assert.That(printDocumentBackground.Size, Is.EqualTo(size));
        }
    }
}