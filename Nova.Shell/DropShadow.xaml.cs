using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for DropShadow.xaml
    /// </summary>
    public partial class DropShadow
    {
        private bool _delay;

        /// <summary>
        /// Prevents a default instance of the <see cref="DropShadow" /> class from being created.
        /// </summary>
        /// <param name="window">The window.</param>
        private DropShadow(Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            DataContext = window;
                        
            InitializeComponent();

            window.Loaded += OnLoaded;
            Left = window.Left + 1;
            Top = window.Top + 1;
        }

        /// <summary>
        /// Drops a shadow behind the passed window.
        /// </summary>
        /// <param name="window">The window.</param>
        public static void ForWindow(Window window)
        {
            var tier = RenderCapability.Tier >> 16;

            //Switch {
                // hardware rendering == 2
                // partially hardware rendering == 1
                // software rendering == default;
            //}

            if (tier != 2) return;

            new DropShadow(window);
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Close();
        }


        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            SetVisibilityDependingOn(sender);
        }

        private void OnStateChanged(object sender, EventArgs eventArgs)
        {
            SetVisibilityDependingOn(sender);
        }

        private async void SetVisibilityDependingOn(object sender)
        {
            var window = sender as Window;

            if (window == null) return;


            if (window.IsVisible && window.WindowState == WindowState.Normal)
            {
                if (_delay)
                {
                    await Task.Delay(380); //400 == Default Windows animation speed.
                }

                Visibility = Visibility.Visible;
                OnLocationChanged(sender);
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }

            _delay = window.WindowState == WindowState.Minimized;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var window = sender as Window;

            if (window == null) return;

            window.Loaded -= OnLoaded;
            window.Closed += OnClosed;

            Show();
            window.Owner = this;

            await Task.Delay(250);
            Initialize(window);
        }

        private void Initialize(Window window)
        {
            Visibility = Visibility.Visible;

            Left = window.Left - 5;
            Top = window.Top - 5;

            Width = window.Width + 10;
            Height = window.Height + 10;

            window.LocationChanged += OnLocationChanged;
            window.SizeChanged += OnSizeChanged;
            window.StateChanged += OnStateChanged;
            window.IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (!IsVisible) return;

            var window = sender as Window;

            if (window == null) return;

            Width = window.Width + 10;
            Height = window.Height + 10;
        }

        private void OnLocationChanged(object sender, EventArgs eventArgs = null)
        {
            if (!IsVisible) return;

            var window = sender as Window;

            if (window == null) return;

            Left = window.Left - 5;
            Top = window.Top - 5;
        }
    }
}