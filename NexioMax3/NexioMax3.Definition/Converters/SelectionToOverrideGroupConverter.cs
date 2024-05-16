namespace NexioMax3.Definition.Converters
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;
  using NexioMax3.Definition.ViewModel;

  public class SelectionToOverrideGroupConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is EmiOverrideGroup optionGroup)
      {
        if (parameter is EmiOverrideGroup pGroup)
        {
          return optionGroup == pGroup ? Visibility.Visible : Visibility.Collapsed;
        }

        if (Enum.TryParse<EmiOverrideGroup>(parameter?.ToString() ?? string.Empty, true, out var converted))
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