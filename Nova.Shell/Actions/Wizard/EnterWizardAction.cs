using Nova.Library;
using Nova.Library.Actions;
using Nova.Shell.Views;

namespace Nova.Shell.Actions.Wizard
{
    /// <summary>
    /// Enter action for wizards.
    /// </summary>
    public class EnterWizardAction : EnterAction<WizardView, WizardViewModel>
    {
        private IViewModel _viewModel;
        private ActionContextEntry[] _parameters;

        public override void OnBefore()
        {
            _parameters = ViewModel.InitialView.Value.Parameters;
            _viewModel = ViewModel.MultiStepView.ViewModel;
        }

        public override bool Enter()
        {
            return _viewModel.Enter(_parameters).Result;
        }
    }
}
