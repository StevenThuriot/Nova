using System;
using System.Windows;

namespace Nova.Test
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public App()
		{
			Startup += NovaFramework.Start;
		}
	}
}
