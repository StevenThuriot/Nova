using System;
using System.Windows.Input;
using Nova.Controls;
using Nova.Library;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Manager that creates navigatable actions.
    /// </summary>
    public interface INavigationActionManager
    {
        /// <summary>
        /// Creates a command that navigates the current session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        ICommand New<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();

        /// <summary>
        /// Creates a command that navigates the current session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        ICommand New<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new();
    }
}