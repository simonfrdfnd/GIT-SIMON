using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NexioMax.Helpers;

namespace NexioMax.Models
{
  public class TraceCallXml : TraceCall
  {
    public TraceCallXml(XNode node, int number)
    {
      XElement elt = (XElement)node;
      IEnumerable<XAttribute> attributes = elt.Attributes();
      Dictionary<string, string> attributesDict = new Dictionary<string, string>();
      foreach (XAttribute attribute in attributes)
        attributesDict.Add(attribute.Name.ToString(), attribute.Value.ToString());

      IEnumerable<XNode> nodes = elt.Nodes();
      Status status = new Status(nodes.ElementAt(0));
      if (status.Attributes["type"] == "success")
        this.Color = Color.Black;
      else
        this.Color = Color.Red;

      IO input = new IO(nodes.ElementAt(1));
      IO output = new IO(nodes.ElementAt(2));
      this.Number = number;
      this.CallItems = new List<string>();
      if (attributesDict["type"].Contains("488.2")) //cas du GPIB
      {
        if (attributesDict["description"] == "Envoyer" || (attributesDict["description"] == "Send"))
        {
          this.Description = $"{attributesDict["description"]}({input.Parameters["IndxCtr "]}, {input.Parameters["adresse "]}, " +
            $"'{ConvertHex(input.Buffer.Value)}', {input.Parameters["compte "]}, {input.Parameters["eotmode "]})";
          this.Address = input.Parameters["adresse "];
          this.BufferHexString = input.Buffer.Value;
          this.BufferAsciiString = ConvertHex(this.BufferHexString);
        }
        else if ((attributesDict["description"] == "Receive"))
        {
          this.Description = $"{attributesDict["description"]}({input.Parameters["IndxCtr "]}, {input.Parameters["adresse "]}, " +
            $"'{ConvertHex(output.Buffer.Value)}', {input.Parameters["compte "]}, {input.Parameters["terminaison "]})";
          this.Address = input.Parameters["adresse "];
          this.BufferHexString = output.Buffer.Value;
          this.BufferAsciiString = ConvertHex(BufferHexString);
        }
        else
        {
          this.Description = $"{attributesDict["description"]}";
          this.Address = "";
        }
      }
      else if (attributesDict["type"] == "NI-VISA")
      {
        if (attributesDict["description"] == "viWrite")
        {
          this.Description = $"{attributesDict["description"]} ({input.Parameters["vi"]}, " +
            $"'{ConvertHex(input.Buffer.Value)}', {input.Parameters["cnt"]}, {output.Parameters["*retCnt"]})";
          this.Address = input.Parameters["vi"].Substring(0, input.Parameters["vi"].IndexOf('('));
          this.BufferAsciiString = ConvertHex(input.Buffer.Value);
          this.BufferHexString = input.Buffer.Value;
        }
        else if (attributesDict["description"] == "viRead")
        {
          this.Description = $"{attributesDict["description"]} ({input.Parameters["vi"]}, " +
            $"'{ConvertHex(output.Buffer.Value)}', {input.Parameters["cnt"]}, {output.Parameters["*retCnt"]})";
          this.Address = input.Parameters["vi"].Substring(0, input.Parameters["vi"].IndexOf('('));
          this.BufferAsciiString = ConvertHex(output.Buffer.Value);
          this.BufferHexString = output.Buffer.Value;
        }
        else
        {
          this.Description = $"{attributesDict["description"]}";
          this.Address = "";
          this.BufferAsciiString = "";
        }
      }
      else
        throw new Exception("Call Type not handled.");

      string result = Regex.Replace(this.Description, @"\r\n?|\n", ".");
      this.CallItems.Insert(0, $"{this.Number}. {result}");
      this.CallItems.Insert(1, $"ID de processus : {attributesDict["process"]}         ID de thread : {attributesDict["thread"]}");
      this.CallItems.Insert(2, $"Temps de début: {this.Timestamp}      Durée de l'appel {this.Duration}");
      this.CallItems.Insert(3, $"État : {status.Attributes["code"]} ({status.Attributes["name"]})");

      if (input.Buffer.Attributes.ContainsKey("splitIndex") || output.Buffer.Attributes.ContainsKey("splitIndex"))
        this.CallItems.Insert(4, $"Contenu du buffer (abrégé)");
      else if (input.Buffer.Attributes.ContainsKey("size") || output.Buffer.Attributes.ContainsKey("size"))
      {
        this.CallItems.Insert(4, $"Contenu du buffer");
        this.CallItems.Insert(5, $"{input.Buffer.Value}{output.Buffer.Value}");
      }

      this.Timestamp = DateTime.ParseExact(input.Attributes["time"].Replace(',', '.'), "HH:mm:ss.ffff", CultureInfo.InvariantCulture);
      DateTime dateTimeInput = DateTime.ParseExact(input.Attributes["time"].Replace('.', ','), "HH:mm:ss,ffff", CultureInfo.InvariantCulture);
      DateTime dateTimeOutput = DateTime.ParseExact(output.Attributes["time"].Replace('.', ','), "HH:mm:ss,ffff", CultureInfo.InvariantCulture);
      this.Duration = dateTimeOutput - dateTimeInput;
      this.Commands = FindCommands();
      this.BufferHex = FindBufferHex();
      this.IsEnabled = true;
      this.IsBufferExtended = true;
      this.Address = this.Address.Trim(' ');
    }

