namespace NexioMax3.Domain.Configuration.Service
{
  using System;
  using System.Collections.Generic;
  using System.Data.OleDb;
  using System.Linq;
  using System.Text.RegularExpressions;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using Nexio.Bat.Common.Domain.Infrastructure.Service;
  using NexioMax3.Domain.Configuration.Model;
  using NexioMax3.Domain.Configuration.Model.EMIOverrides2;
  using NexioMax3.Domain.Model;
  using NexioMax3.Domain.Service;
  using Nexio.Defensive;

  public class GraphicOptionHelper
  {
    private GraphicOptions options;

    private EmiOverrideData emiOverrideCache = null;

    private Guid testId;

    private bool askNewVisuV4 = true;
    private bool newVisuV4 = true;

    public GraphicOptionHelper(GraphicOptions options, Guid testId)
    {
      this.testId = testId;
      this.options = options;

      this.askNewVisuV4 = OptionProvider.GetOptionBoolValue(OptionProvider.Options.AskNewVisuV4);
      this.newVisuV4 = OptionProvider.GetOptionBoolValue(OptionProvider.Options.NewVisuV4);
    }

    public GraphicOptions Options
    {
      get { return this.options; }
      set { this.options = value; }
    }

    public static void Forget(GraphicOptions options, CurveTag tag)
    {
      switch (tag.Type)
      {
        case CurveType.SubRange:
          var curveStyles = options.Curves.Where(c => c.Tag == tag).ToList();
          foreach (var cs in curveStyles)
          {
            options.Curves.Remove(cs);
          }

          break;

        case CurveType.FilteredData:
          var suspectStyles = options.Suspects.Where(c => c.Tag == tag).ToList();
          foreach (var cs in suspectStyles)
          {
            options.Suspects.Remove(cs);
          }

          var finalsStyles = options.Finals.Where(c => c.Tag == tag).ToList();
          foreach (var cs in finalsStyles)
          {
            options.Finals.Remove(cs);
          }

          break;

        case CurveType.Limit:
          var limitStyles = options.Limits.Where(c => c.Tag == tag).ToList();
          foreach (var cs in limitStyles)
          {
            options.Limits.Remove(cs);
          }

          break;

        case CurveType.CourbeExt:
          var externalStyles = options.ExternalCurves.Where(c => c.Tag == tag).ToList();
          foreach (var cs in externalStyles)
          {
            options.ExternalCurves.Remove(cs);
          }

          break;

        default:
          break;
      }
    }

    public static bool ProjectionLineFromCode(int symbol)
    {
      switch (symbol)
      {
        case 11:
        case 12:
        case 13:
          return true;

        default:
          return false;
      }
    }

    /// <summary>
    /// Convert old curve style to OxyPlot ones
    /// </summary>
    /// <param name="id">style id</param>
    /// <returns>OxyPlot Style</returns>
    public static string SymbolFromCode(int id)
    {
      switch (id)
      {
        case 1:
        case 2:
        case 11:
          return "Diamond";

        case 3:
          return "Plus";

        case 4:
        case 6:
        case 12:
          return "Circle";

        case 5:
        case 7:
        case 13:
          return "Square";

        case 8:
        case 14:
          return "Cross";

        case 9:
        case 10:
        case 15:
          return "Triangle";

        default:
          return "None";
      }
    }

