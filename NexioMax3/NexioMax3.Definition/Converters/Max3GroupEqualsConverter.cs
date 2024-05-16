namespace NexioMax3.Definition.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using NexioMax3.Definition.ViewModel;

  public class Max3GroupEqualsConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Max3Group v)
      {
        if (parameter is Max3Group g)
        {
          return v == g;
        }

        if (Max3Group.TryParse(parameter?.ToString(), out g))
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