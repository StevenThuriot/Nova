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

namespace Nova.Base.Actions
{
	internal class SaveStateAction<TView, TViewModel> : BaseAction<TView, TViewModel>
		where TView : class, IView
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
	{
		private DynamicContext _ObjectToSave;

		public override bool CanExecute()
		{
		    _ObjectToSave = ActionContext.GetValue<DynamicContext>("ObjectToSave");
			return !_ObjectToSave.IsEmpty;
		}

		public override bool Execute()
		{
			DynamicContext.Save<TViewModel>(_ObjectToSave);
			return base.Execute();
		}
	}
}
