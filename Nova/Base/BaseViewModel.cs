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
using System.Linq.Expressions;
using System.Windows;
using Nova.Base.Actions;
using Nova.Controls;
using Nova.Validation;

namespace Nova.Base
{
    /// <summary>
    /// The base class for any ViewModel.
    /// </summary>
    /// <typeparam name="TView">The type of the linked View.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract class BaseViewModel<TView, TViewModel> : INotifyPropertyChanged, IDisposable
		where TView : class, IView
        where TViewModel : BaseViewModel<TView, TViewModel>, new()
	{
		/// <summary>
		/// Gets the view.
		/// </summary>
		public TView View { get; internal set; }

		/// <summary>
		/// Gets the action controller.
		/// </summary>
        internal ActionController<TView, TViewModel> ActionController { get; private set; }

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

		private ReadOnlyErrorCollection _ErrorCollection;
		/// <summary>
		/// Gets the error collection.
		/// </summary>
		public ReadOnlyErrorCollection ErrorCollection
		{
			get { return _ErrorCollection; }
			internal set
			{
                if (SetValue(ref _ErrorCollection, value, "ErrorCollection"))
                {
                    IsValid = _ErrorCollection == null || _ErrorCollection.Count == 0;
                }

			}
		}

	    private bool _IsValid;
    	private bool _Disposed;

    	/// <summary>
	    /// Gets a value indicating whether this instance is valid.
	    /// </summary>
	    /// <value>
	    ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
	    /// </value>
        public bool IsValid
	    {
	        get { return _IsValid; }
	        private set { SetValue(ref _IsValid, value, "IsValid"); }
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel&lt;TView, TViewModel&gt;"/> class.
        /// </summary>
		protected BaseViewModel()
		{
			_ErrorCollection = new ReadOnlyErrorCollection();
		    _IsValid = true;
		}

	    #region INotifyPropertyChanged Members

		/// <summary>
		/// Notifies clients that propertyName has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		/// <param name="propertyExpression">The property expression.</param>
		/// <returns></returns>
		private static string GetPropertyName(Expression<Func<object>> propertyExpression)
		{
			MemberExpression memberExpression = propertyExpression.Body as MemberExpression ??
			                                    ((UnaryExpression) propertyExpression.Body).Operand as MemberExpression;

			return memberExpression == null ? string.Empty : memberExpression.Member.Name;
		}

		/// <summary>
		/// Notifies clients that propertyName has changed.
		/// </summary>
		/// <param name="propertyName">The property that changed.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;

			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Notifies clients that propertyName has changed.
		/// </summary>
		/// <param name="propertyExpression">The property that changed.</param>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		protected void OnPropertyChanged(Expression<Func<object>> propertyExpression)
		{
			var propertyName = GetPropertyName(propertyExpression);
			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// Checks if the private member changed. If it did, it will set the new value and call the PropertyChanged event handler
		/// </summary>
		/// <typeparam name="TMember">The type of the member.</typeparam>
		/// <param name="privateMember">The member that is being set.</param>
		/// <param name="value">The new value.</param>
		/// <param name="propertyName">The property that changed.</param>
		/// <returns>True if the property changed.</returns>
		protected bool SetValue<TMember>(ref TMember privateMember, TMember value, string propertyName)
		{
			var returnValue = false;

			if (!Equals(privateMember, value))
			{
				privateMember = value;
				OnPropertyChanged(propertyName);
				returnValue = true;
			}

			return returnValue;
		}

		/// <summary>
		/// Checks if the private member changed. If it did, it will set the new value and call the PropertyChanged event handler
		/// </summary>
		/// <typeparam name="TMember">The type of the member.</typeparam>
		/// <param name="privateMember">The member that is being set.</param>
		/// <param name="value">The new value.</param>
		/// <param name="propertyExpression">The property that changed.</param>
		/// <returns>True if the property changed.</returns>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		protected bool SetValue<TMember>(ref TMember privateMember, TMember value, Expression<Func<object>> propertyExpression)
		{
			var propertyName = GetPropertyName(propertyExpression);
			return SetValue(ref privateMember, value, propertyName);
		}

		/// <summary>
		/// Called when [created].
		/// </summary>
		protected internal virtual void OnCreated()
		{
		}

		/// <summary>
		/// Creates the specified viewmodel.
		/// </summary>
		/// <param name="view">The view.</param>
		/// <returns></returns>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal static TViewModel Create(TView view)
		{
			if (view == null)
				throw new ArgumentNullException("view");

		    var viewModel = new TViewModel {View = view};
            
			viewModel.ActionController = new ActionController<TView, TViewModel>(view, viewModel);
			viewModel.ActionManager = new ActionManager<TView, TViewModel>(view, viewModel);

			viewModel.OnCreated();

		    return viewModel;
		}

		/// <summary>
		/// Saves this instance.
		/// </summary>
		public void Save()
		{
		    var objectToSave = new DynamicContext();
            Save(objectToSave);

            var parameters = new KeyValuePair<string, object>("ObjectToSave", objectToSave);
			ActionController.InvokeAction<SaveStateAction<TView, TViewModel>>(parameters);
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
        public void InvokeAction<T>(params KeyValuePair<string, object>[] arguments)
            where T : BaseAction<TView, TViewModel>, new()
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
        public void InvokeAction<T>(UIElement sender, params KeyValuePair<string, object>[] arguments)
            where T : BaseAction<TView, TViewModel>, new()
        {
            ActionController.InvokeAction<T>(sender, arguments);
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
		/// <see cref="BaseViewModel&lt;TView, TViewModel&gt;"/> is reclaimed by garbage collection.
		/// </summary>
		~BaseViewModel()
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