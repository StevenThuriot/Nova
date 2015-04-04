namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void SaveState()
        {
            var objectToSave = new DynamicContext();
            SaveState(objectToSave);

            if (!objectToSave.IsEmpty)
            {
                DynamicContext.Save<TViewModel>(objectToSave);
            }
        }

        /// <summary>
        /// Loads this instance.
        /// LoadState(value) will only trigger in case the ViewModel has been saved before.
        /// </summary>
        public async void LoadState()
        {
            var dynamicContext = await DynamicContext.Load<TViewModel>();

            if (dynamicContext.IsEmpty) return;

            LoadState(dynamicContext);
        }

        /// <summary>
        /// You can use this method to specify what exactly you want to save.
        /// Add everything to the object to prepare it for serialization. 
        /// All the parameters added should be serializable.
        /// </summary>
        /// <param name="value">The object to save.</param>
        protected virtual void SaveState(dynamic value) { }

        /// <summary>
        /// You can use this method to specify what exactly you want to load.
        /// Retreive everything to the object and load it back onto your ViewModel.
        /// </summary>
        /// <param name="value">The object to load.</param>
        protected virtual void LoadState(dynamic value) { }
    }
}
