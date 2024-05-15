namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public enum DetectorSignature
  {
    Peak = 2008,

    QPeak = 2020,

    AVG = 2034,

    RMS = 2049,

    CISPR_RMS = 2050,

    CISPR_AVERAGE = 2035,

    Gtem_Corellation_PEAK_X = 2013,
    Gtem_Corellation_QPEAK_X = 2017,
    Gtem_Corellation_AVG_X = 2009,
    Gtem_Corellation_PEAK_Y = 2014,
    Gtem_Corellation_QPEAK_Y = 2018,
    Gtem_Corellation_AVG_Y = 2010,
    Gtem_Corellation_PEAK_Z = 2015,
    Gtem_Corellation_QPEAK_Z = 2019,
    Gtem_Corellation_AVG_Z = 2011,
    Gtem_Corellation_PEAK_VCO = 2012,
    Gtem_Corellation_QPEAK_VCO = 2023,
    Gtem_Corellation_AVG_VCO = 2022,
    Gtem_Corellation_PEAK2_VCO = 2024,
    Gtem_Corellation_QPEAK2_VCO = 2025,
    Gtem_Corellation_AVG2_VCO = 2026,

    RBW_DIV_10_PEAK = 2200,
    RBW_DIV_10_QPEAK = 2212,
    RBW_DIV_10_AVG = 2226,
    RBW_DIV_10_RMS = 2241,
    RBW_DIV_10_CAVG = 2227,
    RBW_DIV_10_CRMS = 2242,

    RBW_MUL_10_PEAK = 2264,
    RBW_MUL_10_QPEAK = 2276,
    RBW_MUL_10_AVG = 2290,
    RBW_MUL_10_RMS = 2305,
    RBW_MUL_10_CAVG = 2291,
    RBW_MUL_10_CRMS = 2306,

    CRBM = 2500,

    NarrowBand = 160296,
    BroadBand = 160312,
    Raie_NarrowBand = 160313,

    Verif_Min_Gabarit = 401,
    Verif_Min_Gabarit2 = 65536,
    Verif_Max_Gabarit = 402,
    Verif_Max_Gabarit2 = 131072,
  }

