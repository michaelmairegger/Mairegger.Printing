// Copyright 2017-2022 Michael Mairegger
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

using EnumsNET;

namespace Mairegger.Printing.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Mairegger.Printing.Content;
    using Mairegger.Printing.Definition;
    using Mairegger.Printing.PrintProcessor;

    internal class InternalPrintProcessor
    {
        private const string Description = "Current line is higher than the";
        private readonly FixedDocument _fixedDocument;
        private readonly Thickness _pageMargin = new Thickness(0);
        private readonly IPrintProcessor _printProcessor;
        private bool _alternatingWarningShown;
        private int _itemCount;
        private PageHelper _pageHelper;

        private InternalPrintProcessor(IPrintProcessor printProcessor, FixedDocument fixedDocument)
        {
            _printProcessor = printProcessor;
            _fixedDocument = fixedDocument;
            _pageHelper = CreateNewPageHelper();
        }

        private int CurrentPageNumber { get; set; } = 1;

        private void Process(bool individualPageNumbering)
        {
            var documentPages = _fixedDocument.Pages.Count;
            CurrentPageNumber = 1;
            IList<IPrintContent> itemCollection = _printProcessor.ItemCollection().ToList();

            AddItems(itemCollection);
            if (individualPageNumbering)
            {
                AddPageNumbers(documentPages);
            }

            for (int i = documentPages,
                     j = 1;
                 i < _fixedDocument.Pages.Count;
                 i++, j++)
            {
                AddCustomPositionedContent(_fixedDocument.Pages[i], _printProcessor.GetCustomPageContent(j));
            }
        }

        private static void AddCustomPositionedContent(PageContent page, IEnumerable<IDirectPrintContent> customContent)
        {
            foreach (var content in customContent)
            {
                PositionUiElement(page, content.Content, content.Position);
            }
        }

        private void AddItems(IList<IPrintContent> itemCollection)
        {
            if (itemCollection.Count > 0)
            {
                for (var i = 0; i < itemCollection.Count; i++)
                {
                    var isLast = i == itemCollection.Count - 1;
                    AddLineItem(itemCollection[i], isLast);
                }
            }
            else
            {
                Trace.TraceWarning("PRINTING: There are no ILineItems available to print");
                ConcludeDocument();
            }
        }

        private void AddLineItem(IPrintContent item, bool isLast)
        {
            if (item is PageBreak)
            {
                ConcludeDocumentPage(false);
                _pageHelper = CreateNewPageHelper();
            }
            else if (item is IPageBreakAware pageBreakAware)
            {
                AddLineItem(pageBreakAware, isLast);
            }
            else if (item is IDirectPrintContent directPrintContent)
            {
                var position = new Point(
                    directPrintContent.Position.X + _pageHelper.PrintingDimension.Margin.Left,
                    directPrintContent.Position.Y + _pageHelper.PrintingDimension.Margin.Top
                );
                PositionUiElement(_pageHelper.PageContent, item.Content, position);
            }
            else
            {
                AddLineItemPlaceContentItem(item, isLast);
            }
        }

        private void AddLineItem(IPageBreakAware aware, bool isLast)
        {
            var currentPageHeight = _pageHelper.GetRemainingSpace(CurrentPageNumber, isLast);
            var rangeForBodyGrid = _printProcessor.PrintDimension.GetRangeForBodyGrid(CurrentPageNumber, isLast);
            var printablePageSize = new Size(_printProcessor.PrintDimension.PrintablePageSize.Width, rangeForBodyGrid.Length);

            var pageContents = aware.PageContents(currentPageHeight, printablePageSize).ToList();
            var last = pageContents.Last();

            foreach (var item in pageContents)
            {
                AddLineItem(item.ToPrintContent(), isLast && Equals(item, last));
            }
        }

        private void AddLineItemPlaceContentItem(IPrintContent item, bool isLast)
        {
            var content = item.Content;

            content.Measure(new Size(_pageHelper.BodyGrid.DesiredSize.Width, double.MaxValue));
            var lineHeight = content.DesiredSize.Height;

            if (lineHeight >= _pageHelper.PrintingDimension.GetHeightForBodyGrid(CurrentPageNumber, isLast))
            {
                LogHeightWarning();
            }

            if (isLast)
            {
                PlaceLastItem(lineHeight, content);
                return;
            }

            if (_pageHelper.HasSpace(lineHeight, CurrentPageNumber, true))
            {
                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeight);
            }
            else if (_pageHelper.HasSpace(lineHeight, CurrentPageNumber, false))
            {
                Debug.WriteLine("PRINTING: Second chance because item has no space");
                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeight);
            }
            else
            {
                ConcludeDocumentPage(false);

                _pageHelper = CreateNewPageHelper();

                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeight);
            }

            [ExcludeFromCodeCoverage]
            void LogHeightWarning()
            {
                var helpText = $"Either reduce size of the line or consider deriving {item.GetType()} form {nameof(IPageBreakAware)}";

                if (lineHeight > _pageHelper.PrintingDimension.PageSize.Height)
                {
                    Trace.TraceWarning($"{Description} page-size. {helpText}");
                }
                else if (lineHeight > _pageHelper.PrintingDimension.PrintablePageSize.Height)
                {
                    Trace.TraceWarning($"{Description} printable-page-size. {helpText}");
                }
                else if (lineHeight > _pageHelper.PrintingDimension.GetHeightForBodyGrid(CurrentPageNumber, isLast))
                {
                    Trace.TraceWarning($"{Description} body grid. {helpText}");
                }
            }
        }

        private void PlaceLastItem(double lineHeight, UIElement content)
        {
            // otherwise the last item is put on a new pageContent if desired, or it is left on the current
            // pageContent and the PrintAppendixes that have no space would be print on the next pageContent
            // should occur only if there are PrintAppendixes that have to be print on the last pageContent
            if (_pageHelper.HasSpace(lineHeight, CurrentPageNumber, true))
            {
                AddLineData(content);
                ConcludeDocumentPage(true);
            }
            else
            {
                ConcludePage();
                AddLastLineData(content);
                ConcludeDocument();
            }
        }

        private void ConcludePage()
        {
            ConcludeDocumentPage(false);
            _pageHelper = CreateNewPageHelper();
        }

        private void AddLastLineData(UIElement content)
        {
            AddLineData(content);
            Debug.WriteLine("PRINTING: Last item print");
        }

        private void AddLineData(UIElement lineContent)
        {
            var bodyGrid = _pageHelper.BodyGrid;
            var lineElement = lineContent;
            var contentControl = new ContentControl { Content = lineElement };

            if (_printProcessor.IsAlternatingRowColor)
            {
                var i = _itemCount++ % _printProcessor.AlternatingRowColors.Count;
                var alternatingRowBackground = _printProcessor.AlternatingRowColors[i];

                var propertyInfo = lineElement.GetType().GetProperty("Background");
                if ((propertyInfo?.GetValue(lineElement) != null) && !_alternatingWarningShown)
                {
                    _alternatingWarningShown = true;
                    Trace.TraceWarning(
                        "PRINTING: Control your IPrintContent.Content's background. In order to correct alternate your columns you should not set the background to any value."
                    );
                }

                lineElement.SetValue(Panel.BackgroundProperty, alternatingRowBackground);
                contentControl.Background = alternatingRowBackground;
            }

            bodyGrid.Items.Add(contentControl);
        }

        private PageHelper CreateNewPageHelper()
        {
            _printProcessor.CurrentPage++;
            _printProcessor.OnPageBreak();

            var table = _printProcessor.GetTable(out var reserveHeightOf, out var borderBrush);

            var itemsControl = new ItemsControl();

            if (_printProcessor.ColorPrintPartsForDebug)
            {
                itemsControl.Background = ComputeBackGround(PrintAppendixes.All);
            }

            itemsControl.Height = _printProcessor.PrintDimension.GetHeightForBodyGrid(CurrentPageNumber, false);
            itemsControl.Width = _printProcessor.PrintDimension.PrintablePageSize.Width;
            itemsControl.VerticalAlignment = VerticalAlignment.Top;
            itemsControl.Items.Add(table);

            return new PageHelper(itemsControl, borderBrush, GetNewDocumentPage(), _printProcessor.PrintDimension, reserveHeightOf);
        }

        private PageContent GetNewDocumentPage()
        {
            var fixedPage = new FixedPage
                            {
                                Width = _printProcessor.PrintDimension.PageSize.Width,
                                Height = _printProcessor.PrintDimension.PageSize.Height,
                                Margin = _pageMargin
                            };

            if (_printProcessor.ColorPrintPartsForDebug)
            {
                fixedPage.Background = ComputeBackGround(PrintAppendixes.None);
            }

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);

            return pageContent;
        }

        private void AddPageNumbers(int from = 0, int to = int.MaxValue)
        {
            var currentPageCount = 1;
            var maxPages = _fixedDocument.Pages.Count - from;

            foreach (var pageContent in _fixedDocument.Pages.Skip(from).Take(to))
            {
                if (_printProcessor.PrintDefinition.IsToPrint(PrintAppendixes.PageNumbers, currentPageCount, false))
                {
                    Debug.WriteLine($"PRINTING: Print Page Numbers on page #{currentPageCount}");

                    var count = currentPageCount;
                    AddSpecialElement(
                        currentPageCount == to,
                        currentPageCount,
                        pageContent,
                        PrintAppendixes.PageNumbers,
                        () => _printProcessor.GetPageNumbers(count, maxPages)
                    );
                }

                currentPageCount++;
            }
        }

        private void AddSpecialElement(bool isLastPage, int pageNumber, PageContent pageContent, PrintAppendixes appendix,
                                       Func<UIElement> printElement)
        {
            if (!_printProcessor.PrintDefinition.IsToPrint(appendix, pageNumber, isLastPage))
            {
                return;
            }

            var elementToPrint = printElement();
            if (elementToPrint == null)
            {
                throw new InvalidOperationException($"The {appendix} cannot be null if the corresponding flag in the PrintAppendix is set");
            }

            Debug.WriteLine($"PRINTING: Print {appendix} description on page #{pageNumber} ");
            PositionUiElement(pageContent, elementToPrint, appendix, pageNumber, isLastPage);
        }

        private static void PositionUiElement(PageContent pageContent, UIElement frameworkElement, Point positioningPoint)
        {
            FixedPage.SetTop(frameworkElement, positioningPoint.Y);
            FixedPage.SetLeft(frameworkElement, positioningPoint.X);

            pageContent.Child.Children.Add(frameworkElement);
        }

        private void PositionUiElement(PageContent pageContent, UIElement panel, PrintAppendixes printAppendix, int pageNumber, bool isLastPage)
        {
            var positioningRange = _printProcessor.PrintDimension.GetRangeFor(printAppendix, pageNumber, isLastPage);
            var position = new Point(_printProcessor.PrintDimension.Margin.Left, positioningRange.From);

            var contentControl = new ContentControl { Width = _printProcessor.PrintDimension.PrintablePageSize.Width };

            if (_printProcessor.ColorPrintPartsForDebug)
            {
                contentControl.Content = new Grid
                                         {
                                             Background = ComputeBackGround(printAppendix),
                                             Children =
                                             {
                                                 new Rectangle
                                                 {
                                                     StrokeDashArray = new DoubleCollection(
                                                         new double[]
                                                         {
                                                             20,
                                                             20
                                                         }
                                                     ),
                                                     Stroke = Brushes.Black,
                                                     StrokeThickness = 2d
                                                 },
                                                 new TextBlock
                                                 {
                                                     Text = printAppendix.ToString(),
                                                     FontSize = 48d,
                                                     Opacity = 0.5d,
                                                     HorizontalAlignment = HorizontalAlignment.Center,
                                                     VerticalAlignment = VerticalAlignment.Center
                                                 },
                                                 panel
                                             }
                                         };
            }
            else
            {
                contentControl.Content = panel;
            }

            PositionUiElement(pageContent, contentControl, position);
        }

        private static SolidColorBrush ComputeBackGround(PrintAppendixes printAppendix)
        {
            var factor = (byte)(byte.MaxValue - (byte)((byte.MaxValue / (byte)Enums.GetValues<PrintAppendixes>().Count) * (byte)printAppendix));
            return new SolidColorBrush(Color.FromArgb(128, factor, factor, factor));
        }

        private void ConcludeDocument()
        {
            ConcludeDocumentPage(true);
        }

        private void ConcludeDocumentPage(bool isLastPage)
        {
            Debug.WriteLine("PRINTING: Conclude Document Page");

            var grid = new Grid();
            grid.Children.Add(_pageHelper.BodyGrid);

            var rectangle = new Rectangle
                            {
                                Stroke = _pageHelper.BorderBrush,
                                StrokeThickness = .5d
                            };

            grid.Children.Add(rectangle);
            grid.Height = _printProcessor.PrintDimension.GetRangeForBodyGrid(CurrentPageNumber, isLastPage).Length;

            var positioningPoint = new Point(
                _printProcessor.PrintDimension.Margin.Left,
                _printProcessor.PrintDimension.GetRangeForBodyGrid(CurrentPageNumber, isLastPage).From
            );
            PositionUiElement(_pageHelper.PageContent, grid, positioningPoint);

            AddPrintAppendixes(_pageHelper.PageContent, isLastPage);

            _fixedDocument.Pages.Add(_pageHelper.PageContent);
            CurrentPageNumber++;
        }

        private void AddPrintAppendixes(PageContent content, bool isLastPage)
        {
            AddBackground(content, isLastPage);

            AddSpecialElement(isLastPage, CurrentPageNumber, content, PrintAppendixes.Header, () => _printProcessor.GetHeader());
            AddSpecialElement(
                isLastPage,
                CurrentPageNumber,
                content,
                PrintAppendixes.HeaderDescription,
                () => _printProcessor.GetHeaderDescription()
            );
            AddSpecialElement(isLastPage, CurrentPageNumber, content, PrintAppendixes.Summary, () => _printProcessor.GetSummary());
            AddSpecialElement(isLastPage, CurrentPageNumber, content, PrintAppendixes.Footer, () => _printProcessor.GetFooter());
        }

        private void AddBackground(PageContent pageContent, bool isLastPage)
        {
            if (!_printProcessor.PrintDefinition.IsToPrint(PrintAppendixes.Background, CurrentPageNumber, isLastPage))
            {
                return;
            }

            var background = _printProcessor.GetBackground();
            var positioningPoint = new Point(background.Size.Left, background.Size.Top);

            Debug.WriteLine($"PRINTING: Print background on page #{CurrentPageNumber} ");
            PositionUiElement(pageContent, background.Element, positioningPoint);
        }

        /// <summary>
        ///     Creates the whole documents
        /// </summary>
        public static FixedDocument CreateFixedDocument(PrintProcessor pp)
        {
            return CreateFixedDocument(new PrintProcessorCollection(pp));
        }

        public static FixedDocument CreateFixedDocument(PrintProcessorCollection collection)
        {
            var fixedDocument = new FixedDocument();

            foreach (var pp in collection)
            {
                new InternalPrintProcessor(pp, fixedDocument).Process(collection.IndividualPageNumbers);
            }

            if (!collection.IndividualPageNumbers)
            {
                // AddPageNumbers(FixedDocument);
            }

            return fixedDocument;
        }
    }
}
