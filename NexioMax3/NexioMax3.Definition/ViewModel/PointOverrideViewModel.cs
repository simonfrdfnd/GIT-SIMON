namespace NexioMax3.Definition.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Media;
  using NexioMax3.Domain.Configuration.Model;
  using NexioMax3.Domain.Configuration.Model.EMIOverrides2;
  using NexioMax3.Domain.Model;
  using Nexio.Validation;
  using Nexio.Wpf.Base;
  using Nexio;

  public class PointOverrideViewModel : ViewModelBase
  {
    private static readonly ValidationMessage AllPositionsCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(AllPositionsCovered)}Error", NexioMax3.Definition.Properties.Resources.OneOrMoreStyleCoverAllPositionsForThisSource, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private static readonly ValidationMessage AllColumnsCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(AllColumnsCovered)}Error", NexioMax3.Definition.Properties.Resources.OneOrMoreStyleCoverAllColumnsForThisSource, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private static readonly ValidationMessage SameCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(SameCovered)}Error", NexioMax3.Definition.Properties.Resources.YourCombinationPositionSourceIsAlreadyCovered, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private static readonly ValidationMessage SameColumnCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(SameColumnCovered)}Error", NexioMax3.Definition.Properties.Resources.YourCombinationColumnSourceIsAlreadyCovered, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private static readonly ValidationMessage AllCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(AllCovered)}Error", NexioMax3.Definition.Properties.Resources.AtLeastOneCombinationColumnPositionCoversThisSourceSCombination, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private static readonly ValidationMessage CombinationCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(CombinationCovered)}Error", NexioMax3.Definition.Properties.Resources.ThisCombinationSourceColumnPositionIsAlreadyDefined, new List<object>(), nameof(Position), nameof(SourceName), nameof(Column));

    private string position = NexioMax3.Definition.Properties.Resources.All;

    private Color? color = Colors.Black;

    private bool isVisible = true;

    private bool isValid;

    private double size = 2;

    private int symbolId = 1;

    private bool overrideColor;

    private bool overrideIsVisible;

    private bool overrideSymbol;

    private bool overrideSize;

    private string sourceName = "Peak";

    private string column = NexioMax3.Definition.Properties.Resources.All;

    private bool useProjection;

    private bool overrideUseProjection;

    private bool symbolFilled = false;

    private bool? isStyleApplied = false;

    /// <summary>
    /// Sert a stocké le dictionnaire des sources et colonnes afin de pouvoir afficher les colonnes correspondant à la source
    /// </summary>
    public Dictionary<string, DatasCache> SourceDic { get; set; } = new Dictionary<string, DatasCache>();

    public string Position
    {
      get => this.position;
      set => this.Set(nameof(this.Position), ref this.position, value);
    }

    public Color? Color
    {
      get => this.color;
      set => this.Set(nameof(this.Color), ref this.color, value);
    }

    public bool OverrideColor
    {
      get => this.overrideColor;
      set => this.Set(nameof(this.OverrideColor), ref this.overrideColor, value);
    }

    public bool IsVisible
    {
      get => this.isVisible;
      set => this.Set(nameof(this.IsVisible), ref this.isVisible, value);
    }

    public bool IsValid
    {
      get => this.isValid;
      set => this.Set(nameof(this.IsValid), ref this.isValid, value);
    }

    public double Size
    {
      get => this.size;
      set => this.Set(nameof(this.Size), ref this.size, value);
    }

    public int SymbolId
    {
      get => this.symbolId;
      set
      {
        this.Set(nameof(this.SymbolId), ref this.symbolId, value);
        this.SymbolFilled = value == 2 || value == 6 || value == 7 || value == 15; // Correspond aux ids des symboles pleins
      }
    }

    public bool SymbolFilled
    {
      get => this.symbolFilled;
      set => this.Set(nameof(this.SymbolFilled), ref this.symbolFilled, value);
    }

    public bool UseProjection
    {
      get => this.useProjection;
      set => this.Set(nameof(this.UseProjection), ref this.useProjection, value);
    }

    public bool OverrideSymbol
    {
      get => this.overrideSymbol;
      set => this.Set(nameof(this.OverrideSymbol), ref this.overrideSymbol, value);
    }

    public bool OverrideSize
    {
      get => this.overrideSize;
      set => this.Set(nameof(this.OverrideSize), ref this.overrideSize, value);
    }

    public bool OverrideIsVisible
    {
      get => this.overrideIsVisible;
      set => this.Set(nameof(this.OverrideIsVisible), ref this.overrideIsVisible, value);
    }

    public bool OverrideUseProjection
    {
      get => this.overrideUseProjection;
      set => this.Set(nameof(this.OverrideUseProjection), ref this.overrideUseProjection, value);
    }

    public bool? IsStyleApplied
    {
      get
      {
        return this.isStyleApplied;
      }

      set
      {
        if (this.isStyleApplied == value)
        {
          return;
        }

        if (value == null && this.isStyleApplied.HasValue)
        {
          this.IsStyleApplied = !this.IsStyleApplied.Value;

          return;
        }

        this.Set(nameof(this.IsStyleApplied), ref this.isStyleApplied, value);
        this.SelecteAllStyles();
      }
    }

    public string SourceName
    {
      get
      {
        return this.sourceName;
      }

      set
      {
        if (this.Set(nameof(this.SourceName), ref this.sourceName, value))
        {
          this.RaisePropertyChanged(nameof(this.Columns));
        }
      }
    }

    public ObservableCollection<string> Columns => this.SourceDic.ContainsKey(this.SourceName) ? new ObservableCollection<string>(this.SourceDic[this.SourceName].Columns) : new ObservableCollection<string>();

    public string Column
    {
      get => this.column;
      set => this.Set(nameof(this.Column), ref this.column, value);
    }

    public ObservableCollection<ValidationMessage> Errors { get; } = new ObservableCollection<ValidationMessage>();

    public static PointOverrideViewModel FromData(EmiOverridePoint point, Dictionary<string, DatasCache> sources, IEnumerable<string> positions)
    {
      var color = ParseColor(point.Color);

      if (!positions.Any(p => p == point.Position))
      {
        1.ToNullable();
      }

      var vm = new PointOverrideViewModel()
      {
        SourceName = point.Source,
        SymbolId = point.SymbolId,
        SymbolFilled = point.SymbolFilled,
        Color = color,
        Column = point.Column,
        IsVisible = point.IsVisible,
        Position = positions.FirstOrDefault(model => model == point.Position) ?? string.Empty,
        OverrideSymbol = point.OverrideSymbol,
        OverrideIsVisible = point.OverrideIsVisible,
        OverrideSize = point.OverrideSize,
        OverrideColor = point.OverrideColor,
        OverrideUseProjection = point.OverrideUseProjection,
        UseProjection = point.UseProjection,
        Size = point.Size,
        IsValid = true,
        SourceDic = sources,
      };

      // Initialisation du parametre privé (pas touche au param public sinon ça va cocher tous les autres verrous)
      vm.PropertyChanged += vm.PointOverrideViewModel_PropertyChanged;
      vm.InitAllStyles();

      // Ajout des colonnes utilisé avant la refonte de la sauvegarde des données afin de les lier à la source
      SourcesCache.Append(point.Source, point.Column);

      return vm;
    }

    public static PointOverrideViewModel FromPointDefinition(Detector detector, string position, string source, OverrideCurveDef def)
    {
      return new PointOverrideViewModel()
      {
        SourceName = source,
        Position = position,
        Color = ParseColor(def.Color),
        IsValid = true,
        IsVisible = true,
        OverrideColor = def.OverrideColor,
        SymbolId = def.Symbol,
        SymbolFilled = def.Symbol == 2 || def.Symbol == 6 || def.Symbol == 7 || def.Symbol == 15,
        Size = def.Width,
        OverrideSize = def.OverrideWidth,
        OverrideSymbol = def.OverrideSymbol,
        OverrideIsVisible = false,
        OverrideUseProjection = false,
      };
    }

    public void UpdateValidity(ICollection<PointOverrideViewModel> models)
    {
      this.Errors.Clear();
      var validationMessages = new List<ValidationMessage>();
      foreach (var pointOverrideViewModel in models)
      {
        if (pointOverrideViewModel == this)
        {
          continue;
        }

        validationMessages.AddRange(AreCompatibles(this, pointOverrideViewModel));
      }

      this.IsValid = !validationMessages.Any();
      validationMessages = validationMessages.Distinct(ValidationMessageComparer.Instance).ToList();
      validationMessages.ForEach(message => this.Errors.Add(message));
    }

    public EmiOverridePoint ToData()
    {
      return new EmiOverridePoint()
      {
        Source = this.SourceName,
        OverrideIsVisible = this.OverrideIsVisible,
        OverrideColor = this.OverrideColor,
        OverrideSize = this.OverrideSize,
        OverrideSymbol = this.OverrideSymbol,
        OverrideUseProjection = this.OverrideUseProjection,
        UseProjection = this.UseProjection,
        Column = this.Column,
        IsVisible = this.IsVisible,
        Position = this.Position,
        Size = this.Size,
        SymbolId = this.SymbolId,
        SymbolFilled = this.SymbolFilled,
        Color = this.Color == null ? "0,0,0" : $"{this.Color.Value.R},{this.Color.Value.G},{this.Color.Value.B}",
      };
    }

    private static Color ParseColor(string colorString)
    {
      var color = Colors.Black;

      if (colorString.Count(c => c == ',') == 2)
      {
        var split = colorString.Split(',');

        if (byte.TryParse(split[0], out var r) && byte.TryParse(split[1], out var g) &&
            byte.TryParse(split[2], out var b))
        {
          color = System.Windows.Media.Color.FromRgb(r, g, b);
        }
      }

      return color;
    }

    private static IEnumerable<ValidationMessage> AreCompatibles(PointOverrideViewModel point1, PointOverrideViewModel point2)
    {
      var validationMessages = new List<ValidationMessage>();

      if (point1.SourceName != point2.SourceName)
      {
        return validationMessages;
      }

      var isPXAllPosition = Equals(point1.Position, NexioMax3.Definition.Properties.Resources.All) || Equals(point2.Position, NexioMax3.Definition.Properties.Resources.All);
      var isPXAllColumns = Equals(point1.Column, NexioMax3.Definition.Properties.Resources.All) || Equals(point2.Column, NexioMax3.Definition.Properties.Resources.All);
      var isSamePosition = (string.IsNullOrWhiteSpace(point1.position) && string.IsNullOrWhiteSpace(point2.Position)) || point1.Position.Equals(point2.Position);
      var isSameColumn = point1.Column.Equals(point2.Column);

      if ((isPXAllColumns && isPXAllPosition) ||
        (isPXAllColumns && isSamePosition) ||
        (isPXAllPosition && isSameColumn) ||
        (isSameColumn && isSamePosition))
      {
        validationMessages.Add(AllCovered);
      }

      return validationMessages;
    }

    private void InitAllStyles()
    {
      var properties = new bool[] { this.OverrideSymbol, this.OverrideIsVisible, this.OverrideSize, this.OverrideColor, this.OverrideUseProjection };
      bool same = properties.Distinct().Count() < 2;

      if (this.IsStyleApplied.HasValue && !same)
      {
        this.isStyleApplied = null;
        this.RaisePropertyChanged(nameof(this.IsStyleApplied));
      }
      else if (same)
      {
        this.IsStyleApplied = properties.FirstOrDefault();
      }
    }

    private void SelecteAllStyles()
    {
      if (this.isStyleApplied.HasValue)
      {
        var allstyles = this.isStyleApplied.Value;
        this.OverrideSymbol = allstyles;
        this.OverrideIsVisible = allstyles;
        this.OverrideSize = allstyles;
        this.OverrideColor = allstyles;
        this.OverrideUseProjection = allstyles;
      }
    }

    private void PointOverrideViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var properties = new[] { nameof(this.OverrideSymbol), nameof(this.OverrideIsVisible), nameof(this.OverrideSize), nameof(this.OverrideColor), nameof(this.OverrideUseProjection) };
      if (properties.Any(p => e.PropertyName == p))
      {
        this.InitAllStyles();
      }
    }
  }
}