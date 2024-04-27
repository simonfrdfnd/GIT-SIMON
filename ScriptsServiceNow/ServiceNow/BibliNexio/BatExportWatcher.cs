namespace ServiceNow.BibliNexio
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  public class BatExportWatcher
  {
    private static BatExportWatcher intance;
    private static string tmpSubPath = "Nexio.eSupport.Transfert";

    private FileSystemWatcher watcher;

    private BatExportWatcher()
    {
    }

    public event EventHandler<string> FileReceived;

    public static BatExportWatcher Instance
    {
      get
      {
        return intance ?? (intance = new BatExportWatcher());
      }
    }

    public void Init()
    {
      var watch = GetWatchPath();
      if (!Directory.Exists(watch))
      {
        Directory.CreateDirectory(watch);
      }

      this.watcher = new FileSystemWatcher();
      watcher.Path = watch;
      watcher.NotifyFilter = NotifyFilters.LastWrite;
      watcher.Filter = "*.*";
      watcher.Changed += new FileSystemEventHandler(this.FileAdded);
      watcher.EnableRaisingEvents = true;
    }

    public void ListAndRaise()
    {
      var watch = GetWatchPath();
      var list = Directory.GetFiles(watch);
      foreach (var file in list)
      {
        var name = Path.GetFileName(file);
        this.FileAdded(this, new FileSystemEventArgs(WatcherChangeTypes.Created, watch, name));
      }
    }

    internal void Release()
    {
      this.watcher.Dispose();
      var watch = GetWatchPath();
      if (Directory.Exists(watch))
      {
        Directory.Delete(watch, true);
      }
    }

    private static string GetWatchPath()
    {
      var tmp = Path.GetTempPath();
      return Path.Combine(tmp, tmpSubPath);
    }

    private void FileAdded(object sender, FileSystemEventArgs e)
    {
      Thread.Sleep(100); // Laisse le temps au systeme de liberer le fichier
      this.FileReceived?.Invoke(this, e.FullPath);
    }
  }
}
