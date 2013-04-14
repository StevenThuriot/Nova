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

using Nova.Controls;

namespace Nova.Shell.Library.Interfaces
{
    /// <summary>
    /// A content page that allows navigation.
    /// </summary>
    public interface INavigatablePage
    {
        /// <summary>
        /// Navigates the parent session to the specified page.
        /// </summary>
        /// <typeparam name="TPageView">The type of the page view.</typeparam>
        /// <typeparam name="TPageViewModel">The type of the page view model.</typeparam>
        void Navigate<TPageView, TPageViewModel>()
            where TPageViewModel : ContentViewModel<TPageView, TPageViewModel>, new()
            where TPageView : ExtendedPage<TPageView, TPageViewModel>, new();
    }
}