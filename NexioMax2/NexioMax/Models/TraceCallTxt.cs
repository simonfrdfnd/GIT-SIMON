using System;
using System.Collections.Generic;
using System.Drawing;
using NexioMax.Models;

namespace NexioMax.Models
{
  public class TraceCallTxt : TraceCall
  {
    public TraceCallTxt(List<string> callItems)
    {
      this.CallItems = callItems;
    }
  }
}
