using System.Linq;
using System.Windows.Input;
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
            var session = View.CreatePage<SessionView, SessionViewModel>();
            ViewModel.Sessions.Add(session);

            if (!_HasOpenSessions || Keyboard.Modifiers != ModifierKeys.Control)
            {
                ViewModel.CurrentSession = session;
            }
        }
    }
}
