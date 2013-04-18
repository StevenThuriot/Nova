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

using System;
using System.Diagnostics.CodeAnalysis;
using Nova.Threading;

namespace Nova.Base
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private ActionManager<TView, TViewModel> _ActionManager;
        private IActionQueueManager _ActionQueueManager;

        /// <summary>
        /// Gets the action controller.
        /// </summary>
        internal ActionController<TView, TViewModel> ActionController { get; private set; }

        /// <summary>
        /// Gets the action manager.
        /// </summary>
        public dynamic ActionManager
        {
            get { return _ActionManager; }
        }

        /// <summary>
        /// Sets the known action types.
        /// The action manager will choose from these types to initiate an action.
        /// </summary>
        /// <param name="knownTypes">The known types.</param>
        protected void SetKnownActionTypes(params Type[] knownTypes)
        {
            _ActionManager.SetKnownTypes(knownTypes);
        }
        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="T">The type of action to invoke.</typeparam>
        /// <param name="arguments">The arguments.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void InvokeAction<T>(params ActionContextEntry[] arguments)
            where T : Actionflow<TView, TViewModel>, new()
        {
            ActionController.InvokeAction<T>(arguments);
        }
    }
}
