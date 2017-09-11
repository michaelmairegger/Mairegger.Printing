// Copyright 2017 Michael Mairegger
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
    using System.IO;
    using System.Linq;
    using System.Printing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using JetBrains.Annotations;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.Internal;

    /// <summary>
    ///     Provides a class that processes a printing
    /// </summary>
    public abstract class PrintProcessor : IPrintProcessor
    {
        private static Action<IPrintDialog> _configurePrintDialog;
        private string _fileName = string.Empty;
        private IPrintDialog _printDialog;
        private PrintDimension _printDimension = new PrintDimension();

        protected PrintProcessor()
        {
            PrintDialog = new PrintDialogWrapper();
            _configurePrintDialog?.Invoke(PrintDialog);

            PageOrientation = PageOrientation.Portrait;
        }

        public virtual IList<SolidColorBrush> AlternatingRowColors { get; set; } = new[]
                                                                                   {
                                                                                       Brushes.Transparent,
                                                                                       Brushes.LightGray
                                                                                   };

        public bool ColorPrintPartsForDebug { get; set; }

        public int CurrentPage { get; set; }

        public string FileName
        {
            get => _fileName;
            set => _fileName = ReplaceInvalidCharsFromFilename(value);
        }

        public bool IsAlternatingRowColor { get; set; }

        public PageOrientation PageOrientation
        {
            get => PrintDialog.PrintTicket.PageOrientation.GetValueOrDefault();
            set => PrintDialog.PrintTicket.PageOrientation = value;
        }

        public PrintDefinition PrintDefinition { get; } = new PrintDefinition();

        public IPrintDialog PrintDialog
        {
            get => _printDialog;
            set => _printDialog = value ?? throw new ArgumentNullException(nameof(value));
        }

        public PrintDimension PrintDimension
        {
            get => _printDimension;
            set => _printDimension = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static bool PrintDocument(IPrintDialog printDialog, PrintProcessorCollection pp)
        {
            var pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            var fixedDocument = CreateDocument(pageSize, pp);

            printDialog.PrintDocument(fixedDocument.DocumentPaginator, pp.FileName);

            return true;
        }

        public virtual IEnumerable<IDirectPrintContent> GetCustomPageContent(int pageNumber)
        {
            yield break;
        }

        public virtual UIElement GetPageNumbers(int currentPage, int totalPages)
        {
            return new TextBlock
                   {
                       Text = $"{currentPage} | {totalPages}",
                       TextAlignment = TextAlignment.Center,
                   };
        }

        /// <summary>
        /// Sets the global configuration of the <see cref="IPrintDialog"/>. This action is applied before each print.
        /// </summary>
        /// <param name="configuration"></param>
        public static void ConfigurePrintDialog(Action<IPrintDialog> configuration)
        {
            _configurePrintDialog = configuration;
        }

        public virtual PrintDocumentBackground GetBackgound()
        {
            return null;
        }

        public virtual UIElement GetFooter()
        {
            return null;
        }

        public virtual UIElement GetHeader()
        {
            return null;
        }

        public virtual UIElement GetHeaderDescription()
        {
            return null;
        }

        public virtual UIElement GetSummary()
        {
            return null;
        }

        public abstract UIElement GetTable(out double reserveHeightOf, out Brush borderBrush);

        public abstract IEnumerable<IPrintContent> ItemCollection();

        public virtual void OnPageBreak()
        {
        }

        public void PreviewDocument(IWindowProvider windowsProvider = null)
        {
            PreviewDocument(new PrintProcessorCollection(this), windowsProvider);
        }

        public bool PrintDocument(string printQueueName, PrintServer printServer)
        {
            PrintDialog.PrintQueue = new PrintQueue(printServer, printQueueName);

            return PrintDocument(PrintDialog);
        }

        public bool PrintDocument(string printQueueName)
        {
            using (var printServer = new LocalPrintServer())
            {
                return PrintDocument(printQueueName, printServer);
            }
        }

        public virtual bool PrintDocument()
        {
            if (PrintDialog.ShowDialog().Equals(false))
            {
                return false;
            }

            PrintDocument(PrintDialog);

            return true;
        }

        public void SaveToXps(string file)
        {
            var pd = PrintDialog;
            var fixedDocument = CreateDocument(new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight), new PrintProcessorCollection(this));
            XpsHelper.SaveFixedDocument(fixedDocument, file);
        }

        internal static void PreviewDocument(PrintProcessorCollection ppc, IWindowProvider windowsProvider = null)
        {
            if (!ppc.Any())
            {
                return;
            }
            var pd = ppc.First().PrintDialog;

            var fixedDocument = CreateDocument(new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight), ppc);

            XpsHelper.ShowFixedDocument(fixedDocument, ppc.FileName, windowsProvider);
        }

        [NotNull]
        internal static string ReplaceInvalidCharsFromFilename(string path)
        {
            if (path == null)
            {
                return string.Empty;
            }

            var invalidFileNameChars = Path.GetInvalidFileNameChars().Union(new[]
                                                                            {
                                                                                '.'
                                                                            }).ToList();
            return path.Aggregate(
                new StringBuilder(),
                (sb, c) =>
                {
                    if (invalidFileNameChars.Contains(c))
                    {
                        return sb;
                    }
                    return sb.Append(c);
                }).ToString();
        }

        protected virtual void PreparePrint()
        {
        }

        protected virtual bool PrintDocument(IPrintDialog printDialog)
        {
            return PrintDocument(printDialog, new PrintProcessorCollection(this));
        }

        private static FixedDocument CreateDocument(Size pageSize, PrintProcessorCollection p)
        {
            if (p != null)
            {
                for (var index = 0; index < p.Count; index++)
                {
                    var printProcessor = p[index];

                    if ((index > 0) && p.IndividualPageOrientation)
                    {
                        if (((pageSize.Width > pageSize.Height) && (printProcessor.PageOrientation == PageOrientation.Portrait)) ||
                            ((pageSize.Height > pageSize.Width) && (printProcessor.PageOrientation == PageOrientation.Landscape)))
                        {
                            pageSize = new Size(pageSize.Height, pageSize.Width);
                        }
                    }

                    printProcessor.SetPrintOnAttributes();
                    printProcessor.Prepare(pageSize);
                }
            }

            var internalPrintProcessor = new InternalPrintProcessor();
            return internalPrintProcessor.CreateFixedDocument(p);
        }

        private void Prepare(Size pageSize)
        {
            PrintDimension.PrintProcessor = this;
            PrintDimension.PageSize = pageSize;
            PreparePrint();
            PrintDimension.PositionizeRelative();
        }

        private void SetPrintOnAttributes()
        {
            var customPageAttributes = (IPrintPartDefinition[])GetType().GetCustomAttributes(typeof(IPrintPartDefinition), true);

            foreach (var printOnAttribute in customPageAttributes)
            {
                PrintDefinition.SetPrintAttribute(printOnAttribute);
            }
            PrintDimension.InternalPrintDefinition = PrintDefinition;
        }
    }
}