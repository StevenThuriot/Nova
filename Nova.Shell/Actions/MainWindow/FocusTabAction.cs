using Nova.Base;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action to focus a certain tab of choice.
    /// </summary>
    public class FocusTabAction : BaseAction<MainView, MainViewModel>
    {
        private SessionView _Session;

        public override bool Execute()
        {
            var index = ActionContext.GetValue<string>(RoutedAction.CommandParameter);

            int sessionIndex;

            if (string.IsNullOrWhiteSpace(index) || 
                !int.TryParse(index, out sessionIndex) ||
                ViewModel.Sessions.Count <= sessionIndex)
            {
                return false;
            }

            _Session = ViewModel.Sessions[sessionIndex];
            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentSession = _Session;
        }
    }
}
