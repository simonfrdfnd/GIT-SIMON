using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RepeaterTCP
{
  public class Repeater
  {
    public Listener Listener { get; set; }

    IPEndPoint IpRepeater { get; }
    public IPEndPoint IpRecipient { get; }

    public Repeater(IPEndPoint ipRepeater, IPEndPoint ipRecipient) 
    {
      this.IpRepeater = ipRepeater;
      this.IpRecipient = ipRecipient;
      this.Listener = new Listener(ipRepeater);
      this.Listener.ListenerRun(4096, new System.Threading.CancellationToken());
    }
  }
}
