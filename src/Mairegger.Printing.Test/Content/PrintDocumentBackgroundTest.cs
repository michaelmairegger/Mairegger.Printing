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
using System.Windows.Controls;
using Mairegger.Printing.Content;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintDocumentBackgroundTest
    {
        [Test, STAThreadExecutor]
        public async Task Ctor_Element()
        {
            Panel p = new StackPanel();
            var printDocumentBackground = new PrintDocumentBackground(p, Rect.Empty);
            await Assert.That(printDocumentBackground.Element).IsEqualTo(p);
        }

        [Test, STAThreadExecutor]
        public async Task Ctor_Size()
        {
            var size = new Rect(new Point(4, 4), new Size(10, 10));
            var printDocumentBackground = new PrintDocumentBackground(new StackPanel(), size);
            await Assert.That(printDocumentBackground.Size).IsEqualTo(size);
        }
    }
}
