using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Nova.Controls;
using Nova.Validation;

namespace Nova.Library
{
    /// <summary>
    /// The ViewModel interface.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged, IChangeTracking, IDisposable
    {
        /// <summary>
        /// Gets the view.
        /// </summary>
        IView View { get; }

        /// <summary>
        /// Gets or sets the ViewModel ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        /// <exception cref="System.ArgumentNullException">value</exception>
        Guid ID { get; }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        dynamic ActionManager { get; }

        /// <summary>
        /// Gets the error collection.
        /// </summary>
        ReadOnlyErrorCollection ErrorCollection { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        bool IsValid { get; }

        /// <summary>
        /// Called to trigger all the Entering logic for this ViewModel.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        Task<bool> Enter(params ActionContextEntry[] parameters);

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// True if leaving was successful.
        /// </returns>
        Task<bool> Leave(params ActionContextEntry[] parameters);

        /// <summary>
        /// Saves the state of this instance.
        /// </summary>
        void SaveState();

        /// <summary>
        /// Loads the state of this instance.
        /// Load(value) will only trigger in case the ViewModel has been saved before.
        /// </summary>
        void LoadState();

        /// <summary>
        /// Prepares the change tracking.
        /// </summary>
        void PrepareChangeTracking();

        /// <summary>
        /// Initializes the change tracking.
        /// </summary>
        void InitializeChangeTracking();

        /// <summary>
        /// Stops the change tracking.
        /// </summary>
        void StopChangeTracking();

        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        bool Save();
    }
}