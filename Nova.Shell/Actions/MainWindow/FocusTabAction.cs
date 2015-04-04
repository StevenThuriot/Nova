using Nova.Library;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action to focus a certain tab of choice.
    /// </summary>
    public class FocusTabAction : Actionflow<MainView, MainViewModel>
    {
        private SessionView _session;

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

            _session = ViewModel.Sessions[sessionIndex];
            return true;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.CurrentSession = _session;
        }
    }
}
