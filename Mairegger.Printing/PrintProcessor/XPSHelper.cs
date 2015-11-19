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
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Xps.Packaging;

    /// <summary>
    ///     Represents a helper class for displaying a <see cref="FixedDocument" /> in a <see cref="DocumentViewer" />.
    /// </summary>
    public static class XpsHelper
    {
        public static void Concat(string targetFileName, params string[] filesToConcat)
        {
            var fixedDocumentSequence = new FixedDocumentSequence();

            var tempFileName = Path.GetTempFileName();
            using (var target = new XpsDocument(tempFileName, FileAccess.Write))
            {
                var xpsDocumentWriter = XpsDocument.CreateXpsDocumentWriter(target);

                if (filesToConcat != null)
                {
                    foreach (var doc in filesToConcat)
                    {
                        Add(doc, fixedDocumentSequence);
                    }
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
        ///     Saves the <see cref="FixedDocument" /> to a temporary file.
        /// </summary>
        /// <param name="fixedDocument">The document to save.</param>
        /// <returns>The <see cref="FileInfo" /> for the document that has been created.</returns>
        public static FileInfo SaveFixedDocument(FixedDocument fixedDocument)
        {
            var tempFileName = Path.GetTempFileName();
            SaveFixedDocument(fixedDocument, tempFileName);

            return new FileInfo(tempFileName);
        }

        /// <summary>
        ///     Displays the <see cref="FixedDocument" /> in a <see cref="DocumentViewer" />
        /// </summary>
        /// <param name="fixedDocument">The fixed document to display.</param>
        /// <param name="title">Title of the preview window</param>
        public static void ShowFixedDocument(FixedDocument fixedDocument, string title)
        {
            var tempFileName = Path.GetTempFileName();

            WriteXps(fixedDocument, tempFileName);
            ShowXps(tempFileName, title);
        }

        public static void ShowXps(string fileName, string title)
        {
            var xpsDocument = new XpsDocument(fileName, FileAccess.Read);

            var documentViewer = new DocumentViewer { Document = xpsDocument.GetFixedDocumentSequence() };
            var previewWindow = new Window
                                {
                                    Content = documentViewer,
                                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                    Title = $"Preview: {title}"
                                };

            previewWindow.Show();
            previewWindow.Closed += (sender, args) =>
                                    {
                                        xpsDocument.Close();

                                        File.Delete(fileName);
                                    };
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

        private static void WriteXps(IDocumentPaginatorSource fixedDocument, string tempFileName)
        {
            var paginator = fixedDocument.DocumentPaginator;
            using (var xps = new XpsDocument(tempFileName, FileAccess.Write))
            {
                var documentWriter = XpsDocument.CreateXpsDocumentWriter(xps);
                documentWriter.Write(paginator);
            }
        }
    }
}