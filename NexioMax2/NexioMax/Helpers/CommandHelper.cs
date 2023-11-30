using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexioMax.Helpers
{
  public static class CommandHelper
  {
    public static string GetHeader(string fullCommand)
    {
      if (fullCommand.Contains(' '))
      {
        return fullCommand.Split(' ')[0];
      }
      else
      {
        return fullCommand;
      }
    }
    public static string GetParameter(string fullCommand)
    {
      if (fullCommand.Contains(' '))
      {
        string header = fullCommand.Split(' ')[0];
        return fullCommand.Remove(0, header.Length);
      }
      else
      {
        return null;
      }
    }

    public static bool IsQuestion(string fullCommand)
    {
      if (fullCommand.Contains('?'))
      {
        return true;
      }
      else
      {
        return false;
      }
    }
    public static byte[] GetAnswerHex(string answer)
    {
      return System.Text.Encoding.ASCII.GetBytes(answer);
    }

    public static string GetAnswerHexString(string answer)
    {
      byte[] ba = System.Text.Encoding.ASCII.GetBytes(answer);
      StringBuilder hex = new StringBuilder(ba.Length * 2);
      foreach (byte b in ba)
      {
        hex.AppendFormat("{0:x2}", b);
        hex.Append(" ");
      }
      return hex.ToString();
    }

    public static int GetCommandCount(string fullCommand)
    {
      return fullCommand.Split(';').Count();
    }

    public static List<string> GetCommands(string inputBuffer)
    {
      return inputBuffer.Split(';').ToList();
    }

  }
}