    /// <summary>
    /// Genère un objet de graphic option adapté à la fenêtre settings :
    /// Utilise EMI_Override si present
    /// ou récupère les options de l'essai
    /// ou crée une option par défaut
    /// </summary>
    /// <param name="subRanges">liste des SB</param>
    /// <param name="limits">liste des limites</param>
    /// <param name="extcurves">liste des courbes externe</param>
    /// <returns>Le GraphicOptions des settings</returns>
    public GraphicOptions GetOverridedOptions(List<SubRange> subRanges, List<Limit> limits, List<ExternalCurve> extcurves)
    {
      Domain.Service.Provider.Instance.BatData.GetListePositions(out List<(int Id, string Name)> positions);
      positions.Insert(0, (0, NexioMax3.Domain.Properties.Resources.All));

      var overrideOptions = new GraphicOptions();
      // fill option.Curves
      foreach (var sr in subRanges)
      {
        var functionUsed = sr.PrescanList.Where(p => p != null && p.Function.Action == (int)Fonction_Action.A_PRESCAN).Select(p => p.Function).Distinct();
        var posUsed = sr.PositionScriptName;

        foreach (var func in functionUsed)
        {
          var tag = CurveTag.FromSubRange(sr, func);

          if (!overrideOptions.Curves.Any(c => c.Tag == tag))
          {
            var curvestyle = this.GetCurve(tag);
            overrideOptions.Curves.Add(new ConfigurationStyle(tag, curvestyle));
          }
        }
      }

      // fill overrideOptions.Suspects
      var usedFunc = Domain.Service.Provider.Instance.GetColumnUsed(Fonction_Action.A_SUSPECTS);
      foreach (var sr in subRanges)
      {
        foreach (var dat in sr.SuspectList)
        {
          var func = dat.Function;
          var actionFunc = Domain.Service.Provider.Instance.BatData.GetActionFonction(func.ID);
          int iCol = 0;
          while (Domain.Service.Provider.Instance.BatData.GetInfoColumnFonction(func.ID, iCol, out TypeCol typeCol, out string colName) >= 0)
          {
            if (func.Columns.Count <= iCol)
            {
              iCol++;
              continue;
            }

            if (!func.Columns[iCol].Used)
            {
              iCol++;
              continue;
            }

            if (typeCol != TypeCol.TYPECOL_MESURE)
            {
              iCol++;
              continue;
            }

            var pos = positions.Any(p => p.Name == sr.PositionScriptName) ?
              positions.FirstOrDefault(p => p.Name == sr.PositionScriptName) :
              positions.First();
            var intitule = pos.Id == 0 ? colName : string.Format("{0} ({1})", colName, pos.Name);
            var tag = CurveTag.FromFilteredData(dat, sr.PositionScriptName, intitule, iCol);

            if (!overrideOptions.Suspects.Any(c => c.Tag == tag))
            {
              var style = this.GetSuspect(tag);
              overrideOptions.Suspects.Add(new ConfigurationStyle(tag, style));
            }

            iCol++;
          }
        }
      }

      // fill overrideOptions.Finals
      usedFunc = Domain.Service.Provider.Instance.GetColumnUsed(Fonction_Action.A_FINAUX);
      foreach (var sr in subRanges)
      {
        foreach (var dat in sr.FinalList)
        {
          var func = dat.Function;
          var actionFunc = Domain.Service.Provider.Instance.BatData.GetActionFonction(func.ID);
          int iCol = 0;
          while (Domain.Service.Provider.Instance.BatData.GetInfoColumnFonction(func.ID, iCol, out TypeCol typeCol, out string colName) >= 0)
          {
            if (func.Columns.Count <= iCol)
            {
              iCol++;
              continue;
            }

            if (!func.Columns[iCol].Used)
            {
              iCol++;
              continue;
            }

            if (typeCol != TypeCol.TYPECOL_MESURE)
            {
              iCol++;
              continue;
            }

            var pos = positions.Any(p => p.Name == sr.PositionScriptName) ?
              positions.FirstOrDefault(p => p.Name == sr.PositionScriptName) :
              positions.First();
            var intitule = pos.Id == 0 ? colName : string.Format("{0} ({1})", colName, pos.Name);
            var tag = CurveTag.FromFilteredData(dat, sr.PositionScriptName, intitule, iCol);

            if (!overrideOptions.Finals.Any(c => c.Tag == tag))
            {
              var style = this.GetFinals(tag);
              overrideOptions.Finals.Add(new ConfigurationStyle(tag, style));
            }

            iCol++;
          }
        }
      }

      // fills overrideOptions.ExternalCurves
      foreach (var c in extcurves)
      {
        CurveTag tag = c.Tag;

        if (!overrideOptions.ExternalCurves.Any(o => o.Tag == tag))
        {
          var style = this.GetExternalCurve(tag);
          overrideOptions.ExternalCurves.Add(new ConfigurationStyle(tag, style));
        }
      }

      // fills overrideOptions.ExternalCurves
      foreach (var lim in limits)
      {
        if (lim.IsVisible)
        {
          CurveTag tag = CurveTag.FromLimit(lim);

          if (!overrideOptions.Limits.Any(l => l.Tag == tag))
          {
            var style = this.GetLimit(tag);
            overrideOptions.Limits.Add(new ConfigurationStyle(tag, style));
          }
        }
      }

      return overrideOptions;
    }

