#region License

// 
// Copyright 2012 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;
using Nova.Base;
using Nova.Controls;
using Nova.Shell.Actions.MainWindow;

namespace Nova.Shell
{
    public class MainViewModel : BaseViewModel<MainView, MainViewModel>
    {
        private readonly ObservableCollection<SessionView> _Sessions;
        private SessionView _CurrentSession;
        private bool _HasOpenDocuments;

        private ImageSource _Icon;

        private string _Title = "[ Empty ]";

        public MainViewModel()
        {
            _Sessions = new ObservableCollection<SessionView>();
            _Sessions.CollectionChanged += SessionsChanged;
        }

        public bool HasOpenDocuments
        {
            get { return _HasOpenDocuments; }
            set { SetValue(ref _HasOpenDocuments, value); }
        }

        public ImageSource Icon
        {
            get { return _Icon; }
            set { SetValue(ref _Icon, value); }
        }

        public string Title
        {
            get { return _Title; }
            set { SetValue(ref _Title, value); }
        }

        public SessionView CurrentSession
        {
            get { return _CurrentSession; }
            set { SetValue(ref _CurrentSession, value); }
        }

        public ObservableCollection<SessionView> Sessions
        {
            get { return _Sessions; }
        }

        private void SessionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var list = sender as IList;
            HasOpenDocuments = list != null && list.Count > 0;
        }

        protected internal override void OnCreated()
        {
            InvokeAction<ReadConfigurationAction>();

            View.AddHandler(ClosableTabItem.CloseTabEvent, new RoutedEventHandler(CloseSession));

            SessionView intialSession = SessionView.Create(View, View._ActionQueueManager);
            Sessions.Add(intialSession);
        }

        private void CloseSession(object sender, RoutedEventArgs e)
        {
            using (var session = e.OriginalSource as SessionView)
            {
                if (session == null) return;

                Sessions.Remove(session);
                e.Handled = true;
            }
        }

        public void OnBeforeCloseApplication(ActionContext context)
        {
            context.Add("IsLoading", View.IsLoading);
        }
    }
}