using Nova.Base;

namespace Nova.Shell.Actions.MainWindow
{
    public class CreateNewSessionAction : BaseAction<MainView, MainViewModel>
    {
        public override void ExecuteCompleted()
        {
            var session = SessionView.Create(View, View._ActionQueueManager);
            ViewModel.Sessions.Add(session);
        }
    }
}
