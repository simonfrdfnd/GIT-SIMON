namespace NexioMax3.Domain.Configuration.Model
{
  using Nexio.Helper;

  public class OverrideCurveDef
  {
    private bool overrideStyle;

    private bool overrideSymbol;

    private bool overrideWidth;

    private bool overrideColor;

    private int style;

    private int symbol;

    private int width;

    private string color;

    public OverrideCurveDef()
    {
    }

    public bool OverrideStyle { get => this.overrideStyle; set => this.overrideStyle = value; }

    public bool OverrideSymbol { get => this.overrideSymbol; set => this.overrideSymbol = value; }

    public bool OverrideWidth { get => this.overrideWidth; set => this.overrideWidth = value; }

    public bool OverrideColor { get => this.overrideColor; set => this.overrideColor = value; }

    public int Style { get => this.style; set => this.style = value; }

    public int Symbol { get => this.symbol; set => this.symbol = value; }

    public int Width { get => this.width; set => this.width = value; }

    public string Color { get => this.color; set => this.color = value; }

    internal static OverrideCurveDef Get(string iniFile, string section)
    {
      if (!IniFileHelper.IsSectionExists(section, iniFile))
      {
        return null;
      }

      var result = new OverrideCurveDef();

      result.OverrideStyle = IniFileHelper.GetInt(section, "OverrideStyle", 0, iniFile) != 0;
      result.Style = IniFileHelper.GetInt(section, "Style", 0, iniFile);

      // ABO INC0010801 => Add a control to check if the style defined exists
      if (result.OverrideStyle && result.Style > 4)
      {
        result.OverrideStyle = false;
      }

      // ABO INC0010801 => User can define a symbol in EMI_overrides.ini
      result.OverrideSymbol = IniFileHelper.GetInt(section, "OverrideSymbol", 0, iniFile) != 0;
      result.Symbol = IniFileHelper.GetInt(section, "Symbol", 0, iniFile);

      // ABO INC0010801 => Add a control to check if the symbol defined exists
      if (result.OverrideSymbol && (result.Symbol < 1 || result.Symbol > 14))
      {
        result.OverrideSymbol = false;
      }

      result.OverrideWidth = IniFileHelper.GetInt(section, "OverrideWidth", 0, iniFile) != 0;
      result.Width = IniFileHelper.GetInt(section, "Width", 1, iniFile);
      result.OverrideColor = IniFileHelper.GetInt(section, "OverrideColor", 0, iniFile) != 0;
      result.Color = IniFileHelper.GetString(section, "Color", "255,0,0", iniFile);

      return result;
    }
  }
}