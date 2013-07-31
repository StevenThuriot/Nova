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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Nova.Controls;
using Nova.Helpers;
using Nova.Threading;
using Nova.Validation;
using Nova.Library;

namespace Nova.Library
{
    /// <summary>
    /// Interface to help initialize views.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
	internal interface ICanInjectStuff<TView, in TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
		where TView : class, IView
    {
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void Inject(IView parent, TViewModel viewModel);
	}

	public abstract partial class ViewModel<TView, TViewModel>
    {
	    //ViewModel.Creation
		
	    /// <summary>
	    /// Creates a new view with the current View as parent.
	    /// </summary>
		/// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
		/// <typeparam name="TGeneralView">The type of the view.</typeparam>
		/// <typeparam name="TGeneralViewModel">The type of the view model.</typeparam>
		/// <remarks>
		/// Supported types:
	    ///     - ExtendedPage
	    ///     - ExtendedUserControl
	    ///     - ExtendedContentControl
	    ///     - ExtendedControl
		/// </remarks>
        public TGeneralView CreateView<TGeneralView, TGeneralViewModel>(bool enterOnInitialize = true)		
            where TGeneralViewModel : ViewModel<TGeneralView, TGeneralViewModel>, new()
            where TGeneralView : class, IView, new()
        {
            var view = new TGeneralView();
            
            var injectableView = view as ICanInjectStuff<TGeneralView, TGeneralViewModel>;

            if (injectableView == null)
                throw new NotSupportedException();

            var viewModel = ViewModel<TGeneralView, TGeneralViewModel>.Create(view, _actionQueueManager, enterOnInitialize);
            injectableView.Inject(View, viewModel);

            return view;
        }

	    /// <summary>
	    /// Creates a new page with the current View as parent.
	    /// </summary>
		/// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
		/// <typeparam name="TPageView">The type of the page view.</typeparam>
		/// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
		public TPageView CreatePage<TPageView, TPageViewModel>(bool enterOnInitialize = true)		
			where TPageViewModel : ViewModel<TPageView, TPageViewModel>, new()
			where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
		{
			return ExtendedPage<TPageView, TPageViewModel>.Create(View, _actionQueueManager, enterOnInitialize);
		}
	    /// <summary>
	    /// Creates a new usercontrol with the current View as parent.
	    /// </summary>
		/// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
		/// <typeparam name="TUserControlView">The type of the page view.</typeparam>
		/// <typeparam name="TUserControlViewModel">The type of the page view model.</typeparam>
		public TUserControlView CreateUserControl<TUserControlView, TUserControlViewModel>(bool enterOnInitialize = true)		
			where TUserControlViewModel : ViewModel<TUserControlView, TUserControlViewModel>, new()
			where TUserControlView : ExtendedUserControl<TUserControlView, TUserControlViewModel>, new()
		{
			return ExtendedUserControl<TUserControlView, TUserControlViewModel>.Create(View, _actionQueueManager, enterOnInitialize);
		}
	    /// <summary>
	    /// Creates a new contentcontrol with the current View as parent.
	    /// </summary>
		/// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
		/// <typeparam name="TContentControlView">The type of the page view.</typeparam>
		/// <typeparam name="TContentControlViewModel">The type of the page view model.</typeparam>
		public TContentControlView CreateContentControl<TContentControlView, TContentControlViewModel>(bool enterOnInitialize = true)		
			where TContentControlViewModel : ViewModel<TContentControlView, TContentControlViewModel>, new()
			where TContentControlView : ExtendedContentControl<TContentControlView, TContentControlViewModel>, new()
		{
			return ExtendedContentControl<TContentControlView, TContentControlViewModel>.Create(View, _actionQueueManager, enterOnInitialize);
		}
	    /// <summary>
	    /// Creates a new control with the current View as parent.
	    /// </summary>
		/// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
		/// <typeparam name="TControlView">The type of the page view.</typeparam>
		/// <typeparam name="TControlViewModel">The type of the page view model.</typeparam>
		public TControlView CreateControl<TControlView, TControlViewModel>(bool enterOnInitialize = true)		
			where TControlViewModel : ViewModel<TControlView, TControlViewModel>, new()
			where TControlView : ExtendedControl<TControlView, TControlViewModel>, new()
		{
			return ExtendedControl<TControlView, TControlViewModel>.Create(View, _actionQueueManager, enterOnInitialize);
		}
	}
}


