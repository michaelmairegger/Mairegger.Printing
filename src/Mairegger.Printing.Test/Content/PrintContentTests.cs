// Copyright 2017-2025 Michael Mairegger
//
// Licensed under the Apache License, Version 2.0 (the "License")
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
using Bogus;
using Mairegger.Printing.Content;
using TUnit.Core.Executors;

namespace Mairegger.Printing.Tests.Content
{
    public class PrintContentTests
    {
        private static readonly Faker s_faker = new();
        public static IEnumerable<int> GetRandomList()
        {

            for (int i = 1; i <= 1; i++)
            {
                yield return s_faker.Random.Int(1, 100);
            }
        }

        [Test]
        public void BlankLine_HeightNegative_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PrintContent.BlankLine(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => PrintContent.BlankLine(-1));
        }

        [Test, STAThreadExecutor]
        [MethodDataSource(nameof(GetRandomList))]
        public async Task BlankLine_HeightValid(int height)
        {
            var content = PrintContent.BlankLine(height).Content;
            content.Measure(new Size(double.MaxValue, double.MaxValue));

            await Assert.That(content.DesiredSize.Height).IsEqualTo(height);
        }

        [Test, STAThreadExecutor]
        public async Task Combine()
        {
            var content1 = PrintContent.BlankLine(10);
            var content2 = PrintContent.BlankLine(10);

            var combined = PrintContent.Combine(content1, content2).Content;
            combined.Measure(new Size(double.MaxValue, double.MaxValue));

            await Assert.That(combined.DesiredSize.Height).IsEqualTo(20);
        }

        [Test, STAThreadExecutor]
        [MethodDataSource(nameof(GetRandomList))]
        public async Task HorizontalLine_Height(int height)
        {
            var horizontalLine = PrintContent.HorizontalLine(height).Content;
            horizontalLine.Measure(new Size(double.MaxValue, double.MaxValue));

            await Assert.That(horizontalLine.DesiredSize.Height).IsEqualTo(height);
        }

        [Test]
        public async Task PageBreak_AccessContent_ThrowsInvalidOperationException()
        {
            await Assert.That(() => PrintContent.PageBreak().Content).Throws<InvalidOperationException>();
        }

        [Test, STAThreadExecutor]
        public async Task TextLine()
        {
            var content = PrintContent.TextLine("Test");
            content.FontSize = 32;
            content.Background = Brushes.Bisque;
            content.FontWeight = FontWeights.ExtraBold;
            content.HorizontalAlignment = HorizontalAlignment.Right;
            content.Padding = new Thickness(12);
            content.Margin = new Thickness(24);

            await Assert.That(content.Text).IsEqualTo("Test");

            var icontent = (IPrintContent)content;

            var grid = (Grid)icontent.Content;
            var uiElement = (TextBlock)grid.Children[0];

            using (Assert.Multiple())
            {
                await Assert.That(uiElement.Text).IsEqualTo("Test");

                await Assert.That(uiElement.FontSize).IsEqualTo(32);
                await Assert.That(grid.Background).IsEqualTo(Brushes.Bisque);
                await Assert.That(uiElement.FontWeight).IsEqualTo(FontWeights.ExtraBold);
                await Assert.That(uiElement.HorizontalAlignment).IsEqualTo(HorizontalAlignment.Right);
                await Assert.That(uiElement.Padding).IsEqualTo(new Thickness(12));
                await Assert.That(grid.Margin).IsEqualTo(new Thickness(24));
            }

        }

        [Test, STAThreadExecutor]
        public async Task TextLine_Configuration()
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

            using (Assert.Multiple())
            {
                await Assert.That(uiElement.FontSize).IsEqualTo(10);
                await Assert.That(uiElement.FontFamily).IsEqualTo(new FontFamily("Verdana"));
                await Assert.That(uiElement.HorizontalAlignment).IsEqualTo(HorizontalAlignment.Right);
            }


        }

        [Test, STAThreadExecutor]
        public async Task ToPrintContent()
        {
            var content = new TextBlock { Text = "Test" };

            var icontent = content.ToPrintContent();

            await Assert.That(icontent.Content).IsEqualTo(content);
        }
    }
}
