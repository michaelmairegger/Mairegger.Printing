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

using System.Windows.Controls;
using Mairegger.Printing.PrintProcessor;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PreviewPrintDialogTests
    {
        [Test, STAThreadExecutor]
        public async Task PreviewDocument_DocumentViewerUsesPrintDialogOfPrintProcessor()
        {
            var printDialog = new CapturingPrintDialog();
            var printProcessor = new TestPrintProcessor { PrintDialog = printDialog };
            var windowProvider = new RecordingWindowProvider();

            printProcessor.PreviewDocument(windowProvider);

            await Assert.That(windowProvider.DocumentViewer).IsTypeOf<CustomDocumentViewer>();
            await Assert.That(((CustomDocumentViewer)windowProvider.DocumentViewer!).PrintDialog).IsSameReferenceAs(printDialog);
        }

        private sealed class RecordingWindowProvider : IWindowProvider
        {
            public event EventHandler? Closed
            {
                add { }
                remove { }
            }

            public DocumentViewer? DocumentViewer { get; private set; }

            public void Show(string windowTitle, DocumentViewer documentViewer)
            {
                DocumentViewer = documentViewer;
            }
        }
    }
}
