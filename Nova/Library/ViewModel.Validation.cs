using Nova.Validation;

namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        private bool _isValid = true;
        private ReadOnlyErrorCollection _errorCollection;

        /// <summary>
        /// Gets the error collection.
        /// </summary>
        public ReadOnlyErrorCollection ErrorCollection
        {
            get { return _errorCollection; }
            internal set
            {
                if (SetValue(ref _errorCollection, value))
                {
                    IsValid = _errorCollection == null || _errorCollection.Count == 0;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get { return _isValid; }
            private set { SetValue(ref _isValid, value); }
        }
    }
}
