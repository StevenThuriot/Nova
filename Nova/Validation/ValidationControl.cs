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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Nova.Base;
using System.Windows.Data;

namespace Nova.Validation
{
	/// <summary>
	/// The validation control to make validating easier.
	/// You don't need to specify the Errors property for every child of this control, just the fieldnames.
	/// </summary>
	public class ValidationControl : Grid
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationControl"/> class.
		/// </summary>
		public ValidationControl()
		{
			BindingOperations.SetBinding(this, ErrorsProperty, new ErrorBinding());
		}

		/// <summary>
		/// A cache for the fields on the screen.
		/// </summary>
		private ICollection<UIElement> _Fields;

		/// <summary>
		/// The list of errors.
		/// </summary>
		public static readonly DependencyProperty ErrorsProperty =
				   DependencyProperty.Register("Errors", typeof(ReadOnlyErrorCollection), typeof(ValidationControl), new PropertyMetadata(ErrorListChanged));

		/// <summary>
		/// Gets or sets the errorlist.
		/// </summary>
		/// <value>
		/// The errorlist.
		/// </value>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public ReadOnlyErrorCollection Errors
		{
			get { return (ReadOnlyErrorCollection) GetValue(ErrorsProperty); }
			set
			{
				SetValue(ErrorsProperty, value);
				
				if (_Fields != null)
					_Fields.Clear();
			}
		}

		/// <summary>
		/// How to validate the controls.
		/// Default = LogicalTree
		/// </summary>
		public static readonly DependencyProperty ValidationMethodProperty =
			DependencyProperty.Register("ValidationMethod", typeof (TreeType), typeof (ValidationControl), new PropertyMetadata(TreeType.LogicalTree));

		/// <summary>
		/// Gets or sets the validation method.
		/// Default = LogicalTree
		/// </summary>
		/// <value>
		/// The validation method.
		/// </value>
		public TreeType ValidationMethod
		{
			get { return (TreeType) GetValue(ValidationMethodProperty); }
			set { SetValue(ValidationMethodProperty, value); }
		}

		/// <summary>
		/// A value indicating whether the visual tree gets mapped and cached, or iterated every time the errorlist changes.
		/// </summary>
		public static readonly DependencyProperty MapVisualTreeOnceProperty =
			DependencyProperty.Register("MapVisualTreeOnce", typeof (bool), typeof (ValidationControl), new PropertyMetadata(false));

		/// <summary>
		/// Gets or sets a value indicating whether the visual tree gets mapped and cached, or iterated every time the errorlist changes.
		/// Only set this to true in case you're very sure the screen won't be modified as any new fields will not be validated visually.
		/// </summary>
		/// <value>
		///   <c>true</c> if [map visual tree once]; otherwise, <c>false</c>.
		/// </value>
		public bool MapVisualTreeOnce
		{
			get { return (bool) GetValue(MapVisualTreeOnceProperty); }
			set { SetValue(MapVisualTreeOnceProperty, value); }
		}

		/// <summary>
		/// The error list changed.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ErrorListChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var validationControl = (ValidationControl) dependencyObject;
			var errors = validationControl.Errors;
			
			if (errors == null || errors.Count == 0)
				return;

