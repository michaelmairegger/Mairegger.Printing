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

namespace Mairegger.Printing.Tests.Content
{
    public class PrintQueueNameTests
    {
        [Test]
        [Arguments(@"\\server\queue", @"\\server", "queue")]
        [Arguments(@"\\server.domain.local\My Printer", @"\\server.domain.local", "My Printer")]
        [Arguments("Microsoft Print to PDF", null, "Microsoft Print to PDF")]
        [Arguments(@"\\server", null, @"\\server")]
        [Arguments(@"\\\queue", null, @"\\\queue")]
        public async Task SplitPrintQueueName(string printQueueName, string? expectedPrintServerName, string expectedQueueName)
        {
            var queueName = PrintProcessor.PrintProcessor.SplitPrintQueueName(printQueueName, out var printServerName);

            using (Assert.Multiple())
            {
                await Assert.That(queueName).IsEqualTo(expectedQueueName);
                await Assert.That(printServerName).IsEqualTo(expectedPrintServerName);
            }
        }
    }
}