    public VisuV4ConfigurationCurveStyle GetCurve(CurveTag tag)
    {
      var emioverride = this.GlobalCurveOption(tag);
      ConfigurationStyle curveStyle = null;

      if (this.askNewVisuV4 || this.newVisuV4)
      {
        curveStyle = this.options.Curves.FirstOrDefault(c => c.Tag == tag);
      }

      if (curveStyle == null)
      {
        curveStyle = VisuV3PrefFile.Get(this.testId).ReadOption(tag);
        if (curveStyle == null)
        {
          curveStyle = this.DefaultCurve(tag);
        }

        this.options.Curves.Add(curveStyle);
      }

      var option = this.CurveStyleFromOverride(emioverride, curveStyle);
      return option as VisuV4ConfigurationCurveStyle;
    }

    public VisuV4ConfigurationCurveStyle GetExternalCurve(CurveTag tag)
    {
      var emioverride = this.GlobalCurveOption(tag);
      ConfigurationStyle curveStyle = null;

      if (this.askNewVisuV4 || this.newVisuV4)
      {
        curveStyle = this.options.ExternalCurves.FirstOrDefault(c => c.Tag == tag);
      }

      if (curveStyle == null)
      {
        curveStyle = VisuV3PrefFile.Get(this.testId).ReadOption(tag);
        if (curveStyle == null)
        {
          curveStyle = this.DefaultExtCurve(tag);
        }

        this.options.ExternalCurves.Add(curveStyle);
      }

      var option = this.CurveStyleFromOverride(emioverride, curveStyle);
      return option as VisuV4ConfigurationCurveStyle;
    }

    public VisuV4ConfigurationSuspectStyle GetSuspect(CurveTag tag, bool record = true)
    {
      var emioverride = this.GlobalScatterOption(tag);
      ConfigurationStyle suspectStyle = null;

      if (this.askNewVisuV4 || this.newVisuV4)
      {
        suspectStyle = this.options.Suspects.FirstOrDefault(c => c.Tag == tag);
      }

      if (suspectStyle == null)
      {
        suspectStyle = VisuV3PrefFile.Get(this.testId).ReadOption(tag);
        if (suspectStyle == null)
        {
          suspectStyle = this.DefaultScatterOption(tag);
        }

        this.options.Suspects.Add(suspectStyle);
      }

      var option = this.FilteredDataStyleFromOverride(emioverride, suspectStyle);
      return option;
    }

    public VisuV4ConfigurationSuspectStyle GetFinals(CurveTag tag, bool record = true)
    {
      ConfigurationStyle finalStyle = null;
      var emioverride = this.GlobalScatterOption(tag);

      if (this.askNewVisuV4 || this.newVisuV4)
      {
        finalStyle = this.options.Finals.FirstOrDefault(c => c.Tag == tag);
      }

      if (finalStyle == null)
      {
        finalStyle = VisuV3PrefFile.Get(this.testId).ReadOption(tag);
        if (finalStyle == null)
        {
          finalStyle = this.DefaultScatterOption(tag);
        }

        this.options.Finals.Add(finalStyle);
      }

      var option = this.FilteredDataStyleFromOverride(emioverride, finalStyle);
      return option;
    }

