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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using Nova.Base.Actions;
using Nova.Controls;
using Nova.Threading;
using Nova.Validation;

namespace Nova.Base
{
    /// <summary>
    /// The base class for any ViewModel.
    /// </summary>
    /// <typeparam name="TView">The type of the linked View.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class ViewModel<TView, TViewModel> : INotifyPropertyChanged, IDisposable
		where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
        private bool _IsValid;
        private bool _Disposed;
        private Guid _ID;

        private ReadOnlyErrorCollection _ErrorCollection;

        /// <summary>
		/// Gets the view.
		/// </summary>
        public TView View { get; private set; }

        /// <summary>
        /// The viewmodel's EnterAction, which triggers when entering this view.
        /// If not set, the default EnterAction will be used.
        /// </summary>
        protected EnterAction<TView, TViewModel> EnterAction { get; set; }

        /// <summary>
        /// The viewmodel's LeaveAction, which triggers when leaving this view.
        /// If not set, the default EnterAction will be used.
        /// </summary>
        protected LeaveAction<TView, TViewModel> LeaveAction { get; set; }

        /// <summary>
        /// Gets or sets the ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        public Guid ID
        {
            get { return _ID; }
            protected set
            {
                if (value == Guid.Empty)
                    throw new ArgumentNullException("value");
                
                _ID = value;
            }
        }

        /// <summary>
		/// Gets the action controller.
		/// </summary>
        internal ActionController<TView, TViewModel> ActionController { get; private set; }

        /// <summary>
        /// Gets the validation control.
        /// </summary>
        /// <value>
        /// The validation control. (Possibly null when not found!)
        /// </value>
        internal ValidationControl ValidationControl { get; set; }

        /// <summary>
		/// Gets the action manager.
		/// </summary>
		public dynamic ActionManager { get; private set; }

		/// <summary>
		/// Sets the known action types.
		/// The action manager will choose from these types to initiate an action.
		/// </summary>
		/// <param name="knownTypes">The known types.</param>
		protected void SetKnownActionTypes(params Type[] knownTypes)
		{
			ActionManager.SetKnownTypes(knownTypes);
		}

        /// <summary>
		/// Gets the error collection.
		/// </summary>
		public ReadOnlyErrorCollection ErrorCollection
		{
			get { return _ErrorCollection; }
			internal set
			{
                if (SetValue(ref _ErrorCollection, value))
                {
                    IsValid = _ErrorCollection == null || _ErrorCollection.Count == 0;
                }

			}
		}

        /// <summary>
	    /// Gets a value indicating whether this instance is valid.
	    /// </summary>
	    /// <value>
	    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
	    /// </value>
        public bool IsValid
	    {
	        get { return _IsValid; }
	        private set { SetValue(ref _IsValid, value); }
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel{TView,TViewModel}"/> class.
        /// </summary>
		protected ViewModel()
        {
			_ErrorCollection = new ReadOnlyErrorCollection();
		    _IsValid = true;
            _ID = Guid.NewGuid();
        }

		/// <summary>
		/// Notifies clients that propertyName has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
		/// <summary>
		/// Notifies clients that propertyName has changed.
		/// </summary>
		/// <param name="propertyName">The property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Checks if the private member changed. If it did, it will set the new value and call the PropertyChanged event handler
		/// </summary>
		/// <typeparam name="TMember">The type of the member.</typeparam>
		/// <param name="privateMember">The member that is being set.</param>
		/// <param name="value">The new value.</param>
		/// <param name="propertyName">The property that changed.</param>
		/// <returns>True if the property changed.</returns>
        protected bool SetValue<TMember>(ref TMember privateMember, TMember value, [CallerMemberName] string propertyName = null)
		{
		    if (EqualityComparer<TMember>.Default.Equals(privateMember, value))
		    {
		        return false;
		    }

		    privateMember = value;

// ReSharper disable ExplicitCallerInfoArgument
		    OnPropertyChanged(propertyName);
// ReSharper restore ExplicitCallerInfoArgument

		    return true;
		}

        /// <summary>
        /// Called to trigger all the Entering logic for this ViewModel.
        /// </summary>
        public void Enter()
        {
            if (EnterAction == null)
            {
                InvokeAction<EnterAction<TView, TViewModel>>();
            }
            else
            {
                ActionController.InternalInvokeAction(EnterAction);
            }
        }

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        public void Leave()
        {
            if (LeaveAction == null)
            {
                InvokeAction<LeaveAction<TView, TViewModel>>();
            }
            else
            {
                ActionController.InternalInvokeAction(LeaveAction);
            }
        }

		/// <summary>
		/// Called when this viewmodel is created.
		/// </summary>
		protected internal virtual void OnCreated()
		{
		}

        /// <summary>
        /// Creates the specified viewmodel.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="actionQueueManager"> </param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal static TViewModel Create(TView view, IActionQueueManager actionQueueManager)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

		    var viewModel = new TViewModel();
            viewModel.Initialize(view, actionQueueManager);

		    return viewModel;
		}

        /// <summary>
        /// Initializes the viewmodel and triggers all the needed logic.
        /// </summary>
        internal void Initialize(TView view, IActionQueueManager actionQueueManager)
        {
            if (view == null)
                throw new ArgumentNullException("view");
            if (actionQueueManager == null)
                throw new ArgumentNullException("actionQueueManager");

            var viewModel = (TViewModel)this;

            View = view;
            ActionController = new ActionController<TView, TViewModel>(view, viewModel, actionQueueManager);
            ActionManager = new ActionManager<TView, TViewModel>(view, viewModel);

            OnCreated();
            Enter();
        }

        /// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
		    var objectToSave = new DynamicContext();
            Save(objectToSave);

            var parameter = ActionContextEntry.Create("ObjectToSave", objectToSave);
			ActionController.InvokeAction<SaveStateAction<TView, TViewModel>>(parameter);
		}

		/// <summary>
		/// Loads this instance.
		/// Load(value) will only trigger in case the ViewModel has been saved before.
		/// </summary>
		public void Load()
		{
			ActionController.InvokeAction<LoadStateAction<TView, TViewModel>>();
		}

    	/// <summary>
		/// You can use this method to specify what exactly you want to save.
		/// Add everything to the object to prepare it for serialization. 
		/// All the parameters added should be serializable.
		/// </summary>
		/// <param name="value">The object to save.</param>
		protected internal virtual void Save(dynamic value) { }

		/// <summary>
		/// You can use this method to specify what exactly you want to load.
		/// Retreive everything to the object and load it back onto your ViewModel.
		/// </summary>
		/// <param name="value">The object to load.</param>
		protected internal virtual void Load(dynamic value) { }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void InvokeAction<T>(params ActionContextEntry[] arguments)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionController.InvokeAction<T>(arguments);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void InvokeAction<T>(UIElement sender, params ActionContextEntry[] arguments)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionController.InvokeAction<T>(sender, arguments);
        }

        /// <summary>
        /// Creates a new page with the current window as parent.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        public TPageView CreatePage<TPageView, TPageViewModel>()
            where TPageViewModel : ViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var internalView = (IInternalView) View;
            return ExtendedPage<TPageView, TPageViewModel>.Create(internalView);
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
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ViewModel{TView,TViewModel}"/> is reclaimed by garbage collection.
		/// </summary>
		~ViewModel()
		{
			Dispose(false);
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
				DisposeManagedResources();

				if (ActionManager != null)
					ActionManager.Dispose();
				
				ActionManager = null;
			    View = null;
			    _ErrorCollection = null;
			}

		    DisposeUnmanagedResources();
			_Disposed = true;
    	}
        
        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }
	}
}