using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NexioMax.Helpers
{
  public static class ConsoleHelper
  {
    public static List<string> helpText = new List<string>{
                                                            "> up/down arrow                    Previous/Next command",
                                                            "> cls                              Clear console",
                                                            "> clear                            Clear console",
                                                            "> <stringParam>                    Perform a 'Write' of <stringParam>",
                                                            "> <stringParam>?                   Perform a 'Write' of <stringParam>? and a 'Read'",
                                                            "> <stringParam>? -noread           Perform a 'Write' of <stringParam>?",
                                                            "> <stringParam>? -cnt<byteCount>   Perform a 'Write' of <stringParam>?",
                                                            "> 0x<hexStringParam>               Perform a 'Write' of <hexStringParam> as hex values",   
                                                            "> read                             Perform a 'Read'"
    };

    public static List<string> commands = new List<string> { "help", "cls", "clear", "read"};

    public static bool IsACommand(string line)
    {
      if (commands.Contains(line))
        return true;
      else
        return false;
    }

    public static bool IsAQuestion(string line)
    {
      if (line.Contains('?'))
        return true;
      else
        return false;
    }

    public static string CleanCommand(string line, string eos)
    {
      string cleanCommand = line;

      //Cas ou en veut envoyer de l'hexa
      if (cleanCommand.Contains("0x"))
      {
        return cleanCommand.Remove(cleanCommand.IndexOf("0x"), 2);
      }

      //on enlève le eos char
      if (eos != "")
        cleanCommand = cleanCommand.Replace(eos, ""); //si le eos est déjà contenu dans la commande on l'enleve

      //dans la cas où l'utlisateur veut enovoyer le eos char à la main, il faut remplacer \\ par \
      cleanCommand = Regex.Unescape(cleanCommand);

      //on enlève les paramètres
      if (cleanCommand.Contains('-'))
        return cleanCommand.Remove(cleanCommand.IndexOf('-') - 1);
      else
        return cleanCommand;
    }

    public static bool IsHex(string line)
    {
      if (line.Contains("0x"))
        return true;
      else
        return false;
    }

    public static bool HasParameter(string line)
    {
      if (line.Contains('-'))
        return true;
      else
        return false;
    }

    public static bool IsReadAuthorized(string line)
    {
      if (line.Contains("noread"))
        return false;
      else
        return true;
    }

    public static bool IsReadCount(string line)
    {
      if (line.Contains("cnt"))
        return true;
      else
        return false;
    }

    public static int GetReadCount(string line)
    {
      int index = line.IndexOf("cnt") + 3;
      string sNumber = line.Substring(index, line.Length - index);
      return Convert.ToInt32(sNumber);
    }

  }
}
