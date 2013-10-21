namespace Nova.Library
{
    public abstract partial class ViewModel<TView, TViewModel>
    {
        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            if (!SaveChanges()) 
                return false;

            AcceptChanges();
            return true;
        }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveChanges()
        {
            return true;
        }
    }
}
