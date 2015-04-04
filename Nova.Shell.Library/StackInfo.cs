using System;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Stack Info
    /// </summary>
    internal class StackInfo
    {
        /// <summary>
        /// Gets the stack id.
        /// </summary>
        /// <value>
        /// The stack id.
        /// </value>
        public Guid StackId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackInfo"/> class.
        /// </summary>
        /// <param name="stackId">The stack id.</param>
        public StackInfo(Guid stackId)
        {
            StackId = stackId;
        }
    }
}