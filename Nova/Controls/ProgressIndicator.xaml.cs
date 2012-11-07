using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Nova.Controls
{
    public partial class ProgressIndicator
    {
        public  ProgressIndicator()
        {
            InitializeComponent();
            IsVisibleChanged += StartStopAnimationHandler;
        }

        private void StartStopAnimationHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => StartStopAnimation(e)));
        }

        private void StartStopAnimation(DependencyPropertyChangedEventArgs e)
        {
            var storyboard = (Storyboard) Resources["animate"];

            if ((bool) e.NewValue)
                storyboard.Begin();
            else
                storyboard.Stop();
        }
    }
}
