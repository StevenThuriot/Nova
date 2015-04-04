using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Nova.Library;

namespace Nova.Test
{
	public class MainViewModel : ViewModel<MainWindow, MainViewModel>
    {
		protected override void OnCreated()
		{
			SearchCommand = new RelayCommand(x => MessageBox.Show((string)x), x => !string.IsNullOrEmpty((string)x));

			SetKnownActionTypes(typeof(ThrowExceptionAction));

			Changes = new ObservableCollection<Change>();
			for (int i = 0; i < 10; i++)
			{
			    var changeType = i.ToString(CultureInfo.InvariantCulture);

			    var change = new Change
				             	{
				             		ChangeType = changeType,
				             		ItemType = changeType,
				             		Path = changeType
				             	};

				Changes.Add(change);
			}
		}

		private string _searchText;
		public string SearchText
		{
			get { return _searchText; }
			set { SetValue(ref _searchText, value); }
		}



		private ObservableCollection<Change> _changes;
		public ObservableCollection<Change> Changes
		{
			get { return _changes; }
			set { SetValue(ref _changes, value); }
		}


		private ICommand _searchCommand;
		public ICommand SearchCommand
		{
			get { return _searchCommand; }
			set { SetValue(ref _searchCommand, value); }
		}
		/*
		public void OnBeforeThrowExceptionAction(ActionContext context)
		{
			MessageBox.Show("On before Action VM");
		}
		public void OnBeforeThrowException(ActionContext context)
		{
			MessageBox.Show("On before VM");
		}

		public void OnAfterThrowExceptionAction(ActionContext context)
		{
			MessageBox.Show("On After Action VM");
		}
		*/

        //public void OnAfterThrowException(ActionContext context)
        //{
        //    MessageBox.Show("On After VM");
        //}

        //public void OnAfter(ActionContext context)
        //{
        //    MessageBox.Show("General On After VM: " + context.ActionName);
        //}

		protected override void SaveState(dynamic value)
		{
			value.Test = "test";
		}

		protected override void LoadState(dynamic value)
		{
			if (string.IsNullOrWhiteSpace(value.Test))
				throw new ArgumentNullException("value.Test");
		}
	}
}
