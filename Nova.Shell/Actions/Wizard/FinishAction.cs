namespace Nova.Shell.Actions.Wizard
{
    /// <summary>
    /// Finish action
    /// </summary>
    internal class FinishAction : ReturnAction
    {
        /// <summary>
        /// Gets a value indicating whether this instance is cancelled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is cancelled; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsCancelled
        {
            get { return false; }
        }
    }
}