			if (validationControl.MapVisualTreeOnce)
			{
				ValidateCache(validationControl);
			}
			else
			{
				ValidateTree(dependencyObject, errors, validationControl);
			}
		}

		/// <summary>
		/// Validates the tree.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="errors">The errors.</param>
		/// <param name="validationControl">The validation control.</param>
		private static void ValidateTree(DependencyObject dependencyObject, ReadOnlyErrorCollection errors,
		                                 ValidationControl validationControl)
		{
			switch (validationControl.ValidationMethod)
			{
				case TreeType.LogicalTree:
					ValidateLogicalTree(dependencyObject, errors);
					break;
				case TreeType.VisualTree:
					ValidateVisualTree(dependencyObject, errors);
					break;
			}
		}

		/// <summary>
		/// Validates the cache.
		/// </summary>
		/// <param name="validationControl">The validation control.</param>
		private static void ValidateCache(ValidationControl validationControl)
		{
			if (validationControl._Fields == null)
			{
				validationControl._Fields = new List<UIElement>();
				CacheTree(validationControl);
			}
			else if (validationControl._Fields.Count == 0)
			{
				CacheTree(validationControl);
			}

			var errors = validationControl.Errors;
			foreach (var dependencyObject in validationControl._Fields)
			{
				Validate(dependencyObject, errors);
			}
		}

		/// <summary>
		/// Caches the tree.
		/// </summary>
		/// <param name="validationControl">The validation control.</param>
		private static void CacheTree(ValidationControl validationControl)
		{
			switch (validationControl.ValidationMethod)
			{
				case TreeType.LogicalTree:
					CacheLogicalTree(validationControl, validationControl._Fields);
					break;
				case TreeType.VisualTree:
					CacheVisualTree(validationControl, validationControl._Fields);
					break;
			}
		}

		/// <summary>
		/// Caches the fields.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="fields">The fields dictionary used for caching.</param>
		private static void CacheVisualTree(DependencyObject dependencyObject, ICollection<UIElement> fields)
		{
			var count = VisualTreeHelper.GetChildrenCount(dependencyObject);

			for (var i = 0; i < count; i++)
			{
				var child = VisualTreeHelper.GetChild(dependencyObject, i);

				var uiElement = child as UIElement;
				if (uiElement != null)
				{
					var fieldName = Validation.GetFieldName(child);
					if (!string.IsNullOrEmpty(fieldName))
					{
						fields.Add(uiElement);
					}
				}

				CacheVisualTree(child, fields);
			}
		}

		/// <summary>
		/// Caches the fields.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="fields">The fields dictionary used for caching.</param>
		private static void CacheLogicalTree(DependencyObject dependencyObject, ICollection<UIElement> fields)
		{
			foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
			{
				var uiElement = child as UIElement;
				if (uiElement != null)
				{
					var fieldName = Validation.GetFieldName(child);
					if (!string.IsNullOrEmpty(fieldName))
					{
						fields.Add(uiElement);
					}
				}

				CacheLogicalTree(child, fields);
			}
		}

		/// <summary>
		/// Validates the visual tree.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="errors">The errors.</param>
		private static void ValidateVisualTree(DependencyObject dependencyObject, ReadOnlyErrorCollection errors)
		{
			var count = VisualTreeHelper.GetChildrenCount(dependencyObject);

			for (var i = 0; i < count; i++)
			{
				var child = VisualTreeHelper.GetChild(dependencyObject, i);
				Validate(child as UIElement, errors);

				ValidateVisualTree(child, errors);
			}
		}

		/// <summary>
		/// Validates the visual tree.
		/// </summary>
		/// <param name="dependencyObject">The dependency object.</param>
		/// <param name="errors">The errors.</param>
		private static void ValidateLogicalTree(DependencyObject dependencyObject, ReadOnlyErrorCollection errors)
		{
			foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
			{
				Validate(child as UIElement, errors);
				ValidateLogicalTree(child, errors);
			}
		}

		/// <summary>
		/// Validates the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="errors">The errors.</param>
		private static void Validate(UIElement element, ReadOnlyErrorCollection errors)
		{
			if (element == null)
				return;

			var field = Validation.GetFieldName(element);

			if (string.IsNullOrEmpty(field))
				return;

			if (errors != null)
			{
				var entityID = Validation.GetEntityID(element);
				var validations = errors.GetValidations(field, entityID).ToList();

				if (validations.Count > 0)
				{
					Validation.SetIsValid(element, false);

					//Don't get max ranking/put bullets in front of lines when there is only one message to show.
					if (validations.Count == 1)
					{
						var validation = validations.First();

						Validation.SetValidationTooltip(element, validation.Message);
						Validation.SetSeverity(element, validation.SeverityBrush);

						return;
					}

					var mostSevereValidationRanking = validations.Max(x => x.Ranking);
					var mostSevereValidation = validations.First(x => x.Ranking == mostSevereValidationRanking);

					Validation.SetSeverity(element, mostSevereValidation.SeverityBrush);

					//Since we are showing the most severe brush, show the most severe messages first as well.
					validations = validations.OrderByDescending(x => x.Ranking).ToList();

					if (Validation.GetConcatToolTip(element))
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

						Validation.SetValidationTooltip(element, builder.ToString());
					}
					else
					{
						Validation.SetValidationTooltip(element, mostSevereValidation.Message);
					}

					return;
				}
			}

			Validation.SetIsValid(element, true);
			Validation.SetValidationTooltip(element, null);
		}
	}
}
