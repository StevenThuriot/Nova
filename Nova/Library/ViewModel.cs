using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Nova.Controls;
using Nova.Library.ChangeTracking;

namespace Nova.Library
{
    /// <summary>
    /// The base class for any ViewModel.
    /// </summary>
    /// <typeparam name="TView">The type of the linked View.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract partial class ViewModel<TView, TViewModel> : ChangeTrackingBase, IViewModel
        where TView : class, IView
        where TViewModel : ViewModel<TView, TViewModel>, new()
    {
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

        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentException">Guid.Empty is not allowed.</exception>
        public Guid ID
        {
            get { return _id; }
            protected set
            {
                if (value == Guid.Empty)
                    throw new ArgumentException(@"Guid.Empty is not allowed.", "value");
                
                _id = value;
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