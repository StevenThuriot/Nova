using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Actions.Session;
using Nova.Shell.Builders;
using Nova.Shell.Library;
using Nova.Shell.Managers;
using Nova.Shell.Views;
using RESX = Nova.Shell.Properties.Resources;


namespace Nova.Shell
{
    /// <summary>
    /// The Session ViewModel
    /// </summary>
    public class SessionViewModel : ViewModel<SessionView, SessionViewModel>, ISessionViewModel
    {
        private IView _currentView;
        private readonly dynamic _model;
        private readonly dynamic _applicationModel;
        private int _stackCounter;

        /// <summary>
        /// Gets a value indicating whether this instance is stacked.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stacked; otherwise, <c>false</c>.
        /// </value>
        public bool IsStacked { get; private set; }

        /// <summary>
        /// Gets the navigation action manager.
        /// </summary>
        /// <value>
        /// The navigation action manager.
        /// </value>
        public INavigationActionManager NavigationActionManager { get; private set; }

        /// <summary>
        /// Gets the application model.
        /// </summary>
        /// <value>
        /// The application model.
        /// </value>
        public dynamic ApplicationModel
        {
            get { return _applicationModel; }
        }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic Model
        {
            get { return _model; }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionViewModel" /> class.
        /// </summary>
        public SessionViewModel()
        {
            _applicationModel = ((App) Application.Current).Model;
            _model = new ExpandoObject();
        }
        
        /// <summary>
        /// Called when this viewmodel is created and fully initialized.
        /// </summary>
        protected override void OnCreated()
        {
            SetKnownActionTypes(typeof(SessionLeaveAction), typeof(NavigationAction)); //Optimalization

            var titleBinding = new Binding("CurrentView.Title") { Mode=BindingMode.OneWay };
            BindingOperations.SetBinding(View, SessionView.TitleProperty, titleBinding);

            NavigationActionManager = new NavigationActionManager(View);
        }

        public override Task<bool> Enter(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<SessionEnterAction>(parameters);
        }

        public override Task<bool> Leave(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<SessionLeaveAction>(parameters);
        }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        public IView CurrentView
        {
            get { return _currentView; }
            internal set
            {
                SetValue(ref _currentView, value);
            }
        }

        /// <summary>
        /// Called after entering this session.
        /// </summary>
        public void OnAfterEnter()
        {
            //TODO: Temporary until more data is passed along (e.g. when the user wants to open a certain page in a new session)
            View._NovaTree.NavigateToStartupPage();
        }

        /// <summary>
        /// Called before navigation.
        /// </summary>
        /// <param name="context">The context.</param>
        public void OnBeforeNavigation(ActionContext context)
        {
            var current = ActionContextEntry.Create(ActionContextConstants.CurrentViewConstant, CurrentView, false);
            context.Add(current);
        }

        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public TPageView Create<TPageView, TPageViewModel>(IView parent)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new()
        {
            var page = CreateView<TPageView, TPageViewModel>(parent, false);

            var initializer = new Dictionary<string, object> {{"Session", this}};

            ((ContentViewModel<TPageView, TPageViewModel>)page.ViewModel).Initialize(initializer);


            return page;
        }
        /// <summary>
        /// Creates a page specifically for the content zone and fills in the session model.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public TPageView Create<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : class, IView, new()
        {
            return Create<TPageView, TPageViewModel>(View);
        }

        /// <summary>
        /// Creates the navigational action.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return NavigationActionManager.New<TPageView, TPageViewModel>(parameters);
        }

        /// <summary>
        /// Creates the navigational action.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="nodeId">The node id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters)
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return NavigationActionManager.New<TPageView, TPageViewModel>(nodeId, parameters);
        }

        /// <summary>
        /// Determines whether the session is invalid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the session is invalid; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSessionValid()
        {
            if (!IsValid) //Session level
                return false;

            var currentView = CurrentView;

            return currentView == null || currentView.ViewModel.IsValid; //The content zone level.
        }

        /// <summary>
        /// Creates a wizard builder.
        /// </summary>
        /// <returns></returns>
        public IWizardBuilder CreateWizardBuilder()
        {
            return new WizardBuilder();
        }

        /// <summary>
        /// Stacks a new wizard.
        /// </summary>
        /// <param name="builder">The builder.</param>
        void ISessionViewModel.StackWizard(IWizardBuilder builder)
        {
            var dispatcher = View.Dispatcher;

            if (!dispatcher.CheckAccess())
                dispatcher.Invoke(() => ((ISessionViewModel)this).StackWizard(builder), DispatcherPriority.Send);

            Guid stackId;
            var overlay = CreateWizardOverlay(builder, out stackId);

            var handle = new StackInfo(stackId);
            overlay.Tag = handle;

            IsStacked = Interlocked.Increment(ref _stackCounter) > 0;
        }


