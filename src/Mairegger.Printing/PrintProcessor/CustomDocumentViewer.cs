// -----------------------------------------------------------------------
// <copyright file="MyDocumentViewer.cs"
//            project="Mairegger.Printing"
//            company="Schwer Präzision s.r.l.">
//     Copyright © Mairegger Michael, Valentin Huber, 2009-2025
//     All rights reserved
// </copyright>
// -----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace Mairegger.Printing.PrintProcessor;

public class CustomDocumentViewer : DocumentViewer
{
    static CustomDocumentViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomDocumentViewer), new FrameworkPropertyMetadata(typeof(CustomDocumentViewer)));
    }

    public string? JobTitle { get; set; }

    protected override void OnPrintCommand()
    {
        var dialog = new PrintDialog();
        if (dialog.ShowDialog() == true)
        {
            dialog.PrintDocument(Document.DocumentPaginator, JobTitle);
        }
    }
}
