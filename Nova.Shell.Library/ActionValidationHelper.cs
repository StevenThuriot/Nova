#region License
//   
//  Copyright 2013 Steven Thuriot
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//  
#endregion

using Nova.Controls;
using Nova.Shell.Library.Properties;

namespace Nova.Shell.Library
{
    /// <summary>
    /// Validation helper
    /// </summary>
    internal static class ActionValidationHelper
    {
        /// <summary>
        /// Triggers the validation.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <returns></returns>
        public static bool TriggerValidation<TView, TViewModel>(ContentViewModel<TView, TViewModel> viewModel)
            where TView : class, IView
            where TViewModel : ContentViewModel<TView, TViewModel>, new()
        {
            if (!viewModel.IsChanged)
                return true;


            var result = viewModel.ShowDialogBox(Resources.Changes_Have_Been_Made, new[] { Resources.Save, Resources.Cancel });

            if (result != Resources.Save)
            {
                return false;
            }
            
            if (!viewModel.Save() || viewModel.IsChanged)
            {
                viewModel.ShowDialogBox(Resources.Changes_Could_Not_Be_Saved);
                return false;
            }

            return true;
        }
    }
}
