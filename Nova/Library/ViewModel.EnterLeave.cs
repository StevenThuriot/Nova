using Nova.Library.Actions;

#region License
// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion

using System.Threading.Tasks;

namespace Nova.Library
{
	public abstract partial class ViewModel<TView, TViewModel>
	{
        /// <summary>
        /// The viewmodel's EnterAction, which triggers when entering this view.
        /// If not set, the default EnterAction will be used.
        /// </summary>
        private EnterAction<TView, TViewModel> _EnterAction;

        /// <summary>
        /// The viewmodel's LeaveAction, which triggers when leaving this view.
        /// If not set, the default EnterAction will be used.
        /// </summary>
        private LeaveAction<TView, TViewModel> _LeaveAction;



        /// <summary>
        /// Called to trigger all the Entering logic for this ViewModel.
        /// </summary>
        public Task<bool> Enter()
        {
            if (_EnterAction == null)
                return ActionController.InvokeActionAsync<EnterAction<TView, TViewModel>>();

            var task = ActionController.InternalInvokeActionAsync(_EnterAction, disposeActionDuringCleanup: true);

            _EnterAction = null;

            return task;
        }

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        public Task<bool> Leave()
        {
            return _LeaveAction == null
                       ? ActionController.InvokeActionAsync<LeaveAction<TView, TViewModel>>()
                       : ActionController.InternalInvokeActionAsync(_LeaveAction, disposeActionDuringCleanup: false); //Don't dispose because we might leave several times.
        }

	    /// <summary>
        /// Sets the enter action.
        /// </summary>
        /// <param name="enterAction">The enter action.</param>
        protected void SetEnterAction(EnterAction<TView, TViewModel> enterAction)
        {
            if (_EnterAction != null)
            {
                _EnterAction.Dispose();
            }

            _EnterAction = enterAction;
        }

        /// <summary>
        /// Sets the leave action.
        /// </summary>
        /// <param name="leaveAction">The leave action.</param>
        /// <remarks>When setting a custom leave action, it is possible it will be used several times if leaving a view isn't succesful the first time.</remarks>
        protected void SetLeaveAction(LeaveAction<TView, TViewModel> leaveAction)
        {
            if (_LeaveAction != null)
            {
                _LeaveAction.Dispose();
            }

            _LeaveAction = leaveAction;
        }
	}
}
