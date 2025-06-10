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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Mairegger.Printing.Content;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintContentTests
    {

        [Fact]
        public void BlankLine_HeightNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PrintContent.BlankLine(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => PrintContent.BlankLine(-1));
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList), 1, 100, 1, MemberType = typeof(RandomTest))]
        public void BlankLine_HeightValid(int height)
        {
            var content = PrintContent.BlankLine(height).Content;
            content.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.Equal(height, content.DesiredSize.Height);
        }

        [StaFact]
        public void Combine()
        {
            var content1 = PrintContent.BlankLine(10);
            var content2 = PrintContent.BlankLine(10);

            var combined = PrintContent.Combine(content1, content2).Content;
            combined.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.Equal(20, combined.DesiredSize.Height);
        }

        [StaTheory]
        [MemberData(nameof(RandomTest.NumberList), 1, 100, 1, MemberType = typeof(RandomTest))]
        public void HorizontalLine_Height(int height)
        {
            var horizontalLine = PrintContent.HorizontalLine(height).Content;
            horizontalLine.Measure(new Size(double.MaxValue, double.MaxValue));

            Assert.Equal(height, horizontalLine.DesiredSize.Height);
        }

        [Fact]
        public void PageBreak_AccessContent_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => PrintContent.PageBreak().Content);
        }

        [StaFact]
        public void TextLine()
        {
            var content = PrintContent.TextLine("Test");
            content.FontSize = 32;
            content.Background = Brushes.Bisque;
            content.FontWeight = FontWeights.ExtraBold;
            content.HorizontalAlignment = HorizontalAlignment.Right;
            content.Padding = new Thickness(12);
            content.Margin = new Thickness(24);

            Assert.Equal("Test", content.Text);

            var icontent = (IPrintContent)content;

            var grid = (Grid)icontent.Content;
            var uiElement = (TextBlock)grid.Children[0];
            Assert.Multiple(
                ()=> Assert.Equal("Test", uiElement.Text),

                ()=> Assert.Equal(32, uiElement.FontSize),
                ()=> Assert.Equal(Brushes.Bisque, grid.Background),
                ()=> Assert.Equal(FontWeights.ExtraBold, uiElement.FontWeight),
                ()=> Assert.Equal(HorizontalAlignment.Right, uiElement.HorizontalAlignment),
                ()=> Assert.Equal(new Thickness(12), uiElement.Padding),
                ()=> Assert.Equal(new Thickness(24), grid.Margin));
        }

        [StaFact]
        public void TextLine_Configuration()
        {
            StringLineItemConfiguration configuration = new StringLineItemConfiguration()
                                                        {
                                                            FontFamily = new FontFamily("Verdana"),
                                                            FontSize = 10,
                                                            HorizontalAlignment = HorizontalAlignment.Right
                                                        };

            var content = PrintContent.TextLine("Test", configuration);

            var icontent = (IPrintContent)content;

            var grid = (Grid)icontent.Content;
            var uiElement = (TextBlock)grid.Children[0];

            Assert.Multiple(
                ()=> Assert.Equal(10, uiElement.FontSize),
                ()=> Assert.Equal(new FontFamily("Verdana"), uiElement.FontFamily),
                ()=> Assert.Equal(HorizontalAlignment.Right, uiElement.HorizontalAlignment));

        }

        [StaFact]
        public void ToPrintContent()
        {
            var content = new TextBlock { Text = "Test" };

            var icontent = content.ToPrintContent();

            Assert.Equal(content, icontent.Content);
        }
    }
}
