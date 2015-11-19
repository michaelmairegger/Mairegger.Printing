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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Printing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.Internal;

    /// <summary>
    ///     Provides a class that processes a printing
    /// </summary>
    public abstract class PrintProcessor : IPrintProcessor
    {
        private PrintDialog _printDialog;

        protected PrintProcessor()
        {
            PageOrientation = PageOrientation.Portrait;
        }

        public abstract string FileName { get; }

        public abstract PrintDimension PrintDimension { get; }

        public virtual IList<SolidColorBrush> AlternatingRowColors { get; set; } = new[]
                                                                                   {
                                                                                       Brushes.Transparent,
                                                                                       Brushes.LightGray
                                                                                   };

        public bool ColorPrintPartsForDebug { get; set; }

        public int CurrentPage { get; set; }

        public bool IsAlternatingRowColor { get; set; }

        public PageOrientation PageOrientation { get; set; }

        public PrintDefinition PrintDefinition { get; } = new PrintDefinition();

        internal PrintDialog PrintDialog => _printDialog ?? (_printDialog = new PrintDialog
                                                                            {
                                                                                PrintTicket = { PageOrientation = PageOrientation }
                                                                            });

        public static bool PrintDocument(PrintDialog printDialog, PrintProcessorCollection pp)
        {
            var pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            var fixedDocument = CreateDocument(pageSize, pp);

            printDialog.PrintDocument(fixedDocument.DocumentPaginator, ReplaceInvalidCharsFromFilename(pp.FileName));

            return true;
        }

        public virtual PrintProcessorBackground GetBackgound()
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

        public void PreviewDocument()
        {
            PreviewDocument(new PrintProcessorCollection(this));
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

        protected virtual void PreparePrint()
        {
        }

        protected virtual bool PrintDocument(PrintDialog printDialog)
        {
            return PrintDocument(printDialog, new PrintProcessorCollection(this));
        }

        internal static void PreviewDocument(PrintProcessorCollection ppc)
        {
            var pd = ppc.First().PrintDialog;

            var fixedDocument = CreateDocument(new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight), ppc);

            XpsHelper.ShowFixedDocument(fixedDocument, ppc.FileName);
        }

        private static FixedDocument CreateDocument(Size pageSize, PrintProcessorCollection p)
        {
            if (p != null)
            {
                foreach (var printProcessor in p)
                {
                    printProcessor.SetPrintOnAttributes();
                    printProcessor.Prepare(pageSize);
                }
            }

            var internalPrintProcessor = new InternalPrintProcessor();
            return internalPrintProcessor.CreateFixedDocument(p);
        }

        private static string ReplaceInvalidCharsFromFilename(string path, char? replaceInvalidCharsWith = null)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars().Union(new[]
                                                                            {
                                                                                '.'
                                                                            }).ToList();
            return path.Aggregate(new StringBuilder(), (sb, c) =>
                                                       {
                                                           if (invalidFileNameChars.Contains(c))
                                                           {
                                                               if (replaceInvalidCharsWith != null)
                                                               {
                                                                   return sb.Append(replaceInvalidCharsWith);
                                                               }
                                                               return sb;
                                                           }
                                                           return sb.Append(c);
                                                       }).ToString();
        }

        private void Prepare(Size pageSize)
        {
            PrintDimension.PageSize = pageSize;
            PreparePrint();
            PrintDimension.PositionizeRelative();
        }

        private void SetPrintOnAttributes()
        {
            var customPageAttributes = (IPrintPartDefinition[])GetType().GetCustomAttributes(typeof(IPrintPartDefinition), true);

            foreach (var printOnAttribute in customPageAttributes)
            {
                PrintDefinition.SetPrintOnAttribute(printOnAttribute);
            }
            PrintDimension.InternalPrintDefinition = PrintDefinition;
        }
    }
}