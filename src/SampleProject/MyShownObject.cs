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

namespace Mairegger.Printing.Sample
{
    using System.ComponentModel;
    using System.Linq;

    public class MyShownObject : INotifyPropertyChanged
    {
        private int _lenghtOfText;
        private int _numberOfLines;
        private string _text;

        public MyShownObject(string text)
        {
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int LenghtOfText
        {
            get { return _lenghtOfText; }
            private set
            {
                _lenghtOfText = value;
                OnPropertyChanged("LenghtOfText");
            }
        }

        public int NumberOfLines
        {
            get { return _numberOfLines; }
            private set
            {
                _numberOfLines = value;
                OnPropertyChanged("NumberOfLines");
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (value != null)
                {
                    LenghtOfText = value.Length;
                    NumberOfLines = value.Split('\n').Count();
                }
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}