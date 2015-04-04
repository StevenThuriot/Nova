using System;
using Nova.Library;
using Nova.Validation;

namespace Nova.Controls
{
    /// <summary>
    /// The interface for a view used in this framework.
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Starts the loading.
        /// </summary>
        void StartLoading();

        /// <summary>
        /// Stops the loading.
        /// </summary>
        void StopLoading();

        /// <summary>
        /// Gets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        IViewModel ViewModel { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        bool FocusControl(string fieldName);

        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="entityId">The entity ID.</param>
        /// <returns></returns>
        bool FocusControl(string fieldName, Guid entityId);

        /// <summary>
        /// Gets or sets the validation control.
        /// </summary>
        /// <value>
        /// The validation control.
        /// </value>
        ValidationControl ValidationControl { get; set; }
    }
}