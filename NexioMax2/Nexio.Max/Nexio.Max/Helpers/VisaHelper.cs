using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NationalInstruments.Visa;
using Ivi.Visa;
namespace NexioMax.Helpers
{
  internal static class VisaHelper
  {
    public static bool IsVisaFormat(string address)
    {
      string patternINSTR = @"\b(TCPIP)\d+\b(::)\d+\b(.)\d+\b(.)\d+\b(.)\d+\b(::)\b(inst)\d+\b(::)\b(INSTR)";
      string patternSOCKET = @"\b(TCPIP)\d+\b(::)\d+\b(.)\d+\b(.)\d+\b(.)\d+\b(::)\d+\b(::)\b(SOCKET)";
      string patternGPIB = @"\b(GPIB)\d+\b(::)\d+\b(::)\b(INSTR)";

      if (Regex.IsMatch(address, patternINSTR))
        return true;

      if (Regex.IsMatch(address, patternSOCKET))
        return true;

      if (Regex.IsMatch(address, patternGPIB))
        return true;

      return false;
    }

    public static MessageBasedSession OpenVisaCom(string address, int timeout, string eosRead)
    {
      NationalInstruments.Visa.ResourceManager rm = new NationalInstruments.Visa.ResourceManager();
      IVisaSession iviSession = rm.Open(address, AccessModes.None, timeout);
      MessageBasedSession session = (MessageBasedSession)iviSession;
      session.SendEndEnabled = true;
      session.TerminationCharacterEnabled = true;
      session.TerminationCharacter = AsciiToHex(Regex.Unescape(eosRead)).First();
      return session;
    }

    public static void CloseVisaCom(MessageBasedSession session)
    {
      if (session == null)
        return;
      session.Dispose();
    }

    public static string Write(MessageBasedSession session, string command, string eos)
    {
      string cleanCommand = command;
      string cleanEos = eos;
      
      if (eos != "")
      {
        cleanCommand = command.Replace(eos, ""); //si le eos est déjà contenu dans la commande on l'enleve
        cleanEos = Regex.Unescape(eos);
      }

      try
      {
        session.RawIO.Write(cleanCommand + cleanEos); //Send Write command to VISA resource
        return string.Empty;
      }
      catch (Exception ex)
      {
        return ex.Message.ToString();
      }
    }
    public static string WriteByte(MessageBasedSession session, byte[] command, string eos)
    {
      byte[] commandToSend;
      if (eos != "")
      {
        string cleanEos = Regex.Unescape(eos);
        byte[] hexEos = AsciiToHex(cleanEos);
        commandToSend = new byte[command.Length + hexEos.Length];
        command.CopyTo(commandToSend, 0);
        hexEos.CopyTo(commandToSend, commandToSend.Length - hexEos.Length);
      }
      else
      {
        commandToSend = new byte[command.Length];
        command.CopyTo(commandToSend, 0);
      }

      try
      {
        session.RawIO.Write(commandToSend); //Send Write command to VISA resource
        return string.Empty;
      }
      catch (Exception ex)
      {
        return ex.Message.ToString();
      }
    }

    public static string Read(MessageBasedSession session, int byteCount)
    {
      try
      {
        //Read command from VISA resource
        if (byteCount > 0)
        {
          byte[] b = session.RawIO.Read(byteCount);
          return Encoding.ASCII.GetString(b);
        }
        else
        {
          byte[] b = session.RawIO.Read();
          return Encoding.ASCII.GetString(b);
        }
      }
      catch (Exception ex) 
      {
        return ex.Message;
      }
    }
    public static string AsciiToHexString(string ascii)
    {
      return Convert.ToHexString(Encoding.UTF8.GetBytes(ascii));
    }

    public static byte[] AsciiToHex(string ascii)
    {
      return Encoding.UTF8.GetBytes(ascii);
    }
    public static byte[] HexStringToByte(string hex)
    {
      return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
    }

    public static List<string> ScanGPIB()
    {
      List<string> devices = new List<string>();
      using (var rm = new ResourceManager())
      {
        for (int card = 0; card < 2; card++)
        {
          for (int address = 0; address < 30; address++)
          {
            try
            {
              IVisaSession iviSession = rm.Open($"GPIB{card.ToString()}::{address.ToString()}::INSTR");
              GpibSession session = (GpibSession)iviSession;
              session.TimeoutMilliseconds = 50;
              session.FormattedIO.WriteLine("*IDN?");
              session.FormattedIO.ReadLine();
              devices.Add($"GPIB{card.ToString()}::{address.ToString()}::INSTR");
            }
            catch (Exception e)
            {
              Console.WriteLine(e.Message);
            }
          }
        }

      }
      return devices;
    }
  }
}

