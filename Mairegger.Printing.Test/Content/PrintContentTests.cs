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

namespace Mairegger.Printing.Test.Content
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Mairegger.Printing.Content;
    using NUnit.Framework;

    [TestFixture]
    public class PrintContentTests
    {
        [Test]
        public void BlankLine_HeightNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => PrintContent.BlankLine(0), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => PrintContent.BlankLine(-1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void BlankLine_HeightValid([Random(1, 100, 1)] int height)
        {
            var content = PrintContent.BlankLine(height).Content;
            content.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.That(content.DesiredSize.Height, Is.EqualTo(height));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Combine()
        {
            var content1 = PrintContent.BlankLine(10);
            var content2 = PrintContent.BlankLine(10);

            var combined = PrintContent.Combine(content1, content2).Content;
            combined.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.That(combined.DesiredSize.Height, Is.EqualTo(20));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void HorizontalLine_Height([Random(1, 100, 1)] int height)
        {
            var horizontalLine = PrintContent.HorizontalLine(height).Content;
            horizontalLine.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.That(horizontalLine.DesiredSize.Height, Is.EqualTo(height));
        }

        [Test]
        public void PageBreak_AccessContent_ThrowsInvalidOperationException()
        {
            Assert.That(() => PrintContent.PageBreak().Content, Throws.InvalidOperationException);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TextLine()
        {
            var content = PrintContent.TextLine("Test");
            content.FontSize = 32;
            content.Background = Brushes.Bisque;
            content.FontWeight = FontWeights.ExtraBold;
            content.HorizontalAlignment = HorizontalAlignment.Right;
            content.Padding = new Thickness(12);
            content.Margin = new Thickness(24);

            Assert.That(content.Text, Is.EqualTo("Test"));

            var icontent = (IPrintContent)content;

            var grid = (Grid)icontent.Content;
            var uiElement = (TextBlock)grid.Children[0];
            Assert.That(uiElement.Text, Is.EqualTo("Test"));

            Assert.That(uiElement.FontSize, Is.EqualTo(32));
            Assert.That(grid.Background, Is.EqualTo(Brushes.Bisque));
            Assert.That(uiElement.FontWeight, Is.EqualTo(FontWeights.ExtraBold));
            Assert.That(uiElement.HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Right));
            Assert.That(uiElement.Padding, Is.EqualTo(new Thickness(12)));
            Assert.That(grid.Margin, Is.EqualTo(new Thickness(24)));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ToPrintContent()
        {
            var content = new TextBlock { Text = "Test" };

            var icontent = content.ToPrintContent();

            Assert.That(icontent.Content, Is.EqualTo(content));
        }
    }
}