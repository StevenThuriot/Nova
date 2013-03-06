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
using Nova.Base.Actions;

namespace Nova.Base
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
        public async Task<bool> Enter()
        {
            return _EnterAction == null
                       ? await ActionController.InvokeActionAsync<EnterAction<TView, TViewModel>>()
                       : await ActionController.InternalInvokeActionAsync(_EnterAction);
        }

        /// <summary>
        /// Called to trigger all the Leaving logic for this ViewModel.
        /// </summary>
        public async Task<bool> Leave()
        {
            return _LeaveAction == null
                       ? await ActionController.InvokeActionAsync<LeaveAction<TView, TViewModel>>()
                       : await ActionController.InternalInvokeActionAsync(_LeaveAction);
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
