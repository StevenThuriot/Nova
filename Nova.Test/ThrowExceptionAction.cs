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
using System;
using System.Threading;
using Nova.Base;
using Nova.Validation;

namespace Nova.Test
{
	public class ThrowExceptionAction : BaseAction<MainWindow, MainViewModel>
	{
		public override void Validate(ValidationResults results)
		{
			var validator = new TestValidator(results, ActionContext);
			
			validator.Validate(ViewModel);
		}

		public override bool Execute()
		{
			Thread.Sleep(1500);
			throw new Exception("Lorem ipsum dolor sit amet, consectetur adipiscing elit. In id libero ut erat ultrices tincidunt. Phasellus vitae leo velit, id gravida lacus. Etiam in lectus vitae neque cursus posuere. Suspendisse non nisl metus. Integer odio lacus, pharetra sed placerat vel, ullamcorper vel velit. Nulla sed risus est, vel scelerisque risus. Mauris tincidunt, libero sit amet adipiscing dignissim, sapien dolor tristique orci, vitae vulputate risus nibh ac magna. Donec vel urna sed tellus auctor lobortis sed quis turpis. Pellentesque cursus posuere mauris quis auctor. Fusce ac urna urna. Nulla gravida erat sed neque pharetra gravida. Nullam pharetra erat id lorem congue fermentum. Aliquam porttitor adipiscing ligula, nec ornare ipsum vestibulum rutrum. Integer id eros et quam posuere lacinia. Donec non sem justo, sit amet auctor nulla. Donec fringilla, risus eu blandit volutpat, sem nibh semper tortor, vel posuere metus quam consequat neque.");
		}

		public override void ExecuteCompleted()
		{
			ViewModel.Save();
			ViewModel.Load();
		}
	}
}
