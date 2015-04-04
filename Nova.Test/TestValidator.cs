using System;
using Nova.Library;
using Nova.Validation;

namespace Nova.Test
{
	public class TestValidator : BaseValidator<MainViewModel>
	{
		public TestValidator(ValidationResults results, ActionContext actionContext) : base(results, actionContext)
		{
		}

		public override void Validate(MainViewModel entity)
        {
            Add("TextBox", "[Suggestion] Put in text!", ValidationSeverity.Suggestion);
            Add("TextBox", "[Warning] Then put in some more! (Shown first because it's a warning, even though it was added afterwards.)", ValidationSeverity.Warning);
            Add("ComboBox", "Select something...", ValidationSeverity.Warning);
            Add("ComboBox", "Then select something else!", ValidationSeverity.Error);
            Add("NumericTextBox", "Are you sure that's a valid number?", ValidationSeverity.Error);
            Add("MaskedTextBox", "Are you sure that's valid?", ValidationSeverity.Warning);
            AddRequired("CheckBox");
            Add("CheckBox2", "This is another CheckBox");
            Add("Password", "This isn't a secure password.", ValidationSeverity.Warning);
            Add("SearchTextBox", "Search somthing!", ValidationSeverity.Suggestion);
            Add("RadioButton", "This is the RadioButton with guid de475614-6405-414b-8e38-73b3953136aa", new Guid("de475614-6405-414b-8e38-73b3953136aa"));
            Add("RadioButton", "This is the RadioButton with guid 1ff8e3f3-09c7-424f-8aff-9559cbbf1243", new Guid("1ff8e3f3-09c7-424f-8aff-9559cbbf1243"));
            Add("Validation Border", "This is a validation border");
		}
	}
}