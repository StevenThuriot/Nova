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

		private void OpenNewWindow(object sender, RoutedEventArgs e)
		{
			var mainWindow = new MainWindow();
			mainWindow.ShowDialog();
		}

        public void OnAfter(ActionContext context)
        {
            MessageBox.Show("General On After V: " + context.ActionName);
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
