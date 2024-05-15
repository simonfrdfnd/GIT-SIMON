namespace Nexio.Bat.Emi.VisuV4.Domain.Engine
{
  using System.ComponentModel;
  using Nexio.Bat.Common.Domain.Equipment.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public enum Axis
  {
    Angle,
    Height,
    Height2,
    Angle2,
    Slider,
  }

  [TypeConverter(typeof(DomainEnumConverter<eSpeed>))]
  public enum eSpeed
  {
    Speed_very_slow = 1,
    Speed_slow = 2,
    Speed_medium = 3,
    Speed_fast = 4,
    Speed_very_fast = 5,
  }

  public class Pilote
  {
    public static void GoTo(Axis axis, double value, eSpeed speed)
    {
      switch (axis)
      {
        case Axis.Angle:
          Provider.Instance.Pilote3.Motion_Start(TypeScriptDomaine.SD_ReglerAngle, value, (int)speed);
          break;
        case Axis.Height:
        case Axis.Slider:
          Provider.Instance.Pilote3.Motion_Start(TypeScriptDomaine.SD_ReglerHauteur, value, (int)speed);
          break;
        case Axis.Angle2:
          Provider.Instance.Pilote3.Motion_Start(TypeScriptDomaine.SD_ReglerAngle2, value, (int)speed);
          break;
        default:
          break;
      }
    }

    public static void GetPos(out double curVal, out bool move)
    {
      Provider.Instance.Pilote3.Motion_GetPos(out curVal, out move);
    }

    public static int GetCurrentMotionScript()
    {
      return Provider.Instance.Pilote3.GetCurrentMotionScript();
    }

    public static void Stop()
    {
      Provider.Instance.Pilote3.Motion_Stop();
    }

    public static void ReadAxis(Axis axis, out double value)
    {
      switch (axis)
      {
        case Axis.Angle:
          Provider.Instance.Pilote3.MesurerPosition((int)TypeScriptDomaine.SD_MesurerAngle, out value);
          break;
        case Axis.Height:
        case Axis.Slider:
          Provider.Instance.Pilote3.MesurerPosition((int)TypeScriptDomaine.SD_MesurerHauteur, out value);
          break;
        case Axis.Angle2:
          Provider.Instance.Pilote3.MesurerPosition((int)TypeScriptDomaine.SD_MesurerAngle2, out value);
          break;
        default:
          value = double.NaN;
          break;
      }
    }

    public static bool GetCurrentAxis(Axis axis, out double value)
    {
      switch (axis)
      {
        case Axis.Angle:
          return Provider.Instance.Pilote3.GetCurrentPosition((int)TypeScriptDomaine.SD_MesurerAngle, out value);
        case Axis.Height:
        case Axis.Slider:
          return Provider.Instance.Pilote3.GetCurrentPosition((int)TypeScriptDomaine.SD_MesurerHauteur, out value);
        case Axis.Height2:
          return Provider.Instance.Pilote3.GetCurrentPosition((int)TypeScriptDomaine.SD_MesurerHauteur2, out value);
        case Axis.Angle2:
          return Provider.Instance.Pilote3.GetCurrentPosition((int)TypeScriptDomaine.SD_MesurerAngle2, out value);
        default:
          value = double.NaN;
          return false;
      }
    }

    public static void ReadSpeed(Axis axis, out double value)
    {
      switch (axis)
      {
        case Axis.Angle:
          Provider.Instance.Pilote3.GetSpeedPosition((int)TypeScriptDomaine.SD_ReglerAngle, out value);
          break;
        case Axis.Height:
        case Axis.Slider:
          Provider.Instance.Pilote3.GetSpeedPosition((int)TypeScriptDomaine.SD_ReglerHauteur, out value);
          break;
        case Axis.Height2:
          Provider.Instance.Pilote3.GetSpeedPosition((int)TypeScriptDomaine.SD_ReglerHauteur2, out value);

          break;
        case Axis.Angle2:
          Provider.Instance.Pilote3.GetSpeedPosition((int)TypeScriptDomaine.SD_ReglerAngle2, out value);
          break;
        default:
          value = double.NaN;
          break;
      }
    }
  }
}
