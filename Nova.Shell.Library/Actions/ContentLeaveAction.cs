using Nova.Controls;
using Nova.Library.Actions;

namespace Nova.Shell.Library.Actions
{
    /// <summary>
    /// Leave action for the content view model
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public sealed class ContentLeaveAction<TView, TViewModel> : LeaveAction<TView, TViewModel>
        where TView : class, IView
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        public override bool Leave()
        {
            return ActionValidationHelper.TriggerValidation(ViewModel);
        }
    }
}
