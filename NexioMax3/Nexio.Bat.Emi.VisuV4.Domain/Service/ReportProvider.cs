namespace Nexio.Bat.Emi.VisuV4.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Service;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Helper;

  public class ReportProvider
  {
    private static ReportProvider intance;

    private Dictionary<int, int> fusionFinalsColumns = new Dictionary<int, int>();

    private Dictionary<int, int> fusionSuspectColumns = new Dictionary<int, int>();

    private ReportProvider()
    {
    }

    public static ReportProvider Instance
    {
      get
      {
        return intance ?? (intance = new ReportProvider());
      }
    }

    public Guid CurrentId { get; private set; } = Guid.Empty;

    public bool ForceReload { get; set; }

    public Test Test { get; private set; }

    public byte SignificantFigures
    {
      get
      {
        return Configuration.Model.VisuV4Config.Instance.TableValuesSignificantFigures;
      }
    }

    public Dictionary<string, DataTable> GetFinalsBySourceForSubRange(Guid testId, Guid idSB)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      List<SubRange> sbList = new List<SubRange>();
      var sb = this.Test.SubRangeList.FirstOrDefault(s => s.GuidSB == idSB);
      if (sb != null)
      {
        sbList.Add(sb);
        return this.GetFinalsBySource(sbList);
      }

      return null;
    }

    public Dictionary<string, DataTable> GetFinalsBySourceForTest(Guid testId)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      return this.GetFinalsBySource(this.Test.SubRangeList);
    }

    public DataTable GetFinalsDataTableForSubRange(Guid testId, Guid idSB)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      List<SubRange> sbList = new List<SubRange>();
      var sb = this.Test.SubRangeList.FirstOrDefault(s => s.GuidSB == idSB);
      if (sb == null)
      {
        return null;
      }

      sbList.Add(sb);

      List<Column> columns = null;
      DataTable table = this.GetFilteredFinalsDataTable(sbList, out columns);
      return table;
    }

    public DataTable GetFinalsDataTableForTest(Guid testId, out List<Column> columns)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      DataTable table = this.GetFilteredFinalsDataTable(this.Test.SubRangeList, out columns);
      return table;
    }

    public Dictionary<string, DataTable> GetSuspectsBySourceForSubRange(Guid testId, int idSB)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      List<SubRange> sbList = new List<SubRange>();
      var sb = this.Test.SubRangeList.FirstOrDefault(s => s.Id == idSB);
      if (sb != null)
      {
        sbList.Add(sb);
        return this.GetSuspectsBySource(sbList);
      }

      return null;
    }

    public Dictionary<string, DataTable> GetSuspectsBySourceForTest(Guid testId)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      return this.GetSuspectsBySource(this.Test.SubRangeList);
    }

    public DataTable GetSuspectsDataTableForSubRange(Guid testId, Guid idSB)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      List<SubRange> sbList = new List<SubRange>();
      var sb = this.Test.SubRangeList.FirstOrDefault(s => s.GuidSB == idSB);
      if (sb == null)
      {
        return null;
      }

      sbList.Add(sb);

      return this.GetFilteredSuspectsDataTable(sbList);
    }

    public DataTable GetSuspectsDataTableForTest(Guid testId)
    {
      if (this.CurrentId != testId)
      {
        this.InitTest(testId);
      }

      return this.GetFilteredSuspectsDataTable(this.Test.SubRangeList);
    }

    public void InitTest(Guid testId)
    {
      if (testId != this.CurrentId || this.ForceReload)
      {
        this.ForceReload = false;

        if (this.CurrentId != Guid.Empty)
        {
          Provider.Instance.BatData.Terminer((Test_State)(-2)); // On "cloture" l'essai pour pouvoir en ouvrir un antre dans DATA (-2 => pas de sauvegarde)
        }

        Guid guidRoot = ATDBProvider.Instance.GetRootGuid(testId);

        string resultPath = RegistryHelper.GetResultPath() + "\\" + guidRoot.ToString("B");

        this.CurrentId = testId;
        int numEssai = Provider.Instance.GetNumEssai(testId);
        this.Test = Provider.Instance.GetTestById(testId, numEssai, resultPath);
      }
    }

    public string FormatFrequency(double freq, bool hideUnit = false)
    {
      //double val = Math.Floor(freq); // On tronque au Hertz près
      double val = freq.RemoveNoise();

      int precision = Configuration.Model.VisuV4Config.Instance.TableFrequencySignificantFigures;

      return val.ToStringUnit(precision, hideUnit ? string.Empty : "Hz", culture: System.Threading.Thread.CurrentThread.CurrentCulture);
    }

    public void ExitBatData()
    {
      Provider.Instance.BatData.Terminer((Test_State)(-2)); // On "cloture" l'essai
      Provider.Instance.DechargerBatData();
      this.CurrentId = Guid.Empty;
    }

    private DataTable GetFilteredSuspectsDataTable(List<SubRange> sbList)
    {
      this.fusionSuspectColumns.Clear();

      // 25/02/2022 MC On recharge le fichier VisuV4 pour éviter de na pas prendre en compte les nom de colonnes customisés (sinon il faut relancer Bat)
      VisuV4Config.Reload();

      var columnsList = Provider.Instance.GetSuspects(sbList);

      var table = new DataTable();

      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(string),
        ColumnName = "Source",
        Caption = "Source",
        ReadOnly = false,
      });
      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(object),
        ColumnName = "Frequency",
        Caption = "Frequency",
        ReadOnly = false,
      });
      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(string),
        ColumnName = "SR #",
        Caption = "Sub-Range",
        ReadOnly = false,
      });

      int i = 3;
      foreach (var colDef in columnsList)
      {
        string name = colDef.Name;

        var customHeaders = VisuV4Config.Instance.SuspectTables.Headers.Where(t => t.Source == colDef.Function.Name && t.ColumnName == colDef.Name && t.Value != null);
        if (customHeaders.Count() > 0)
        {
          name = customHeaders.First().Value;
        }

        if (table.Columns.Contains(name))
        {
          int index = table.Columns.IndexOf(name);
          this.fusionSuspectColumns.Add(i, index);
        }
        else
        {
          table.Columns.Add(new DataColumn()
          {
            DataType = typeof(object),
            ColumnName = name,
            Caption = colDef.Name.Trim(),
            ReadOnly = false,
          });
          this.fusionSuspectColumns.Add(i, table.Columns.Count - 1);
        }

        i++;
      }

      this.LoadSuspectsRows(sbList, table);

      return table;
    }

    private DataTable GetFilteredFinalsDataTable(List<SubRange> sbList, out List<Column> columnsList)
    {
      this.fusionFinalsColumns.Clear();

      // 25/02/2022 MC On recharge le fichier VisuV4 pour éviter de na pas prendre en compte les nom de colonnes customisés (sinon il faut relancer Bat)
      VisuV4Config.Reload();

      columnsList = Provider.Instance.GetFinals(sbList);

      var table = new DataTable();

      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(string),
        ColumnName = "Source",
        Caption = "Source",
        ReadOnly = false,
      });
      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(object),
        ColumnName = "Frequency",
        Caption = "Frequency",
        ReadOnly = false,
      });
      table.Columns.Add(new DataColumn()
      {
        DataType = typeof(string),
        ColumnName = "SR #",
        Caption = "Sub-Range",
        ReadOnly = false,
      });

      DataColumn measTimeColumn = null;
      DataColumn rbwColumn = null;
      int i = 3;
      foreach (var colDef in columnsList)
      {
        string name = colDef.Name;

        var customHeaders = VisuV4Config.Instance.SuspectTables.Headers.Where(t => t.Source == colDef.Function.Name && t.ColumnName == colDef.Name && t.Value != null);
        if (customHeaders.Count() > 0)
        {
          name = customHeaders.First().Value;
        }

        if (table.Columns.Contains(name))
        {
          int index = table.Columns.IndexOf(name);
          this.fusionFinalsColumns.Add(i, index);
        }
        else if (colDef.Type == TypeCol.TYPECOL_MEAS_TIME)
        {
          measTimeColumn = new DataColumn()
          {
            DataType = typeof(object),
            ColumnName = name,
            Caption = colDef.Name.Trim(),
            ReadOnly = false,
          };

          table.Columns.Add(measTimeColumn);
          this.fusionFinalsColumns.Add(i, table.Columns.Count - 1);
        }
        else if (colDef.Type == TypeCol.TYPECOL_RBW)
        {
          rbwColumn = new DataColumn()
          {
            DataType = typeof(object),
            ColumnName = name,
            Caption = colDef.Name.Trim(),
            ReadOnly = false,
          };

          table.Columns.Add(rbwColumn);
          this.fusionFinalsColumns.Add(i, table.Columns.Count - 1);
        }
        else
        {
          table.Columns.Add(new DataColumn()
          {
            DataType = typeof(object),
            ColumnName = name,
            Caption = colDef.Name.Trim(),
            ReadOnly = false,
          });
          this.fusionFinalsColumns.Add(i, table.Columns.Count - 1);
        }

        i++;
      }

      this.LoadFinalsRows(sbList, table);

      if (this.IsMeasTimeFinalColumnHidden() && measTimeColumn != null)
      {
        table.Columns.Remove(measTimeColumn);
      }

      if (this.IsRBWFinalColumnHidden() && rbwColumn != null)
      {
        table.Columns.Remove(rbwColumn);
      }

      return table;
    }

    private bool IsRBWFinalColumnHidden()
    {
      bool isRBWHidden = false;
      if (Configuration.Model.VisuV4Config.Instance.SuspectTables.HiddenColumns.ContainsKey(Configuration.Model.OptionalColumn.RBW))
      {
        isRBWHidden = Configuration.Model.VisuV4Config.Instance.SuspectTables.HiddenColumns[Configuration.Model.OptionalColumn.RBW];
      }

      return isRBWHidden;
    }

    private bool IsMeasTimeFinalColumnHidden()
    {
      bool isMeasTimeHidden = false;
      if (Configuration.Model.VisuV4Config.Instance.SuspectTables.HiddenColumns.ContainsKey(Configuration.Model.OptionalColumn.Time))
      {
        isMeasTimeHidden = Configuration.Model.VisuV4Config.Instance.SuspectTables.HiddenColumns[Configuration.Model.OptionalColumn.Time];
      }

      return isMeasTimeHidden;
    }

    private Dictionary<string, DataTable> GetFinalsBySource(List<SubRange> subRangeList)
    {
      List<Column> columns;
      DataTable table = this.GetFilteredFinalsDataTable(subRangeList, out columns);

      List<DataTable> result = table.AsEnumerable().GroupBy(row => row.Field<string>("Source")).Select(g => g.CopyToDataTable()).ToList();

      Dictionary<string, DataTable> listDt = new Dictionary<string, DataTable>();
      foreach (var dt in result)
      {
        if (dt.Rows.Count > 0)
        {
          string src = dt.Rows[0]["Source"].ToString();
          var col = columns.First(c => c.Function.Name == src);

          for (int i = dt.Columns.Count - 1; i >= 0; i--)
          {
            if (dt.Columns[i].ColumnName == "Frequency" || dt.Columns[i].ColumnName == "SR #")
            {
              continue;
            }
            else
            {
              // on recherche dans la liste des colonnes
              bool bFound = false;
              foreach (var colDef in col.Function.Columns)
              {
                string colName = colDef.Name;
                // On va comparer les deux noms de colonnes en enlevant au préablable les unités avant
                var lastIndexOf = colName.IndexOf('(');
                if (lastIndexOf != -1)
                {
                  colName = colName.Remove(lastIndexOf);
                  colName = colName.Trim();
                }

                string colNameFromDT = dt.Columns[i].ColumnName;
                lastIndexOf = colNameFromDT.IndexOf('(');
                if (lastIndexOf != -1)
                {
                  colNameFromDT = colNameFromDT.Remove(lastIndexOf);
                  colNameFromDT = colNameFromDT.Trim();
                }

                if (colName == colNameFromDT)
                {
                  bFound = true;
                }
              }

              if (!bFound)
              {
                dt.Columns.RemoveAt(i);
              }
            }
          }

          listDt.Add(src, dt);
        }
      }

      return listDt;
    }

    private Dictionary<string, DataTable> GetSuspectsBySource(List<SubRange> subRangeList)
    {
      DataTable table = this.GetFilteredSuspectsDataTable(subRangeList);

      List<DataTable> result = table.AsEnumerable().GroupBy(row => row.Field<string>("Source")).Select(g => g.CopyToDataTable()).ToList();

      Dictionary<string, DataTable> listDt = new Dictionary<string, DataTable>();
      foreach (var dt in result)
      {
        if (dt.Rows.Count > 0)
        {
          string src = dt.Rows[0]["Source"].ToString();

          listDt.Add(src, dt);
        }
      }

      return listDt;
    }

    private void LoadFinalsRows(IEnumerable<SubRange> subranges, DataTable table)
    {
      int rowId = 0;

      foreach (var sr in subranges)
      {
        foreach (var final in sr.FinalList.OrderBy(s => s.Frequency).ToList())
        {
          DataRow dataRow = table.NewRow();
          dataRow[0] = final.Function.Name;
          dataRow[1] = final.Frequency;
          dataRow[2] = sr.Id + 1;
          int iCurrent = 3;

          foreach (var val in final.Values)
          {
            if (this.fusionFinalsColumns.ContainsKey(iCurrent))
            {
              int index = this.fusionFinalsColumns[iCurrent];
              if (string.IsNullOrEmpty(dataRow[index].ToString()))
              {
                if (final.DoubleValues.Count > (iCurrent - 3) && !double.IsNaN(final.DoubleValues[iCurrent - 3]) && !string.IsNullOrWhiteSpace(val))
                {
                  dataRow[index] = final.DoubleValues[iCurrent - 3];
                }
                else
                {
                  dataRow[index] = val;
                }
              }
            }
            else
            {
              throw new Exception("fusionFinalsColumns should be correct");
            }

            iCurrent++;
          }

          table.Rows.Add(dataRow);
          rowId++;
        }
      }
    }

    private void LoadSuspectsRows(IEnumerable<SubRange> subranges, DataTable table)
    {
      int rowId = 0;

      foreach (var sr in subranges)
      {
        foreach (var suspect in sr.SuspectList.OrderBy(s => s.Frequency).ToList())
        {
          DataRow dataRow = table.NewRow();
          dataRow[0] = suspect.Function.Name;
          dataRow[1] = suspect.Frequency;
          dataRow[2] = sr.Id + 1;
          int iCurrent = 3;

          foreach (var val in suspect.Values)
          {
            if (this.fusionSuspectColumns.ContainsKey(iCurrent))
            {
              int index = this.fusionSuspectColumns[iCurrent];
              if (dataRow.ItemArray.Count() > index && string.IsNullOrEmpty(dataRow[index].ToString()))
              {
                if (suspect.DoubleValues.Count > (iCurrent - 3) && !double.IsNaN(suspect.DoubleValues[iCurrent - 3]) && !string.IsNullOrWhiteSpace(val))
                {
                  dataRow[index] = suspect.DoubleValues[iCurrent - 3];
                }
                else
                {
                  dataRow[index] = val;
                }
              }
            }

            iCurrent++;
          }

          table.Rows.Add(dataRow);
          rowId++;
        }
      }
    }
  }
}