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

  public class CurveOverrideViewModel : ValidationViewModelBase
  {
    private static readonly ValidationMessage AllPositionError = new ValidationMessage(ValidationLevel.Error, typeof(CurveOverrideViewModel), "IncompatiblePositionAll", Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OneOrMoreStyleCoverAllPositionsForThisDetector, new List<object>(), nameof(Position));

    private static readonly ValidationMessage SamePositionError = new ValidationMessage(ValidationLevel.Error, typeof(CurveOverrideViewModel), "IncompatiblePositionSame", Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.OneOrMoreStyleCoverHaveTheSamePositionForThisDetector, new List<object>(), nameof(Position));

    private Detector detector = Detector.AVERAGE;
    private string position = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All;
    private Color? color = Colors.Black;
    private bool isVisible = true;
    private bool isValid;
    private bool overrideColor;
    private double width = 2;
    private bool overrideWidth;
    private bool overrideIsVisible;
    private bool isLimit;

    public Detector Detector
    {
      get => this.detector;
      set => this.Set(nameof(this.Detector), ref this.detector, value);
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

    public bool IsLimit
    {
      get => this.isLimit;
      set => this.Set(nameof(this.IsLimit), ref this.isLimit, value);
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
        Position = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All,
        OverrideIsVisible = false,
        OverrideWidth = def.OverrideWidth,
        OverrideColor = def.OverrideColor,
        Width = def.Width,
        IsValid = true,
      };

      return vm;
    }

    public static CurveOverrideViewModel FromData(EmiOverrideCurve curve, IEnumerable<string> positions)
    {
      var colorString = curve.Color;
      var color = ParseColor(colorString);

      var vm = new CurveOverrideViewModel()
      {
        Color = color,
        Detector = curve.Detector,
        IsVisible = curve.IsVisible,
        Position = positions.FirstOrDefault(model => model == curve.Position),
        OverrideIsVisible = curve.OverrideIsVisible,
        OverrideWidth = curve.OverrideSize,
        OverrideColor = curve.OverrideColor,
        Width = curve.Size,
        IsValid = true,
      };

      return vm;
    }

    public EmiOverrideCurve ToData()
    {
      return new EmiOverrideCurve()
      {
        Position = this.Position,
        OverrideColor = this.OverrideColor,
        OverrideIsVisible = this.OverrideIsVisible,
        IsVisible = this.IsVisible,
        OverrideSize = this.OverrideWidth,
        Size = this.Width,
        Color = this.Color == null ? "0,0,0" : $"{this.Color.Value.R},{this.Color.Value.G},{this.Color.Value.B}",
        Detector = this.Detector,
      };
    }

    public void UpdateValidity(ICollection<CurveOverrideViewModel> models)
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

    private static IEnumerable<ValidationMessage> AreCompatibles(CurveOverrideViewModel curve1, CurveOverrideViewModel curve2)
    {
      var validationMessages = new List<ValidationMessage>();
      if (curve1.Detector != curve2.Detector)
      {
        // Les détecteurs sont différents donc les courbes sont compatibles
        return validationMessages;
      }

      if (curve1.IsLimit && curve2.IsLimit)
      {
        return validationMessages;
      }

      if (Equals(curve1.Position, Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All) || Equals(curve2.Position, Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All))
      {
        // si une des position est All alors l'autre est forcément incompatible
        validationMessages.Add(AllPositionError);
      }

      if (Equals(curve1.Position, curve2.Position))
      {
        validationMessages.Add(SamePositionError);

        // Si ce sont les mêmes positions les courbes sont incompatibles : à ce point, les détecteurs sont les mêmes et les courbes ont la même position
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