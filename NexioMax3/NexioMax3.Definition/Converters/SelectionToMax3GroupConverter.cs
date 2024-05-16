namespace NexioMax3.Definition.Converters
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;
  using NexioMax3.Definition.ViewModel;

  public class SelectionToMax3GroupConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Max3Group optionGroup)
      {
        if (parameter is Max3Group pGroup)
        {
          return optionGroup == pGroup ? Visibility.Visible : Visibility.Collapsed;
        }

        if (Enum.TryParse<Max3Group>(parameter?.ToString() ?? string.Empty, true, out var converted))
        {
          return converted == optionGroup ? Visibility.Visible : Visibility.Collapsed;
        }
      }

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("OneWay");
    }
  }
}