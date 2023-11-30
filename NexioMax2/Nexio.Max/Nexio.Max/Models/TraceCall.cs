using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexioMax.Models
{
  public class TraceCall : ICloneable
  {
    public int Number { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public string Address { get; set; }
    public bool IsEnabled { get; set; }
    public string BufferAsciiString { get; set; }
    public string BufferHexString { get; set; }
    public byte[] BufferHex { get; set; }
    public bool IsBufferExtended { get; set; }
    public string[] Commands { get; set; }
    public List<string> CallItems { get; set; }
    public Color Color { get; set; }
    public object Clone()
    {
      TraceCall clone = new TraceCall();
      clone.Number = Number;
      clone.Description = Description;
      clone.Timestamp = Timestamp;
      clone.Duration = Duration;
      clone.Address = Address;
      clone.IsEnabled = IsEnabled;
      clone.BufferAsciiString = BufferAsciiString;
      clone.BufferHexString = BufferHexString;
      clone.BufferHex = BufferHex;
      clone.IsBufferExtended = IsBufferExtended;
      clone.Commands = Commands;
      clone.CallItems = CallItems;
      clone.Color = Color;
      return clone;
    }
  }
}
