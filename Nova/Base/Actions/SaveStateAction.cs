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
