using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using NationalInstruments.Visa;
using Ivi.Visa;

namespace RepeaterTCP
{
  public class SenderVISA
  {
    ResourceManager RessourceManager { get; set; }
    MessageBasedSession MbSession { get; set; }
    public SenderVISA(string strVISAsrc)
    {
      ResourceManager rmSession = new NationalInstruments.Visa.ResourceManager();
      MessageBasedSession mbSession = (MessageBasedSession)rmSession.Open(strVISAsrc, AccessModes.LoadConfig, 2000);
      mbSession.TerminationCharacter = Encoding.ASCII.GetBytes("\n")[0];
      mbSession.TerminationCharacterEnabled = true;
      ((INativeVisaSession)mbSession).SetAttributeBoolean(NativeVisaAttribute.SuppressEndEnabled, false);
    }

    public void Write(string data)
    {
      this.MbSession.RawIO.Write(data);
    }

    public string Read() 
    {
      return this.MbSession.RawIO.ReadString();
    }
  }
}
