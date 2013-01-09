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
    /// <summary>
    /// The main view.
    /// </summary>
    public class MainViewModel : BaseViewModel<MainView, MainViewModel>
    {
        private readonly ObservableCollection<SessionView> _Sessions;
        private SessionView _CurrentSession;
        private bool _HasOpenDocuments;
        private ImageSource _Icon;
        private string _Title = "[ Empty ]";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _Sessions = new ObservableCollection<SessionView>();
            _Sessions.CollectionChanged += SessionsChanged;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has open documents.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has open documents; otherwise, <c>false</c>.
        /// </value>
        public bool HasOpenDocuments
        {
            get { return _HasOpenDocuments; }
            set { SetValue(ref _HasOpenDocuments, value); }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public ImageSource Icon
        {
            get { return _Icon; }
            set { SetValue(ref _Icon, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return _Title; }
            set { SetValue(ref _Title, value); }
        }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public SessionView CurrentSession
        {
            get { return _CurrentSession; }
            set { SetValue(ref _CurrentSession, value); }
        }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public ObservableCollection<SessionView> Sessions
        {
            get { return _Sessions; }
        }

        /// <summary>
        /// Triggers when the list of sessions changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void SessionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var list = sender as IList;
            HasOpenDocuments = list != null && list.Count > 0;
        }

        /// <summary>
        /// Called when [created].
        /// </summary>
        protected internal override void OnCreated()
        {
            InvokeAction<ReadConfigurationAction>();

            View.AddHandler(ClosableTabItem.CloseTabEvent, new RoutedEventHandler(CloseSession));

            var intialSession = SessionView.Create(View, View.ActionQueueManager);
            Sessions.Add(intialSession);
        }

        /// <summary>
        /// Closes the selected session.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CloseSession(object sender, RoutedEventArgs e)
        {
            using (var session = e.OriginalSource as SessionView)
            {
                if (session == null) return;

                Sessions.Remove(session);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called before closing the application so we can pass parameters to the action.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnBeforeCloseApplication(ActionContext context)
        {
            context.Add("IsLoading", View.IsLoading);
        }
    }
}