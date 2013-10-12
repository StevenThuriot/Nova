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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using Nova.Controls;
using System.Windows.Input;
using Nova.Library;
using Nova.Shell.Library.Actions;
using Nova.Shell.Library.Actions.Wizard;

namespace Nova.Shell.Library
{
    /// <summary>
    /// A viewmodel for pages that belong to a session.
    /// </summary>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    public abstract class ContentViewModel<TView, TViewModel> : ViewModel<TView, TViewModel>, IContentViewModel 
        where TView : class, IView
        where TViewModel : ContentViewModel<TView, TViewModel>, new()
    {
        private readonly IDisposable _deferral;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentViewModel{TView, TViewModel}"/> class.
        /// </summary>
        protected ContentViewModel()
        {
            _deferral = DeferCreated(); //Defer Created logic so we can call it manually in our extended initialize method.
        }

        /// <summary>
        /// Initializes the ContentViewModel using the parent session instance.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        /// <param name="triggerDeferal">if set to <c>true</c> [triggerDeferal].</param>
        internal virtual void Initialize(IDictionary<string, object> initializer, bool triggerDeferal = true)
        {
            Session = (ISessionViewModel) initializer["Session"];

            if (triggerDeferal) TriggerDeferal();
        }

        /// <summary>
        /// Triggers the deferal.
        /// </summary>
        internal void TriggerDeferal()
        {
            _deferral.Dispose();
        }

        /// <summary>
        /// Gets the session view model.
        /// </summary>
        /// <value>
        /// The session view model.
        /// </value>
        internal ISessionViewModel Session { get; private set; }

        /// <summary>
        /// Gets the application model.
        /// </summary>
        /// <value>
        /// The application model.
        /// </value>
        public dynamic ApplicationModel
        {
            get { return Session.ApplicationModel; }
        }

        /// <summary>
        /// Gets the session model.
        /// </summary>
        /// <value>
        /// The session model.
        /// </value>
        public dynamic SessionModel
        {
            get { return Session.Model; }
        }

        /// <summary>
        /// Creates a navigational action that navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(params ActionContextEntry[] parameters)
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new()
        {
            return Session.CreateNavigationalAction<TPageView, TPageViewModel>(parameters);
        }

        /// <summary>
        /// Creates a navigational action that navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        /// <param name="nodeId">The node id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public ICommand CreateNavigationalAction<TPageView, TPageViewModel>(Guid nodeId, params ActionContextEntry[] parameters) 
            where TPageView : ExtendedContentControl<TPageView, TPageViewModel>, new() 
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
        {
            return Session.CreateNavigationalAction<TPageView, TPageViewModel>(nodeId, parameters);
        }


        /// <summary>
        /// Returns to use case.
        /// </summary>
        /// <param name="entries">The entries.</param>
        public virtual void ReturnToUseCase(IEnumerable<ActionContextEntry> entries)
        {
            var actionContextEntries = entries.ToArray();
            InvokeAction<ReturnAction<TView, TViewModel>>(actionContextEntries);
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="image">The image.</param>
        public void ShowDialogBox(string message, ImageSource image = null)
        {
            Session.ShowDialogBox(message, image);
        }

        /// <summary>
        /// Shows the dialog box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="image">The image.</param>
        public T ShowDialogBox<T>(string message, IEnumerable<T> buttons, ImageSource image = null)
        {
            return Session.ShowDialogBox(message, buttons, image);
        }

        /// <summary>
        /// Leaves the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public override Task<bool> Leave(params ActionContextEntry[] parameters)
        {
            return InvokeActionAsync<ContentLeaveAction<TView, TViewModel>>(parameters);
        }
    }
}
