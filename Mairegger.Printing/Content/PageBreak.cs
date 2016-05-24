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

    public sealed class PageBreak : IPrintContent
    {
        private static readonly Lazy<PageBreak> LazyInstance = new Lazy<PageBreak>(() => new PageBreak());

        private PageBreak()
        {
        }

        public static IPrintContent Instance => LazyInstance.Value;

        UIElement IPrintContent.Content
        {
            get { throw new InvalidOperationException("There is no available content."); }
        }
    }
}