namespace NexioMax3.Domain.Repository
{
  using System;
  using System.IO;
  using System.IO.Compression;
  using log4net;
  using Nexio.Bat.Common.Domain.Infrastructure;

  internal class SingleFileTestRepository
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(SingleFileTestRepository));

    static SingleFileTestRepository()
    {
      System.AppDomain.CurrentDomain.ProcessExit += (o, e) => ClearTempFolder();
    }

    public SingleFileTestRepository(string resultPath)
    {
      this.ResultPath = resultPath;
    }

    public string ResultPath { get; private set; }

    /// <summary>
    /// Detruit le cache et nettoie les dossiers temporaire
    /// </summary>
    public static void ClearTempFolder()
    {
      string tempPath = Path.Combine(PathHelper.GetTempPath(), "BAT-EMC", "VisuV4");
      if (Directory.Exists(tempPath))
      {
        Log.DebugFormat("Delete temp directory {0}", tempPath);
        DeleteDirectory(tempPath);
      }
    }

    /// <summary>
    /// Supprime le fichier de test du repertoire de résultat
    /// </summary>
    /// <param name="testId">id du test</param>
    public void DeleteSingleFileTest(Guid testId)
    {
      TestDescription desc = new TestDescription(this.ResultPath, testId);
      Log.DebugFormat("Delete  Single File Test {0}", desc.ToString());

      if (Directory.Exists(desc.TempPath))
      {
        Directory.Delete(desc.TempPath, true);
      }

      if (File.Exists(desc.FilePathInResult))
      {
        File.Delete(desc.FilePathInResult);
      }
    }

    /// <summary>
    /// Crée un repertoire temporaire pour les résultats de tests et retourne son chemin
    /// Si le reperetoire temporaire existe deja on le retourne
    /// </summary>
    /// <param name="testId">id du test</param>
    /// <returns>chemin d'accés temporaire au dossier de test</returns>
    public string GetOrCreateTempPath(Guid testId)
    {
      var desc = new TestDescription(this.ResultPath, testId);
      Log.DebugFormat("GetOrCreateTempPath : TestId:{0}", testId);
      Log.DebugFormat("GetOrCreateTempPath : TestDescription:{0}", desc.ToString());

      bool validTempPath = false;
      if (Directory.Exists(desc.TempPath))
      {
        if (Directory.GetFiles(desc.TempPath, "TestDefinition.json").Length == 0)
        {
          Log.WarnFormat("GetOrCreateTempPath : TestDefinition.emi4 should exist in the temporary file, TempPath:{0}", desc.TempPath);
          validTempPath = false;
        }
        else
        {
          validTempPath = true;
        }
      }

      if (validTempPath)
      {
        return desc.TempPath;
      }
      else
      {
        // on recrée le dossier temporaire
        if (Directory.Exists(desc.TempPath))
        {
          Log.DebugFormat("GetOrCreateTempPath : recreate temp directory {0}", desc.TempPath);
          Directory.Delete(desc.TempPath, true);
        }

        Directory.CreateDirectory(desc.TempPath);

        if (File.Exists(desc.FilePathInResult))
        {
          File.Copy(desc.FilePathInResult, desc.FilePathInTempPath, true);
        }
        else
        {
          Log.ErrorFormat("CACHE : File not found desc.FilePathInResult {0}", desc.FilePathInResult);
        }

        if (File.Exists(desc.FilePathInTempPath))
        {
          ZipFile.ExtractToDirectory(desc.FilePathInTempPath, desc.TempPath);
        }
        else
        {
          Log.ErrorFormat("CACHE : File not found desc.FilePathInTempPath {0}", desc.FilePathInTempPath);
        }

        if (File.Exists(desc.FilePathInTempPath))
        {
          File.Delete(desc.FilePathInTempPath);
        }

        return desc.TempPath;
      }
    }

    /// <summary>
    /// Enregistre le test
    /// </summary>
    /// <param name="testId">id du test</param>
    public void SaveToPathResult(Guid testId)
    {
      TestDescription desc = new TestDescription(this.ResultPath, testId);
      Log.DebugFormat("CACHE : Save {0} To Path Result {1}", desc.FilePathInTempPath, desc.FilePathInResult);
      if (File.Exists(desc.FilePathInTempPath))
      {
        Log.WarnFormat("CACHE : Deleting file that should not exists {0}", desc.FilePathInTempPath);
        File.Delete(desc.FilePathInTempPath);
      }

      if (!Directory.Exists(this.ResultPath))
      {
        Log.DebugFormat("CACHE : Create Directory result Path {0}", this.ResultPath);
        Directory.CreateDirectory(this.ResultPath);
      }

      var filesInTempath = Directory.GetFiles(desc.TempPath);
      if (filesInTempath != null && filesInTempath.Length == 0)
      {
        Log.ErrorFormat("CACHE : No files found in desc.TempPath: {0}", desc.TempPath);
        throw new DomainException(NexioMax3.Domain.Properties.Resources.CanNotSaveTheTestRiskToDeleteFiles);
      }
      else
      {
        ZipFile.CreateFromDirectory(desc.TempPath, desc.FilePathInTempPath, CompressionLevel.Fastest, false);

        File.Copy(desc.FilePathInTempPath, desc.FilePathInResult, true);

        if (Directory.Exists(desc.TempPath))
        {
          DeleteDirectory(desc.TempPath);
        }
      }

      return;
    }

    /// <summary>
    /// Recursively delete directory structure
    /// </summary>
    /// <param name="target_dir">the directory to delete</param>
    private static void DeleteDirectory(string target_dir)
    {
      string[] files = Directory.GetFiles(target_dir);
      string[] dirs = Directory.GetDirectories(target_dir);

      foreach (string file in files)
      {
        File.SetAttributes(file, FileAttributes.Normal);
        File.Delete(file);
      }

      foreach (string dir in dirs)
      {
        DeleteDirectory(dir);
      }

      const int magicDust = 10;
      for (var gnomes = 1; gnomes <= magicDust; gnomes++)
      {
        try
        {
          Directory.Delete(target_dir, true);
          return;  // good!
        }
        catch (DirectoryNotFoundException)
        {
          return;  // good!
        }
        catch (Exception)
        {
          // System.IO.IOException: The directory is not empty
          Log.ErrorFormat("Unable to delete {0}! attempt #{1}.", target_dir, gnomes);

          // see http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true for more magic
          System.Threading.Thread.Sleep(50);
          continue;
        }
      }
    }

    /// <summary>
    /// Stocke les chemins liés au test en cours
    /// </summary>
    private class TestDescription
    {
      public TestDescription(string resultPath, Guid testId)
      {
        var testFileName = this.GetSingleTestFileName(testId);
        this.FilePathInResult = Path.Combine(resultPath, testFileName);
        this.TempPath = Path.Combine(PathHelper.GetTempPath(), "BAT-EMC", "VisuV4", testId.ToString("B"));
        this.FilePathInTempPath = Path.Combine(PathHelper.GetTempPath(), "BAT-EMC", "VisuV4", testFileName);
      }

      /// <summary>
      /// Gets le chemin d'origine (celui du zip)
      /// </summary>
      public string FilePathInResult { get; }

      /// <summary>
      /// Gets le repertoire temporaire
      /// </summary>
      public string TempPath { get; }

      /// <summary>
      /// Gets le chemin du test dans le repertoire temporaire
      /// </summary>
      public string FilePathInTempPath { get; }

      public override string ToString()
      {
        return string.Format("FilePathInResult:{0}, TempPath:{1}, FilePathInTempPath:{2}", this.FilePathInResult, this.TempPath, this.FilePathInTempPath);
      }

      private string GetSingleTestFileName(Guid testId)
      {
        return testId.ToString("B") + ".visu4";
      }
    }
  }
}