    private static string ConvertHex(string hexString)
    {
      try
      {
        if (hexString == null)
          return string.Empty;
        
        hexString = hexString.Replace(" ", "");
        string ascii = string.Empty;
        for (int i = 0; i < hexString.Length; i += 2)
        {
          string hs = string.Empty;
          hs = hexString.Substring(i, 2);
          uint decval = System.Convert.ToUInt32(hs, 16);
          char character = System.Convert.ToChar(decval);
          ascii += character;
        }

        return ascii;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }

      return string.Empty;
    }

    private Color FindColor(List<string> callLines)
    {
      if (callLines[3].Contains("ERROR"))
        return Color.Red;
      else
        return Color.Black;
    }

    private string[] FindCommands()
    {
      if (TraceHelper.IsCallWrite(this))
        return this.BufferAsciiString.Split(';');
      else
        return null;
    }

    private string FindAddressTxt(List<string> callLines)
    {
      string line = callLines[0];
      string address = "";
      if (line.Contains("viOpenDefaultRM"))
        return address;
      else if (line.Contains("viOpen"))
      {
        address = line.Substring(line.IndexOf("\"") + 1);
        address = address.Substring(0, address.IndexOf("\""));
        return address;
      }
      else if (line.Contains("viSetAttribute"))
      {
        address = line.Substring(line.IndexOf("(") + 1);
        address = address.Substring(0, address.IndexOf("(") - 1);
        return address;
      }
      else if (line.Contains("viWrite"))
      {
        address = line.Substring(line.IndexOf("(") + 1);
        address = address.Substring(0, address.IndexOf("(") - 1);
        return address;
      }
      else if (line.Contains("viRead"))
      {
        address = line.Substring(line.IndexOf("(") + 1);
        address = address.Substring(0, address.IndexOf("(") - 1);
        return address;
      }
      else if (line.Contains("Envoyer"))
      {
        string card;
        string addr;
        address = line.Substring(line.IndexOf("Envoyer") + 8);
        address = address.Substring(0, address.IndexOf("\""));
        card = address.Substring(0, address.IndexOf(","));
        addr = address.Substring(address.IndexOf(",") + 4);
        addr = addr.Remove(addr.Length - 2, 2);
        int value = Convert.ToInt32(addr, 16);
        return "GPIB" + card + "::" + value.ToString() + (" IEEE");
      }
      else if (line.Contains("Send") && !line.Contains("SendIFC"))
      {
        string card;
        string addr;
        address = line.Substring(line.IndexOf("Send") + 8);
        address = address.Substring(0, address.IndexOf("\""));
        card = address.Substring(0, address.IndexOf(","));
        addr = address.Substring(address.IndexOf(",") + 4);
        addr = addr.Remove(addr.Length - 2, 2);
        int value = Convert.ToInt32(addr, 16);
        return "GPIB" + card + "::" + value.ToString() + (" IEEE");
      }
      else if (line.Contains("Receive"))
      {
        string card;
        string addr;
        address = line.Substring(line.IndexOf("Receive") + 8);
        address = address.Substring(0, address.IndexOf("\""));
        card = address.Substring(0, address.IndexOf(","));
        addr = address.Substring(address.IndexOf(",") + 4);
        addr = addr.Remove(addr.Length - 2, 2);
        int value = Convert.ToInt32(addr, 16);
        return "GPIB" + card + "::" + value.ToString() + (" IEEE");
      }
      else
        return "";
    }

