namespace NexioMax3.Domain
{
  using System;
  using Newtonsoft.Json;

  public class LimitIdentifier
  {
    [JsonProperty(nameof(Id))]
    public Guid Id { get; set; }

    [JsonProperty(nameof(Index))]
    public int Index { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is LimitIdentifier li)
      {
        return this.Equals(li);
      }

      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (this.Id.GetHashCode() * 397) ^ this.Index;
      }
    }

    protected bool Equals(LimitIdentifier other)
    {
      return this.Id.Equals(other.Id) && this.Index == other.Index;
    }
  }
}