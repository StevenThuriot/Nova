#region License
// 
//  Copyright 2012 Steven Thuriot
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace Nova.Validation
{
	/// <summary>
	/// The validation class.
	/// </summary>
    public static class NovaValidation
    {
		#region Validate

		/// <summary>
		/// Wether or not the dependency object should be validated.
		/// </summary>
		public static readonly DependencyProperty ValidateProperty =
			DependencyProperty.RegisterAttached("Validate", typeof(bool), typeof(NovaValidation), new PropertyMetadata(true));

		/// <summary>
		/// Gets wether or not the dependency object should be validated.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns>Wether or not the dependency object should be validated.</returns>
		public static bool GetValidate(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(ValidateProperty);
		}

		/// <summary>
		/// Sets wether or not the dependency object should be validated.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetValidate(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(ValidateProperty, value);
		}

		#endregion Validate

        #region IsRequired

        /// <summary>
        /// Wether or not the dependency object is required.
        /// </summary>
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.RegisterAttached("IsRequired", typeof(bool), typeof(NovaValidation), new PropertyMetadata(false));

        /// <summary>
        /// Gets wether or not the dependency object is required.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns>Wether or not the dependency object is required.</returns>
        public static bool GetIsRequired(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsRequiredProperty);
        }

        /// <summary>
        /// Sets wether or not the dependency object is required.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsRequired(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsRequiredProperty, value);
        }

        #endregion IsRequired

        #region EntityID

        /// <summary>
		/// This is used when validating in a grid to the correct boxes will be shown.
		/// For normal scenarios, this does not have to be set.
		/// </summary>
		public static readonly DependencyProperty EntityIDProperty =
			DependencyProperty.RegisterAttached("EntityID", typeof(Guid), typeof(NovaValidation), new PropertyMetadata(Guid.Empty));

		/// <summary>
		/// Gets the entity ID.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
		public static Guid GetEntityID(DependencyObject dependencyObject)
		{
			return (Guid)dependencyObject.GetValue(EntityIDProperty);
		}

		/// <summary>
		/// Sets the entity ID.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetEntityID(DependencyObject dependencyObject, Guid value)
		{
			dependencyObject.SetValue(EntityIDProperty, value);
		}

		#endregion EntityID

		#region ValidationTooltip

		/// <summary>
		/// The error message that will be shown as a tooltip.
		/// </summary>
		public static readonly DependencyProperty ValidationTooltipProperty =
			DependencyProperty.RegisterAttached("ValidationTooltip", typeof(string), typeof(NovaValidation), new PropertyMetadata(null));

		/// <summary>
		/// Gets the validation tooltip.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
		public static string GetValidationTooltip(DependencyObject dependencyObject)
		{
			return (string)dependencyObject.GetValue(ValidationTooltipProperty);
		}

		/// <summary>
		/// Sets the validation tooltip.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">The value.</param>
		public static void SetValidationTooltip(DependencyObject dependencyObject, string value)
		{
			dependencyObject.SetValue(ValidationTooltipProperty, value);
		}

		/// <summary>
		/// If true, concat all messages in the tooltip. 
		/// If false, show the first one.
		/// </summary>
		public static readonly DependencyProperty ConcatToolTipProperty =
			DependencyProperty.RegisterAttached("ConcatToolTip", typeof(bool), typeof(NovaValidation), new PropertyMetadata(true));

		/// <summary>
		/// Checks if the tooltips need to be concatted.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
		public static bool GetConcatToolTip(DependencyObject dependencyObject)
		{
			return (bool) dependencyObject.GetValue(ConcatToolTipProperty);
		}

		/// <summary>
		/// Sets if the tooltips need to be concatted.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> concat the tooltips.</param>
		public static void SetConcatToolTip(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(ConcatToolTipProperty, value);
		}

		#endregion ValidationTooltip

		#region Severity

		/// <summary>
		/// Dependency property for the severity of the validation error.
		/// Returns a brush to paint said validations in.
		/// </summary>
		public static readonly DependencyProperty SeverityProperty =
			DependencyProperty.RegisterAttached("Severity", typeof(Brush), typeof(NovaValidation), new PropertyMetadata(null));

		/// <summary>
		/// Gets the right brush for the set severity.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
		public static Brush GetSeverity(DependencyObject dependencyObject)
		{
			return (Brush)dependencyObject.GetValue(SeverityProperty);
		}

		/// <summary>
		/// Sets the right brush for the set severity.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">The value.</param>
		public static void SetSeverity(DependencyObject dependencyObject, Brush value)
		{
			dependencyObject.SetValue(SeverityProperty, value);
		}

		#endregion Severity

		#region IsValid

		/// <summary>
		/// Wether or not the dependency object is valid.
		/// </summary>
		public static readonly DependencyProperty IsValidProperty =
			DependencyProperty.RegisterAttached("IsValid", typeof(bool), typeof(NovaValidation), new PropertyMetadata(true));

		/// <summary>
		/// Gets wether or not the dependency object is valid.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns>Wether or not the dependency object is valid.</returns>
		public static bool GetIsValid(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(IsValidProperty);
		}

		/// <summary>
		/// Sets wether or not the dependency object is valid.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetIsValid(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(IsValidProperty, value);
		}

		#endregion IsValid

		#region Validate When Disabled

		/// <summary>
		/// Wether or not to validate the dependency object when it is disabled.
		/// </summary>
		public static readonly DependencyProperty ValidateWhenDisabledProperty =
			DependencyProperty.RegisterAttached("ValidateWhenDisabled", typeof(bool), typeof(NovaValidation), new PropertyMetadata(false));

		/// <summary>
		/// Gets wether or not to validate the dependency object when it is disabled.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns>Wether or not to validate the dependency object when it is disabled.</returns>
		public static bool GetValidateWhenDisabled(DependencyObject dependencyObject)
		{
			return (bool)dependencyObject.GetValue(ValidateWhenDisabledProperty);
		}

		/// <summary>
		/// Sets wether or not to validate the dependency object when it is disabled.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">if set to <c>true</c> [value].</param>
		public static void SetValidateWhenDisabled(DependencyObject dependencyObject, bool value)
		{
			dependencyObject.SetValue(ValidateWhenDisabledProperty, value);
		}

		#endregion Validate When Disabled

		#region Field Name

		/// <summary>
		/// The field name.
		/// </summary>
		public static readonly DependencyProperty FieldNameProperty =
				DependencyProperty.RegisterAttached("FieldName", typeof(string), typeof(NovaValidation), new PropertyMetadata(string.Empty));

		/// <summary>
		/// Gets the name of the field.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns>The name of the field.</returns>
		public static string GetFieldName(DependencyObject dependencyObject)
		{
			return (string)dependencyObject.GetValue(FieldNameProperty);
		}

		/// <summary>
		/// Sets the name of the field.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">The value.</param>
		public static void SetFieldName(DependencyObject dependencyObject, string value)
		{
			dependencyObject.SetValue(FieldNameProperty, value);
		}

		#endregion Field Name

		#region Errors

		/// <summary>
		/// The list of errors.
		/// </summary>
		public static readonly DependencyProperty ErrorsProperty =
				DependencyProperty.RegisterAttached("Errors", typeof(ReadOnlyErrorCollection), typeof(NovaValidation), new PropertyMetadata(ErrorListChanged));

		/// <summary>
		/// The error list changed.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ErrorListChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var control = dependencyObject as Control;
			if (control == null)
				return;

			var field = GetFieldName(dependencyObject);
			if (string.IsNullOrEmpty(field))
				return;

			var errors = GetErrors(dependencyObject);
			if (errors != null)
			{
				var entityID = GetEntityID(dependencyObject);
				var validations = errors.GetValidations(field, entityID).ToList();

				if (validations.Count > 0)
				{
					SetIsValid(dependencyObject, false);

					//Don't get max ranking/put bullets in front of lines when there is only one message to show.
					if (validations.Count == 1)
					{
						var validation = validations.First();

						SetValidationTooltip(dependencyObject, validation.Message);
						SetSeverity(dependencyObject, validation.SeverityBrush);

						return;
					}

					var mostSevereValidationRanking = validations.Max(x => x.Ranking);
					var mostSevereValidation = validations.First(x => x.Ranking == mostSevereValidationRanking);

					SetSeverity(dependencyObject, mostSevereValidation.SeverityBrush);

					//Since we are showing the most severe brush, show the most severe messages first as well.
					validations = validations.OrderByDescending(x => x.Ranking).ToList();

					if (GetConcatToolTip(dependencyObject))
					{
						var builder = new StringBuilder();

						for (int index = 0; index < validations.Count; index++)
						{
							var validation = validations[index];
							builder.Append("• ").Append(validation.Message);

							if (index + 1 < validations.Count)
							{
								builder.AppendLine();
							}
						}

						SetValidationTooltip(dependencyObject, builder.ToString());
					}
					else
					{
						SetValidationTooltip(dependencyObject, mostSevereValidation.Message);
					}
					
					return;
				}
			}

			SetIsValid(dependencyObject, true);
			SetValidationTooltip(dependencyObject, null);
		}

		/// <summary>
		/// Gets the errorlist.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <returns></returns>
		public static ReadOnlyErrorCollection GetErrors(DependencyObject dependencyObject)
		{
			return (ReadOnlyErrorCollection)dependencyObject.GetValue(ErrorsProperty);
		}

		/// <summary>
		/// Sets the errorlist.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="value">The value.</param>
		public static void SetErrors(DependencyObject dependencyObject, ReadOnlyErrorCollection value)
		{
			dependencyObject.SetValue(ErrorsProperty, value);
		}

		#endregion Errors
    }
}
