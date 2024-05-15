namespace NexioMax3.Domain.Repository
{
  using System;
  using System.Data.OleDb;
  using System.IO;
  using System.Linq;
  using log4net;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;
  using Newtonsoft.Json.Linq;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using NexioMax3.Domain.Configuration.Service;
  using Nexio.Helper;

  public class TestResultRepository : IRepository<TestResult>
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(TestResultRepository));

    private SingleFileTestRepository testRepository;

    public TestResultRepository()
    {
      string path = RegistryHelper.CreateStoragePathForCurrentProject();
      this.testRepository = new SingleFileTestRepository(path);
    }

    public void Delete(Guid id)
    {
      // TODO suppression
      throw new NotImplementedException();
    }

    public TestResult GetById(Guid id)
    {
      Log.Debug("Start GetById");
      string tempPath = this.testRepository.GetOrCreateTempPath(id);
      string filePath = string.Format("{0}\\TestResultDefinition.result.json", tempPath, id.ToString("B"));

      var testResult = this.DeserializeJson(filePath);
      if (testResult == null)
      {
        testResult = new TestResult() { ID = id };

        var v3pref = VisuV3PrefFile.Get(id);
        if (v3pref.YRange != Engine.AxisRange.None)
        {
          // restore le scale Y des pref v3 dans le cas ou on utilise les valeurs par defaut.
          testResult.YLimits.AdjustLevel = false;
          testResult.YLimits.Range = v3pref.YRange;
        }

        if (v3pref.EchelleLog.HasValue)
        {
          // restore l'echelle Y des pref v3 dans le cas ou on utilise les valeurs par defaut.
          testResult.PlotSettings.IsLogarithmicAxis = v3pref.EchelleLog.Value;
          testResult.PlotSettings.IsLogarithmicAxisSetByUser = true;
        }

        var testLimits = Service.Provider.Instance.GetLimits();

        foreach (var limit in v3pref.MapPrefLimits.Where(item => item.Visible))
        {
          Guid guidLimit = Guid.Parse(limit.GuidLimite);
          var indexLimit = testLimits.FirstOrDefault(l => l.Id == guidLimit)?.Index;
          if (indexLimit != null)
          {
            testResult.VisibleLimits.Add(new LimitIdentifier()
            {
              Id = guidLimit,
              Index = indexLimit.Value,
            });
          }
        }
      }
      else
      {
        testResult.ID = id;
      }

      Log.Debug("End GetById");
      return testResult;
    }

    public void Save(TestResult testResult)
    {
      string tempPath = this.testRepository.GetOrCreateTempPath(testResult.ID);

      string fileJsonPath = string.Format("{0}\\TestResultDefinition.result.json", tempPath, testResult.ID.ToString("B"));

      this.SerializeJson(fileJsonPath, testResult);
    }

    private TestResult DeserializeJson(string filePath)
    {
      if (File.Exists(filePath))
      {
        string jsonContent = File.ReadAllText(filePath);

        JObject reqObject = JObject.Parse(jsonContent);

        if (reqObject.Property("Version") != null)
        {
          var sVersion = reqObject.Property("Version").Value.ToString();
          var version = new Version(sVersion);
        }

        var myTest = JsonConvert.DeserializeObject<TestResult>(jsonContent, this.GetJsonSettings());

        return myTest;
      }
      else
      {
        return null;
      }
    }

    private JsonSerializerSettings GetJsonSettings()
    {
      var settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects, TypeNameHandling = TypeNameHandling.Auto };
      settings.Converters.Add(new StringEnumConverter());
      settings.Converters.Add(new VersionConverter());
      settings.Converters.Add(new GuidJsonConverter());
      return settings;
    }

    private void SerializeJson(string filePath, TestResult testResult)
    {
      var json = JsonConvert.SerializeObject(testResult, this.GetJsonSettings());
      File.WriteAllText(filePath, json);
      this.testRepository.SaveToPathResult(testResult.ID);
    }
  }
}