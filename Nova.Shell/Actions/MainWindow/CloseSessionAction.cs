using Nova.Library;

namespace Nova.Shell.Actions.MainWindow
{
    /// <summary>
    /// Action to close a session.
    /// </summary>
    public class CloseSessionAction : Actionflow<MainView, MainViewModel>
    {
        private SessionView _session;

        public override bool Execute()
        {
            var canComplete = ActionContext.TryGetValue(out _session) && _session != null;

            if (!canComplete)
                return false;

            var canLeave = _session.ViewModel.Leave().Result;

            return canLeave;
        }

        public override void ExecuteCompleted()
        {
            ViewModel.Sessions.Remove(_session);
        }
    }
}