namespace Nova.Controls
{

    /// <summary>
    /// A default Page class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedPage<TView, TViewModel> : Page, IView, ICanInjectStuff<TView, TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedPage<TView, TViewModel>, new()
    {
        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedPage<TView, TViewModel>), new PropertyMetadata(false));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _parent;

        private TViewModel _viewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            private set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    DataContext = value;
                }
            }
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID = new Guid())
        {
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is loading.
        ///     This can also be interpreted as "busy".
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedPage{TView, TViewModel}"/> class.
        /// </summary>
        protected ExtendedPage()
        {
            SnapsToDevicePixels = true;

			FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;
        }
        
        /// <summary>
        /// Creates the specified Page.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");
            
            var page = new TView();
			var viewModel = ViewModel<TView, TViewModel>.Create(page, actionQueueManager, enterOnInitialize);

			((ICanInjectStuff<TView, TViewModel>)page).Inject(parent, viewModel);

            return page;
        }   
		
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void ICanInjectStuff<TView, TViewModel>.Inject(IView parent, TViewModel viewModel)
        {
            _parent = parent;
            ViewModel = viewModel;
        }     
        
        
        private int _loadingCounter;
        private readonly object _lock = new object();

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            lock (_lock)
            {
                var isLoading = ++_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _parent != null)
                {
                    _parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            lock (_lock)
            {
                var isLoading = --_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _parent != null)
                {
                    _parent.StopLoading();
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ExtendedPage is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedPage()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _parent.StopLoading();
                    }

                    _parent = null;
                }

                if (_viewModel != null)
                {
                    _viewModel.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// A default UserControl class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedUserControl<TView, TViewModel> : UserControl, IView, ICanInjectStuff<TView, TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedUserControl<TView, TViewModel>, new()
    {
        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedUserControl<TView, TViewModel>), new PropertyMetadata(false));
		
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExtendedUserControl<TView, TViewModel>), new PropertyMetadata(""));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _parent;

        private TViewModel _viewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            private set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    DataContext = value;
                }
            }
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID = new Guid())
        {
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is loading.
        ///     This can also be interpreted as "busy".
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
		
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedUserControl{TView, TViewModel}"/> class.
        /// </summary>
        protected ExtendedUserControl()
        {
            SnapsToDevicePixels = true;

			FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;
        }
        
        /// <summary>
        /// Creates the specified UserControl.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");
            
            var usercontrol = new TView();
			var viewModel = ViewModel<TView, TViewModel>.Create(usercontrol, actionQueueManager, enterOnInitialize);

			((ICanInjectStuff<TView, TViewModel>)usercontrol).Inject(parent, viewModel);

            return usercontrol;
        }   
		
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void ICanInjectStuff<TView, TViewModel>.Inject(IView parent, TViewModel viewModel)
        {
            _parent = parent;
            ViewModel = viewModel;
        }     
        
        
        private int _loadingCounter;
        private readonly object _lock = new object();

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            lock (_lock)
            {
                var isLoading = ++_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _parent != null)
                {
                    _parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            lock (_lock)
            {
                var isLoading = --_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _parent != null)
                {
                    _parent.StopLoading();
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ExtendedUserControl is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedUserControl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _parent.StopLoading();
                    }

                    _parent = null;
                }

                if (_viewModel != null)
                {
                    _viewModel.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// A default ContentControl class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedContentControl<TView, TViewModel> : ContentControl, IView, ICanInjectStuff<TView, TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedContentControl<TView, TViewModel>, new()
    {
        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedContentControl<TView, TViewModel>), new PropertyMetadata(false));
		
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExtendedContentControl<TView, TViewModel>), new PropertyMetadata(""));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _parent;

        private TViewModel _viewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            private set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    DataContext = value;
                }
            }
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID = new Guid())
        {
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is loading.
        ///     This can also be interpreted as "busy".
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
		
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedContentControl{TView, TViewModel}"/> class.
        /// </summary>
        protected ExtendedContentControl()
        {
            SnapsToDevicePixels = true;

			FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;
        }
        
        /// <summary>
        /// Creates the specified ContentControl.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");
            
            var contentcontrol = new TView();
			var viewModel = ViewModel<TView, TViewModel>.Create(contentcontrol, actionQueueManager, enterOnInitialize);

			((ICanInjectStuff<TView, TViewModel>)contentcontrol).Inject(parent, viewModel);

            return contentcontrol;
        }   
		
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void ICanInjectStuff<TView, TViewModel>.Inject(IView parent, TViewModel viewModel)
        {
            _parent = parent;
            ViewModel = viewModel;
        }     
        
        
        private int _loadingCounter;
        private readonly object _lock = new object();

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            lock (_lock)
            {
                var isLoading = ++_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _parent != null)
                {
                    _parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            lock (_lock)
            {
                var isLoading = --_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _parent != null)
                {
                    _parent.StopLoading();
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ExtendedContentControl is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedContentControl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _parent.StopLoading();
                    }

                    _parent = null;
                }

                if (_viewModel != null)
                {
                    _viewModel.Dispose();
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// A default Control class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedControl<TView, TViewModel> : Control, IView, ICanInjectStuff<TView, TViewModel>
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedControl<TView, TViewModel>, new()
    {
        // ReSharper disable StaticFieldInGenericType

        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedControl<TView, TViewModel>), new PropertyMetadata(false));
		
        /// <summary>
        /// The title property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ExtendedControl<TView, TViewModel>), new PropertyMetadata(""));

        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Flag wether this instance is disposed.
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// The parent view
        /// </summary>
        private IView _parent;

        private TViewModel _viewModel;
        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            private set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    DataContext = value;
                }
            }
        }
        
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityID = new Guid())
        {
            return FocusHelper.FocusControl(this, fieldName, entityID);
        }

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        public ValidationControl ValidationControl { get; set; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel IView.ViewModel
        {
            get { return ViewModel; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is loading.
        ///     This can also be interpreted as "busy".
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }
		
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedControl{TView, TViewModel}"/> class.
        /// </summary>
        protected ExtendedControl()
        {
            SnapsToDevicePixels = true;

			FocusVisualStyle = null;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;
        }
        
        /// <summary>
        /// Creates the specified Control.
        /// </summary>
        /// <param name="parent">The parent view.</param>
        /// <param name="actionQueueManager">The action queue manager.</param>
        /// <param name="enterOnInitialize">if set to <c>true</c>, the Enter Action will be triggered automatically. Default is true.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">window</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        internal static TView Create(IView parent, IActionQueueManager actionQueueManager, bool enterOnInitialize)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");
            
            var control = new TView();
			var viewModel = ViewModel<TView, TViewModel>.Create(control, actionQueueManager, enterOnInitialize);

			((ICanInjectStuff<TView, TViewModel>)control).Inject(parent, viewModel);

            return control;
        }   
		
        /// <summary>
        /// Injects the specified viewmodel and parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="viewModel">The view model.</param>
        void ICanInjectStuff<TView, TViewModel>.Inject(IView parent, TViewModel viewModel)
        {
            _parent = parent;
            ViewModel = viewModel;
        }     
        
        
        private int _loadingCounter;
        private readonly object _lock = new object();

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            lock (_lock)
            {
                var isLoading = ++_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (isLoading && _parent != null)
                {
                    _parent.StartLoading();
                }
            }
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            lock (_lock)
            {
                var isLoading = --_loadingCounter > 0;

                if (IsLoading == isLoading) return;

                IsLoading = isLoading;

                if (!isLoading && _parent != null)
                {
                    _parent.StopLoading();
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// ExtendedControl is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedControl()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_parent != null)
                {
                    if (IsLoading)
                    {
                        //Make sure the parent doesn't keep thinking the child is still loading yet unexistent.
                        _parent.StopLoading();
                    }

                    _parent = null;
                }

                if (_viewModel != null)
                {
                    _viewModel.Dispose();
                }
            }

            _disposed = true;
        }
    }
}
