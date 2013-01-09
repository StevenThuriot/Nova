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