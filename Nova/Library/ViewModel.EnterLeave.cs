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
