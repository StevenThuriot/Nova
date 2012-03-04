using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Nova.Controls
{
	/// <summary>
	/// Class making it easy to use image buttons.
	/// </summary>
	public class ImageButton : Button
	{
		/// <summary>
		/// An icon property.
		/// </summary>
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof (ImageSource),
		                                                                                     typeof (ImageButton));

		/// <summary>
		/// A text property.
		/// </summary>
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Content", typeof (string),
		                                                                                     typeof (ImageButton));

		private readonly Image _Icon;
		private readonly TextBlock _TextBlock;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageButton"/> class.
		/// </summary>
		public ImageButton()
		{
            SetResourceReference(HeightProperty, "DefaultControlHeight");

			var panel = new StackPanel {Orientation = Orientation.Horizontal};

			_Icon = new Image {Margin = new Thickness(0, 0, 6, 0)};
			var imageBinding = new Binding("Icon")
			                   	{
			                   		Source = this,
			                   		UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			                   	};
			_Icon.SetBinding(Image.SourceProperty, imageBinding);

			panel.Children.Add(_Icon);

			_TextBlock = new TextBlock();
			var textBinding = new Binding("Content")
			                  	{
			                  		Source = this,
			                  		UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			                  	};
			_TextBlock.SetBinding(TextBlock.TextProperty, textBinding);
			panel.Children.Add(_TextBlock);

			base.Content = panel;

			SetResourceReference(StyleProperty, typeof (Button));
		}

		/// <summary>
		/// Gets or sets the icon.
		/// </summary>
		/// <value>
		/// The icon.
		/// </value>
		public ImageSource Icon
		{
			get { return (ImageSource) GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		/// <summary>
		/// Gets or sets the content of a <see cref="T:System.Windows.Controls.ContentControl"/>.
		/// </summary>
		/// <returns>An object that contains the control's content. The default value is null.</returns>
		public new string Content
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}