namespace NexioMax3.Domain.Service
{
  using System;
  using System.IO;
  using log4net;
  using log4net.Core;
  using NexioMax3.Domain.Model;

  public class UserInteractionLogger
  {
    private static ILog log = LogManager.GetLogger(typeof(UserInteractionLogger));
    private readonly Test test;

    public UserInteractionLogger(Test test)
    {
      this.test = test;
    }

    public void Log(string text)
    {
      try
      {
        File.AppendAllText(Path.Combine(this.test.ResultPath, $"{this.test.NumEssai}.log"),
                           $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {text}\n");
      }
      catch (Exception e)
      {
        log.Fatal(e);
      }
    }

    public void LogMode(string source, Mode newMode)
    {
      this.Log($"{source} changed mode to {newMode}");
    }
  }
}