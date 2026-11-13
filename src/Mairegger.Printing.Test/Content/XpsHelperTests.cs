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

using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using Mairegger.Printing.Content;
using Mairegger.Printing.PrintProcessor;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class XpsHelperTests
    {
        [Test, STAThreadExecutor]
        public async Task Concat_ContainsAllPagesOfAllDocuments()
        {
            var first = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var second = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var target = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                new PagesPrintProcessor(1) { PrintDialog = new CapturingPrintDialog() }.SaveToXps(first);
                new PagesPrintProcessor(2) { PrintDialog = new CapturingPrintDialog() }.SaveToXps(second);

                XpsHelper.Concat(target, first, second);

                using (var xpsDocument = new XpsDocument(target, FileAccess.Read))
                {
                    var documents = xpsDocument.GetFixedDocumentSequence().References.Select(r => r.GetDocument(false)).ToList();
                    var pages = documents.SelectMany(d => d.Pages).ToList();

                    using (Assert.Multiple())
                    {
                        await Assert.That(pages.Count).IsEqualTo(3);
                        await Assert.That(pages).All().Satisfy(p => p.GetPageRoot(false), root => root.IsNotNull());
                    }
                }
            }
            finally
            {
                File.Delete(first);
                File.Delete(second);
                File.Delete(target);
            }
        }

        private sealed class PagesPrintProcessor(int pageCount) : PrintProcessor.PrintProcessor
        {
            public override UIElement GetTable(out double reserveHeightOf, out Brush borderBrush)
            {
                reserveHeightOf = 0;
                borderBrush = Brushes.Transparent;
                return new UIElement();
            }

            public override IEnumerable<IPrintContent> ItemCollection()
            {
                for (var i = 0; i < pageCount; i++)
                {
                    if (i > 0)
                    {
                        yield return PrintContent.PageBreak();
                    }

                    yield return PrintContent.BlankLine(10);
                }
            }

            protected override void PreparePrint()
            {
                base.PreparePrint();
                PrintDimension.PageSize = new Size(500, 500);
            }
        }
    }
}
