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

namespace Mairegger.Printing.Content
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public sealed class PrintProcessorBackground
    {
        public PrintProcessorBackground(Panel panel, Rect size = new Rect())
        {
            if (panel == null)
            {
                throw new ArgumentNullException(nameof(panel));
            }

            Size = size;
            Element = panel;
        }

        public Panel Element { get; private set; }

        public Rect Size { get; private set; }
    }
}