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

namespace Mairegger.Printing.Content
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using JetBrains.Annotations;

    public static class PrintContent
    {
        /// <summary>
        /// Returns a new <see cref="IPrintContent"/> defining a horizontal line.
        /// </summary>
        /// <returns></returns>
        public static IPrintContent HorizontalLine(double? lineHeight = 1, Thickness margin = new Thickness(), Brush lineColor = null)
        {
            return new HorizontalLine(lineHeight, margin, lineColor);
        }

        /// <summary>
        /// Breaks the page immediately
        /// </summary>
        public static IPrintContent PageBreak()
        {
            return Content.PageBreak.Instance;
        }

        /// <summary>
        /// Returns a line containing a specified <paramref name="text"/>
        /// </summary>
        /// <param name="text">The text to print.</param>
        /// <param name="fontSize">The size of the text.</param>
        /// <param name="horizontalAlignment">The alignment of the text.</param>
        /// <param name="fontFamily">The font type.</param>
        /// <returns></returns>
        public static StringLineItem TextLine(string text, double? fontSize = null, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, FontFamily fontFamily = null)
        {
            return new StringLineItem(text, fontSize, horizontalAlignment)
            {
                FontFamily = fontFamily
            };
        }

        public static StringLineItem TextLine(string text, [NotNull] StringLineItemConfiguration configuration)
        {
            return new StringLineItem(text, configuration ?? throw new ArgumentNullException(nameof(configuration)));
        }

        /// <summary>
        /// Returns a blank line with a predefined height
        /// </summary>
        /// <param name="height">The positive height of the blank space.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="height"/> is negative.</exception>
        public static IPrintContent BlankLine(double height)
        {
            return new BlankLine(height);
        }

        public static IPrintContent ToPrintContent(this UIElement content)
        {
            return new DirectContentLineItem(content);
        }

        /// <summary>
        ///     Returns a <see cref="IPrintContent" /> that merges multiple <see cref="IPrintContent" />s into one
        ///     in order to ensure that each item of the collection gets printed preventing a page break.
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="elements"/> is null.</exception>
        public static IPrintContent Combine(params IPrintContent[] elements)
        {
            return new CombinedPrintContentCollection(elements);
        }
    }
}