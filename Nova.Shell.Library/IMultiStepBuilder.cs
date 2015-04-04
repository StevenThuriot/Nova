using System;
using Nova.Controls;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Multi step builder interface.
    /// </summary>
    public interface IMultiStepBuilder
    {
        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="title">The title.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IMultiStepBuilder AddStep<TPageView, TPageViewModel>(string title = null, params ActionContextEntry[] parameters)
            where TPageViewModel : MultistepContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Adds a navigational action which will populate the tree.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="id">The id.</param>
        /// <param name="title">The title.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The module builder instance.
        /// </returns>
        IMultiStepBuilder AddStep<TPageView, TPageViewModel>(Guid id, string title = null, params ActionContextEntry[] parameters)
            where TPageViewModel : MultistepContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();
    }
}
