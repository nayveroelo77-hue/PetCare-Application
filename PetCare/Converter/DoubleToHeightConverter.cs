using System.Globalization;

namespace PetCare.Converter
{
    public class DoubleToHeightConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double percentage && parameter is string maxHeightStr && double.TryParse(maxHeightStr, out double maxHeight))
            {
                return percentage * maxHeight;
            }
            return 0.0;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
