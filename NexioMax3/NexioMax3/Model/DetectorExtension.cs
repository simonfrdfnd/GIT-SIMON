namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  public static class DetectorExtension
  {
    public static string ToStringDesc(this Detector d)
    {
      switch (d)
      {
        case Detector.RMS:
          return "RMS";
        case Detector.AVERAGE:
          return "Average";
        case Detector.QPEAK:
          return "QPeak";
        case Detector.PEAK:
          return "Peak";
#pragma warning disable CS0612
        case Detector.CHANNEL_POWER:
          return "Channel Power";
        case Detector.UMTS:
          return "UMTS";
        case Detector.QPE_AVG:
          return "QPeak Average";
#pragma warning restore CS0612
        case Detector.CISPR_RMS:
          return "CISPR RMS";
        case Detector.CISPR_AVERAGE:
          return "CISPR Average";
        default:
          return string.Empty;
      }
    }
  }
}