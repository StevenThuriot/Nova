using System.Collections.Generic;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// The content viewmodel
    /// </summary>
    internal interface IContentViewModel : INavigatablePage, IViewModel
    {
        /// <summary>
        /// Returns to use case.
        /// </summary>
        /// <param name="entries">The entries.</param>
        void ReturnToUseCase(IEnumerable<ActionContextEntry> entries);
    }
}