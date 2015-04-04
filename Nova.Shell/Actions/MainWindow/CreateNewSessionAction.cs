using System.Linq;
using System.Windows.Input;
using Nova.Library;

namespace Nova.Shell.Actions.MainWindow
{
    public class CreateNewSessionAction : Actionflow<MainView, MainViewModel>
    {
        private bool _hasOpenSessions;

        public override bool Execute()
        {
            _hasOpenSessions = ViewModel.Sessions.Any();
            return true;
        }

        public override void ExecuteCompleted()
        {
            var session = ViewModel.CreateSession();

            if (session == null) return;

            ViewModel.Sessions.Add(session);
            
            if (!_hasOpenSessions || Keyboard.Modifiers != (ModifierKeys.Control | ModifierKeys.Shift))
            {
                ViewModel.CurrentSession = session;
            }
        }
    }
}
