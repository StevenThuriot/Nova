using System;
using System.Windows;
using Nova.Base;

namespace Nova.Test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void CloseWindow(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ToggleMaximize(object sender, RoutedEventArgs e)
		{
			CanMaximize = !CanMaximize;
		}

		private void OpenNewWindow(object sender, RoutedEventArgs e)
		{
			var mainWindow = new MainWindow();
			mainWindow.ShowDialog();
		}
		
		private void OpenExceptionWindow(object sender, RoutedEventArgs e)
		{
			throw new Exception("This is a test", new Exception("This is an inner exception", new Exception("This is a second inner exception.")));
		}
		/*
		public void OnBeforeThrowExceptionAction(ActionContext context)
		{
			MessageBox.Show("On before Action V");
		}
		public void OnBeforeThrowException(ActionContext context)
		{
			MessageBox.Show("On before V");
		}

		public void OnAfterThrowExceptionAction(ActionContext context)
		{
			MessageBox.Show("On After Action V");
		}

		public void OnAfterThrowException(ActionContext context)
		{
			MessageBox.Show("On After V");
		}*/
	}
}
