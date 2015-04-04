using System;
using System.Windows.Input;
using Nova.Controls;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// A content page that allows navigation.
    /// </summary>
    internal interface INavigatablePage
    {
        /// <summary>
        /// Creates a navigational action that navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        ICommand CreateNavigationalAction<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Creates a navigational action that navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="nodeId">The node id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        ICommand CreateNavigationalAction<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();
    }
}