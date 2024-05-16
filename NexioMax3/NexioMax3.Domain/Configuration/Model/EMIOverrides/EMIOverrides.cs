namespace NexioMax3.Domain.Configuration.Model
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using global::NexioMax3.Domain.Model;
  using Nexio.Helper;

  public class EMIOverrides
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(EMIOverrides));

    private static string customFile;
    private static EMIOverrides intance;

    private static string iniFile;

    private Dictionary<Detector, OverrideCurveDef> overridePrescan = new Dictionary<Detector, OverrideCurveDef>();

    private Dictionary<Detector, OverrideCurveDef> overrideLimit = new Dictionary<Detector, OverrideCurveDef>();

    private Dictionary<string, OverrideCurveDef> overrideOther = new Dictionary<string, OverrideCurveDef>();

    private EMIOverrides()
    {
      this.LoadOverrideTables();
    }

    public static EMIOverrides Instance
    {
      get
      {
        return intance ?? (intance = new EMIOverrides());
      }
    }

    public static string IniFile
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(customFile))
        {
          return customFile;
        }

        if (string.IsNullOrEmpty(iniFile))
        {
          string path = ExecPathHelper.GetExecDirectory();
          iniFile = Path.Combine(path, "EMI_Overrides.ini");

          if (!System.IO.File.Exists(iniFile))
          {
            Log.Debug(new Exception(string.Format("EMI_Overrides.ini not found ({0})", iniFile)));
          }
        }

        return iniFile;
      }
    }

    public Dictionary<Detector, OverrideCurveDef> OverridePrescan { get => this.overridePrescan; set => this.overridePrescan = value; }

    public Dictionary<Detector, OverrideCurveDef> OverrideLimit { get => this.overrideLimit; set => this.overrideLimit = value; }

    public Dictionary<string, OverrideCurveDef> OverrideOther { get => this.overrideOther; set => this.overrideOther = value; }

    public void SetCustomFile(string file)
    {
      customFile = file;
      this.LoadOverrideTables();
    }

    public void ResetCustomFile()
    {
      customFile = null;
      this.LoadOverrideTables();
    }

    private void LoadOverrideTables()
    {
      this.ClearOverrideTables();

      if (!File.Exists(IniFile))
      {
        return;
      }

      var prescanSections = new (Detector detector, string section)[]
      {
        (Detector.PEAK, "PrescanPeak"),
        (Detector.AVERAGE, "PrescanAvg"),
        (Detector.QPEAK, "PrescanQPeak"),
        (Detector.RMS, "PrescanRMS"),
        (Detector.CISPR_AVERAGE, "PrescanCAvg"),
        (Detector.CISPR_RMS, "PrescanCRMS"),
      };

      foreach (var sec in prescanSections)
      {
        var curvedef = OverrideCurveDef.Get(IniFile, sec.section);
        if (curvedef != null)
        {
          this.OverridePrescan.Add(sec.detector, curvedef);
        }
      }

      var limitSection = new (Detector detector, string section)[]
      {
        (Detector.PEAK, "LimitPeak"),
        (Detector.AVERAGE, "LimitAvg"),
        (Detector.QPEAK, "LimitQPeak"),
        (Detector.RMS, "LimitRMS"),
      };
      foreach (var sec in limitSection)
      {
        var curvedef = OverrideCurveDef.Get(IniFile, sec.section);
        if (curvedef != null)
        {
          this.OverrideLimit.Add(sec.detector, curvedef);
        }
      }

      string[] sections = IniFileHelper.GetSectionNames(IniFile);
      foreach (var sec in sections)
      {
        if (!prescanSections.Any(s => s.section == sec) && !limitSection.Any(s => s.section == sec))
        {
          this.OverrideOther.Add(sec, OverrideCurveDef.Get(IniFile, sec));
        }
      }
    }

    private void ClearOverrideTables()
    {
      this.OverridePrescan.Clear();
      this.OverrideLimit.Clear();
      this.OverrideOther.Clear();
    }
  }
}