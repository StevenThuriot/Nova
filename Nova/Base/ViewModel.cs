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
using System.Runtime.CompilerServices;
using Nova.Controls;

namespace Nova.Base
{
    /// <summary>
    /// The base class for any ViewModel.
    /// </summary>
    /// <typeparam name="TView">The type of the linked View.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
	public abstract partial class ViewModel<TView, TViewModel> : IViewModel
        where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
        /// <summary>
        /// Notifies clients that propertyName has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
		/// Gets the view.
		/// </summary>
        public TView View { get; private set; }

        /// <summary>
        /// Gets the view.
        /// </summary>
        IView IViewModel.View
        {
            get { return View; }
        }

        private Guid _ID = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentException">Guid.Empty is not allowed.</exception>
        public Guid ID
        {
            get { return _ID; }
            protected set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException("Guid.Empty is not allowed.", "value");
                
                _ID = value;
            }
        }
		
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
        /// <param name="comparer">The comparer.</param>
        /// <param name="propertyName">The property that changed.</param>
        /// <returns>
        /// True if the property changed.
        /// </returns>
        protected bool SetValue<TMember>(ref TMember privateMember, TMember value, EqualityComparer<TMember> comparer = null, [CallerMemberName] string propertyName = null)
		{
            if (comparer == null)
            {
                comparer = EqualityComparer<TMember>.Default;
            }

		    if (comparer.Equals(privateMember, value))
		    {
		        return false;
		    }

		    privateMember = value;

// ReSharper disable ExplicitCallerInfoArgument
		    OnPropertyChanged(propertyName);
// ReSharper restore ExplicitCallerInfoArgument

		    return true;
		}
    }
}