using System;
using System.Windows;
using System.Windows.Media;
using Nova.Validation;

namespace Nova.Helpers
{
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

            if (controlToFocus != null)
            {
                controlToFocus.Focus();
                return true;
            }

            return false;
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
                {
                    return child;
                }

                var childOfChild = FindControl(child, field, entityID);

                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }
    }
}
