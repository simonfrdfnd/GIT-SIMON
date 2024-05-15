namespace Nexio.Bat.Emi.VisuV4.Definition.ViewModel
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Windows.Media;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model.EMIOverrides2;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Validation;
  using Nexio.Wpf.Base;

  public class ExternalCurveOverrideViewModel : ValidationViewModelBase
  {
    public static readonly string ImportedFromFile = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.ImportedFromFile;
    public static readonly string OldProject = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OldProjectV3OrMax;
    public static readonly string AllProject = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All;
    public static readonly string AllPosition = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All;

    private static readonly ValidationMessage AllPositionsCovered = new ValidationMessage(ValidationLevel.Error, typeof(PointOverrideViewModel), $"{nameof(AllPositionsCovered)}Error", Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OneOrMoreStyleCoverAllPositionsForThisCurve, new List<object>(), nameof(Position), nameof(Project), nameof(Name));
    private static readonly ValidationMessage AllProjectError = new ValidationMessage(ValidationLevel.Error, typeof(ExternalCurveOverrideViewModel), nameof(AllProjectError), Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OneOrMoreStyleCoverAllProjectsForThisDetector, new List<object>(), nameof(Project));
    private static readonly ValidationMessage SameProject = new ValidationMessage(ValidationLevel.Error, typeof(ExternalCurveOverrideViewModel), nameof(SameProject), Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OneOrMoreStyleCoverHaveTheSameProjectForThisDetector, new List<object>(), nameof(project));
    private static readonly ValidationMessage AllCoveredError = new ValidationMessage(ValidationLevel.Error, typeof(ExternalCurveOverrideViewModel), nameof(AllCoveredError), Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.AtLeastOneCombinationProjectPositionCoversThisSourceSCombination, new List<object>(), nameof(Position), nameof(Project), nameof(Name));
    private static readonly ValidationMessage CombinationCoveredError = new ValidationMessage(ValidationLevel.Error, typeof(ExternalCurveOverrideViewModel), nameof(CombinationCoveredError), Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.ThisCombinationProjectNamePositionIsAlreadyDefined, new List<object>(), nameof(Position), nameof(Project), nameof(Name));
    private static readonly ValidationMessage NoNameError = new ValidationMessage(ValidationLevel.Error, typeof(ExternalCurveOverrideViewModel), nameof(NoNameError), Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.NoNameForThisCurve, new List<object>(), nameof(Name));

    private string project = AllProject;
    private Color? color = Colors.Black;
    private bool isVisible = true;
    private bool isValid;
    private bool overrideColor;
    private double width = 2;
    private bool overrideWidth;
    private bool overrideIsVisible;
    private string name;
    private string position = AllPosition;

    public string Name
    {
      get => this.name;
      set => this.Set(nameof(this.Name), ref this.name, value);
    }

    public string Project
    {
      get => this.project;
      set => this.Set(nameof(this.Project), ref this.project, value);
    }

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

    public bool OverrideIsVisible
    {
      get => this.overrideIsVisible;
      set => this.Set(nameof(this.OverrideIsVisible), ref this.overrideIsVisible, value);
    }

    public double Width
    {
      get => this.width;
      set => this.Set(nameof(this.Width), ref this.width, value);
    }

    public bool OverrideWidth
    {
      get => this.overrideWidth;
      set => this.Set(nameof(this.OverrideWidth), ref this.overrideWidth, value);
    }

    public ObservableCollection<ValidationMessage> Errors { get; } = new ObservableCollection<ValidationMessage>();

    public static CurveOverrideViewModel FromCurveDefinition(Detector detector, OverrideCurveDef def)
    {
      var colorString = def.Color;
      var color = ParseColor(colorString);

      var vm = new CurveOverrideViewModel()
      {
        Color = color,
        Detector = detector,
        IsVisible = true,
        Position = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All, // PositionViewModel.All,
        OverrideIsVisible = false,
        OverrideWidth = def.OverrideWidth,
        OverrideColor = def.OverrideColor,
        Width = def.Width,
        IsValid = true,
      };

      return vm;
    }

    public static ExternalCurveOverrideViewModel FromData(EmiOverrideExternalCurve curve)
    {
      var colorString = curve.Color;
      var color = ParseColor(colorString);

      var vm = new ExternalCurveOverrideViewModel()
      {
        Color = color,
        Name = curve.Name,
        IsVisible = curve.IsVisible,
        Project = curve.Project,
        OverrideIsVisible = curve.OverrideIsVisible,
        OverrideWidth = curve.OverrideSize,
        OverrideColor = curve.OverrideColor,
        Width = curve.Size,
        Position = curve.Position,
        IsValid = true,
      };

      return vm;
    }

    public EmiOverrideExternalCurve ToData()
    {
      return new EmiOverrideExternalCurve()
      {
        Project = this.Project,
        OverrideColor = this.OverrideColor,
        OverrideIsVisible = this.OverrideIsVisible,
        IsVisible = this.IsVisible,
        OverrideSize = this.OverrideWidth,
        Size = this.Width,
        Color = this.Color == null ? "0,0,0" : $"{this.Color.Value.R},{this.Color.Value.G},{this.Color.Value.B}",
        Name = this.Name,
        Position = this.Position,
      };
    }

    public void UpdateValidity(ICollection<ExternalCurveOverrideViewModel> models)
    {
      this.Errors.Clear();

      var validationMessages = new List<ValidationMessage>();

      foreach (var curveOverrideViewModel in models)
      {
        if (curveOverrideViewModel == this)
        {
          continue;
        }

        validationMessages.AddRange(AreCompatibles(this, curveOverrideViewModel));
      }

      this.IsValid = !validationMessages.Any();

      validationMessages = validationMessages.Distinct(ValidationMessageComparer.Instance).ToList();
      validationMessages.ForEach(message => this.Errors.Add(message));
    }

    private static IEnumerable<ValidationMessage> AreCompatibles(ExternalCurveOverrideViewModel curve1, ExternalCurveOverrideViewModel curve2)
    {
      var validationMessages = new List<ValidationMessage>();
      if (curve1.Name != curve2.Name && !(string.IsNullOrWhiteSpace(curve1.Name) && string.IsNullOrWhiteSpace(curve1.Name)))
      {
        return validationMessages;
      }

      var isC1AllPosition = curve1.Position == AllPosition;
      var isC2AllPosition = curve2.Position == AllPosition;
      var isC1AllProject = curve1.Project == AllProject;
      var isC2AllProject = curve2.Project == AllProject;
      var isSamePosition = curve1.Position == curve2.Position;
      var isSameProject = curve1.Project == curve2.Project;

      if ((isC1AllProject || isC2AllProject) && (isC1AllPosition || isC2AllPosition))
      {
        validationMessages.Add(AllCoveredError);
      }

      if (isSameProject || isC1AllProject || isC2AllProject)
      {
        if (isSamePosition)
        {
          validationMessages.Add(CombinationCoveredError);
        }

        if (isC1AllPosition || isC2AllPosition)
        {
          validationMessages.Add(AllPositionsCovered);
        }
      }

      if (isSamePosition || isC1AllPosition || isC2AllPosition)
      {
        if (isSameProject)
        {
          validationMessages.Add(SameProject);
        }

        if (isC1AllProject || isC2AllProject)
        {
          validationMessages.Add(AllProjectError);
        }
      }

      return validationMessages;
    }

    private static Color ParseColor(string colorString)
    {
      var color = Colors.Black;

      if (colorString.Count(c => c == ',') == 2)
      {
        var split = colorString.Split(',');

        if (byte.TryParse(split[0], out var r) && byte.TryParse(split[1], out var g) && byte.TryParse(split[2], out var b))
        {
          color = System.Windows.Media.Color.FromRgb(r, g, b);
        }
      }

      return color;
    }
  }
}