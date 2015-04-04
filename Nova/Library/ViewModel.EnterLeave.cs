using System.Threading.Tasks;
using Nova.Library.Actions;

namespace Nova.Library
{
	public abstract partial class ViewModel<TView, TViewModel>
	{
        /// <summary>
        /// Called to trigger all the Entering logic for this ViewModel.
        /// </summary>
        public virtual Task<bool> Enter(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<EnterAction<TView, TViewModel>>(parameters);
        }

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        public virtual Task<bool> Leave(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<LeaveAction<TView, TViewModel>>(parameters);
        }
	}
}
