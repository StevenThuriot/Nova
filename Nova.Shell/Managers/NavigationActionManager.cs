﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Nova.Controls;
using Nova.Library;
using Nova.Shell.Actions.Session;
using Nova.Shell.Library;

namespace Nova.Shell.Managers
{
    /// <summary>
    /// Manager that creates navigatable actions.
    /// </summary>
    internal class NavigationActionManager : IDisposable, INavigationActionManager
    {
        private bool _disposed;

        private SessionView _session;
        private readonly List<IDisposable> _navigatableActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationActionManager" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public NavigationActionManager(SessionView session)
        {
            _session = session;
            _navigatableActions = new List<IDisposable>();
        }

        /// <summary>
        /// News the specified id.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand New<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
        {
            return New<TPageView, TPageViewModel>(Guid.Empty, parameters);
        }

        /// <summary>
        /// Creates a command that navigates the current session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public ICommand New<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
        {
            var viewModel = _session.ViewModel;
            var createNextView = new Func<IView>(viewModel.Create<TPageView, TPageViewModel>);

            var next = ActionContextEntry.Create(ActionContextConstants.CreateNextViewConstant, createNextView, false);
            var viewtype = ActionContextEntry.Create(ActionContextConstants.ViewTypeConstant, typeof(TPageView), false);
            var viewModeltype = ActionContextEntry.Create(ActionContextConstants.ViewModelTypeConstant, typeof(TPageViewModel), false);


            var actionContextEntries = new List<ActionContextEntry>(parameters)
            {
                next,
                viewtype,
                viewModeltype
            };

            if (nodeId != Guid.Empty)
            {
                var id = ActionContextEntry.Create(ActionContextConstants.NodeId, nodeId, false);
                actionContextEntries.Add(id);
            }

            var command = RoutedAction.New<NavigationAction, SessionView, SessionViewModel>(_session, viewModel, actionContextEntries.ToArray());

            _navigatableActions.Add((IDisposable) command);

            return command;
        }


        /// <summary>
        /// Finalizes an instance of the <see cref="NavigationActionManager" /> class.
        /// </summary>
        ~NavigationActionManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _session = null;
                foreach (var disposable in _navigatableActions)
                {
                    disposable.Dispose();
                }

                _navigatableActions.Clear();
            }

            _disposed = true;
        }
    }
}
