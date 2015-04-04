using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nova.Threading;

namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private ActionManager<TView, TViewModel> _actionManager;
        private IActionQueueManager _actionQueueManager;

        /// <summary>
        /// Gets the action controller.
        /// </summary>
        internal ActionController<TView, TViewModel> ActionController { get; private set; }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        public dynamic ActionManager
        {
            get { return _actionManager; }
        }

        /// <summary>
        /// Sets the known action types.
        /// The action manager will choose from these types to initiate an action.
        /// </summary>
        /// <param name="knownTypes">The known types.</param>
        protected void SetKnownActionTypes(params Type[] knownTypes)
        {
            _actionManager.SetKnownTypes(knownTypes);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="entries">The entries.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void InvokeAction<T>(params ActionContextEntry[] entries)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionController.InvokeAction<T>(entries);
        }

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="entries">The entries.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public Task<bool> InvokeActionAsync<T>(params ActionContextEntry[] entries)
            where T : Actionflow<TView, TViewModel>, new()
        {
            return ActionController.InvokeActionAsync<T>(entries);
        }
    }
}