    /// <summary>
    /// Retourne le style applicable à la limite.
    /// Ordre de priorités
    /// -Définie dans EMI Override
    /// -Style présent dans le .visu4
    /// -Style présent dans le .pref v3 exporté en xml
    /// -Style par defaut
    /// </summary>
    /// <param name="tag">Identifiant du type de limite</param>
    /// <param name="defaultVisibility">Visibilité de la limite utilisé par le style par defaut.</param>
    /// <returns>Le style applicable</returns>
    public VisuV4ConfigurationCurveStyle GetLimit(CurveTag tag, bool defaultVisibility = false)
    {
      var emioverride = this.GlobalLimitOption(tag);
      ConfigurationStyle limitStyle = null;

      if (this.askNewVisuV4 || this.newVisuV4)
      {
        limitStyle = this.options.Limits.FirstOrDefault(c => c.Tag == tag);
      }

      if (limitStyle == null)
      {
        limitStyle = VisuV3PrefFile.Get(this.testId).ReadOption(tag);
        if (limitStyle == null)
        {
          limitStyle = this.DefaultLimitOption(tag, defaultVisibility);
        }

        this.options.Limits.Add(limitStyle);
      }

      var option = this.CurveStyleFromOverride(emioverride, limitStyle);
      return option;
    }

    public void RemoveEmiOverrideCache()
    {
      this.emiOverrideCache = null;
    }

    private VisuV4ConfigurationCurveStyle CurveStyleFromOverride(StyleDefinition emiOverride, ConfigurationStyle curveStyle)
    {
      VisuV4ConfigurationCurveStyle style;
      var original = curveStyle.Style as VisuV4ConfigurationCurveStyle;
      if (emiOverride != null)
      {
        style = new VisuV4ConfigurationCurveStyle()
        {
          Color = emiOverride.OverrideColor ? this.RGB2Color(emiOverride.Color) : original.Color,
          Size = emiOverride.OverrideSize ? emiOverride.Size : original.Size,
          OverrideColor = emiOverride.OverrideColor,
          OverrideSize = emiOverride.OverrideSize,
          OverrideIsVisible = emiOverride.OverrideIsVisible,
          IsVisible = emiOverride.OverrideIsVisible ? emiOverride.IsVisible : original.IsVisible,
        };
      }
      else
      {
        style = new VisuV4ConfigurationCurveStyle()
        {
          Color = original.Color,
          Size = original.Size,
          OverrideColor = original.OverrideColor,
          OverrideSize = original.OverrideSize,
          IsVisible = original.IsVisible,
        };
      }

      return style;
    }

    private VisuV4ConfigurationSuspectStyle FilteredDataStyleFromOverride(EmiOverridePoint emiOverride, ConfigurationStyle suspectStyle)
    {
      VisuV4ConfigurationSuspectStyle style;
      var original = suspectStyle.Style as VisuV4ConfigurationSuspectStyle;
      if (emiOverride != null)
      {
        style = new VisuV4ConfigurationSuspectStyle()
        {
          Color = emiOverride.OverrideColor ? this.RGB2Color(emiOverride.Color) : original.Color,
          Symbol = emiOverride.OverrideSymbol ? SymbolFromCode(emiOverride.SymbolId) : original.Symbol,
          SymbolFilled = emiOverride.OverrideSymbol ? emiOverride.SymbolFilled : original.SymbolFilled, // 2, 6, 7, 15 sont les identifiants des symboles pleins
          Size = emiOverride.OverrideSize ? emiOverride.Size : original.Size,
          OverrideColor = emiOverride.OverrideColor,
          OverrideSymbol = emiOverride.OverrideSymbol,
          OverrideSize = emiOverride.OverrideSize,
          OverrideIsVisible = emiOverride.OverrideIsVisible,
          IsVisible = emiOverride.OverrideIsVisible ? emiOverride.IsVisible : original.IsVisible,
          OverrideUseProjection = emiOverride.OverrideUseProjection,
          ProjectionLine = emiOverride.OverrideUseProjection ? emiOverride.UseProjection : original.ProjectionLine,
        };
      }
      else
      {
        style = new VisuV4ConfigurationSuspectStyle()
        {
          Color = original.Color,
          Size = original.Size,
          Symbol = original.Symbol,
          SymbolFilled = original.SymbolFilled,
          ProjectionLine = original.ProjectionLine,
          OverrideColor = original.OverrideColor,
          OverrideSymbol = original.OverrideSymbol,
          OverrideIsVisible = original.OverrideIsVisible,
          OverrideSize = original.OverrideSize,
          IsVisible = original.IsVisible,
        };
      }

      return style;
    }

