namespace NexioMax3.Domain.Model
{
  public class Suspect : FilteredData
  {
    public override FilteredDataType DataType { get; } = FilteredDataType.Suspect;
  }
}