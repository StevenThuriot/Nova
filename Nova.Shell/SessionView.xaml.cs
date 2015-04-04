using System.Windows;
using System.Windows.Input;
using RESX = Nova.Shell.Properties.Resources;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for SessionView.xaml
    /// </summary>
    public partial class SessionView
    {
        /// <summary>
        /// Initializes the <see cref="SessionView" /> class.
        /// </summary>
        static SessionView()
        {
            var propertyMetadata = new FrameworkPropertyMetadata(RESX.EmptySession)
            {
                CoerceValueCallback = OnCoerceTitleCallBack
            };

            TitleProperty.OverrideMetadata(typeof (SessionView), propertyMetadata);
        }

        private static object OnCoerceTitleCallBack(DependencyObject d, object basevalue)
        {
            return string.IsNullOrWhiteSpace(basevalue as string)
                ? RESX.EmptySession
                : basevalue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionView" /> class.
        /// </summary>
        public SessionView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Starts the animated loading.
        /// </summary>
        public override void StartLoading()
        {
            base.StartLoading();
            Cursor = Cursors.AppStarting;
        }

        /// <summary>
        /// Stops the animated loading.
        /// </summary>
        public override void StopLoading()
        {
            base.StopLoading();

            if (!IsLoading)
            {
                Cursor = Cursors.Arrow;
            }
        }
    }
}
