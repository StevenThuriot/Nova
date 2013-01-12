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
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Nova.Shell.Converters
{
    internal class StateConverter : IMultiValueConverter
    {
        public SolidColorBrush HasOpenDocumentsResource { get; set; }
        public SolidColorBrush IsWaitingResource { get; set; }
        public SolidColorBrush IsInErrorResource { get; set; }
        public SolidColorBrush Unfocused { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var focused = values[2] as bool?;
            if (!focused.GetValueOrDefault(true)) return Unfocused;

            var isValid = values[1] as bool?;
            if (!isValid.GetValueOrDefault(true)) return IsInErrorResource;

            var isExecuting = values[0] as bool?;
            if (isExecuting.GetValueOrDefault()) return HasOpenDocumentsResource;

            return IsWaitingResource;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return Enumerable.Repeat(Binding.DoNothing, targetTypes.Length).ToArray();
        }
    }
}
