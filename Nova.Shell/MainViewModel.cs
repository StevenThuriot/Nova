#region License

// 
//  Copyright 2012 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System.Windows.Media;
using Nova.Base;
using Nova.Shell.Actions.MainWindow;

namespace Nova.Shell
{
    public class MainViewModel : BaseViewModel<MainView, MainViewModel>
    {
        private bool _HasOpenDocuments;
        public bool HasOpenDocuments
        {
            get { return _HasOpenDocuments; }
            set
            {
                if (_HasOpenDocuments == value) return;

                _HasOpenDocuments = value;
                OnPropertyChanged(() => HasOpenDocuments);
            }
        }

        private ImageSource _Icon;
        public ImageSource Icon
        {
            get { return _Icon; }
            set
            {
                _Icon = value;
                OnPropertyChanged(() => Icon);
            }
        }

        private string _Title = "[ Empty ]";
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                _Title = value;
                OnPropertyChanged(() => Title);
            }
        }

        protected override void OnCreated()
        {
            InvokeAction<ReadConfigurationAction>();
        }

        public void OnBeforeCloseApplication(ActionContext context)
        {
            context.Add("IsLoading", View.IsLoading);
        }
    }
}