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

using System;
using System.Globalization;

namespace Nova.Shell.Converters
{
    /// <summary>
    /// State converter for wizards.
    /// </summary>
    internal class WizardStateConverter : StateConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isValidContent = values[1] as bool?;
            if (!isValidContent.GetValueOrDefault(true)) return IsInErrorResource;

            var isValidWizard = values[0] as bool?;
            if (!isValidWizard.GetValueOrDefault(true)) return IsInErrorResource;

            return DefaultStateResource;
        }
    }
}