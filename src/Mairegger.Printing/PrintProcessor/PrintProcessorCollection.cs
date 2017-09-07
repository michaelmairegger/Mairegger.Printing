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

namespace Mairegger.Printing.PrintProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Printing;
    using JetBrains.Annotations;

    public class PrintProcessorCollection : Collection<PrintProcessor>, IPrintProcessorPrinter
    {
        private string _fileName = string.Empty;

        public PrintProcessorCollection([NotNull] PrintProcessor printProcessor)
            : this(new List<PrintProcessor> { printProcessor })
        {
            if (printProcessor == null)
            {
                throw new ArgumentNullException(nameof(printProcessor));
            }

            _fileName = printProcessor.FileName;
        }

        public PrintProcessorCollection([NotNull] IEnumerable<PrintProcessor> coll, string fileName = "")
            : this(new List<PrintProcessor>(coll), fileName)
        {
        }

        public PrintProcessorCollection([NotNull] IList<PrintProcessor> coll, string fileName = "")
        {
            if (coll == null)
            {
                throw new ArgumentNullException(nameof(coll));
            }

            FileName = fileName;
            foreach (var printProcessor in coll)
            {
                if (printProcessor != null)
                {
                    Add(printProcessor);
                }
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = PrintProcessor.ReplaceInvalidCharsFromFilename(value); }
        }

        /// <summary>
        ///     Sets whether for each <see cref="PrintProcessor" /> in <see cref="Collection{T}.Items" /> the page
        ///     numbers begins with 0. The default value is true.
        /// </summary>
        public bool IndividualPageNumbers { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether each <see cref="PrintProcessor" /> uses its own <see cref="PrintProcessor.PageOrientation" />.
        ///     If false the first <see cref="PrintProcessor.PageOrientation" /> of the first <see cref="PrintProcessor" /> is
        ///     used. The default value is true.
        /// </summary>
        public bool IndividualPageOrientation { get; set; } = true;

        public virtual void OnPageBreak()
        {
        }

        /// <summary>
        ///     Creates the document in order to provide a preview of the document
        /// </summary>
        public void PreviewDocument(IWindowProvider windowsProvider = null)
        {
            PrintProcessor.PreviewDocument(this, windowsProvider);
        }

        /// <summary>
        ///     Prints the document.
        /// </summary>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument()
        {
            if (Count == 0)
            {
                return false;
            }

            var p = this.First();
            IPrintDialog pd = p.PrintDialog;

            if (pd.ShowDialog().Equals(false))
            {
                return false;
            }

            return PrintProcessor.PrintDocument(pd, this);
        }

        /// <summary>
        ///     Prints the document to the given <see cref="PrintServer" /> and the given <see cref="PrintQueue.Name" />
        /// </summary>
        /// <param name="printQueueName"> The name of the print queue. </param>
        /// <param name="printServer"> The print server to host the print queue. </param>
        /// ///
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument(string printQueueName, PrintServer printServer)
        {
            if (Count == 0)
            {
                return false;
            }

            var p = this.First();
            IPrintDialog pd = p.PrintDialog;
            pd.PrintQueue = new PrintQueue(printServer, printQueueName);

            return PrintProcessor.PrintDocument(pd, this);
        }

        /// <summary>
        ///     Prints the document to a <see cref="LocalPrintServer" /> and the given <see cref="PrintQueue.Name" /> of the print
        ///     queue.
        /// </summary>
        /// <param name="printQueueName"> The Print-server to print on. </param>
        /// <returns> True if succeeds, false otherwise, or if the use cancels the print process. </returns>
        public bool PrintDocument(string printQueueName)
        {
            using (var printServer = new LocalPrintServer())
            {
                return PrintDocument(printQueueName, printServer);
            }
        }
    }
}