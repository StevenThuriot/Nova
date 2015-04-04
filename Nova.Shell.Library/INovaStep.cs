using System;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Step
    /// </summary>
    public interface INovaStep
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        Guid Group { get; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>
        /// The type of the view.
        /// </value>
        Type ViewType { get; }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        Type ViewModelType { get; }
    }
}
