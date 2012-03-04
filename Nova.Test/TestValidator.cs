using System;
using Nova.Base;
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
			Add("TextBox", "Put in text!", ValidationSeverity.Suggestion);
			Add("TextBox", "Then put in some more!", ValidationSeverity.Warning);
			Add("ComboBox", "Select something...", ValidationSeverity.Warning);
			Add("ComboBox", "Then select something else!", ValidationSeverity.Error);
			AddRequired("CheckBox");
			Add("CheckBox2", "This is another CheckBox");
			Add("Password", "This isn't a secure password.", ValidationSeverity.Warning);
			Add("SearchTextBox", "Search somthing!", ValidationSeverity.Suggestion);
			Add("RadioButton", "This is the RadioButton with guid de475614-6405-414b-8e38-73b3953136aa", new Guid("de475614-6405-414b-8e38-73b3953136aa"));
			Add("RadioButton", "This is the RadioButton with guid 1ff8e3f3-09c7-424f-8aff-9559cbbf1243", new Guid("1ff8e3f3-09c7-424f-8aff-9559cbbf1243"));
		}
	}
}