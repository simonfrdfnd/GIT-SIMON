namespace NexioMax3.Domain.Model
{
  public class Final : FilteredData
  {
    public override FilteredDataType DataType { get; } = FilteredDataType.Final;
  }
}