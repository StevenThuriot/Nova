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