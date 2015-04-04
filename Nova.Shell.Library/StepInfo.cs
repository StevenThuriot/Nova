using System;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Step Info
    /// </summary>
    public class StepInfo
    {
        public StepInfo(string title, Type viewType, Type viewModelType, Guid nodeId)
        {
            Title = title;
            ViewType = viewType;
            ViewModelType = viewModelType;
            NodeId = nodeId;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the type of the view.
        /// </summary>
        /// <value>
        /// The type of the view.
        /// </value>
        public Type ViewType { get; private set; }

        /// <summary>
        /// Gets the type of the view model.
        /// </summary>
        /// <value>
        /// The type of the view model.
        /// </value>
        public Type ViewModelType { get; private set; }
        
        /// <summary>
        /// Gets the node ID.
        /// </summary>
        /// <value>
        /// The node ID.
        /// </value>
        public Guid NodeId { get; private set; }
    }
}
