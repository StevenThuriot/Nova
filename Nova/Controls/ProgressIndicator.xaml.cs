using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Nova.Controls
{
    /// <summary>
    /// A progress indicator.
    /// </summary>
    public partial class ProgressIndicator
    {
        private readonly Action<FrameworkElement, DependencyPropertyChangedEventArgs> _startStopAnimationAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressIndicator" /> class.
        /// </summary>
        public  ProgressIndicator()
        {
            InitializeComponent();
            _startStopAnimationAction = StartStopAnimation;
            IsVisibleChanged += StartStopAnimationHandler;
        }

        /// <summary>
        /// Handler to start or stop the animation depending on the current state.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void StartStopAnimationHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(_startStopAnimationAction, sender, e);
        }

        /// <summary>
        /// Starts or stops the animation depending on the current state.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void StartStopAnimation(FrameworkElement element, DependencyPropertyChangedEventArgs e)
        {
            var storyboard = (Storyboard) element.Resources["animate"];

            if ((bool) e.NewValue)
                storyboard.Begin();
            else
                storyboard.Stop();
        }
    }
}
