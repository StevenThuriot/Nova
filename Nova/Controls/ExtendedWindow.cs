using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Nova.Base;
using RESX = Nova.Properties.Resources;

namespace Nova.Controls
{
	/// <summary>
	/// A default Window class that has added logic for MVVM.
	/// </summary>
	/// <typeparam name="TView">The type of the view.</typeparam>
	/// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class ExtendedWindow<TView, TViewModel> : Window, IView, IDisposable
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
		where TView : class, IView
	{
		private bool _Disposed;

		private TViewModel _ViewModel;
		/// <summary>
		/// Gets the view model.
		/// </summary>
		public TViewModel ViewModel
		{
			get { return _ViewModel; }
			private set
			{
				if (_ViewModel != value)
				{
					_ViewModel = value;
					DataContext = value;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtendedWindow&lt;TView, TViewModel&gt;"/> class.
		/// </summary>
		protected ExtendedWindow()
		{
			UseLayoutRounding = true;
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
			VisualTextRenderingMode = TextRenderingMode.ClearType;

			WindowStartupLocation = WindowStartupLocation.CenterScreen;

            ViewModel = BaseViewModel<TView, TViewModel>.Create(this as TView);

			Closed += (sender, args) => ViewModel.Dispose();
		}


// ReSharper disable StaticFieldInGenericType
		/// <summary>
		/// A value indicating whether this instance is loading.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty IsLoadingProperty =
			DependencyProperty.Register("IsLoading", typeof(bool), typeof(ExtendedWindow<TView, TViewModel>), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether this instance is loading.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
		/// </value>
		public bool IsLoading
		{
			get { return (bool)GetValue(IsLoadingProperty); }
			set { SetValue(IsLoadingProperty, value); }
		}

		/// <summary>
		/// How to dispose this view's controls.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static readonly DependencyProperty DisposeMethodProperty =
			DependencyProperty.Register("DisposeMethod", typeof(TreeType), typeof(ExtendedWindow<TView, TViewModel>), new PropertyMetadata(TreeType.LogicalTree));

		/// <summary>
		/// Gets or sets the dispose method.
		/// Default = LogicalTree
		/// </summary>
		/// <value>
		/// The dispose method.
		/// </value>
		public TreeType DisposeMethod
		{
			get { return (TreeType) GetValue(DisposeMethodProperty); }
			set { SetValue(DisposeMethodProperty, value); }
		}

// ReSharper restore StaticFieldInGenericType

		/// <summary>
		/// Starts the animated loading.
		/// </summary>
		public virtual void StartLoading()
		{
			IsLoading = true;
			Cursor = Cursors.AppStarting;
		}

		/// <summary>
		/// Stops the animated loading.
		/// </summary>
		public virtual void StopLoading()
		{
			IsLoading = false;
			Cursor = Cursors.Arrow;
		}
		
		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		public void InvokeOnMainThread(Action work)
		{
			InvokeOnMainThread(work, DispatcherPriority.Normal);
		}

		/// <summary>
		/// Invokes the specified action on the main thread.
		/// </summary>
		/// <param name="work">The work.</param>
		/// <param name="priority">The priority.</param>
		public void InvokeOnMainThread(Action work, DispatcherPriority priority)
		{
			if (Dispatcher.CheckAccess())
			{
				RunMethodSafely(work);
			}
			else
			{
				Dispatcher.BeginInvoke(priority, new Action(() => RunMethodSafely(work)));
			}
		}

		/// <summary>
		/// Runs the method safely.
		/// </summary>
		/// <param name="work">The work.</param>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private static void RunMethodSafely(Action work)
		{
			try
			{
				work();
			}
			catch (Exception exception)
			{
				Base.ExceptionHandler.Handle(exception, RESX.ErrorTitle);
			}
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ExtendedWindow&lt;TView, TViewModel&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~ExtendedWindow()
		{
			Dispose(false);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose(bool disposing)
		{
			if (_Disposed) return;
			
			if (disposing)
			{
				BindingOperations.ClearAllBindings(this);

				CommandBindings.Clear();
				InputBindings.Clear();

				switch (DisposeMethod)
				{
					case TreeType.LogicalTree:
						DisposeLogicalTree(this);
						break;
					case TreeType.VisualTree:
						DisposeVisualTree(this);
						break;
				}

				if (_ViewModel != null)
				{
					_ViewModel.Dispose();
				}
			}

			_Disposed = true;
		}

		/// <summary>
		/// Disposes the logical tree.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		private static void DisposeLogicalTree(DependencyObject dependencyObject)
		{
			foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject))
			{
				var childDependencyObject = child as DependencyObject;
				if (childDependencyObject != null)
				{
					DisposeLogicalTree(childDependencyObject);
					BindingOperations.ClearAllBindings(childDependencyObject);
				}

				var disposable = child as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Disposes the visual tree.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		private static void DisposeVisualTree(DependencyObject dependencyObject)
		{
			var childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);

			for (int i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild(dependencyObject, i);
				
				DisposeVisualTree(child);
				BindingOperations.ClearAllBindings(child);


				var disposable = child as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
