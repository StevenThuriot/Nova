using Nova.Base;
using Nova.Threading;

namespace Nova.Shell.Actions.Session
{
    [Terminating]
    public class SessionLeaveStep : Actionflow<SessionView, SessionViewModel>
    {
        public override bool Execute()
        {
            if (!ViewModel.IsValid)
            {
                //Todo: Handle error state.
            }

            //if (!ViewModel.IsDirty)
            //{
            //    //Todo: Handle dirty state.
            //}

            return base.Execute();
        }

        public override void ExecuteCompleted()
        {
            View.Dispose();
        }
    }
}
