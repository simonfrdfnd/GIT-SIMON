namespace NexioMax3.Domain.Configuration.Model
{
  using System.Collections.Generic;
  using System.Linq;

  public class SuspectTablesConfig
  {
    public Dictionary<OptionalColumn, bool> HiddenColumns { get; set; } = new Dictionary<OptionalColumn, bool>();

    public string SuspectsDefaultGroupBy { get; set; }

    public string FinalsDefaultGroupBy { get; set; }

    public List<CustomColumnHeader> Headers { get; set; } = new List<CustomColumnHeader>();

    public string GetCustomHeader(string sourceName, string colDefName)
    {
      var header = this.Headers.FirstOrDefault(columnHeader =>
                                            columnHeader.Source == sourceName && columnHeader.ColumnName == colDefName);

      if (header == null)
      {
        this.Headers.Add(header = new CustomColumnHeader()
        {
          ColumnName = colDefName,
          Source = sourceName,
          Value = null,
        });
      }

      return header.Value;
    }

    public void SetCustomHeader(string sourceName, string colDefName, string newValue)
    {
      var header = this.Headers.FirstOrDefault(columnHeader => columnHeader.Source == sourceName && columnHeader.ColumnName == colDefName);

      if (header == null)
      {
        this.Headers.Add(new CustomColumnHeader()
        {
          ColumnName = colDefName,
          Source = sourceName,
          Value = newValue,
        });
      }
      else
      {
        header.Value = newValue;
      }
    }
  }
}