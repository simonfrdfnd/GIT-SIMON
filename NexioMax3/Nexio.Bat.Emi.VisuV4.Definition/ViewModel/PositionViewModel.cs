namespace Nexio.Bat.Emi.VisuV4.Definition.ViewModel
{
  using Nexio.Wpf.Base;

  public class PositionViewModel : ViewModelBase
  {
    public static readonly PositionViewModel All = new PositionViewModel() { Id = -1, Name = Nexio.Bat.Emi.VisuV4.Definition.Properties.Resources.All };
    private int id;
    private string name;

    public int Id
    {
      get => this.id;
      set => this.Set(nameof(this.Id), ref this.id, value);
    }

    public string Name
    {
      get => this.name;
      set => this.Set(nameof(this.Name), ref this.name, value);
    }

    public override bool Equals(object obj)
    {
      return obj is PositionViewModel p && p.id == this.id;
    }
  }
}