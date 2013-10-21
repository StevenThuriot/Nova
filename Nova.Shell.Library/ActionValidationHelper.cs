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

using System.Linq;
using Nova.Controls;

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

            const string saveConstant = "Save"; //TODO: Replace with resource

            var result = viewModel.ShowDialogBox("Changes have been made", new[] { saveConstant, "Cancel" });

            if (result == saveConstant)
            {
                if (!viewModel.Save())
                {
                    viewModel.ShowDialogBox("Changes could not be saved succesfully...");
                    return false;
                }

                if (viewModel.IsChanged)
                {
                    viewModel.ShowDialogBox("Changes could not be saved succesfully...");
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
