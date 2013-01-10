using System;
using System.Windows;

namespace Nova.Shell
{
    /// <summary>
    /// Interaction logic for DropShadowWindow.xaml
    /// </summary>
    public partial class DropShadowWindow
    {
        private readonly Window _Parent;

        public DropShadowWindow(Window parent)
        {
            _Parent = parent;
            DataContext = parent;
                        
            InitializeComponent();

            parent.LocationChanged += ParentOnLocationChanged;
            parent.SizeChanged += ParentOnSizeChanged;
            parent.StateChanged += ParentOnStateChanged;
            parent.Loaded += ParentOnLoaded;
            parent.Closed += ParentOnClosed;
        }

        private void ParentOnClosed(object sender, EventArgs eventArgs)
        {
            Close();
        }

        private void ParentOnStateChanged(object sender, EventArgs eventArgs)
        {
            switch (_Parent.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Minimized:
                    Visibility = Visibility.Visible;
                    break;
                case WindowState.Maximized:
                    Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void ParentOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Show();
            _Parent.Owner = this;
        }

        private void ParentOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            Width = _Parent.Width + 10;
            Height = _Parent.Height + 10;
        }

        private void ParentOnLocationChanged(object sender, EventArgs eventArgs)
        {
            Left = _Parent.Left - 5;
            Top = _Parent.Top - 5;
        }
    }
}