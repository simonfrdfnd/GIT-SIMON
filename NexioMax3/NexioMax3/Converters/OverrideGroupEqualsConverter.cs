namespace NexioMax3.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using NexioMax3.ViewModel;

  public class OverrideGroupEqualsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is EmiOverrideGroup v)
      {
        if (parameter is EmiOverrideGroup g)
        {
          return v == g;
        }

        if (EmiOverrideGroup.TryParse(parameter?.ToString(), out g))
        {
          return v == g;
        }
      }

      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("OneWay");
    }
  }
}