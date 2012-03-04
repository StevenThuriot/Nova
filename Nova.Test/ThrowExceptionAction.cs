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
