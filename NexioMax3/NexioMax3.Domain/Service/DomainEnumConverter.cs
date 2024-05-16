namespace NexioMax3.Domain.Service
{
  using Nexio.Resx;

  public class DomainEnumConverter<Template> : ResourceEnumConverter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComEnumConverter{Template}"/> class.
    /// </summary>
    public DomainEnumConverter()
      : base(typeof(Template), Properties.Resources.ResourceManager)
    {
    }
  }
}