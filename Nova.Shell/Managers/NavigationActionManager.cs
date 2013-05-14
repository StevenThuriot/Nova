﻿using Nova.Library;

#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Nova.Controls;
using Nova.Shell.Actions.Session;
using Nova.Shell.Library;

namespace Nova.Shell.Managers
{
    /// <summary>
    /// Manager that creates navigatable actions.
    /// </summary>
    internal class NavigationActionManager : IDisposable, INavigationActionManager
    {
        private bool _Disposed;

        private SessionView _Session;
        private readonly List<IDisposable> _NavigatableActions;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationActionManager" /> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public NavigationActionManager(SessionView session)
        {
            _Session = session;
            _NavigatableActions = new List<IDisposable>();
        }

        /// <summary>
        /// Creates a command that navigates the current session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <returns></returns>
        public ICommand New<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new()
        {
            var viewModel = _Session.ViewModel;

            var createNextView = new Func<IView>(viewModel.CreatePage<TPageView, TPageViewModel>);
            var next = ActionContextEntry.Create(SessionViewModel.NextViewConstant, createNextView, false);

            var command = RoutedAction.New<NavigationAction, SessionView, SessionViewModel>(_Session, viewModel, next);

            _NavigatableActions.Add((IDisposable) command);

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
            if (_Disposed) return;

            if (disposing)
            {
                _Session = null;
                foreach (var disposable in _NavigatableActions)
                {
                    disposable.Dispose();
                }

                _NavigatableActions.Clear();
            }

            _Disposed = true;
        }
    }
}
