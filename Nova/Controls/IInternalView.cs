using Nova.Threading;

namespace Nova.Controls
{
    /// <summary>
    /// The interface for a view used internally in this framework.
    /// </summary>
    internal interface IInternalView : IView
    {
        IActionQueueManager ActionQueueManager { get; }
    }
}