    private string RGB2Color(string color)
    {
      var rgb = color.Split(new[] { ',' });
      byte r = byte.Parse(rgb[0]);
      byte g = byte.Parse(rgb[1]);
      byte b = byte.Parse(rgb[2]);
      var convert = System.Drawing.Color.FromArgb(r, g, b);
      return System.Drawing.ColorTranslator.ToHtml(convert);
    }

    private EmiOverrideCurve GlobalLimitOption(CurveTag tag)
    {
      var data = this.emiOverrideCache ?? this.emiOverrideCache ?? (this.emiOverrideCache = EmiOverrideData.Load());
      var detector = (Detector)int.Parse(tag.Properties[CurveTag.DetectorKey]);
      {
        var style = data.Limits.FirstOrDefault(curve => curve.Detector == detector) ??
                    data.Limits.FirstOrDefault(curve => curve.Detector == detector && (curve.Position?.Equals(NexioMax3.Domain.Properties.Resources.All, StringComparison.InvariantCultureIgnoreCase) ?? false));

        if (style != null)
        {
          return style;
        }
      }

      return null;
    }

    /// <summary>
    /// Génère un style par defaut pour le type de limite en paramètre.
    /// </summary>
    /// <param name="tag">Identifiant du type de limite</param>
    /// <param name="defaultVisibility">Visibilité de la limite utilisé par le style par defaut.</param>
    /// <returns>Le style par defaut</returns>
    private ConfigurationStyle DefaultLimitOption(CurveTag tag, bool defaultVisibility = false)
    {
      var detector = (Detector)int.Parse(tag.Properties[CurveTag.DetectorKey]);
      VisuV4ConfigurationCurveStyle style;
      switch (detector)
      {
        case Detector.RMS:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("155,88,181"), // Pink
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

        case Detector.AVERAGE:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("45,204,112"), // Green
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

        case Detector.QPEAK:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("232,76,61"), // Red
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

        case Detector.PEAK:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("53,152,219"), // Blue
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

        case Detector.CISPR_RMS:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("241,196,15"), // Yellow
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

        case Detector.CISPR_AVERAGE:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("231,126,35"), // Orange
            Size = 5,
            IsVisible = defaultVisibility,
          };
          break;

#pragma warning disable CS0612
        case Detector.CHANNEL_POWER:
        case Detector.UMTS:
        case Detector.QPE_AVG:
#pragma warning restore CS0612
        default:
          throw new NotImplementedException();
      }

      return new ConfigurationStyle(tag, style);
    }

