using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexioMax.Models
{
  public abstract class CallRow
  {
    public int Number { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public bool Enabled { get; set; }
  }
}
