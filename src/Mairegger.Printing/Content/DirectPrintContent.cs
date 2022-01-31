// Copyright 2017 Michael Mairegger
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

    /// <inheritdoc />
    /// <summary>
    ///     A print content that can be positioned freely on the page.
    /// </summary>
    /// <remarks>
    ///     By returning an instance of this class from a
    ///     <see cref="M:Mairegger.Printing.PrintProcessor.IPrintProcessor.ItemCollection" />
    ///     or <see cref="M:Mairegger.Printing.PrintProcessor.IPrintProcessor.GetCustomPageContent(System.Int32)" /> following
    ///     behaviors apply:
    ///     <list type="bullets">
    ///         <listheader>
    ///             <term>Method</term>
    ///             <description>Behavior</description>
    ///         </listheader>
    ///         <item>
    ///             <term>
    ///                 <see cref="M:Mairegger.Printing.PrintProcessor.IPrintProcessor.ItemCollection" />
    ///             </term>
    ///             <description>The object is printed on the current page.</description>
    ///         </item>
    ///         <item>
    ///             <term>
    ///                 <see cref="M:Mairegger.Printing.PrintProcessor.IPrintProcessor.GetCustomPageContent(System.Int32)" />
    ///             </term>
    ///             <description>The item can be printed on any page that is desired.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class DirectPrintContent : IDirectPrintContent
    {
        private UIElement? _content;

        public virtual UIElement Content
        {
            get => _content ?? throw new InvalidOperationException($"{nameof(Content)} must be set before accessing this property");
            set => _content = value;
        }

        public Point Position { get; set; }
    }
}