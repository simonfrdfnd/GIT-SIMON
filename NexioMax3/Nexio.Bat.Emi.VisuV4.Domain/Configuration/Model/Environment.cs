namespace NexioMax3.Domain.Configuration.Model
{
  using System.Collections.Generic;

  public class Environment
  {
    public EnvGraphicOptions GraphicOptions { get; set; }

    public ScaleSettings ScaleSettings { get; set; }

    public List<LimitIdentifier> ExternalLimits { get; set; }
  }
}
