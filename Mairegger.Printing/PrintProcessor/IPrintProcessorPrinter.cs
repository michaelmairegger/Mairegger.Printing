// Copyright 2015 Michael Mairegger
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

namespace Mairegger.Printing.PrintProcessor
{
    using System.Printing;

    public interface IPrintProcessorPrinter
    {
        void OnPageBreak();

        /// <summary>
        ///     Creates the document in order to provide a preview of the document
        /// </summary>
        void PreviewDocument();

        /// <summary>
        ///     Prints the document.
        /// </summary>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        bool PrintDocument();

        /// <summary>
        ///     Prints the document to the given <see cref="PrintServer" /> and the given <see cref="PrintQueue.Name" />
        /// </summary>
        /// <param name="printQueueName"> The name of the print queue. </param>
        /// <param name="printServer"> The print server to host the print queue. </param>
        /// ///
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        bool PrintDocument(string printQueueName, PrintServer printServer);

        /// <summary>
        ///     Prints the document to a <see cref="LocalPrintServer" /> and the given <see cref="PrintQueue.Name" /> of the print
        ///     queue.
        /// </summary>
        /// <param name="printQueueName"> The Print-server to print on. </param>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        bool PrintDocument(string printQueueName);
    }
}