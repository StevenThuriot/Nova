using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Nova.Helpers;
using Nova.Library;
using Nova.Library.Actions;
using Nova.Threading;
using Nova.Validation;

namespace Nova.Controls
{
    /// <summary>
    ///     A default Window class that has added logic for MVVM.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ExtendedWindow<TView, TViewModel> : Window, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
        where TView : ExtendedWindow<TView, TViewModel>
    {
        /// <summary>
        ///     A value indicating whether this instance is loading.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")] public static readonly
// ReSharper disable StaticFieldInGenericType
            DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof (bool), typeof (ExtendedWindow<TView, TViewModel>), new PropertyMetadata(false));

// ReSharper restore StaticFieldInGenericType

        private bool _disposed;

        private int _loadingCounter;
        private ActionQueueManager _actionQueueManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedWindow&lt;TView, TViewModel&gt;" /> class.
        /// </summary>
        protected ExtendedWindow()
        {
            SnapsToDevicePixels = true;

            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            VisualTextRenderingMode = TextRenderingMode.ClearType;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            _actionQueueManager = new ActionQueueManager();

            ViewModel = ViewModel<TView, TViewModel>.Create((TView) this, _actionQueueManager);

            Closing += (sender, args) => ViewModel.InvokeAction<LeaveAction<TView, TViewModel>>();
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName)
        {
            return FocusControl(fieldName, (Guid)NovaValidation.EntityIDProperty.DefaultMetadata.DefaultValue);
        }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityId">The entity ID.</param>
        /// <returns></returns>
        public bool FocusControl(string fieldName, Guid entityId)
        {
            return FocusHelper.FocusControl(this, fieldName, entityId);
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
            get { return (bool) GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public virtual void StartLoading()
        {
            IsLoading = Interlocked.Increment(ref _loadingCounter) > 0;
            UpdateCursor(true);
        }

        /// <summary>
        ///     Stops the animated loading.
        /// </summary>
        public virtual void StopLoading()
        {
            IsLoading = Interlocked.Decrement(ref _loadingCounter) > 0;

            if (!IsLoading)
            {
                UpdateCursor(false);
            }
        }

        /// <summary>
        /// Updates the cursor.
        /// </summary>
        /// <param name="isloading">if set to <c>true</c> [isloading].</param>
        protected virtual void UpdateCursor(bool isloading)
        {
            Cursor = isloading ? Cursors.AppStarting : Cursors.Arrow;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged resources and performs other cleanup operations before the
        ///     <see cref="ExtendedWindow&lt;TView, TViewModel&gt;" /> is reclaimed by garbage collection.
        /// </summary>
        ~ExtendedWindow()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (ViewModel != null)
                {
                    ViewModel.Dispose();
                }

                if (_actionQueueManager != null)
                {
                    _actionQueueManager.Dispose();
                    _actionQueueManager = null;
                }
            }

            _disposed = true;
        }
    }
}