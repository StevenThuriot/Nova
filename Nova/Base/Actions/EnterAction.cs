#region License

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
using Nova.Threading;

namespace Nova.Base.Actions
{
    /// <summary>
    /// Action that triggers when a certain step is entered.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    [Creational]
    internal class EnterAction<TView, TViewModel> : Actionflow<TView, TViewModel>
		where TView : class, IView
		where TViewModel : ViewModel<TView, TViewModel>, new()
	{
        public override void ExecuteCompleted()
        {
            ViewModel.OnCreated();
        }
	}
}
