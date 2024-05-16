namespace NexioMax3.Domain.Configuration.Service
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using NexioMax3.Domain.Configuration.Model;
  using NexioMax3.Domain.Model;
  using Nexio.Helper;
  using Nexio;

  public class VisuV3PrefFile
  {
    private static List<VisuV3PrefFile> cache = new List<VisuV3PrefFile>();

    private readonly Guid guidEssai;

    /// <summary>
    /// format de stockage des prefs dans v3
    /// <clé : position, <clé : signature, pref>>
    /// </summary>
    private Dictionary<int, Dictionary<int, PrefV3>> mapPrefCourbe = new Dictionary<int, Dictionary<int, PrefV3>>();

    private Dictionary<int, Dictionary<int, PrefV3>> mapPrefCourbeExt = new Dictionary<int, Dictionary<int, PrefV3>>();
    private List<PrefLimitV3> mapPrefLimits = new List<PrefLimitV3>();
    private Dictionary<string, int> posMap = new Dictionary<string, int>();

    private VisuV3PrefFile(Guid guidEssai)
    {
      this.guidEssai = guidEssai;
      this.Init(guidEssai);
    }

    public bool IsMaxHoldTrace { get; set; } = true;

    public Engine.AxisRange YRange { get; internal set; } = Engine.AxisRange.None;

    /// <summary>
    /// Indique si les preferences v3 sont en echelle log
    /// On utilise null pour indiquer que l'information n'a pas pu être chargé
    /// </summary>
    public bool? EchelleLog { get; internal set; } = null;

    public bool HasPrf { get; private set; } = false;

    public bool Loaded { get; private set; } = false;

    internal Dictionary<int, Dictionary<int, PrefV3>> MapPrefCourbe { get => this.mapPrefCourbe; private set => this.mapPrefCourbe = value; }

    internal Dictionary<int, Dictionary<int, PrefV3>> MapPrefCourbeExt { get => this.mapPrefCourbeExt; set => this.mapPrefCourbeExt = value; }

    internal List<PrefLimitV3> MapPrefLimits { get => this.mapPrefLimits; set => this.mapPrefLimits = value; }

    /// <summary>
    /// Remplace l'ancien Singleton qui posait soucis lors de la génération des rapports en forcant l'appel à Init(guid) trop souvent
    /// </summary>
    /// <param name="guidEssai">guid de l'essai</param>
    /// <returns>les prefs v3</returns>
    public static VisuV3PrefFile Get(Guid guidEssai)
    {
      var stored = cache.FirstOrDefault(p => p.guidEssai == guidEssai);
      if (stored != null)
      {
        return stored;
      }
      else
      {
        var inst = new VisuV3PrefFile(guidEssai);
        cache.Add(inst);
        return inst;
      }
    }

    /// <summary>
    /// TODO : Appeler en fin de génération de rapport pour éviter les fuites mémoires
    /// </summary>
    public static void ClearCache()
    {
      cache.Clear();
    }

    /// <summary>
    /// Premet de recharger les preferences de l'essai si le fichier pref à été modifié
    /// </summary>
    public void Reload()
    {
      this.Init(this.guidEssai);
    }

    internal ConfigurationStyle ReadOption(CurveTag tag)
    {
      switch (tag.Type)
      {
        case CurveType.SubRange:
          return this.ReadSubrangeOption(tag);

        case CurveType.FilteredData:
          return this.ReadFilteredDataOption(tag);

        case CurveType.Limit:
          return this.ReadLimitOption(tag);

        case CurveType.CourbeExt:
          return this.ReadCourbeExtOption(tag);

        default:
          return null;
      }
    }

    private void Init(Guid guidEssai)
    {
      var resultPath = Domain.Service.Provider.Instance.BatData.GetPathResult();
      var numEssai = Domain.Service.Provider.Instance.BatData.GetIDEssai();
      string prefFile = Path.Combine(resultPath, numEssai.ToString() + ".prf");
      string xmlFile = Path.Combine(resultPath, numEssai.ToString() + ".xml");

      this.HasPrf = File.Exists(prefFile);
      var hasXml = File.Exists(xmlFile);
      if (this.HasPrf)
      {
        if (!hasXml)
        {
          this.GenerateXml(guidEssai);
        }

        if (!File.Exists(xmlFile))
        {
          this.Loaded = false;
          return;
        }

        VisuV3PrefProvider.Read(xmlFile, this);
        this.Loaded = true;
      }

      this.posMap.Clear();
      Domain.Service.Provider.Instance.BatData.GetListePositions(out List<(int Pos, string Name)> listePos);
      listePos.ForEach(t => this.posMap.Add(t.Name, t.Pos));
    }

    private void GenerateXml(Guid guidEssai)
    {
      var apppath = ExecPathHelper.GetExecDirectory();
      var app = Path.Combine(apppath, "Bat_Visu3.exe");
      var resultPath = Domain.Service.Provider.Instance.BatData.GetPathResult();
      var numEssai = Domain.Service.Provider.Instance.BatData.GetIDEssai();
      var args = string.Format("{0} {1} 0 {2} -x", guidEssai.ToString("B"), numEssai, resultPath);
      var process = System.Diagnostics.Process.Start(app, args);
      process.WaitForExit();
    }

    private void Load()
    {
    }

    private ConfigurationStyle ReadCourbeExtOption(CurveTag tag)
    {
      var posId = tag.Properties[CurveTag.PositionKey].ToInt();

      if (!this.MapPrefCourbeExt.ContainsKey(posId))
      {
        return null;
      }

      var signatures = this.MapPrefCourbeExt[posId];
      int idFonction = tag.Properties[CurveTag.FunctionIDKey].ToInt();

      if (!signatures.ContainsKey(idFonction))
      {
        return null;
      }

      var pref = signatures[idFonction];

      var confStyle = new VisuV4ConfigurationCurveStyle()
      {
        Color = System.Drawing.ColorTranslator.ToHtml(pref.Color),
        Size = pref.Width,
        IsVisible = pref.Visible,
      };

      return new ConfigurationStyle(tag, confStyle);
    }

    private ConfigurationStyle ReadFilteredDataOption(CurveTag tag)
    {
      var posName = tag.Properties[CurveTag.PositionKey];
      int posId = 0;
      if (this.posMap.ContainsKey(posName))
      {
        posId = this.posMap[posName];
      }

      if (!this.MapPrefCourbe.ContainsKey(posId))
      {
        return null;
      }

      var signatures = this.MapPrefCourbe[posId];
      int idSignature = tag.Properties[CurveTag.DetectorKey].ToInt();
      // Si colonne différente de 0 la signature est différente B_Preferences.cpp -> l.752 GD
      idSignature = (idSignature & 0x00FFFFFF) | (tag.Properties[CurveTag.ColumnKey].ToInt() << 24);
      if (!signatures.ContainsKey(idSignature))
      {
        return null;
      }

      var pref = signatures[idSignature];

      var confStyle = new VisuV4ConfigurationSuspectStyle()
      {
        Color = System.Drawing.ColorTranslator.ToHtml(pref.Color),
        Size = 5, // V3 n'utilise pas de size sur les symbol
        Symbol = GraphicOptionHelper.SymbolFromCode(pref.Symbol),
        // Si c'est un symbole plein (cercle plein, carré plein, etc...) on met symbolfilled à true,
        // les identifiants de ces symboles étant 2, 6, 7, 11, 12, 13
        SymbolFilled = pref.Symbol == 2 || pref.Symbol == 6 || pref.Symbol == 7 ||
          pref.Symbol == 11 || pref.Symbol == 12 || pref.Symbol == 13,
        ProjectionLine = GraphicOptionHelper.ProjectionLineFromCode(pref.Symbol),
        IsVisible = pref.Visible,
      };

      return new ConfigurationStyle(tag, confStyle);
    }

    private ConfigurationStyle ReadLimitOption(CurveTag tag)
    {
      var limName = tag.FunctionName;
      var detector = tag.Properties[CurveTag.DetectorKey].ToInt();
      var distance = tag.Properties[CurveTag.DistanceKey].ToDouble();
      var classe = tag.Properties[CurveTag.ClasseKey];
      var typeSignal = tag.Properties[CurveTag.TypeSignalKey].ToInt();
      var guidLimit = tag.Properties[CurveTag.GuidLimitKey];
      foreach (var item in this.mapPrefLimits)
      {
        if (item.Signature == detector && item.Distance == distance && item.Classe == classe && item.TypeSignal == typeSignal && item.GuidLimite == guidLimit)
        {
          var confStyle = new VisuV4ConfigurationCurveStyle()
          {
            Color = System.Drawing.ColorTranslator.ToHtml(item.Color),
            Size = item.Width,
            IsVisible = item.Visible,
          };

          return new ConfigurationStyle(tag, confStyle);
        }
      }

      return null;
    }

    private ConfigurationStyle ReadSubrangeOption(CurveTag tag)
    {
      var posName = tag.Properties[CurveTag.PositionKey];
      int posId = 0;
      if (this.posMap.ContainsKey(posName))
      {
        posId = this.posMap[posName];
      }

      if (!this.MapPrefCourbe.ContainsKey(posId))
      {
        return null;
      }

      var signatures = this.MapPrefCourbe[posId];
      int idFonction = tag.Properties[CurveTag.DetectorKey].ToInt();

      if (!signatures.ContainsKey(idFonction))
      {
        return null;
      }

      var pref = signatures[idFonction];

      var confStyle = new VisuV4ConfigurationCurveStyle()
      {
        Color = System.Drawing.ColorTranslator.ToHtml(pref.Color),
        Size = pref.Width,
        IsVisible = pref.Visible,
      };

      return new ConfigurationStyle(tag, confStyle);
    }
  }
}