        private Overlay CreateWizardOverlay(IWizardBuilder builder, out Guid stackId)
        {
            var wizardBuilder = (WizardBuilder) builder;

            var overlay = new Overlay();

            Grid.SetRowSpan(overlay, 2);
            Grid.SetColumnSpan(overlay, 2);

            Grid.SetRow(overlay, 0);
            Grid.SetColumn(overlay, 0);

            overlay.Delay = 0;
            overlay.MinimumDuration = 0;
            overlay.AnimationSpeed = 1;

            overlay.IsLoading = true;

            var wizard = CreateContentControl<WizardView, WizardViewModel>();

            WizardViewModel wizardViewModel = wizard.ViewModel;

            var size = builder.Size;

            wizard.Width = size.Width;
            wizard.Height = size.Height;

            wizard.MinWidth = size.MinWidth;
            wizard.MinHeight = size.MinHeight;
            wizardViewModel.Initialize(this, wizardBuilder);

            var canvas = new Canvas();
            canvas.Children.Add(wizard);

            overlay.Content = canvas;
            View._root.Children.Add(overlay);

            stackId = wizardViewModel.ID;
            return overlay;
        }

        /// <summary>
        /// Unstacks the session dialog.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="result">The result.</param>
        public void UnstackSessionDialog(Guid id, string result)
        {
            var stackHandle = Unstack(id) as StackHandle<string>;
            
            if (stackHandle == null) return;
            stackHandle.Release(result);
        }

        /// <summary>
        /// unstacks a wizard.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entries">The entries.</param>
        public void UnstackWizard(Guid id, IEnumerable<ActionContextEntry> entries)
        {
            var stackInfo = Unstack(id);

            if (stackInfo != null)
            {
                var actionContextEntries = entries.ToList();
                var handleEntry = ActionContextEntry.Create(ActionContextConstants.StackHandle, stackInfo, false);
                actionContextEntries.Add(handleEntry);
                entries = actionContextEntries;
            }

            ((IContentViewModel)CurrentView.ViewModel).ReturnToUseCase(entries);
        }

        private StackInfo Unstack(Guid id)
        {
            StackInfo stackInfo = null;

            var elements = View._root.Children.OfType<FrameworkElement>().Where(x => x.Tag != null);

            foreach (var element in elements)
            {
                var loopingInfo = element.Tag as StackInfo;

                if (loopingInfo == null) continue;

                if (loopingInfo.StackId != id) continue;

                stackInfo = loopingInfo;
                View._root.Children.Remove(element);
                IsStacked = Interlocked.Decrement(ref _stackCounter) > 0;

                break;
            }

            return stackInfo;
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="image">The image.</param>
        public void ShowDialogBox(string message, ImageSource image = null)
        {
            var handle = PrepareAndShowDialogBox(message, new[] { "OK" }, image);
            handle.Dispose();
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        /// <returns>
        /// The dialog results
        /// </returns>
        /// <remarks>
        /// This call blocks the current thread.
        /// </remarks>
        public T ShowDialogBox<T>(string message, IEnumerable<T> buttons, ImageSource image = null)
        {
            var handle = PrepareAndShowDialogBox(message, buttons, image);

            using (handle)
            {
                handle.Wait();
                return handle.Result;
            }
        }
        
        private StackHandle<T> PrepareAndShowDialogBox<T>(string message, IEnumerable<T> buttons, ImageSource image)
        {
            var dispatcher = View.Dispatcher;

            if (!dispatcher.CheckAccess())
                return dispatcher.Invoke(() => PrepareAndShowDialogBox<T>(message, buttons, image), DispatcherPriority.Send);

            var entries = new List<ActionContextEntry>();

            var entry = ActionContextEntry.Create(ActionContextConstants.DialogBoxMessage, message, false);
            entries.Add(entry);

            if (image != null)
            {
                var imageEntry = ActionContextEntry.Create(ActionContextConstants.DialogBoxImage, image, false);
                entries.Add(imageEntry);
            }

            if (buttons != null)
            {
                var boxedButtons = buttons.Where(x => x != null).Cast<object>().ToList().AsReadOnly();

                if (boxedButtons.Count > 0)
                {
                    var buttonEntry = ActionContextEntry.Create(ActionContextConstants.DialogBoxButtons, boxedButtons, false);
                    entries.Add(buttonEntry);
                }
            }

            var builder = CreateWizardBuilder();
            builder.AddStep<DialogView, DialogViewModel>(parameters: entries.ToArray());
            builder.Size = new ExtendedSize(480, 120, minWidth: 480, minHeight: 120);
            
            Guid stackId;
            var overlay = CreateWizardOverlay(builder, out stackId);

            var handle = new StackHandle<T>(stackId);
            overlay.Tag = handle;
            IsStacked = Interlocked.Increment(ref _stackCounter) > 0;
            
            return handle;
        }


        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            ((IDisposable)NavigationActionManager).Dispose();
        }
    }
}
