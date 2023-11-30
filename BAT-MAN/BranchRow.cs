using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAT_MAN
{
  internal class BranchRow
  {
    public BranchRow(string branchPath, string branchName, BranchType branchType, string binPath, bool hasExe, string exeVersion, string exePath, bool hasReferential, string referentialPath, bool hasSolution, string solutionPath, List<string> databases, int databaseSelectedIndex = 0)
    {
      BranchPath = branchPath;
      BranchName = branchName;
      BranchType = branchType;
      BinPath = binPath;
      HasExe = hasExe;
      ExeVersion = exeVersion;
      ExePath = exePath;
      ExeFolder = Path.GetDirectoryName(exePath);
      InitPath = $"{ExeFolder}\\BAT_EMC.ini";
      HasReferential = hasReferential;
      ReferentialPath = referentialPath;
      HasSolution = hasSolution;
      SolutionPath = solutionPath;
      Databases = databases;
      DatabaseSelectedIndex = databaseSelectedIndex;
    }
    public string BranchPath { get; set; }
    public string BranchName { get; set; }
    public BranchType BranchType { get; set; } 
    public string BinPath { get; set; }
    public bool HasExe { get; set; }
    public string ExePath { get; set; }
    public string ExeVersion { get; set; }
    public string ExeFolder { get; set; }
    public string InitPath { get; set; }
    public bool HasReferential { get; set; }
    public string ReferentialPath { get; set; }
    public bool HasSolution { get; set; }
    public string SolutionPath { get; set; }
    public List<string> Databases { get; set; }
    public int DatabaseSelectedIndex { get; set; }
    public string GetIniFileContent()
    {
      if (ExeVersion.Contains("2023"))
      {
        if (BranchType == BranchType.Version)
        {
          return $"[Databases]\r\nBAT-EMC3={Databases[DatabaseSelectedIndex]}\r\n[Directories]\r\nResults={BranchPath}\\Results\\\r\nTempPathFolder=";
        }
        else
        {
          return $"[Databases]\r\nBAT-EMC3={Databases[DatabaseSelectedIndex]}\r\n[Directories]\r\nResults={BinPath}\\Results\\\r\nTempPathFolder=";
        }
      }
      else
      {
        if (BranchType == BranchType.Version)
        {
          return $"[Databases]\r\nBAT-EMC3={Databases[DatabaseSelectedIndex]}\r\n[Directories]\r\nBAT-CLICK={BranchPath}\\BAT-CLICK\r\nBAT-ELEC={BranchPath}\\BAT-ELEC\r\nBAT-EMC={BranchPath}\\\r\nBAT-EMI={BranchPath}\\BAT-EMI\r\nBAT-EMS={BranchPath}\\BAT-EMS\r\nBAT-MANAGER={BranchPath}\\BAT-MANAGER\r\nBAT-SCAN={BranchPath}\\BAT-SCAN\r\nCommon={BranchPath}\\Common\r\nLibraries={BranchPath}\r\nResults={BranchPath}\\Results";
        }
        else
        {
          return $"[Databases]\r\nBAT-EMC3={Databases[DatabaseSelectedIndex]}\r\n[Directories]\r\nBAT-CLICK={BinPath}\\BAT-CLICK\r\nBAT-ELEC={BinPath}\\BAT-ELEC\r\nBAT-EMC={BinPath}\\\r\nBAT-EMI={BinPath}\\BAT-EMI\r\nBAT-EMS={BinPath}\\BAT-EMS\r\nBAT-MANAGER={BinPath}\\BAT-MANAGER\r\nBAT-SCAN={BinPath}\\BAT-SCAN\r\nCommon={BinPath}\\Common\r\nLibraries={BinPath}\r\nResults={BinPath}\\Results";
        }
      }
    }

  }

  public enum BranchType 
  {
    Debug,
    Release,
    Version
  }

  public static class BranchHelper
  {
    public static string GetBinFolder(BranchType branchType)
    {
      if (branchType == BranchType.Debug)
      {
        return "binDebug";
      }
      else if (branchType == BranchType.Release)
      {
        return "bin";
      }
      else
      {
        return String.Empty;
      }
    }
  }
}
