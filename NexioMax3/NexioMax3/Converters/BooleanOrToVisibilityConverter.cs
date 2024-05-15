namespace NexioMax3.Converters
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  public class BooleanOrToVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      foreach (var value in values)
      {
        if (value is bool b && b)
        {
          return Visibility.Visible;
        }
      }

      return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("One way");
    }
  }
}