using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Nova.Controls;
using Nova.Helpers;
using Nova.Library;

namespace Nova.Validation
{
	/// <summary>
	/// The validation control to make validating easier.
	/// You don't need to specify the Errors property for every child of this control, just the fieldnames.
	/// </summary>
	public class ValidationControl : Grid
    {
        /// <summary>
        /// A cache for the fields on the screen.
        /// </summary>
        private ICollection<UIElement> _fields;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationControl"/> class.
		/// </summary>
		public ValidationControl()
		{
		    Loaded += ControlLoaded;
            BindingOperations.SetBinding(this, ErrorsProperty, new ErrorBinding());
		}

        /// <summary>
        /// Triggers when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var view = GetView(sender as DependencyObject);

                if (view == null) return;

                view.ValidationControl = this;
            }
            finally
            {
                Loaded -= ControlLoaded;
            }
	    }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <returns></returns>
        private static IView GetView(DependencyObject dependencyObject)
        {
            if (dependencyObject == null) return null;

            var parent = VisualTreeHelper.GetParent(dependencyObject);

            return parent as IView ?? GetView(parent);
        }

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
				
				if (_fields != null)
					_fields.Clear();
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
		/// Default: True
		/// </summary>
		public static readonly DependencyProperty MapVisualTreeOnceProperty =
			DependencyProperty.Register("MapVisualTreeOnce", typeof (bool), typeof (ValidationControl), new PropertyMetadata(true));

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
			if (validationControl._fields == null)
			{
				validationControl._fields = new List<UIElement>();
				CacheTree(validationControl);
			}
			else if (validationControl._fields.Count == 0)
			{
				CacheTree(validationControl);
			}

			var errors = validationControl.Errors;
			foreach (var dependencyObject in validationControl._fields)
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
					CacheLogicalTree(validationControl, validationControl._fields);
					break;
				case TreeType.VisualTree:
					CacheVisualTree(validationControl, validationControl._fields);
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
					var fieldName = NovaValidation.GetFieldName(child);
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
					var fieldName = NovaValidation.GetFieldName(child);
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

			var field = NovaValidation.GetFieldName(element);

			if (string.IsNullOrEmpty(field))
				return;

			if (errors != null)
			{
				var entityID = NovaValidation.GetEntityID(element);
                
                var validations = errors.GetValidations(field, entityID).ToList();
				if (validations.Count > 0)
				{
					NovaValidation.SetIsValid(element, false);

					//Don't get max ranking/put bullets in front of lines when there is only one message to show.
					if (validations.Count == 1)
					{
						var validation = validations.First();

						NovaValidation.SetValidationTooltip(element, validation.Message);
						NovaValidation.SetSeverity(element, validation.SeverityBrush);

						return;
					}

					var mostSevereValidationRanking = validations.Max(x => x.Severity);
					var mostSevereValidation = validations.First(x => x.Severity == mostSevereValidationRanking);

					NovaValidation.SetSeverity(element, mostSevereValidation.SeverityBrush);

					//Since we are showing the most severe brush, show the most severe messages first as well.
					validations = validations.OrderByDescending(x => x.Severity).ToList();

					if (NovaValidation.GetConcatToolTip(element))
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

						NovaValidation.SetValidationTooltip(element, builder.ToString());
					}
					else
					{
						NovaValidation.SetValidationTooltip(element, mostSevereValidation.Message);
					}

					return;
				}
			}

			NovaValidation.SetIsValid(element, true);
			NovaValidation.SetValidationTooltip(element, null);
		}

        internal IEnumerable<KeyValuePair<Guid, string>> ValidateRequiredFields()
        {
            return MapVisualTreeOnce ? ValidateRequiredCache() : ValidateRequiredTree(this);
        }

        private IEnumerable<KeyValuePair<Guid, string>> ValidateRequiredTree(DependencyObject dependencyObject)
        {
            switch (ValidationMethod)
            {
                case TreeType.LogicalTree:
                    return ValidateRequiredLogicalTree(dependencyObject);
                case TreeType.VisualTree:
                    return ValidateRequiredVisualTree(dependencyObject);
            }

            return new List<KeyValuePair<Guid, string>>();
        }

	    private IEnumerable<KeyValuePair<Guid, string>> ValidateRequiredLogicalTree(DependencyObject dependencyObject)
        {
            var fields = new List<KeyValuePair<Guid, string>>();
            foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
            {
                if (!CheckIfFilledIn(child as UIElement))
                {
                    var entityID = NovaValidation.GetEntityID(child);
                    var field = NovaValidation.GetFieldName(child);
                    var kvp = new KeyValuePair<Guid, string>(entityID, field);
                    fields.Add(kvp);
                }

                fields = fields.Union(ValidateRequiredLogicalTree(child)).ToList();
            }

	        return fields;
        }

	    private IEnumerable<KeyValuePair<Guid, string>> ValidateRequiredVisualTree(DependencyObject dependencyObject)
        {
            var count = VisualTreeHelper.GetChildrenCount(dependencyObject);
            var fields = new List<KeyValuePair<Guid, string>>();

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                if (!CheckIfFilledIn(child as UIElement))
                {
                    var entityID = NovaValidation.GetEntityID(child);
                    var field = NovaValidation.GetFieldName(child);

                    var kvp = new KeyValuePair<Guid, string>(entityID, field);
                    fields.Add(kvp);
                }

                fields = fields.Union(ValidateRequiredVisualTree(child)).ToList();
            }

	        return fields;
        }

	    private IEnumerable<KeyValuePair<Guid, string>> ValidateRequiredCache()
        {
            if (_fields == null)
            {
                _fields = new List<UIElement>();
                CacheTree(this);
            }

            return _fields.Where(x => !CheckIfFilledIn(x))
                          .Select(x => new KeyValuePair<Guid, string>(NovaValidation.GetEntityID(x), NovaValidation.GetFieldName(x)))
                          .ToList();
        }

        private static bool CheckIfFilledIn(UIElement element)
        {
            if (element == null)
                return true;

            if (ViewMode.GetIsReadOnly(element)) //Read Only elements can't be required.
                return true;

            if (!NovaValidation.GetIsRequired(element))
                return true;

            var textbox = element as TextBox;
            if (textbox != null)
            {
                return !string.IsNullOrEmpty(textbox.Text);
            }

            var combobox = element as ComboBox;
            if (combobox != null)
            {
                return !string.IsNullOrEmpty(combobox.Text);
            }

            var passwordBox = element as PasswordBox;
            if (passwordBox != null)
            {
                return PasswordBoxMonitor.IsFilledIn(passwordBox);
            }

            var validationBorder = element as ValidationBorder;
            if (validationBorder != null)
            {
                return validationBorder.IsFilledIn;
            }

            return true; //return true if we don't know what it is, so we don't block anything because of possibly faulty validation.
        }
    }
}
