using Nova.Controls;

namespace Nova.Base.Actions
{
	internal class LoadStateAction<TView, TViewModel> : BaseAction<TView, TViewModel>
		where TView : class, IView
		where TViewModel : BaseViewModel<TView, TViewModel>, new()
	{
		private DynamicContext _ObjectToLoad;

		public override bool Execute()
		{
			_ObjectToLoad = DynamicContext.Load<TViewModel>();
			return !_ObjectToLoad.IsEmpty;
		}

		public override void ExecuteCompleted()
		{
			ViewModel.Load(_ObjectToLoad);
		}
	}
}
