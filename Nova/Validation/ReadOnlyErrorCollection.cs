using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nova.Validation
{
	/// <summary>
	/// An Error Collection used for validation.
	/// </summary>
	public class ReadOnlyErrorCollection : ReadOnlyCollection<BaseValidation>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyErrorCollection"/> class.
		/// </summary>
		internal ReadOnlyErrorCollection()
			: base(new ObservableCollection<BaseValidation>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyErrorCollection"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		internal ReadOnlyErrorCollection(IEnumerable<BaseValidation> list)
			: this(new ObservableCollection<BaseValidation>(list))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyErrorCollection"/> class.
		/// </summary>
		/// <param name="list">The list.</param>
		internal ReadOnlyErrorCollection(ObservableCollection<BaseValidation> list)
			: base(list)
		{
		}
		
		/// <summary>
		/// Determines whether the specified field contains field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>
		///   <c>true</c> if the specified field contains field; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsField(string field)
		{
			return this.Any(x => x.Field == field);
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>The message.</returns>
		public string GetMessage(string field)
		{
			return this.First(x => x.Field == field).Message;
		}

		/// <summary>
		/// Gets the message or default value.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>The message or default value.</returns>
		public string GetMessageOrDefault(string field)
		{
			return GetMessageOrDefault(field, string.Empty);
		}

		/// <summary>
		/// Gets the message or default value.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The message or default value.</returns>
		public string GetMessageOrDefault(string field, string defaultValue)
		{
			BaseValidation firstOrDefault = this.FirstOrDefault(x => x.Field == field);
			return firstOrDefault == null ? defaultValue : firstOrDefault.Message;
		}

		/// <summary>
		/// Determines whether the specified field contains field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns>
		///   <c>true</c> if the specified field contains field; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsField(string field, Guid entityID)
		{
			return this.Any(x => x.Field == field && x.EntityID == entityID);
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns>
		/// The message.
		/// </returns>
		public string GetMessage(string field, Guid entityID)
		{
			return this.First(x => x.Field == field && x.EntityID == entityID).Message;
		}

		/// <summary>
		/// Gets the message or default value.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns>
		/// The message or default value.
		/// </returns>
		public string GetMessageOrDefault(string field, Guid entityID)
		{
			return GetMessageOrDefault(field, entityID, string.Empty);
		}

		/// <summary>
		/// Gets the message or default value.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>
		/// The message or default value.
		/// </returns>
		public string GetMessageOrDefault(string field, Guid entityID, string defaultValue)
		{
			var firstOrDefault = this.FirstOrDefault(x => x.Field == field && x.EntityID == entityID);

			return firstOrDefault == null ? defaultValue : firstOrDefault.Message;
		}

		/// <summary>
		/// Tries to get a relevant validation.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <param name="validation">The validation.</param>
		/// <returns>
		/// True if a validation message is found, false if not.
		/// </returns>
		public bool TryGetValidation(string field, Guid entityID, out BaseValidation validation)
		{
			foreach (var item in Items.Where(x => x.Field == field && x.EntityID == entityID))
			{
				validation = item;
				return true;
			}

			validation = null;
			return false;
		}

		/// <summary>
		/// Gets the validations for a certain field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <param name="entityID">The entity ID.</param>
		/// <returns></returns>
		public IEnumerable<BaseValidation> GetValidations(string field, Guid entityID)
		{
			return Items.Where(x => x.Field == field && x.EntityID == entityID);
		}
	}
}