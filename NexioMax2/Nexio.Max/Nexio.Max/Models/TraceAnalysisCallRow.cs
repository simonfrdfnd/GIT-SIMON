using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexioMax.Models
{
  public class TraceAnalysisCallRow : CallRow
  {
    public TraceAnalysisCallRow(int number, string description, DateTime timestamp, double timestampPercent, TimeSpan duration, double durationPercent, string address, bool enabled)
    {
      this.Number = number;
      this.Description = description;
      this.Address = address;
      this.Enabled = enabled;
      this.Timestamp = timestamp.TimeOfDay.ToString();
      this.TimestampPercent = timestampPercent;
      this.Duration = duration.ToString();
      this.DurationPercent = durationPercent;
    }

    public string Timestamp { get; set; }
    public double TimestampPercent { get; set; }
    public string Duration { get; set; }
    public double DurationPercent { get; set; }
  }
}
