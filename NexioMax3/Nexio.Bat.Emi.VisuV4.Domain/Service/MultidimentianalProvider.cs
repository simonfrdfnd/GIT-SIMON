namespace Nexio.Bat.Emi.VisuV4.Domain.Service
{
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

  public class MultidimentianalProvider
  {
    private static MultidimentianalProvider intance;

    private MultidimentianalProvider()
    {
    }

    public static MultidimentianalProvider Instance
    {
      get
      {
        return intance ?? (intance = new MultidimentianalProvider());
      }
    }

    public bool IsMDSPrescan
    {
      get
      {
        return Provider.Instance.BatData.IsMDSPrescan();
      }
    }

    public bool IsMDSFinal
    {
      get
      {
        return Provider.Instance.BatData.IsMDSFinal();
      }
    }

    public MultidimentionalAngleHeight GetMultidimensional(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur, int position)
    {
      MultidimentionalAngleHeight mds = null;
      switch (iTypeMeas)
      {
        case MDS_Types.MDS_FINALS:
        case MDS_Types.MDS_PRESCAN:
        case MDS_Types.MDS_ANGLE_SEARCH:
        case MDS_Types.MDS_ANGLE2_SEARCH:
        case MDS_Types.MDS_HEIGHT_SEARCH:
          mds = new MultidimentionalAngleHeight(iTypeMeas, idPoint, iDetecteur, position);
          break;
        case MDS_Types.MDS_CORRECTION:
        default:
          break;
      }

      mds?.Load();
      return mds;
    }
  }
}