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
using System.Windows;
using System.Windows.Media;
using Nova.Validation;

namespace Nova.Helpers
{
    /// <summary>
    /// Helper class to easily set focus.
    /// </summary>
    public static class FocusHelper
    {
        /// <summary>
        /// Focuses the control.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="field">The field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public static bool FocusControl(DependencyObject root, string field, Guid entityID)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentNullException("field");


            var controlToFocus = FindControl(root, field, entityID) as FrameworkElement;

            return controlToFocus != null && controlToFocus.Focus();
        }

        /// <summary>
        /// Finds the control.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="field">The field.</param>
        /// <param name="entityID">The entity ID.</param>
        /// <returns></returns>
        public static DependencyObject FindControl(DependencyObject root, string field, Guid entityID)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);

                var entityId = NovaValidation.GetEntityID(child);
                var fieldName = NovaValidation.GetFieldName(child);

                if (entityId == entityID && fieldName == field)
                    return child;

                var childOfChild = FindControl(child, field, entityID);

                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }
    }
}
