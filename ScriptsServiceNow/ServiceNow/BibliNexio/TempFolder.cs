namespace ServiceNow.BibliNexio
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  internal class TempFolder
  {
    private static readonly string TempLocation = Path.GetTempPath() + "TempNexio\\";
    private static readonly string TempLogsPath = TempLocation + "Logs\\";
    private static TempFolder intance;

    private TempFolder()
    {
      // Delete the temporary directory if it exists
      if (Directory.Exists(TempLocation))
      {
        Directory.Delete(TempLocation, true);
      }

      Directory.CreateDirectory(TempLocation);
    }

    public static TempFolder Instance
    {
      get
      {
        return intance ?? (intance = new TempFolder());
      }
    }

    public static void Release()
    {
      intance = null;
      if (Directory.Exists(TempLocation))
      {
        Directory.Delete(TempLocation, true);
      }
    }

    internal string GetFilePath(string filename, bool checkExists = true)
    {
      var path = Path.Combine(TempLocation, filename);
      if (!checkExists || File.Exists(path))
      {
        return path;
      }

      throw new FileNotFoundException(path);
    }

    internal string GetUniqueName(string filename, string ext)
    {
      // Process the list of files found in the directory.
      string[] allFiles = Directory.GetFiles(TempLocation, "*" + ext);

      // Recurse into subdirectories of this directory.
      string[] allDirs = Directory.GetDirectories(TempLocation, "*" + ext);
      var all = allFiles.Select(f => Path.GetFileNameWithoutExtension(f)).ToList();
      all.AddRange(allDirs.Select(d => Path.GetFileNameWithoutExtension(d)));
      if (all.Any(f => f == filename))
      {
        var unique = GetNextName(all, filename);
        return unique;
      }

      return filename;
    }

    internal void MoveFile(string name, string displayName, string ext)
    {
      var src = this.FullPath(name + ext);
      var dest = this.FullPath(displayName + ext);
      System.IO.FileInfo fi = new System.IO.FileInfo(src);
      if (fi.Exists)
      {
        fi.MoveTo(dest);
      }
    }

    internal void CopyFile(string src, string dest)
    {
      File.Copy(src, TempLocation + dest, true);
    }

    internal bool FileExists(string filename)
    {
      return File.Exists(Path.Combine(TempLocation, filename));
    }

    internal string FullPath(string filename)
    {
      return Path.Combine(TempLocation, filename);
    }

    internal void FileDelete(string filename)
    {
      if (this.FileExists(filename))
      {
        File.Delete(this.FullPath(filename));
      }
    }

    internal string GetLogsPath(string batEmcLogsPath)
    {
      if (!Directory.Exists(TempLogsPath))
      {
        Directory.CreateDirectory(TempLogsPath);
        CopyDirectory(batEmcLogsPath, TempLogsPath);
      }
      else
      {
        Directory.Delete(TempLogsPath, true);
        Directory.CreateDirectory(TempLogsPath);
        CopyDirectory(batEmcLogsPath, TempLogsPath);
      }

      return TempLogsPath;
    }

    private static string GetNextName(IEnumerable<string> list, string baseName)
    {
      int numero;
      int numeroMax = 0;
      foreach (string nom in list.Where(n => n.StartsWith(baseName)).Select(n => n.Replace(baseName, string.Empty)))
      {
        numero = 0;
        Int32.TryParse(nom, out numero);
        if (numero != 0)
        {
          numeroMax = Math.Max(numeroMax, numero);
        }
      }

      numeroMax++;
      return baseName + " " + numeroMax.ToString();
    }

    /// <summary>
    /// Used to copy the "Logs" directory from BAT-EMC in the temporary directory
    /// in order to add it to the archive file
    /// </summary>
    /// <param name="sourceDir">Directory to be copied</param>
    /// <param name="destDir">Directory where to copy</param>
    private static void CopyDirectory(string sourceDir, string destDir)
    {
      if (Directory.Exists(destDir))
      {
        foreach (string file in Directory.GetFiles(sourceDir))
        {
          FileInfo fi = new FileInfo(file);
          File.Copy(file, destDir + fi.Name);
        }

        foreach (string d in Directory.GetDirectories(sourceDir))
        {
          DirectoryInfo di = new DirectoryInfo(d);
          Directory.CreateDirectory(destDir + di.Name + "\\");

          foreach (string file in Directory.GetFiles(sourceDir + di.Name + "\\", "*.log"))
          {
            File.Copy(file, destDir + di.Name + "\\" + Path.GetFileName(file));
          }
        }
      }
    }
  }
}
