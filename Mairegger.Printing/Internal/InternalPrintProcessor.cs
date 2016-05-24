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

namespace Mairegger.Printing.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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

    /// <summary>
    ///     Internal helper class for Printing
    /// </summary>
    internal class InternalPrintProcessor
    {
        private readonly Thickness _pageMargin = new Thickness(0);
        private bool _alternatingWarningShown;
        private int _itemCount;
        private PageHelper _pageHelper;
        private IPrintProcessor _printProcessor;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InternalPrintProcessor" /> class.
        /// </summary>
        public InternalPrintProcessor()
        {
            CurrentPageNumber = 1;
        }

        private int CurrentPageNumber { get; set; }

        private FixedDocument FixedDocument { get; set; }

        /// <summary>
        ///     Creates the whole documents
        /// </summary>
        public FixedDocument CreateFixedDocument(PrintProcessor pp)
        {
            return CreateFixedDocument(new PrintProcessorCollection(pp));
        }

        public FixedDocument CreateFixedDocument(PrintProcessorCollection collection)
        {
            FixedDocument = new FixedDocument();

            if (collection != null)
            {
                foreach (var pp in collection)
                {
                    var currentPage = FixedDocument.Pages.Count;
                    _printProcessor = pp;
                    CurrentPageNumber = 1;
                    _printProcessor = pp;
                    IList<IPrintContent> itemCollection = _printProcessor.ItemCollection().ToList();

                    AddItems(itemCollection);
                    if (collection.IndividualPageNumbers)
                    {
                        AddPageNumbers(currentPage);
                    }
                }

                if (!collection.IndividualPageNumbers)
                {
                    AddPageNumbers();
                }
            }
            return FixedDocument;
        }

        private static Brush ComputeBackGround(PrintAppendixes printAppendix)
        {
            var factor = (byte)(byte.MaxValue - (byte)((byte.MaxValue / (byte)Enum.GetValues(typeof(PrintAppendixes)).Length) * (byte)printAppendix));
            return new SolidColorBrush(Color.FromArgb(128, factor, factor, factor));
        }

        private static void PositionizeUiElement(PageContent pageContent, UIElement frameworkElement, Point positioningPoint)
        {
            FixedPage.SetTop(frameworkElement, positioningPoint.Y);
            FixedPage.SetLeft(frameworkElement, positioningPoint.X);

            pageContent.Child.Children.Add(frameworkElement);
        }

        private void AddBackground(PageContent pageContent, bool isLastpage)
        {
            if (!_printProcessor.PrintDefinition.IsToPrint(PrintAppendixes.Background, CurrentPageNumber, isLastpage))
            {
                return;
            }

            var backgound = _printProcessor.GetBackgound();
            if (backgound == null)
            {
                throw new InvalidOperationException("The Background cannot be null if the corresponding flag in the PrintAppendix is set");
            }

            var positioningPoint = new Point(backgound.Size.Left, backgound.Size.Top);

            Trace.TraceInformation($"Print background on page #{CurrentPageNumber} ");
            PositionizeUiElement(pageContent, backgound.Element, positioningPoint);
        }

        private void AddItems(IList<IPrintContent> itemCollection)
        {
            _pageHelper = CreateNewPageHelper();

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
                Trace.TraceInformation("There are no ILineItems available to print");
                ConcludeDocument();
            }
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

                if ((lineElement.GetValue(Panel.BackgroundProperty) != null) && !_alternatingWarningShown)
                {
                    _alternatingWarningShown = true;
                    Trace.TraceInformation("Control your IPrintContent.Content's background. In order to correct alternate your columns you should not set the background to any value.");
                }

                lineElement.SetValue(Panel.BackgroundProperty, alternatingRowBackground);
            }

            bodyGrid.Items.Add(contentControl);
        }

        private void AddLineItem(IPrintContent item, bool isLast)
        {
            if (item is PageBreak)
            {
                ConcludeDocumentPage(_pageHelper, false);
                _pageHelper = CreateNewPageHelper();
                return;
            }

            var content = item.Content;

            content.Measure(new Size(_pageHelper.BodyGrid.DesiredSize.Width, double.MaxValue));
            var lineHeiht = content.DesiredSize.Height;

            if (isLast)
            {
                // otherwise the last item is put on a new pageContent if desired, or it is left on the current pageContent and the PrintAppendixes that have no space would be print on the next pageContent
                // should occur only if there are PrintAppendixes that have to be print on the last pageContent
                Action concludePage = () =>
                                      {
                                          ConcludeDocumentPage(_pageHelper, false);
                                          _pageHelper = CreateNewPageHelper();
                                      };
                Action addLastLineData = () =>
                                         {
                                             AddLineData(content);
                                             Trace.TraceInformation("Last item print");
                                         };

                Action<Action, Action> doAction = (first, second) =>
                                                  {
                                                      first();
                                                      second();
                                                      ConcludeDocument();
                                                  };

                if (_pageHelper.HasSpace(lineHeiht, CurrentPageNumber, true))
                {
                    // if (_printProcessor.BreakLastItemIfLastPageWouldBeEmpty)
                    // { 
                    //     doAction(concludePage, addLastLineData);
                    // }
                    // else
                    {
                        AddLineData(content);
                        ConcludeDocumentPage(_pageHelper, true);
                    }
                }
                else
                {
                    doAction(concludePage, addLastLineData);
                }
                return;
            }

            if (_pageHelper.HasSpace(lineHeiht, CurrentPageNumber, true))
            {
                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeiht);
            }
            else if (_pageHelper.HasSpace(lineHeiht, CurrentPageNumber, false))
            {
                Trace.TraceInformation("Second chance because item has no space");
                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeiht);
            }
            else
            {
                ConcludeDocumentPage(_pageHelper, false);

                _pageHelper = CreateNewPageHelper();

                AddLineData(content);
                _pageHelper.RemoveRemainingSpace(lineHeiht);
            }
        }

        private void AddPageNumbers(int from = 0, int to = int.MaxValue)
        {
            var currentPageCount = 0;

            foreach (var pageContent in FixedDocument.Pages.Skip(from).Take(to))
            {
                currentPageCount++;
                if (!_printProcessor.PrintDefinition.IsToPrint(PrintAppendixes.PageNumbers, currentPageCount, false))
                {
                    continue;
                }

                Trace.TraceInformation($"Print Page Numbers on page #{currentPageCount}");
                var textBlock = new TextBlock
                                {
                                    Text = $"{currentPageCount} | {FixedDocument.Pages.Count - from}",
                                    TextAlignment = TextAlignment.Center,
                                    Width = _printProcessor.PrintDimension.PrintablePageSize.Width,
                                    Height = _printProcessor.PrintDimension.GetPageNumberHeight(currentPageCount)
                                };

                AddSpecialElement(currentPageCount == to, pageContent, PrintAppendixes.PageNumbers, () => textBlock);
            }
        }

        private void AddPrintAppendixes(PageContent content, bool isLastPage)
        {
            AddBackground(content, isLastPage);

            AddSpecialElement(isLastPage, content, PrintAppendixes.Header, () => _printProcessor.GetHeader());
            AddSpecialElement(isLastPage, content, PrintAppendixes.HeaderDescription, () => _printProcessor.GetHeaderDescription());
            AddSpecialElement(isLastPage, content, PrintAppendixes.Summary, () => _printProcessor.GetSummary());
            AddSpecialElement(isLastPage, content, PrintAppendixes.Footer, () => _printProcessor.GetFooter());
        }

        private void AddSpecialElement(bool isLastpage, PageContent pageContent, PrintAppendixes appendix, Func<UIElement> printElement)
        {
            if (printElement == null)
            {
                throw new ArgumentNullException(nameof(printElement));
            }
            if (!_printProcessor.PrintDefinition.IsToPrint(appendix, CurrentPageNumber, isLastpage))
            {
                return;
            }

            var elementToPrint = printElement();
            if (elementToPrint == null)
            {
                throw new InvalidOperationException($"The {appendix} cannot be null if the corresponding flag in the PrintAppendix is set");
            }

            Trace.TraceInformation($"Print {appendix} desciption on page #{CurrentPageNumber} ");
            PositionizeUiElement(pageContent, elementToPrint, appendix, isLastpage);
        }

        private void ConcludeDocument()
        {
            ConcludeDocumentPage(_pageHelper, true);
        }

        private void ConcludeDocumentPage(PageHelper pageHelper, bool isLastPage)
        {
            Trace.TraceInformation("Conclude Document Page");

            var content = GetNewDocumentPage();

            var grid = new Grid();
            grid.Children.Add(pageHelper.BodyGrid);

            var rectangle = new Rectangle
                            {
                                Stroke = pageHelper.BorderBrush,
                                StrokeThickness = .5d
                            };

            grid.Children.Add(rectangle);
            grid.Height = _printProcessor.PrintDimension.GetBodyGridRange(CurrentPageNumber, isLastPage).Length;

            var positioningPoint = new Point(_printProcessor.PrintDimension.Margin.Left, _printProcessor.PrintDimension.GetBodyGridRange(CurrentPageNumber, isLastPage).From);
            PositionizeUiElement(content, grid, positioningPoint);

            AddPrintAppendixes(content, isLastPage);

            FixedDocument.Pages.Add(content);
            CurrentPageNumber++;
            _pageHelper = null;
        }

        private PageHelper CreateNewPageHelper()
        {
            _printProcessor.CurrentPage++;
            _printProcessor.OnPageBreak();

            Brush borderBrush;
            double gridTableHeight;
            var table = _printProcessor.GetTable(out gridTableHeight, out borderBrush);

            var itemsControl = new ItemsControl();

            if (_printProcessor.ColorPrintPartsForDebug)
            {
                itemsControl.Background = ComputeBackGround(PrintAppendixes.All);
            }

            itemsControl.Height = _printProcessor.PrintDimension.GetMaxGridHeight(CurrentPageNumber, false);
            itemsControl.Width = _printProcessor.PrintDimension.PrintablePageSize.Width;
            itemsControl.VerticalAlignment = VerticalAlignment.Top;
            itemsControl.Items.Add(table);

            var pageHelper = new PageHelper();

            pageHelper.RemoveRemainingSpace(gridTableHeight);

            pageHelper.PrintingDimension = _printProcessor.PrintDimension;
            pageHelper.BodyGrid = itemsControl;
            pageHelper.BodyGrid.Measure(new Size(double.MaxValue, double.MaxValue));
            pageHelper.BorderBrush = borderBrush;
            return pageHelper;
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

        private void PositionizeUiElement(PageContent pageContent, UIElement panel, PrintAppendixes printAppendix, bool isLastPage)
        {
            var positioninRange = _printProcessor.PrintDimension.GetRange(printAppendix, CurrentPageNumber, isLastPage);
            var position = new Point(_printProcessor.PrintDimension.Margin.Left, positioninRange.From);

            var panelHeight = positioninRange.Length;

            var contentControl = new ContentControl
                                 {
                                     Height = panelHeight,
                                     Width = _printProcessor.PrintDimension.PrintablePageSize.Width
                                 };

            if (_printProcessor.ColorPrintPartsForDebug)
            {
                contentControl.Content = new Grid
                                         {
                                             Background = ComputeBackGround(printAppendix),
                                             Children =
                                             {
                                                 new Rectangle
                                                 {
                                                     StrokeDashArray = new DoubleCollection(new double[]
                                                                                            {
                                                                                                20,
                                                                                                20
                                                                                            }),
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

            PositionizeUiElement(pageContent, contentControl, position);
        }
    }
}