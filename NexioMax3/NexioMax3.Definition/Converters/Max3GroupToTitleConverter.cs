namespace NexioMax3.Definition.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using NexioMax3.Definition.ViewModel;

  public class Max3GroupToTitleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Max3Group group)
      {
        switch (group)
        {
          case Max3Group.VisaConsole:
            return "Visa Console";
          case Max3Group.VirtualImmunitySetup:
            return "Virtual Immunity Setup";
          case Max3Group.TraceAnalysis:
            return "Trace Analysis";
          case Max3Group.VirtualInstrument:
            return "Virtual Instrument";
          default:
            return group.ToString();
        }
      }

      throw new ArgumentException($"Value must be an {nameof(Max3Group)}");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new InvalidOperationException("One way");
    }
  }
}
