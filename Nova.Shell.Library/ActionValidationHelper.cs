using Nova.Controls;
using Nova.Shell.Library.Properties;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Validation helper
    /// </summary>
    internal static class ActionValidationHelper
    {
        /// <summary>
        /// Triggers the validation.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        public static bool TriggerValidation<TView, TViewModel>(ContentViewModel<TView, TViewModel> viewModel)
            where TView : class, IView
            where TViewModel : ContentViewModel<TView, TViewModel>, new()
        {
            if (!viewModel.IsChanged)
                return true;


            var result = viewModel.ShowDialogBox(Resources.Changes_Have_Been_Made, new[] { Resources.Save, Resources.Cancel });

            if (result != Resources.Save)
            {
                return false;
            }
            
            if (!viewModel.Save() || viewModel.IsChanged)
            {
                viewModel.ShowDialogBox(Resources.Changes_Could_Not_Be_Saved);
                return false;
            }

            return true;
        }
    }
}
