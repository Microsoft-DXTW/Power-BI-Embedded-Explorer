namespace Power_BI_Embedded_Explorer.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  public class InverseConverter : IValueConverter
  {
    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return !(bool)value;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
