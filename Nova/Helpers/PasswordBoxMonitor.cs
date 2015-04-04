using System.Windows;
using System.Windows.Controls;

namespace Nova.Helpers
{
    /// <summary>
    /// A helper class to see if a passwordbox is filled in or not without actually putting the value as a string in memory.
    /// </summary>
    public class PasswordBoxMonitor : DependencyObject
    {
        /// <summary>
        /// The monitoring property
        /// </summary>
        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof (bool), typeof (PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));

        /// <summary>
        /// The password length property
        /// </summary>
        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof (int), typeof (PasswordBoxMonitor), new UIPropertyMetadata(0));

        /// <summary>
        /// Gets wether the passed object is being monitored.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        public static bool GetIsMonitoring(DependencyObject dependencyObject)
        {
            return (bool) dependencyObject.GetValue(IsMonitoringProperty);
        }

        /// <summary>
        /// Sets wether the passed object is being monitored.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsMonitoring(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsMonitoringProperty, value);
        }

        /// <summary>
        /// Determines whether the specified object is filled in.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>
        ///   <c>true</c> if the specified object is filled in; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFilledIn(DependencyObject dependencyObject)
        {
            return GetPasswordLength(dependencyObject) > 0;
        }

        /// <summary>
        /// Gets the length of the password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        public static int GetPasswordLength(DependencyObject dependencyObject)
        {
            return (int) dependencyObject.GetValue(PasswordLengthProperty);
        }

        /// <summary>
        /// Sets the length of the password.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetPasswordLength(DependencyObject dependencyObject, int value)
        {
            dependencyObject.SetValue(PasswordLengthProperty, value);
        }

        /// <summary>
        /// Called when is monitoring changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnIsMonitoringChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var pb = dependencyObject as PasswordBox;
            if (pb == null) return;

            if ((bool) e.NewValue)
            {
                pb.PasswordChanged += OnPasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= OnPasswordChanged;
            }
        }

        /// <summary>
        /// Called when the password changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private static void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            SetPasswordLength(passwordBox, passwordBox.SecurePassword.Length);
        }
    }
}