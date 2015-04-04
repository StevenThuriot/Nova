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

		private readonly Image _icon;
		private readonly TextBlock _textBlock;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageButton"/> class.
		/// </summary>
		public ImageButton()
		{
            SetResourceReference(HeightProperty, "DefaultControlHeight");

			var panel = new StackPanel {Orientation = Orientation.Horizontal};

			_icon = new Image {Margin = new Thickness(0, 0, 6, 0)};
			var imageBinding = new Binding("Icon")
			                   	{
			                   		Source = this,
			                   		UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			                   	};
			_icon.SetBinding(Image.SourceProperty, imageBinding);

			panel.Children.Add(_icon);

			_textBlock = new TextBlock();
			var textBinding = new Binding("Content")
			                  	{
			                  		Source = this,
			                  		UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			                  	};
			_textBlock.SetBinding(TextBlock.TextProperty, textBinding);
			panel.Children.Add(_textBlock);

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