using System;
using System.Globalization;

namespace Nova.Shell.Converters
{
    /// <summary>
    /// State Converter for windows.
    /// </summary>
    internal class WindowStateConverter : StateConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 3)
            {
                var focused = values[3] as bool?;
                if (!focused.GetValueOrDefault(true)) return UnfocusedResource;
            }

            var isValidContent = values[2] as bool?;
            if (!isValidContent.GetValueOrDefault(true)) return IsInErrorResource;

            var isValidSession = values[1] as bool?;
            if (!isValidSession.GetValueOrDefault(true)) return IsInErrorResource;

            var isExecuting = values[0] as bool?;
            if (isExecuting.GetValueOrDefault()) return DefaultStateResource;

            return IsWaitingResource;
        }
    }
}