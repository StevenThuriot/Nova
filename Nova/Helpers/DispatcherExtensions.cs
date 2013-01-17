using System;
using System.Windows.Threading;

namespace Nova.Helpers
{
    /// <summary>
    /// Extension methods for the WPF Dispatcher.
    /// </summary>
    public static class DispatcherExtensions
    {
        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="T:System.Windows.Threading.Dispatcher"/> is associated with, after the given time span.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="method">The method.</param>
        /// <param name="argument">The argument.</param>
        public static void DelayInvoke(this Dispatcher dispatcher, TimeSpan timeSpan, Delegate method, object argument = null)
        {
            dispatcher.DelayInvoke(timeSpan, DispatcherPriority.Normal, method, argument);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the <see cref="T:System.Windows.Threading.Dispatcher"/> is associated with, after the given time span.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="method">The method.</param>
        /// <param name="argument">The argument.</param>
        /// <exception cref="System.ArgumentNullException">method</exception>
        public static void DelayInvoke(this Dispatcher dispatcher, TimeSpan timeSpan, DispatcherPriority priority, Delegate method, object argument = null)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            var timer = new DispatcherTimer(priority) { Interval = timeSpan };
            timer.Tick += (sender, args) =>
            {
                if (argument == null)
                    dispatcher.BeginInvoke(priority, method);
                else
                    dispatcher.BeginInvoke(priority, method, argument);

                ((DispatcherTimer)sender).Stop();
            };

            timer.Start();
        }
    }
}
