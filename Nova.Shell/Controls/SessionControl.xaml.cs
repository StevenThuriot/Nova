using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nova.Shell.Controls
{
    /// <summary>
    /// Interaction logic for SessionControl.xaml
    /// </summary>
    public partial class SessionControl
    {
        /// <summary>
        /// The sessions property
        /// </summary>
        public static readonly DependencyProperty SessionsProperty =
            DependencyProperty.Register("Sessions", typeof (IEnumerable<SessionView>), typeof (SessionControl), new PropertyMetadata(null));

        /// <summary>
        /// The current session property
        /// </summary>
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof (SessionView), typeof (SessionControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        /// <summary>
        /// The add command property
        /// </summary>
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof (ICommand), typeof (SessionControl), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionControl"/> class.
        /// </summary>
        public SessionControl()
        {
            TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            VisualTextRenderingMode = TextRenderingMode.ClearType;

            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the sessions.
        /// </summary>
        /// <value>
        /// The sessions.
        /// </value>
        public IEnumerable<SessionView> Sessions
        {
            get { return (IEnumerable<SessionView>) GetValue(SessionsProperty); }
            set { SetValue(SessionsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current session.
        /// </summary>
        /// <value>
        /// The current session.
        /// </value>
        public SessionView CurrentSession
        {
            get { return (SessionView) GetValue(CurrentSessionProperty); }
            set { SetValue(CurrentSessionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the add command.
        /// </summary>
        /// <value>
        /// The add command.
        /// </value>
        public ICommand AddCommand
        {
            get { return (ICommand) GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }
    }
}
