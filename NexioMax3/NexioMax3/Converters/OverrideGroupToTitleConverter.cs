namespace NexioMax3.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using NexioMax3.ViewModel;

  public class OverrideGroupToTitleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is EmiOverrideGroup group)
      {
        switch (group)
        {
          case EmiOverrideGroup.Prescans:
            return "Prescans";
          case EmiOverrideGroup.Limits:
            return "Limits";
          case EmiOverrideGroup.Suspects:
            return "Suspects";
          case EmiOverrideGroup.Finals:
            return "Finals";
          default:
            return group.ToString();
        }
      }

      throw new ArgumentException($"Value must be an {nameof(EmiOverrideGroup)}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("One way");
    }
  }
}
