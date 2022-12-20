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
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Mairegger.Printing.PrintProcessor;
    using Mairegger.Printing.Properties;

    [ExcludeFromCodeCoverage]
    internal class WindowsProvider : IWindowProvider
    {
        private readonly Window _window = new Window { WindowStartupLocation = WindowStartupLocation.CenterOwner };

        public event EventHandler Closed
        {
            add => _window.Closed += value;
            remove => _window.Closed -= value;
        }

        public void Show(string windowTitle, DocumentViewer documentViewer)
        {
            _window.SetCurrentValue(ContentControl.ContentProperty, documentViewer);
            _window.SetCurrentValue(Window.TitleProperty, $"{l10n.Preview}: {windowTitle}");
            _window.KeyDown += WindowOnKeyDown;
            _window.Show();

            void WindowOnKeyDown(object? sender, KeyEventArgs args)
            {
               if (args.Key == Key.Escape && sender is Window window)
               {
                   window.Close();
               }
            }
        }
    }
}