#pragma warning disable SA1649 // File name must match first type name
  public static class DetectorSignatureExt
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(DetectorSignatureExt));

    public static Detector ToDetector(this DetectorSignature detector)
    {
      if (!detector.TryToDetector(out var result))
      {
        Log.ErrorFormat("Unknwn detector {0} : {1}", nameof(detector), detector);
        throw new ArgumentOutOfRangeException(nameof(detector), detector, null);
      }

      return result;
    }

    public static bool TryToDetector(this DetectorSignature signature, out Detector detector)
    {
      switch (signature)
      {
        case DetectorSignature.Peak:
          detector = Detector.PEAK;
          return true;

        case DetectorSignature.QPeak:
          detector = Detector.QPEAK;
          return true;

        case DetectorSignature.AVG:
          detector = Detector.AVERAGE;
          return true;

        case DetectorSignature.RMS:
          detector = Detector.RMS;
          return true;

        case DetectorSignature.CISPR_RMS:
          detector = Detector.CISPR_RMS;
          return true;

        case DetectorSignature.CISPR_AVERAGE:
          detector = Detector.CISPR_AVERAGE;
          return true;

        case DetectorSignature.Gtem_Corellation_PEAK_X:
        case DetectorSignature.Gtem_Corellation_PEAK_Y:
        case DetectorSignature.Gtem_Corellation_PEAK_Z:
        case DetectorSignature.Gtem_Corellation_PEAK_VCO:
        case DetectorSignature.Gtem_Corellation_PEAK2_VCO:
          detector = Detector.PEAK;
          return true;

        case DetectorSignature.Gtem_Corellation_QPEAK_X:
        case DetectorSignature.Gtem_Corellation_QPEAK_Y:
        case DetectorSignature.Gtem_Corellation_QPEAK_Z:
        case DetectorSignature.Gtem_Corellation_QPEAK_VCO:
        case DetectorSignature.Gtem_Corellation_QPEAK2_VCO:
          detector = Detector.QPEAK;
          return true;

        case DetectorSignature.Gtem_Corellation_AVG_X:
        case DetectorSignature.Gtem_Corellation_AVG_Y:
        case DetectorSignature.Gtem_Corellation_AVG_Z:
        case DetectorSignature.Gtem_Corellation_AVG_VCO:
        case DetectorSignature.Gtem_Corellation_AVG2_VCO:
          detector = Detector.AVERAGE;
          return true;

        case DetectorSignature.BroadBand:
        case DetectorSignature.NarrowBand:
        case DetectorSignature.Raie_NarrowBand:
          detector = Detector.PEAK;
          return true;

        case DetectorSignature.CRBM:
          detector = Detector.PEAK;
          return true;

        case DetectorSignature.RBW_MUL_10_PEAK:
        case DetectorSignature.RBW_DIV_10_PEAK:
          detector = Detector.PEAK;
          return true;
        case DetectorSignature.RBW_MUL_10_QPEAK:
        case DetectorSignature.RBW_DIV_10_QPEAK:
          detector = Detector.QPEAK;
          return true;
        case DetectorSignature.RBW_MUL_10_AVG:
        case DetectorSignature.RBW_DIV_10_AVG:
          detector = Detector.AVERAGE;
          return true;
        case DetectorSignature.RBW_MUL_10_RMS:
        case DetectorSignature.RBW_DIV_10_RMS:
          detector = Detector.RMS;
          return true;
        case DetectorSignature.RBW_MUL_10_CAVG:
        case DetectorSignature.RBW_DIV_10_CAVG:
          detector = Detector.CISPR_AVERAGE;
          return true;
        case DetectorSignature.RBW_MUL_10_CRMS:
        case DetectorSignature.RBW_DIV_10_CRMS:
          detector = Detector.CISPR_RMS;
          return true;
        case DetectorSignature.Verif_Min_Gabarit:
        case DetectorSignature.Verif_Min_Gabarit2:
          detector = Detector.PEAK;
          return true;
        case DetectorSignature.Verif_Max_Gabarit:
        case DetectorSignature.Verif_Max_Gabarit2:
          detector = Detector.PEAK;
          return true;
        default:

          int idFonction = Service.Provider.Instance.BatData.GetIdFonction((int)signature, (int)Fonction_Action.A_PRESCAN);
          Service.Provider.Instance.BatData.GetInfoColumnFonction(idFonction, 0, out TypeCol colType, out string colName);

          if (colName.ToLower().Contains("qpeak"))
          {
            detector = Detector.QPEAK;
          }
          else if (colName.ToLower().Contains("peak"))
          {
            detector = Detector.PEAK;
          }
          else if (colName.ToLower().Contains("avg"))
          {
            detector = Detector.AVERAGE;
          }
          else
          {
            // ERROR : On ne devrait arriver là
            detector = Detector.PEAK;
            return false;
          }

          return true;
      }
    }

    public static DetectorSignature ToSignature(this Detector detector)
    {
      switch (detector)
      {
        case Detector.PEAK:
          return DetectorSignature.Peak;

        case Detector.QPEAK:
          return DetectorSignature.QPeak;

        case Detector.AVERAGE:
          return DetectorSignature.AVG;

        case Detector.RMS:
          return DetectorSignature.RMS;

        case Detector.CISPR_RMS:
          return DetectorSignature.CISPR_RMS;

        case Detector.CISPR_AVERAGE:
          return DetectorSignature.CISPR_AVERAGE;

        default:
          Log.ErrorFormat("Unknwn detector {0} : {1}", nameof(detector), detector);
          throw new ArgumentOutOfRangeException(nameof(detector), detector, null);
      }
    }
  }
#pragma warning restore SA1313 // File name must match first type name
}