    private EmiOverridePoint GlobalScatterOption(CurveTag tag)
    {
      {
        var dataType = (FilteredDataType)int.Parse(tag.Properties[CurveTag.DataTypeKey]);
        var datas = this.emiOverrideCache ?? (this.emiOverrideCache = EmiOverrideData.Load());
        var styleList = dataType == FilteredDataType.Final ? datas.Finals : datas.Suspects;
        var position = tag.Properties[CurveTag.PositionKey];
        var source = tag.Properties[CurveTag.SourceKey];

        var str = tag.ToString();
        // Permet de récupérer le nom de la colonne en entier dans un cas de format [column] ([source]) ([position])
        string tagColumn = str.Contains("(") ? str.Split('(')[0].Trim() : str.Split()[0];

        if (!tagColumn.Equals("selectedSuspect") &&
            !tagColumn.StartsWith("selectedFilteredData_", StringComparison.InvariantCultureIgnoreCase) &&
            !tagColumn.Equals("selectedFinal"))
        {
          SourcesCache.Append(source, tagColumn);
        }

        var style = styleList.FirstOrDefault(point => $"{point.Column} ({point.Source}) ({point.Position})" == str);

        if (style != null)
        {
          return style;
        }

        // NC (INC0018729): Reecriture de l'algo initial qui devanait difficile à maintenir et avait des failles
        // Si des cas m'ont échapé, faire un "blame previous revision" pour rattraper le coup

        // On récupère l'ensemble des clés disponibles
        var allcolumns = styleList.Select(item => item.Column).Distinct().ToList();
        var allpositions = styleList.Select(item => item.Position).Distinct().ToList();
        var allsources = styleList.Select(item => item.Source).Distinct().ToList();

        // On choisis la clé correspondant au tag (tag ou all)
        var colKey = allcolumns.Any(c => c == tagColumn) ? tagColumn : NexioMax3.Domain.Properties.Resources.All;
        var posKey = allpositions.Any(p => p == position) ? position : NexioMax3.Domain.Properties.Resources.All;
        var sourceKey = allsources.Any(s => s == source) ? source : NexioMax3.Domain.Properties.Resources.All;

        // On filtre les styles corrspondants au clés (en théorie 1 ou 0)
        var filtered = styleList.Where(point => point.Column == colKey && point.Position == posKey && point.Source == sourceKey);
        return filtered.FirstOrDefault(); // renvois le style trouvé ou null
      }
    }

    private ConfigurationStyle DefaultScatterOption(CurveTag tag)
    {
      FilteredDataType type = (FilteredDataType)int.Parse(tag.Properties[CurveTag.DataTypeKey]);
      VisuV4ConfigurationSuspectStyle style;
      switch (type)
      {
        case FilteredDataType.Final:
          style = new VisuV4ConfigurationSuspectStyle()
          {
            Color = this.RGB2Color("39,174,97"), // Green
            Symbol = "Diamond",
            SymbolFilled = false,
            Size = 5,
          };
          break;

        case FilteredDataType.Suspect:
        default:
          style = new VisuV4ConfigurationSuspectStyle()
          {
            Color = this.RGB2Color("241,196,15"), // Yellow
            Symbol = "Cross",
            SymbolFilled = false,
            Size = 5,
          };
          break;
      }

      return new ConfigurationStyle(tag, style);
    }

    private StyleDefinition GlobalCurveOption(CurveTag tag)
    {
      if (tag.Type == CurveType.CourbeExt)
      {
        var project = tag.Properties[CurveTag.ProjectNameKey];
        var curveName = tag.Properties[CurveTag.ExternalCurveNameKey];
        var position = tag.Properties[CurveTag.PositionKey];
        var datas = this.emiOverrideCache ?? this.emiOverrideCache ?? (this.emiOverrideCache = EmiOverrideData.Load());

        if (int.TryParse(position, out var parsedPosition))
        {
          var positions = Provider.Instance.GetAllPositions().Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToArray();
          position = positions.FirstOrDefault(p => p.Id == parsedPosition)?.Name;
        }

        var styles = datas.Externals.Where(curve => (curve.Project == project || curve.Project == NexioMax3.Domain.Properties.Resources.All)
                                                 && (position == curve.Position || curve.Position == NexioMax3.Domain.Properties.Resources.All)
                                                 && (string.IsNullOrWhiteSpace(curve.Name)
                                                     || (curveName.StartsWith(curve.Name)
                                                        && curve.Name.Length <= curveName.Length))).OrderBy(curve => curveName.Length).ToArray();

        return styles.FirstOrDefault(curve => curve.Project == project && curve.Name == curveName && curve.Position == position) ??
               styles.FirstOrDefault(curve => curve.Project == project && (string.IsNullOrWhiteSpace(curve.Name) || curveName.StartsWith(curve.Name)) && curve.Position == NexioMax3.Domain.Properties.Resources.All) ??
               styles.FirstOrDefault(curve => curve.Project == NexioMax3.Domain.Properties.Resources.All && curve.Name == curveName && curve.Position == position) ??
               styles.FirstOrDefault(curve => curve.Project == NexioMax3.Domain.Properties.Resources.All && curve.Name == curveName && curve.Position == NexioMax3.Domain.Properties.Resources.All) ??
               styles.FirstOrDefault(curve => curve.Project == NexioMax3.Domain.Properties.Resources.All && (string.IsNullOrWhiteSpace(curve.Name) || curveName.StartsWith(curve.Name)) && curve.Position == position) ??
               styles.FirstOrDefault(curve => curve.Project == NexioMax3.Domain.Properties.Resources.All && (string.IsNullOrWhiteSpace(curve.Name) || curveName.StartsWith(curve.Name)) && curve.Position == NexioMax3.Domain.Properties.Resources.All);
      }

      // else
      {
        var detector = ((DetectorSignature)int.Parse(tag.Properties[CurveTag.DetectorKey])).ToDetector();
        var position = tag.Properties[CurveTag.PositionKey];
        {
          var datas = this.emiOverrideCache ?? this.emiOverrideCache ?? (this.emiOverrideCache = EmiOverrideData.Load());

          var style =
            datas.Prescans.FirstOrDefault(curve => curve.Detector == detector && curve.Position == position) ??
            datas.Prescans.FirstOrDefault(curve => curve.Detector == detector &&
                                                   (curve.Position?.Equals(NexioMax3.Domain.Properties.Resources.All, StringComparison.InvariantCultureIgnoreCase) ?? false));

          return style;
        }
      }
    }

