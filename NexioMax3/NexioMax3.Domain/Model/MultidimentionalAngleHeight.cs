namespace NexioMax3.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using NexioMax3.Domain.Engine;

  public enum MDS_Types
  {
    MDS_FINALS = 2,
    MDS_PRESCAN = 3,
    MDS_ANGLE_SEARCH = 4,
    MDS_ANGLE2_SEARCH = 5,
    MDS_HEIGHT_SEARCH = 6,
    MDS_CORRECTION = 9,
  }

  public class MultidimentionalAngleHeight
  {
    public MultidimentionalAngleHeight(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur, int position)
    {
      this.TypeMeas = iTypeMeas;
      this.IdPoint = idPoint;
      this.Detecteur = iDetecteur;
      this.Position = position;
    }

    public MDS_Types TypeMeas { get; private set; }

    public int IdPoint { get; private set; }

    public Detector Detecteur { get; private set; }

    public int Position { get; private set; }

    public bool AnyAngles1 { get; private set; }

    public bool AnyAngles2 { get; private set; }

    public bool AnyHeights { get; private set; }

    public double Radius
    {
      get
      {
        var distance = 1d;
        var idSB = Service.Provider.Instance.BatData.GetIdSousBande(this.IdPoint);
        if (Service.Provider.Instance.BatData.GetPositionInitialeSB(idSB, out double read, out _, out _ , out _, out _, out _))
        {
          if (read != Service.Provider.UndefVal)
          {
            distance = read;
          }
        }

        return distance;
      }
    }

    public bool Any
    {
      get { return this.AnyAngles1 || this.AnyAngles2 || this.AnyHeights; }
    }

    public bool GetCurveAngles1(double ang2, double height, out List<double> ang1, out List<double> values)
    {
      Service.Provider.Instance.BatData.MDSGetAngles1(this.TypeMeas, this.IdPoint, this.Detecteur, out ang1, out values, this.Position, height, ang2);
      return values.Any() && values.Count == ang1.Count;
    }

    public bool GetCurveAngles2(double ang1, double height, out List<double> ang2, out List<double> values)
    {
      Service.Provider.Instance.BatData.MDSGetAngles2(this.TypeMeas, this.IdPoint, this.Detecteur, out ang2, out values, this.Position, ang1, height);
      return values.Any() && values.Count == ang2.Count;
    }

    public bool GetCurveHeight(double ang1, double ang2, out List<double> height, out List<double> values)
    {
      Service.Provider.Instance.BatData.MDSGetHeights(this.TypeMeas, this.IdPoint, this.Detecteur, out height, out values, this.Position, ang1, ang2);
      return values.Any() && values.Count == height.Count;
    }

    public void Load()
    {
      if (!this.AnyAngles1)
      {
        Service.Provider.Instance.BatData.MDSGetAngles1(this.TypeMeas, this.IdPoint, this.Detecteur, out List<double> ang1, out _, this.Position);
        this.AnyAngles1 = ang1.Where(v => v != Service.Provider.UndefVal).Any();
      }

      if (!this.AnyAngles2)
      {
        Service.Provider.Instance.BatData.MDSGetAngles2(this.TypeMeas, this.IdPoint, this.Detecteur, out List<double> ang2, out _, this.Position);
        this.AnyAngles2 = ang2.Where(v => v != Service.Provider.UndefVal).Any();
      }

      if (!this.AnyHeights)
      {
        Service.Provider.Instance.BatData.MDSGetHeights(this.TypeMeas, this.IdPoint, this.Detecteur, out List<double> haut, out _, this.Position);
        this.AnyHeights = haut.Where(v => v != Service.Provider.UndefVal).Any();
      }
    }

    public bool MDSGetPositions(DetectorSignature det, double freq, out List<double> a1, out List<double> h, out List<double> a2, out List<double> vals)
    {
      Service.Provider.Instance.BatData.MDSGetPositions(this.TypeMeas, this.IdPoint, this.Detecteur, -1, out List<double> angle1, out List<double> hauteur, out List<double> angle2, out List<double> values);
      a1 = angle1;
      a2 = angle2;
      h = hauteur;
      vals = values;
      return values.Any();
    }

    public bool MDSGetDetectors(double freq, out List<Detector> detectors)
    {
      Service.Provider.Instance.BatData.MDSGetDetectors(this.TypeMeas, this.IdPoint, -1, out List<Detector> values);
      detectors = values;
      return values.Any();
    }
  }
}