namespace NexioMax3.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;
  using Nexio.Bat.Common.Domain.ATDB.Model;
  using Nexio.Bat.Common.Domain.ATDB.Service;

  [JsonObject(MemberSerialization.OptIn)]
  public class CurveTag : IEquatable<CurveTag>
  {
    public static readonly string PositionKey = "Position";

    public static readonly string PositionNameKey = "PositionName";

    public static readonly string DetectorKey = "Detector";

    public static readonly string SubRangeIdKey = "SubRangeId";

    public static readonly string FunctionIDKey = "FunctionID";

    public static readonly string DataTypeKey = "DataType";

    public static readonly string DistanceKey = "Distance";

    public static readonly string ClasseKey = "Classe";

    public static readonly string TypeSignalKey = "TypeSignal";

    public static readonly string GuidLimitKey = "GuidLimit";

    public static readonly string LimitDisplayName = "LimitDisplayName";

    public static readonly string SourceKey = "Source";

    public static readonly string ProjectNameKey = "ProjectName";

    public static readonly string ExternalCurveNameKey = "ExternalCurveName";

    public static readonly string ColumnKey = "Column";

    private const string FileGuid = "{11111111-1111-1111-1111-111111111111}";

    [JsonProperty(PropertyName = "FunctionName")]
    public string FunctionName { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "Properties")]
    public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    [JsonProperty(PropertyName = "Type")]
    public CurveType Type { get; set; }

    public bool CanUseSuspectOrFinal => this.Type == CurveType.SubRange || this.Type == CurveType.FilteredData;

    public bool CanUseManual => this.Type == CurveType.SubRange || this.Type == CurveType.FilteredData;

    public static bool operator ==(CurveTag t1, CurveTag t2)
    {
      return t1.Equals(t2);
    }

    public static bool operator !=(CurveTag t1, CurveTag t2)
    {
      return !t1.Equals(t2);
    }

    public static CurveTag FromSubRange(SubRange sr, Function fn)
    {
      var tag = new CurveTag { FunctionName = fn.Name, Type = CurveType.SubRange };
      tag.Properties.Add(PositionKey, sr.PositionScriptName);
      tag.Properties.Add(DetectorKey, ((int)fn.Signature).ToString());
      tag.Properties.Add(FunctionIDKey, fn.ID.ToString());
      return tag;
    }

    public static CurveTag FromFilteredData(FilteredData fd, string scriptName, string intitule, int iCol)
    {
      var fn = fd.Function;
      var tag = new CurveTag { FunctionName = intitule, Type = CurveType.FilteredData };
      tag.Properties.Add(FunctionIDKey, fn.ID.ToString());
      tag.Properties.Add(PositionKey, scriptName);
      tag.Properties.Add(DataTypeKey, ((int)fd.DataType).ToString());
      tag.Properties.Add(DetectorKey, ((int)fn.Signature).ToString());
      tag.Properties.Add(SourceKey, fd.Function.Name);
      tag.Properties.Add(ColumnKey, iCol.ToString());
      return tag;
    }

    public static CurveTag FromLimit(Limit lim)
    {
      var tag = new CurveTag { FunctionName = lim.Name, Type = CurveType.Limit };
      tag.Properties.Add(DetectorKey, ((int)lim.Detector).ToString());
      tag.Properties.Add(DistanceKey, lim.Distance.ToString().Replace(',', '.'));
      tag.Properties.Add(ClasseKey, lim.Classe);
      tag.Properties.Add(TypeSignalKey, lim.TypeSignal.ToString());
      tag.Properties.Add(GuidLimitKey, lim.Guid);
      tag.Properties.Add(LimitDisplayName, lim.GetFormatedName());
      return tag;
    }

    public static CurveTag FromExternalCurve(ExternalCurve curve,
                                             int iPositionCourbe,
                                             string posName,
                                             List<string> nomsPositions,
                                             List<int> listPosition,
                                             int typeSignals)
    {
      var curveTag = new CurveTag { Type = CurveType.CourbeExt };
      curveTag.Properties.Add(FunctionIDKey, curve.Id.ToString());
      curveTag.Properties.Add(DetectorKey, "0"); // Detecteur par défaut
      curveTag.Properties.Add(TypeSignalKey, typeSignals.ToString());
      string projectName = null;

      if (curve.GuidSource == Guid.Empty)
      {
        projectName = "Old Project (V3) or Max";
      }
      else if (curve.GuidSource == Guid.Parse(FileGuid))
      {
        projectName = "Imported From File";
      }
      else
      {
        var projects = ATDBProvider.Instance.GetListProjects();
        foreach (var project in projects)
        {
          if (HasChild(ATDBProvider.Instance.ReadProjectTree(project.GuidObject), curve.GuidSource))
          {
            projectName = project.Name;
            break;
          }
        }

        if (projectName == null)
        {
          projectName = "Unknown Project";
        }
      }

      curveTag.Properties.Add(ProjectNameKey, projectName);

      curveTag.Properties.Add(ExternalCurveNameKey, curve.Name);

      curveTag.FunctionName = string.Empty;
      int iPos;
      for (iPos = 0; iPos < listPosition.Count; iPos++)
      {
        if (listPosition[iPos] == iPositionCourbe)
        {
          if (iPositionCourbe != 32767 || nomsPositions[iPos] == posName)
          {
            curveTag.FunctionName = string.Format("{0} ({1})", curve.Name, nomsPositions[iPos]);
            curveTag.Properties.Add(PositionNameKey, nomsPositions[iPos]);
            break;
          }
        }
      }

      if (string.IsNullOrEmpty(curveTag.FunctionName))
      {
        curveTag.FunctionName = curve.Name;
      }

      curveTag.Properties.Add(PositionKey, iPositionCourbe.ToString());

      return curveTag;
    }

    public override string ToString()
    {
      switch (this.Type)
      {
        case CurveType.SubRange:
          var posName = this.Properties[PositionKey];
          if (!string.IsNullOrEmpty(posName))
          {
            return string.Format("{0} ({1})", this.FunctionName, posName);
          }
          else
          {
            return string.Format("{0}", this.FunctionName);
          }

        case CurveType.Limit:
          return this.Properties[LimitDisplayName];
        case CurveType.CourbeExt:
          return this.FunctionName;
        case CurveType.FilteredData:
        default:
          return string.Format("{0}", this.FunctionName);
      }
    }

    public override bool Equals(object obj)
    {
      if (obj is CurveTag other)
      {
        return this.Equals(other);
      }

      return false;
    }

    public override int GetHashCode()
    {
      var hashDict = 773;
      foreach (var item in this.Properties.Where(kvp => kvp.Key != LimitDisplayName))
      {
        hashDict = (37 * hashDict) ^ item.Value.GetHashCode();
      }

      return (this.FunctionName.GetHashCode() * 521) ^ hashDict;
    }

    public bool Equals(CurveTag other)
    {
      if (other?.Properties == null)
      {
        return false;
      }

      bool sameProperties = !other.Properties.Where(kvp => kvp.Key != LimitDisplayName).Where(kvp => !this.Properties.Contains(kvp)).Any();
      return sameProperties && other.FunctionName == this.FunctionName;
    }

    private static bool HasChild(AbstractATDBObject project, Guid testId)
    {
      if (project?.Children == null)
      {
        return false;
      }

      if (project.Children.Any(o => o.GuidObject == testId) || project.Children.Any(o => HasChild(o, testId)))
      {
        return true;
      }

      return false;
    }
  }
}