    private ConfigurationStyle DefaultCurve(CurveTag tag)
    {
      var detector = (DetectorSignature)int.Parse(tag.Properties[CurveTag.DetectorKey]);

      VisuV4ConfigurationCurveStyle style;
      switch (detector)
      {
        case DetectorSignature.RMS:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("155,88,181"), // Pink
            Size = 1,
          };
          break;

        case DetectorSignature.AVG:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("45,204,112"), // Green
            Size = 1,
          };
          break;

        case DetectorSignature.QPeak:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("232,76,61"), // Red
            Size = 1,
          };
          break;

        case DetectorSignature.Peak:
        default:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("53,152,219"), // Blue
            Size = 1,
          };
          break;

        case DetectorSignature.CISPR_RMS:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("241,196,15"), // Yellow
            Size = 1,
          };
          break;

        case DetectorSignature.CISPR_AVERAGE:
          style = new VisuV4ConfigurationCurveStyle()
          {
            Color = this.RGB2Color("231,126,35"), // Orange
            Size = 1,
          };
          break;
      }

      return new ConfigurationStyle(tag, style);
    }

    private ConfigurationStyle DefaultExtCurve(CurveTag tag)
    {
      var colorUsed = this.options.ExternalCurves.Select(s => s.Style.Color).ToList();

      // généré avec https://medialab.github.io/iwanthue/
      var palette = new List<string>
      {
        "#8574e8",
        "#75e94c",
        "#b259ec",
        "#54b432",
        "#df4bd2",
        "#bad847",
        "#5588e9",
        "#e4c746",
        "#c37bd7",
        "#8ee280",
        "#df5aac",
        "#3fb866",
        "#ed3f7c",
        "#4beab7",
        "#eb4c2b",
        "#629a39",
        "#e35b6d",
        "#a09a30",
        "#dc7245",
        "#d68f31",
      };

      // on cherche la dernière couleur utilisé
      var lastIdxUsed = -1;
      foreach (var col in colorUsed.AsEnumerable().Reverse())
      {
        if (palette.Contains(col))
        {
          lastIdxUsed = palette.IndexOf(col);
          break;
        }
      }

      VisuV4ConfigurationCurveStyle style = new VisuV4ConfigurationCurveStyle()
      {
        Color = palette[(lastIdxUsed + 1) % palette.Count], // Pink
        Size = 1,
      };

      return new ConfigurationStyle(tag, style);
    }
  }
}