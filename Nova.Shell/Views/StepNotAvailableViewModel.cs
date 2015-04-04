using Nova.Library;

namespace Nova.Shell.Views
{
    public class StepNotAvailableViewModel : ViewModel<StepNotAvailableView, StepNotAvailableViewModel>
    {
        private string _stepName;

        /// <summary>
        /// Gets or sets the name of the step.
        /// </summary>
        /// <value>
        /// The name of the step.
        /// </value>
        public string StepName
        {
            get { return _stepName; }
            set { SetValue(ref _stepName, value); }
        }
    }
}