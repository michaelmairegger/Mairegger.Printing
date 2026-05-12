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


using System.Windows;
using Mairegger.Printing.Content;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class CombinedPrintContentCollectionTests
    {
        // [Test, STAThreadExecutor]
        // public async Task Content()
        // {
        //     IPrintContentMock[] m1 = [IPrintContent.Mock(), IPrintContent.Mock(), IPrintContent.Mock(), IPrintContent.Mock()];
        //
        //     foreach (var mock in m1)
        //     {
        //         mock.Content.Returns(new UIElement());
        //     }
        //
        //     var collection = new CombinedPrintContentCollection(m1.Select(i => i.Object).ToArray());
        //
        //     using (Assert.Multiple())
        //     {
        //         await Assert.That(collection.Content).IsNotNull();
        //         await Assert.That(m1.Select(i => i.Object)).IsEquivalentTo(collection);
        //     }
        //
        //     foreach (var mock in m1)
        //     {
        //         ((IMock)mock).VerifyAll();
        //     }
        // }
    }
}
