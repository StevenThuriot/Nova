using System.Linq;
using Nova.Base;

namespace Nova.Shell.Actions.MainWindow
{
    public class CreateNewSessionAction : BaseAction<MainView, MainViewModel>
    {
        private bool _HasOpenSessions;

        public override bool Execute()
        {
            _HasOpenSessions = ViewModel.Sessions.Any();
            return true;
        }

        public override void ExecuteCompleted()
        {
            var session = SessionView.Create(View, View._ActionQueueManager);
            ViewModel.Sessions.Add(session);

            if (!_HasOpenSessions)
            {
                ViewModel.CurrentSession = session;
            }
        }
    }
}
