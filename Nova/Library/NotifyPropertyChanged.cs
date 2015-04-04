using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nova.Library
{
    /// <summary>
    /// Base class for easily implementing the PropertyChanged event.
    /// </summary>
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// Notifies clients that propertyName has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies clients that propertyName has changed.
        /// </summary>
        /// <param name="propertyName">The property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}