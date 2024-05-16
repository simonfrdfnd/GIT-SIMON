﻿namespace NexioMax3.Domain.Configuration.Model
{
  using global::NexioMax3.Domain.Model;
  using Newtonsoft.Json;

  public class EnvCurveConfigurationStyle
  {
    [JsonProperty(PropertyName = "Tag")]
    public CurveTag Tag { get; set; }

    [JsonProperty(PropertyName = "Style")]
    public VisuV4ConfigurationCurveStyle Style { get; set; }

    public static EnvCurveConfigurationStyle From(ConfigurationStyle style)
    {
      if (style.Style is VisuV4ConfigurationCurveStyle s)
      {
        return new EnvCurveConfigurationStyle()
               {
                 Tag = style.Tag,
                 Style = s,
               };
      }

      return null;
    }
  }
}