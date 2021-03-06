using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Actions.MainWindow;
using RESX = Nova.Shell.Properties.Resources;

namespace Nova.Shell
{
    /// <summary>
    /// The main view.
    /// </summary>
    public class MainViewModel : ViewModel<MainView, MainViewModel>
    {
        private readonly ObservableCollection<SessionView> _sessions;
        private SessionView _currentSession;
        private bool _hasOpenDocuments;
        private ImageSource _icon;
        private string _title = RESX.Empty;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            MaximizeCommand = new RelayCommand(MaximizeView);
            MinimizeCommand = new RelayCommand(MinimizeView);

            _sessions = new ObservableCollection<SessionView>();
            _sessions.CollectionChanged += SessionsChanged;
        }

        /// <summary>
        /// Gets the maximize command.
        /// </summary>
        /// <value>
        /// The maximize command.
        /// </value>
        public ICommand MaximizeCommand { get; private set; }
        /// <summary>
        /// Gets the minimize command.
        /// </summary>
        /// <value>
        /// The minimize command.
        /// </value>
        public ICommand MinimizeCommand { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has open documents.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has open documents; otherwise, <c>false</c>.
        /// </value>
        public bool HasOpenDocuments
        {
            get { return _hasOpenDocuments; }
            set { SetValue(ref _hasOpenDocuments, value); }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public ImageSource Icon
        {
            get { return _icon; }
            set { SetValue(ref _icon, value); }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return _title; }
            set { SetValue(ref _title, value); }
        }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public SessionView CurrentSession
        {
            get { return _currentSession; }
            set { SetValue(ref _currentSession, value); }
        }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public ObservableCollection<SessionView> Sessions
        {
            get { return _sessions; }
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
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(CloseSessionAction),
                                typeof(CreateNewSessionAction),
                                typeof(FocusTabAction),
                                typeof(ShutdownAction));
            
            View.AddHandler(ClosableTabItem.CloseTabEvent, new RoutedEventHandler(CloseSession));
        }

        public override Task<bool> Enter(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<ComposeAndInitializeAction>(parameters);
        }

        /// <summary>
        /// Creates a new session using the default module.
        /// </summary>
        internal SessionView CreateSession()
        {
            var sessionView = CreateContentControl<SessionView, SessionViewModel>();
            return sessionView;
        }

        /// <summary>
        /// Closes the selected session.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void CloseSession(object sender, RoutedEventArgs e)
        {
            var sessionView = e.OriginalSource as SessionView;

            if (sessionView == null) return;

            var entry = ActionContextEntry.Create(sessionView, false);

            InvokeAction<CloseSessionAction>(entry);
            e.Handled = true;
        }

        /// <summary>
        /// Sets the current session in the action context.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public void OnBeforeCloseSession(ActionContext actionContext)
        {
            //Check if already added by our event handler. If not, insert the current session.
            if (actionContext.ContainsKey(typeof (SessionView).FullName)) return;

            if (CurrentSession == null) return;

            var entry = ActionContextEntry.Create(CurrentSession, false);
            actionContext.Add(entry);
        }

        /// <summary>
        /// Minimizes the view.
        /// </summary>
        private void MinimizeView()
        {
            SystemCommands.MinimizeWindow(View);
            FixWindowChromeBug();
        }

        /// <summary>
        /// Maximizes or restores the view.
        /// </summary>
        private void MaximizeView()
        {
            var mainView = View;

            if (mainView.WindowState == WindowState.Normal)
            {
                SystemCommands.MaximizeWindow(mainView);
            }
            else
            {
                SystemCommands.RestoreWindow(mainView);
            }

            FixWindowChromeBug();
        }

        /// <summary>
        /// Fixes the window chrome bug.
        /// If the window chrome stays attached, the taskbar button might disappear.
        /// </summary>
        private void FixWindowChromeBug()
        {
            var mainView = View;

            var chrome = WindowChrome.GetWindowChrome(mainView);
            WindowChrome.SetWindowChrome(mainView, null);

            var setChrome = new Action<Window, WindowChrome>(WindowChrome.SetWindowChrome);
            mainView.Dispatcher.BeginInvoke(setChrome, DispatcherPriority.Render, mainView, chrome);
        }
    }
}