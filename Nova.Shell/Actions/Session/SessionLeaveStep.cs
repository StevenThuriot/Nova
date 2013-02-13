using Nova.Base.Actions;
using Nova.Threading;

namespace Nova.Shell.Actions.Session
{
    [Terminating]
    public class SessionLeaveStep : LeaveAction<SessionView, SessionViewModel>
    {
        public override bool Leave()
        {
            if (!ViewModel.IsValid)
            {
                //Todo: Handle error state.
            }

            //if (!ViewModel.IsDirty)
            //{
            //    //Todo: Handle dirty state.
            //}

            return base.Leave();
        }
    }
}
