namespace Nexio.Bat.Emi.VisuV4.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Nexio.Bat.Emi.VisuV4.Domain.Engine;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  /// <summary>
  /// Stocke les mesures Manual Point le temps que ce soit implémenté dans MesureManuelle_MaxHold
  /// </summary>
  public class MDSManualPointBuffer
  {
    private readonly List<MDSIndex> buffer = new List<MDSIndex>();
    private readonly List<DetectorSignature> detectors = new List<DetectorSignature>();

    public event EventHandler BufferChanged;

    public List<DetectorSignature> Detectors
    {
      get { return this.detectors; }
    }

    public AxisRange LevelRange { get; set; }

    public void Add(DetectorSignature detector, double frequency, long scriptPos, double angle1, double angle2, double height, double value)
    {
      var index = new MDSIndex()
      {
        Detector = detector,
        Frequency = frequency,
        ScriptPos = scriptPos,
        Angle1 = angle1,
        Angle2 = angle2,
        Height = height,
        Value = value,
      };

      if (this.AddMax(index))
      {
        this.LevelRange += value;
        this.BufferChanged?.Invoke(this, new EventArgs());
      }
    }

    public double Radius
    {
      get;
      private set;
    } = Service.Provider.UndefVal;

    public bool Any()
    {
      return this.buffer.Any();
    }

    public void Clear()
    {
      this.buffer.Clear();
      this.detectors.Clear();
      this.LevelRange = AxisRange.None;
      this.BufferChanged?.Invoke(this, new EventArgs());
    }

    public void SetRadius(int idPoint)
    {
      var idSB = Service.Provider.Instance.BatData.GetIdSousBande(idPoint);
      if (idSB >= 0)
      {
        if (Service.Provider.Instance.BatData.GetPositionInitialeSB(idSB, out double read, out _, out _, out _, out _, out _))
        {
          if (read != Service.Provider.UndefVal)
          {
            this.Radius = read;
          }
        }
      }
    }

    public void GetCurveAngles1(double angle2, double height, DetectorSignature det, double freq, out List<double> x, out List<double> y)
    {
      var ptList = this.buffer.Where(idx => this.CompAng(idx.Angle2, angle2) && this.CompHeight(idx.Height, height) && idx.Detector == det && idx.Frequency == freq).OrderBy(idx => idx.Angle1);

      var dict = ptList.GroupBy(idx => idx.Angle1).Select(g => (X: g.Key, Y: g.Max(v => v.Value))); // MaxHold maison

      x = dict.Select(idx => idx.X).ToList();
      y = dict.Select(idx => idx.Y).ToList();
    }

    public void GetCurveAngles2(double angle1, double height, DetectorSignature det, double freq, out List<double> x, out List<double> y)
    {
      var ptList = this.buffer.Where(idx => this.CompAng(idx.Angle1, angle1) && this.CompHeight(idx.Height, height) && idx.Detector == det && idx.Frequency == freq).OrderBy(idx => idx.Angle2);
      var dict = ptList.GroupBy(idx => idx.Angle2).Select(g => (X: g.Key, Y: g.Max(v => v.Value))); // MaxHold maison

      x = dict.Select(idx => idx.X).ToList();
      y = dict.Select(idx => idx.Y).ToList();
    }

    public void GetCurveHeight(double angle1, double angle2, DetectorSignature det, double freq, out List<double> x, out List<double> y)
    {
      var ptList = this.buffer.Where(idx => this.CompAng(idx.Angle1, angle1) && this.CompAng(idx.Angle2, angle2) && idx.Detector == det && idx.Frequency == freq).OrderBy(idx => idx.Height);
      var dict = ptList.GroupBy(idx => idx.Height).Select(g => (X: g.Key, Y: g.Max(v => v.Value))); // MaxHold maison

      x = dict.Select(idx => idx.X).ToList();
      y = dict.Select(idx => idx.Y).ToList();
    }

    public void GetTrace(DetectorSignature det, double freq, out List<double> a1, out List<double> h, out List<double> a2, out List<double> vals)
    {
      var ptList = this.buffer.Where(idx => idx.Detector == det && idx.Frequency == freq);
      var dict = ptList.Select(idx => (A1: idx.Angle1, H: idx.Height, A2: idx.Angle2, V: idx.Value));

      a1 = dict.Select(idx => idx.A1).ToList();
      h = dict.Select(idx => idx.H).ToList();
      a2 = dict.Select(idx => idx.A2).ToList();
      vals = dict.Select(idx => idx.V).ToList();
    }

    public void MDSGetDetectors(double freq, out List<Detector> detectors)
    {
      var ptList = this.buffer.Where(idx => idx.Frequency == freq);
      var allDet = ptList.Select(idx => idx.Detector.ToDetector()).Distinct();
      detectors = allDet.ToList();
    }

    private bool AddMax(MDSIndex index)
    {
      foreach (var item in this.buffer)
      {
        if (item.Detector == index.Detector)
        {
          if (item.Frequency == index.Frequency)
          {
            if (item.Angle1 == index.Angle1)
            {
              if (item.Angle2 == index.Angle2)
              {
                if (item.Height == index.Height)
                {
                  if (index.Value > item.Value)
                  {
                    item.Value = index.Value;
                    return true;
                  }
                  else
                  {
                    return false;
                  }
                }
              }
            }
          }
        }
      }

      this.detectors.Add(index.Detector);
      this.buffer.Add(index);
      return true;
    }

    private bool CompAng(double ang1, double ang2)
    {
      if (ang2 == Provider.UndefVal)
      {
        return true;
      }

      double angleTol = 5;
      return Math.Abs(ang1 - ang2) < angleTol;
    }

    private bool CompHeight(double h1, double h2)
    {
      if (h2 == Provider.UndefVal)
      {
        return true;
      }

      double heightTol = 0.1;
      return Math.Abs(h1 - h2) < heightTol;
    }

    public class MDSIndex
    {
      public double Angle1 { get; set; }

      public double Angle2 { get; set; }

      public DetectorSignature Detector { get; set; }

      public double Frequency { get; set; }

      public double Height { get; set; }

      public long ScriptPos { get; set; }

      public DateTime Timestamp { get; set; }

      public double Value { get; set; }
    }
  }
}