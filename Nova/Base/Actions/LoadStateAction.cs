﻿#region License
// 
//  Copyright 2012 Steven Thuriot
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
using Nova.Controls;

namespace Nova.Base.Actions
{
    /// <summary>
    /// An action used to load the state of the View Model.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
	internal class LoadStateAction<TView, TViewModel> : BaseAction<TView, TViewModel>
		where TView : class, IView
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
	{
		private DynamicContext _ObjectToLoad;

        /// <summary>
        /// Executes async.
        /// </summary>
        /// <returns>A value indicating wether to continue execution.</returns>
		public override bool Execute()
		{
			_ObjectToLoad = DynamicContext.Load<TViewModel>();
			return !_ObjectToLoad.IsEmpty;
		}

        /// <summary>
        /// Executes when the async execution succesfully completed.
        /// </summary>
		public override void ExecuteCompleted()
		{
			ViewModel.Load(_ObjectToLoad);
		}
	}
}
