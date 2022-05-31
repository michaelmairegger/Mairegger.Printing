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
    using System.IO;
    using System.IO.Packaging;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Xps.Packaging;
    using Mairegger.Printing.Internal;

    /// <summary>
    ///     Represents a helper class for displaying a <see cref="FixedDocument" /> in a <see cref="DocumentViewer" />.
    /// </summary>
    public static class XpsHelper
    {
        public static void Concat(string targetFileName, params string[] filesToConcat)
        {
            var fixedDocumentSequence = new FixedDocumentSequence();

            var tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var target = new XpsDocument(tempFileName, FileAccess.Write))
            {
                var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(target);

                foreach (var doc in filesToConcat)
                {
                    Add(doc, fixedDocumentSequence);
                }

                xpsDocumentWriter.Write(fixedDocumentSequence);
            }

            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }
            File.Move(tempFileName, targetFileName);
        }

        /// <summary>
        ///     Saves the <see cref="FixedDocument" /> to the given location.
        /// </summary>
        /// <param name="fixedDocument">The document to save.</param>
        /// <param name="fileName">The location where to the document should be saved.</param>
        public static void SaveFixedDocument(FixedDocument fixedDocument, string fileName)
        {
            WriteXps(fixedDocument, fileName);
        }

        /// <summary>
        ///     Displays the <see cref="FixedDocument" /> in a <see cref="DocumentViewer" />
        /// </summary>
        /// <param name="fixedDocument">The fixed document to display.</param>
        /// <param name="title">Title of the preview window</param>
        /// <param name="windowProvider">An implementation for creating a customized window. If null, default implementation is used.</param>
        public static void ShowFixedDocument(FixedDocument fixedDocument, string title, IWindowProvider? windowProvider = null)
        {
            var tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            WriteXps(fixedDocument, tempFileName);
            InternalShowXps(tempFileName, title, true, windowProvider);
        }

        /// <summary>
        /// Displays the <paramref name="fileName"/> in a <see cref="DocumentViewer"/>
        /// </summary>
        /// <param name="fileName">The file to open.</param>
        /// <param name="title">The tile of the window.</param>
        /// <param name="windowProvider">An implementation for creating a customized window. If null, default implementation is used.</param>
        public static void ShowXps(string fileName, string title, IWindowProvider? windowProvider = null)
        {
            InternalShowXps(fileName, title, false, windowProvider);
        }

        private static void InternalShowXps(string fileName, string title, bool deleteFileOnClose, IWindowProvider? windowProvider = null)
        {
            var xpsDocument = new XpsDocument(fileName, FileAccess.Read);

            var documentViewer = new DocumentViewer { Document = xpsDocument.GetFixedDocumentSequence() };

            if (windowProvider == null)
            {
                windowProvider = new WindowsProvider();
            }

            windowProvider.Closed += PreviewWindowOnClosed(fileName, xpsDocument, deleteFileOnClose);
            windowProvider.Show(title, documentViewer);
        }

        private static void Add(string path, FixedDocumentSequence fixedDocumentSequence)
        {
            using (var doc = new XpsDocument(path, FileAccess.Read))
            {
                var sourceSequence = doc.GetFixedDocumentSequence();
                if (sourceSequence != null)
                {
                    foreach (var dr in sourceSequence.References)
                    {
                        var newDocumentReference = new DocumentReference
                                                   {
                                                       Source = dr.Source
                                                   };
                        var baseUri = ((IUriContext)dr).BaseUri;
                        ((IUriContext)newDocumentReference).BaseUri = baseUri;
                        var fd = newDocumentReference.GetDocument(true);
                        newDocumentReference.SetDocument(fd);
                        fixedDocumentSequence.References.Add(newDocumentReference);
                    }
                }
            }
        }

        private static EventHandler PreviewWindowOnClosed(string fileName, XpsDocument xpsDocument, bool deleteFile)
        {
            return (_, _) =>
                   {
                       xpsDocument.Close();

                       if (deleteFile)
                       {
                           File.Delete(fileName);
                       }
                   };
        }

        private static void WriteXps(IDocumentPaginatorSource fixedDocument, string tempFileName)
        {
            using (var stream = File.Open(tempFileName, FileMode.Create))
            {
                using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed))
                    {
                        var docWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                        docWriter.Write(fixedDocument.DocumentPaginator);
                    }
                }
            }
        }
    }
}