    private string FindBufferTxt(List<string> callLines)
    {
      string line;
      string cmd = "";
      string temp;
      int bufferSize = 0;
      if (callLines.Count >= 5)
      {
        if (callLines[0].Contains("viRead") || callLines[0].Contains("viWrite"))
        {
          //Dans le cas ou l'équipement n'est pas connecté, le retCnt est différent du Cnt
          temp = callLines[0].Substring(0, callLines[0].LastIndexOf(","));
          temp = temp.Substring(temp.LastIndexOf(",") + 2);
          temp = temp.Substring(0, temp.IndexOf('(') - 1);
          bufferSize = Convert.ToInt32(temp);
        }
        else if (callLines[0].Contains("Receive") || callLines[0].Contains("Envoyer"))
        {
          temp = callLines[0].Substring(0, callLines[0].LastIndexOf(','));
          temp = temp.Substring(temp.LastIndexOf(',') + 2);
          temp = temp.Substring(0, temp.IndexOf('(') - 1);
          bufferSize = Convert.ToInt32(temp);
        }
      }

      int bReste = bufferSize;
      for (int i = 5; i < callLines.Count; i++)
      {
        line = callLines[i];
        if (bReste > 16)
        {
          cmd += line.Substring(line.IndexOf(": ") + 2, 3 * 16); //on lit un octet + 1 espace, 16 fois
          bReste -= 16;
        }
        else
        {
          cmd += " ";
          cmd += line.Substring(line.IndexOf(": ") + 3, 3 * bReste);
          bReste = 0;
        }
      }
      if (cmd.Length > 0)
      {
        cmd = cmd.Remove(cmd.Length - 1, 1);
        if (cmd.ElementAt(0) == ' ')
          cmd = cmd.Remove(0, 1);
      }
      return cmd;
    }

    private string FindCommandTxt(List<string> callLines)
    {
      string line;
      string cmd = "";
      for (int i = 5; i < callLines.Count; i++)
      {
        line = callLines[i];
        cmd += line.Substring(60);
      }
      if (cmd.Length > 0)
        cmd = cmd.Remove(cmd.Length - 1, 1);
      return cmd;
    }

    private byte[]? FindBufferHex()
    {
      if (String.IsNullOrEmpty(this.BufferHexString))
      {
        this.IsBufferExtended = false;
        return null;
      }

      try
      {
        string s = String.Concat(this.BufferHexString.Where(c => !Char.IsWhiteSpace(c)));
        if (String.IsNullOrEmpty(s))
          return null;
        return Enumerable.Range(0, s.Length / 2).Select(x => Convert.ToByte(s.Substring(x * 2, 2), 16)).ToArray();
      }
      catch (ArgumentException ex)
      {
        this.IsBufferExtended = false;
        return null;
      }
    }

    private List<string> CleanLinesTxt(List<string> callLines)
    {
      List<string> lines = new List<string>();
      foreach (string line in callLines)
      {
        if (line.ElementAt(0) == '>') //une fois j'ai eu un call qui commençait par "> 999". c'est pour gérer ce cas
          lines.Add(line.Substring(2, line.Length - 2));
        else
          lines.Add(line);
      }
      return lines;
    }
  }

  public class Status
  {
    public Dictionary<string, string> Attributes { get; set; }

    public string Value { get; set; }

    public Status(XNode node)
    {
      XElement elt = (XElement)node;
      this.Value = elt.Value.ToString();
      IEnumerable<XAttribute> attributes = elt.Attributes();
      this.Attributes = new Dictionary<string, string>();
      foreach (XAttribute attribute in attributes)
      {
        this.Attributes.Add(attribute.Name.ToString(), attribute.Value.ToString());
      }
    }
  }

  public class IO
  {
    public Dictionary<string, string> Attributes { get; set; }

    public Dictionary<string, string> Parameters { get; set; }

    public Buffer Buffer { get; set; }

    public IO(XNode node)
    {
      XElement elt = (XElement)node;
      IEnumerable<XAttribute> attributes = elt.Attributes();
      this.Attributes = new Dictionary<string, string>();
      this.Parameters = new Dictionary<string, string>();
      foreach (XAttribute attribute in attributes)
      {
        this.Attributes.Add(attribute.Name.ToString(), attribute.Value.ToString());
      }
      IEnumerable<XNode> nodes = elt.Nodes();
      foreach (XNode parameter in nodes)
      {
        elt = (XElement)parameter;
        if (elt.Name == "parameter")
        {
          attributes = elt.Attributes();
          this.Parameters.Add(attributes.ElementAt(0).Value.ToString(), elt.Value.ToString());
        }
        else
        {
          this.Buffer = new Buffer(elt);
        }
      }
      if (this.Buffer == null)
      {
        this.Buffer = new Buffer();
      }
    }
  }

  public class Buffer
  {
    public Dictionary<string, string> Attributes { get; set; }

    public string Value { get; set; }

    public Buffer(XElement elt)
    {
      IEnumerable<XAttribute> attributes = elt.Attributes();
      this.Attributes = new Dictionary<string, string>();
      foreach (XAttribute attribute in attributes)
      {
        this.Attributes.Add(attribute.Name.ToString(), attribute.Value.ToString());
      }
      this.Value = elt.Value.ToString();
    }

    public Buffer()
    {
      this.Value = "";
      this.Attributes = new Dictionary<string, string>();
    }
  }
}
