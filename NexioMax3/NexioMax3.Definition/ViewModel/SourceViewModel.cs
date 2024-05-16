namespace NexioMax3.Definition.ViewModel
{
  using Nexio.Wpf.Base;

  public class SourceViewModel : ViewModelBase
  {
    public static readonly SourceViewModel All = new SourceViewModel(NexioMax3.Definition.Properties.Resources.All);
    private string displayName;

    public SourceViewModel()
    {
    }

    public SourceViewModel(string sourceName)
    {
      this.DisplayName = this.SourceName = sourceName;
    }

    public string SourceName { get; set; }

    public string DisplayName
    {
      get => this.displayName ?? NexioMax3.Definition.Properties.Resources.Other;
      set => this.Set(nameof(this.DisplayName), ref this.displayName, value);
    }
  }
}