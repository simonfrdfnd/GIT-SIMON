namespace NexioMax3.Domain.Helper
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  /// <summary>
  /// Helper qui peut être utilisé afin de récupérer des données depuis la ligne de commande envoyé au module exécution de visuv4
  /// </summary>
  public static class CommandLineHelper
  {
    public static bool IsAutoCloseDialog()
    {
      return Environment.GetCommandLineArgs().Contains("-AutomaticallyCloseDialogWindows");
    }

    public static bool IsValidAuto()
    {
      return Environment.GetCommandLineArgs().Contains("-auto");
    }

    public static string GetTestNumber()
    {
      var arg = Environment.GetCommandLineArgs().FirstOrDefault(a => a.Contains("-numessai"));
      return arg.Substring(arg.IndexOf('=') + 1);
    }
  }
}
