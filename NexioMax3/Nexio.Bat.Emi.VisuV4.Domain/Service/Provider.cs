namespace Nexio.Bat.Emi.VisuV4.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using log4net;
  using Nexio.Bat.Common.Domain.ATDB.Model;
  using Nexio.Bat.Common.Domain.Grammar.Service;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using Nexio.Bat.Common.Domain.TestDefinition.Model;
  using Nexio.Bat.Common.Domain.TestDefinition.Repository;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Configuration.Service;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Repository;
  using Nexio.Bat.Emi.VisuV4.Domain.Wrapper;
  using Nexio.Tools;

  public class Provider
  {
    public const double UndefVal = -1e10;

    private static readonly ILog log = LogManager.GetLogger(typeof(Provider));

    private static Provider instance;

    private readonly int CUSTOM_LINE = 32767;

    private Provider()
    {
      this.InitialiserFonctionDLL();
    }

    public static Provider Instance
    {
      get
      {
        if (instance != null)
        {
          if (instance.BatData == null)
          {
            instance.BatData = new BatDataWrapper();
          }

          if (instance.BatData.IsInitialized == false)
          {
            instance.BatData.InitialiserFonctionDLL();
          }
        }

        return instance ?? (instance = new Provider());
      }
    }

    public Pilote3Wrapper Pilote3 { get; private set; }

    public BatDataWrapper BatData { get; private set; }

    public void DechargerFonctionDLL()
    {
      this.Pilote3.DechargerFonctionDLL();
      this.BatData.DechargerFonctionDLL();
      this.Pilote3 = null;
      this.BatData = null;
      System.GC.Collect();
    }

    public void InitialiserFonctionDLL()
    {
      this.Pilote3 = new Pilote3Wrapper();
      this.BatData = new BatDataWrapper();
    }

    public void DechargerBatData()
    {
      this.BatData.DechargerFonctionDLL();
      this.BatData = null;
    }

    public Function GetFunctionById(Fonction_Action action, int functionId)
    {
      this.BatData.SelectFonction(action);

      string sName;
      int typeFonction;
      bool bUsed;
      int currentId;
      while ((currentId = this.BatData.GetFonctionSuivante(out sName, out typeFonction, out bUsed)) != -1)
      {
        if (currentId == functionId)
        {
          if (bUsed)
          {
            int sig = this.BatData.GetSignatureFonction(functionId);
            int actionFunction = this.GetActionFonction(typeFonction);
            int typeSignal = this.GetTypeSignal(typeFonction);
            var func = new Function() { ID = functionId, Name = sName, FunctionType = typeFonction, Action = actionFunction, TypeSignal = typeSignal, Signature = (DetectorSignature)sig };

            // Récupérer le nombre de valeurs pour la fonction demandee
            int iNbValeurs;
            string[] paColNames;
            bool[] paColUsed;
            this.BatData.GetInfosResultatsFonction(func.ID, out iNbValeurs, out paColNames, out paColUsed);

            for (int icol = 0; icol < paColNames.Length; icol++)
            {
              this.BatData.GetCompleteInfoColumnFonction(func.ID, icol, out TypeCol typeCol, out string colName, out var iParam1, out var iParam2, out var dOffset);

              func.Columns.Add(new Column()
              {
                Function = func,
                Name = paColNames[icol],
                Param1 = iParam1,
                Param2 = iParam2,
                Used = paColUsed[icol],
                Offset = dOffset,
                Type = typeCol,
              });
            }

            return func;
          }
        }
      }

      return null;
    }

    public List<Column> GetColumnUsed(Fonction_Action action)
    {
      this.BatData.SelectFonction(action);
      string sName;
      int typeFonction;
      bool bUsed;
      int idFunction;
      List<Function> functionUsedList = new List<Function>();
      while ((idFunction = this.BatData.GetFonctionSuivante(out sName, out typeFonction, out bUsed)) != -1)
      {
        if (bUsed)
        {
          int sig = this.BatData.GetSignatureFonction(idFunction);
          int actionFunction = this.GetActionFonction(typeFonction);
          int typeSignal = this.GetTypeSignal(typeFonction);
          var func = new Function() { ID = idFunction, Name = sName, FunctionType = typeFonction, Action = actionFunction, TypeSignal = typeSignal, Signature = (DetectorSignature)sig };

          // Récupérer le nombre de valeurs pour la fonction demandee
          int iNbValeurs;
          string[] paColNames;
          bool[] paColUsed;
          this.BatData.GetInfosResultatsFonction(func.ID, out iNbValeurs, out paColNames, out paColUsed);

          for (int icol = 0; icol < paColNames.Length; icol++)
          {
            string name = paColNames[icol];
            this.BatData.GetCompleteInfoColumnFonction(func.ID, icol, out TypeCol typeCol, out string colName, out var iParam1, out var iParam2, out var dOffset);
            if (typeCol == TypeCol.TYPECOL_LIMITE_RELATIVE || typeCol == TypeCol.TYPECOL_MESURE || typeCol == TypeCol.TYPECOL_LIMITE)
            {
              if (!string.IsNullOrEmpty(this.GetUniteEssai(typeSignal)))
              {
                if (!name.EndsWith("(" + this.GetUniteEssai(typeSignal) + ")"))
                {
                  name += " (" + this.GetUniteEssai(typeSignal) + ")";
                }
              }
            }

            if (typeCol == TypeCol.TYPECOL_RBW)
            {
              if (!name.EndsWith("(Hz)"))
              {
                name += " (Hz)";
              }
            }

            if (typeCol == TypeCol.TYPECOL_MEAS_TIME)
            {
              if (!name.EndsWith("(s)"))
              {
                name += " (s)";
              }
            }

            if (typeCol == TypeCol.TYPECOL_DELTA)
            {
              if (!name.EndsWith("(dB)"))
              {
                name += " (dB)";
              }
            }

            func.Columns.Add(new Column()
            {
              Function = func,
              Name = name,
              Param1 = iParam1,
              Param2 = iParam2,
              Used = paColUsed[icol],
              Offset = dOffset,
              Type = typeCol,
            });
          }

          functionUsedList.Add(func);
        }
      }

      var listAllColumns = functionUsedList.SelectMany(f => f.Columns).Distinct().ToList();
      return listAllColumns.Where(c => c.Used).ToList();
    }

    /// <summary>
    /// This function fills the list of Suspect/Finals of the subranges and send back the List of Columns
    /// </summary>
    /// <param name="listsr">The listsr.</param>
    /// <returns>List of columns.</returns>
    public List<Column> GetFilteredData(IEnumerable<SubRange> listsr, Fonction_Action action)
    {
      // using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.GetFilteredData)}"))
      {
        this.BatData.SelectFonction(action);
        string sName;
        int typeFonction;
        bool bUsed;
        int idFunction;
        List<Function> functionUsedList = new List<Function>();

        while ((idFunction = this.BatData.GetFonctionSuivante(out sName, out typeFonction, out bUsed)) != -1)
        {
          if (bUsed)
          {
            int sig = this.BatData.GetSignatureFonction(idFunction);
            int actionFunction = this.GetActionFonction(typeFonction);
            int typeSignal = this.GetTypeSignal(typeFonction);
            var func = new Function() { ID = idFunction, Name = sName, FunctionType = typeFonction, Action = actionFunction, TypeSignal = typeSignal, Signature = (DetectorSignature)sig };

            // Récupérer le nombre de valeurs pour la fonction demandee
            int iNbValeurs;
            string[] paColNames;
            bool[] paColUsed;
            this.BatData.GetInfosResultatsFonction(func.ID, out iNbValeurs, out paColNames, out paColUsed);

            for (int icol = 0; icol < paColNames.Length; icol++)
            {
              this.BatData.GetCompleteInfoColumnFonction(func.ID, icol, out TypeCol typeCol, out string colName, out var iParam1, out var iParam2, out var dOffset);

              func.Columns.Add(new Column()
              {
                Function = func,
                Name = paColNames[icol],
                Param1 = iParam1,
                Param2 = iParam2,
                Used = paColUsed[icol],
                Offset = dOffset,
                Type = typeCol,
              });
            }

            functionUsedList.Add(func);
          }
        }

        var listAllColumns = functionUsedList.SelectMany(f => f.Columns).Distinct().ToList();
        var listUsedColumns = listAllColumns.Where(c => c.Used).ToList();
        foreach (SubRange sb in listsr)
        {
          if (action == Fonction_Action.A_FINAUX)
          {
            sb.FinalList.Clear();
          }
          else if (action == Fonction_Action.A_SUSPECTS)
          {
            sb.SuspectList.Clear();
          }
          else
          {
            break;
          }

          foreach (var func in functionUsedList)
          {
            this.BatData.FastSelectResultatsSBFonction(func.ID, sb.Id);

            int idPointFreq = this.BatData.FastGetIdResultatSuivant();
            while (idPointFreq != -1)
            {
              object[] vals = new object[listUsedColumns.Count];
              double[] doubleVals = new double[listUsedColumns.Count];
              TypeCol[] typeVals = new TypeCol[listUsedColumns.Count];
              List<string> resultat = new List<string>();
              List<double> resultatDouble = new List<double>();
              for (int i = 0; i < func.Columns.Count; i++)
              {
                if (func.Columns[i].Used)
                {
                  string val = this.BatData.GetResultatString(idPointFreq, func.ID, i);
                  if (val.IsNumeric())
                  {
                    if (val.ToDouble() <= UndefVal)
                    {
                      val = string.Empty;
                    }
                  }

                  var val2 = this.BatData.GetResultatDouble(idPointFreq, func.ID, i);

                  if (func.Columns[i].Type == TypeCol.TYPECOL_MONITORING)
                  {
                    if (!func.Columns[i].Name.Contains("(dB"))
                    {
                      // Si Monitoring avec une unité en dB quelque chose alors on traite comme des doubles sinon formatage scientifique
                      string format = VisuV4Config.Instance.TableValuesSignificantFigures == 0 ? "0E0" : $"{Enumerable.Range(0, VisuV4Config.Instance.TableValuesSignificantFigures).Aggregate("0.", (s, j) => s + "#")}E0";
                      val = val2.ToString(format);
                    }
                  }

                  resultatDouble.Add(val2 <= UndefVal ? double.NaN : val2);

                  resultat.Add(val);
                }
                else
                {
                  resultat.Add(string.Empty);
                  resultatDouble.Add(double.NaN);
                }
              }

              int nbCol = func.Columns.Count;

              for (int i = 0; i < resultat.Count; i++)
              {
                var res = resultat[i];
                if (func.Columns[i].Used && i < func.Columns.Count)
                {
                  var key = func.Columns[i];
                  int indexOf = listUsedColumns.FindIndex(c => c == key);

                  if (key.Type == TypeCol.TYPECOL_LIMITE_RELATIVE || key.Type == TypeCol.TYPECOL_MESURE || key.Type == TypeCol.TYPECOL_LIMITE)
                  {
                    if (!string.IsNullOrEmpty(this.GetUniteEssai(func.TypeSignal)))
                    {
                      if (!key.Name.EndsWith("(" + this.GetUniteEssai(func.TypeSignal) + ")") &&
                        !key.Name.EndsWith("[" + this.GetUniteEssai(func.TypeSignal) + "]"))
                      {
                        key.Name += " (" + this.GetUniteEssai(func.TypeSignal) + ")";
                      }
                    }
                  }

                  if (key.Type == TypeCol.TYPECOL_RBW)
                  {
                    if (!key.Name.EndsWith("(Hz)") && !key.Name.EndsWith("[Hz]"))
                    {
                      key.Name += " (Hz)";
                    }
                  }

                  if (key.Type == TypeCol.TYPECOL_MEAS_TIME)
                  {
                    if (!key.Name.EndsWith("(s)") && !key.Name.EndsWith("[s]"))
                    {
                      key.Name += " (s)";
                    }
                  }

                  if (key.Type == TypeCol.TYPECOL_DELTA)
                  {
                    if (!key.Name.EndsWith("(dB)") && !key.Name.EndsWith("[dB]"))
                    {
                      key.Name += " (dB)";
                    }
                  }

                  vals[indexOf] = res;
                  doubleVals[indexOf] = resultatDouble[i];
                  typeVals[indexOf] = key.Type;
                }
              }

              int posId = Array.IndexOf(typeVals, TypeCol.TYPECOL_POSITION);
              if (action == Fonction_Action.A_FINAUX)
              {
                Final finalObj = new Final();
                finalObj.Function = func;
                finalObj.IdPoint = idPointFreq;
                finalObj.Frequency = this.BatData.GetFrequenceMesure(idPointFreq, func.ID); // Hz
                finalObj.Position = posId >= 0 ? vals[posId].ToString() : string.Empty;
                foreach (var item in vals)
                {
                  finalObj.Values.Add(item == null ? string.Empty : item.ToString());
                }

                finalObj.DoubleValues.AddRange(doubleVals);

                sb.FinalList.Add(finalObj);
              }
              else if (action == Fonction_Action.A_SUSPECTS)
              {
                Suspect suspectObj = new Suspect();
                suspectObj.Function = func;
                suspectObj.IdPoint = idPointFreq;
                suspectObj.Frequency = this.BatData.GetFrequenceMesure(idPointFreq, func.ID); // Hz
                suspectObj.Position = posId >= 0 ? vals[posId].ToString() : string.Empty;
                foreach (var item in vals)
                {
                  suspectObj.Values.Add(item == null ? string.Empty : item.ToString());
                }

                suspectObj.DoubleValues.AddRange(doubleVals);

                sb.SuspectList.Add(suspectObj);
              }

              idPointFreq = this.BatData.FastGetIdResultatSuivant();
            }
          }
        }

        return listUsedColumns.ToList();
      }
    }

    public bool TerminerDLL(Test_State state)
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.TerminerDLL)}"))
      {
        this.Pilote3.Terminer();
        return this.BatData.Terminer(state);
      }
    }

    /// <summary>
    /// Gets the finals.
    /// </summary>
    /// <param name="listsr">The listsr.</param>
    /// <returns>List of columns.</returns>
    public List<Column> GetFinals(IEnumerable<SubRange> listsr)
    {
      return this.GetFilteredData(listsr, Fonction_Action.A_FINAUX);
    }

    public List<Column> GetSuspects(IEnumerable<SubRange> listsr)
    {
      return this.GetFilteredData(listsr, Fonction_Action.A_SUSPECTS);
    }

    public List<SubRange> GetSubRanges()
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.GetSubRanges)}"))
      {
        var listSR = new List<SubRange>();
        this.BatData.SelectSB(-1, string.Empty, true);

        // Select the first sub range
        int idSB = this.BatData.GetSBSuivante();
        while (idSB != -1)
        {
          // Information about SB
          string s_NomScriptPosition = string.Empty;
          string s_NomTypePosition = string.Empty;

          int stateSB = this.BatData.GetInfosSB(idSB, out double fMin, out double fMax, out double fPas,
            out TypesProgression iTypeProgress, out int nbPoint, out s_NomScriptPosition, out s_NomTypePosition);
          string sbName = this.BatData.GetNomSB(idSB);
          Guid guidSB = this.BatData.GetGuidSB(idSB);
          int idPos = this.BatData.GetPositionSB(idSB);

          var sb = new SubRange()
          {
            Id = idSB,
            GuidSB = guidSB,
            Name = sbName,
            FMin = fMin * 1e6,
            FMax = fMax * 1e6,
            FStep = fPas * 1e6,
            ProgressionType = iTypeProgress,
            Count = nbPoint,
            PositionScriptName = s_NomScriptPosition,
            PositionScriptId = idPos,
            PositionTypeName = s_NomTypePosition,
            State = (SB_State)stateSB,
          };

          foreach (var item in SubRangeProvider.Instance.GetFunctions(sb.GuidSB))
          {
            sb.FunctionList.Add(item);
          }

          listSR.Add(sb);

          // Select the next sub range
          idSB = this.BatData.GetSBSuivante();
        }

        return listSR;
      }
    }

    public Test GetTestById(Guid testId, int numEssai, string resultPath)
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.GetTestById)}"))
      {
        log.Debug("Start GetTestById");
        Instance.GetInfoEssai(testId, out int tid, out string nameEssai);

        var test = new Test()
        {
          TestId = testId,
          Name = nameEssai,
          TestType = (ObjectType)tid,
          NumEssai = numEssai,
          ResultPath = resultPath,
        };

        test.Unit = Instance.GetUniteEssai(testId);
        test.UnitBL = Instance.GetUniteEssaiBL(testId);

        test.Conclusion = Instance.GetConclusion(testId);

        this.GetLabels(test);
        this.GetLabelValues(test);

        bool ret = Instance.BatData.Initialiser(test.TestId.ToString("B"), true, test.NumEssai, test.ResultPath, true);

        var repo = new Domain.Repository.TestResultRepository();
        test.Result = repo.GetById(test.TestId);

        GraphicOptionHelper graphicHelper = new GraphicOptionHelper(test.Result.Options, test.TestId);

        // Chargement SubRanges
        test.SubRangeList.Clear();
        test.SubRangeList = Instance.GetSubRanges();
        Instance.GetPrescans(test.SubRangeList);

        Instance.GetSuspects(test.SubRangeList);
        Instance.GetFinals(test.SubRangeList);

        // suppression des marqueurs dont le point associé n'existe plus
        var existingMarkers = test.Result.MarkersList.Where(marker =>
          test.SubRangeList.Any(range => range.GuidSB == marker.SubRangeID
                                                  && range.PrescanList.Any(prescan => prescan.ValuesAny(tuple => Math.Abs(tuple.x - marker.Frequency) < 0.001)))).ToList();
        test.Result.MarkersList.Clear();
        test.Result.MarkersList.AddRange(existingMarkers);

        test.Markers = existingMarkers;

        List<ExternalCurve> curves = new List<ExternalCurve>();
        curves = Instance.GetCourbesExt();

        test.ExternalCurves = curves;

        log.Debug("End GetTestById");
        return test;
      }
    }

    public EUT GetEUTById(Guid id)
    {
      var followup = new FollowUp() { Labels = FollowUpProvider.GetLabels(id, (int)ObjectType.EUT) };
      FollowUpProvider.GetLabelValues(id, followup);
      return new EUT(id, this.GetEUTNameById(id), followup);
    }

    public string GetEUTNameById(Guid id)
    {
      string query = $"SELECT sName FROM g_ListeObjets WHERE guidObject={id.ToSQL()}";

      using (var reader = DataBase.Instance.ExecuteReader(query))
      {
        if (reader.Read())
        {
          return reader.GetString(0);
        }
      }

      return string.Empty;
    }

    public Test GetTestByFile(string path)
    {
      log.Debug("Start GetTestById");

      this.BatData.InitialiserFromFile(path);
      var testId = Guid.Parse(this.BatData.GetEssaiGuid());
      this.BatData.GetInfosEssai(out var name, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _);
      var test = new Test()
      {
        TestId = testId,
        Name = name,
        TestType = (ObjectType)this.BatData.GetIdTypeEssai(),
        NumEssai = this.BatData.GetIDEssai(),
        ResultPath = Path.GetDirectoryName(path),
      };

      test.Unit = Instance.GetUniteEssai(testId);
      test.UnitBL = Instance.GetUniteEssaiBL(testId);

      test.Conclusion = Instance.GetConclusion(testId);

      this.GetLabels(test);
      this.GetLabelValues(test);

      bool ret = Instance.BatData.Initialiser(test.TestId.ToString("B"), true, test.NumEssai, test.ResultPath, false);

      var repo = new Domain.Repository.TestResultRepository();
      test.Result = repo.GetById(test.TestId);

      GraphicOptionHelper graphicHelper = new GraphicOptionHelper(test.Result.Options, test.TestId);

      // Chargement SubRanges
      test.SubRangeList.Clear();
      test.SubRangeList = Instance.GetSubRanges();
      Instance.GetPrescans(test.SubRangeList);

      Instance.GetSuspects(test.SubRangeList);
      Instance.GetFinals(test.SubRangeList);

      // suppression des marqueurs dont le point associé n'existe plus
      var existingMarkers = test.Result.MarkersList.Where(marker =>
                                                            test.SubRangeList.Any(range => range.GuidSB == marker.SubRangeID
                                                           && range.PrescanList.Any(prescan => prescan.ValuesAny(tuple => Math.Abs(tuple.x - marker.Frequency) < 0.001)))).ToList();
      test.Result.MarkersList.Clear();
      test.Result.MarkersList.AddRange(existingMarkers);

      test.Markers = existingMarkers;

      List<ExternalCurve> curves = new List<ExternalCurve>();
      curves = Instance.GetCourbesExt();

      test.ExternalCurves = curves;

      log.Debug("End GetTestById");
      return test;
    }

    /// <summary>
    /// Update the prescan list in each subRange of the list.
    /// </summary>
    /// <param name="srList">The sub-range list.</param>
    public void GetPrescans(List<SubRange> srList)
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.GetPrescans)}"))
      {
        this.BatData.SelectFonction(Fonction_Action.A_PRESCAN);
        string sName;
        int typeFonction;
        bool bUsed;
        int idFunction;
        List<Function> functionUsedList = new List<Function>();
        while ((idFunction = this.BatData.GetFonctionSuivante(out sName, out typeFonction, out bUsed)) != -1)
        {
          if (bUsed)
          {
            int sig = this.BatData.GetSignatureFonction(idFunction);

            int actionFunction = this.GetActionFonction(typeFonction);
            int typeSignal = this.GetTypeSignal(typeFonction);
            var func = new Function() { ID = idFunction, Name = sName, FunctionType = typeFonction, Action = actionFunction, TypeSignal = typeSignal, Signature = (DetectorSignature)sig };
            functionUsedList.Add(func);
          }
        }

        foreach (var sb in srList)
        {
          this.BatData.GetIdFreqSousbande(sb.Id, out int idFreqMin, out int nbFreq);

          int idFreqMax = idFreqMin + nbFreq - 1;
          lock (sb.PrescanList)
          {
            foreach (var func in functionUsedList)
            {
              int icol = 0;
              while (this.BatData.GetInfoColumnFonction(func.ID, icol, out TypeCol typeCol, out string colName) >= 0)
              {
                var prescan = sb.PrescanList.FirstOrDefault(m => m.Name == colName);
                if (prescan == null)
                {
                  prescan = new Prescan() { Function = func, Name = colName };
                  sb.PrescanList.Add(prescan);
                }

                if (typeCol == TypeCol.TYPECOL_MESURE)
                {
                  for (int id = idFreqMin; id <= idFreqMax; id++)
                  {
                    var resultat = this.BatData.GetResultat(id, func.ID);
                    if (resultat != null)
                    {
                      double frequency = this.BatData.GetFrequence(id); // Hz
                      var pt = (frequency, resultat[0] == UndefVal ? double.NaN : resultat[0]);
                      if (resultat[0] > UndefVal)
                      {
                        prescan.AddPoint(pt, id);
                      }
                    }
                  }
                }
                else
                {
                  1.ToString();
                }

                icol++;
              }
            }
          }
        }
      }
    }

    public void SaveTest(Test test)
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.SaveTest)}"))
      {
        if (test.TestId == Guid.Empty)
        {
          return;
        }

        // update test name
        var transaction = DataBase.Instance.BeginTransaction();
        try
        {
          FollowUpProvider.SaveLabels(test.TestId, test.FollowUp.Labels, transaction);
          DataBase.Instance.CommitTransaction(transaction);

          Instance.SetConclusion(test.TestId, test.Conclusion);
        }
        catch (Exception ex)
        {
          log.Error(ex);
          try
          {
            DataBase.Instance.RollBackTransaction(transaction);
          }
          catch (Exception ex2)
          {
            log.Error(ex2);
            throw;
          }

          throw;
        }
        finally
        {
          DataBase.Instance.Reconnect();
        }
      }
    }

    public bool GetCommentaireResultat(int idPoint, int function, out string comment)
    {
      return this.BatData.GetCommentaireResultat(idPoint, function, out comment);
    }

    public double GetDistanceSB(int idSB)
    {
      this.BatData.GetPositionInitialeSB(idSB, out double distance, out int _, out bool _, out double _, out bool _, out double _);
      return distance;
    }

    public List<ExternalCurve> GetCourbesExt()
    {
      using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.GetCourbesExt)}"))
      {
        var listCurves = new List<ExternalCurve>();

        this.BatData.SelectCourbesExt();
        int idCourbe;
        while ((idCourbe = this.BatData.GetCourbeExtSuivante()) != -1)
        {
          List<ExternalCurve> curves = this.AjouterCourbeExt(idCourbe);
          listCurves.AddRange(curves);
        }

        return listCurves;
      }
    }

    public void SetCommentaireResultat(int idPoint, int fonction, string commentaire)
    {
      this.BatData.SetCommentaireResultat(idPoint, fonction, commentaire);
    }

    /// <summary>
    /// Renvoi la liste des limites disponibles selon BatData. /!\ Les limites renvoyées sont en plusieurs parties : les limites ayant plusieurs classes, distances, détecteurs, etc. seront renvoyées autant de fois qu'il y a de différences avec le même ID
    /// </summary>
    /// <returns>Limites de BAT-Data à plat</returns>
    public List<Limit> GetLimits()
    {
      var limits = new List<Limit>();
      string szNom, szClasse, szDesc;
      int iSignature, iTypeSignal, iTypeInterpolation;
      double fDistance;

      int iNbVisuLimite = this.BatData.GetNbVisuLimite();
      for (int indexLimite = 0; indexLimite < iNbVisuLimite; indexLimite++)
      {
        string guidLimite;
        bool bVisible = true;

        this.BatData.GetInfoVisuLimiteDesc(indexLimite, out szNom, out szClasse,
                                            out iSignature, out fDistance, out iTypeSignal,
                                            out guidLimite, out bVisible,
                                            out iTypeInterpolation, out szDesc);

        if (!this.BatData.GetLimiteClasseId(indexLimite, out var classeId))
        {
          classeId = -1;
        }

        var limit = new Limit()
        {
          Name = szNom,
          Detector = (Detector)iSignature,
          Distance = fDistance,
          Classe = szClasse,
          Interpolation = (Nexio.Enums.InterpolationEnum)iTypeInterpolation,
          Guid = guidLimite,
          Id = Guid.Parse(guidLimite),
          TypeSignal = iTypeSignal,
          IsVisible = bVisible,
          ClasseId = classeId,
          Index = indexLimite,
          Description = szDesc,
        };

        limits.Add(limit);
      }

      return limits;
    }

    public bool GetLimiteNiveaux(int indexLimite, out int iNbVal, out double[] fFreq, out double[] fNiveaux)
    {
      return this.BatData.GetVisuLimiteNiveaux(indexLimite, out iNbVal, out fFreq, out fNiveaux);
    }

    public string GetUniteEssai(int typeSignal)
    {
      return this.BatData.GetUniteEssai(typeSignal);
    }

    public string GetUniteEssai(Guid idTest)
    {
      return GrammarProvider.Instance.GetUniteEssai(idTest);
    }

    public string GetUniteEssaiBL(Guid idTest)
    {
      return GrammarProvider.Instance.GetUniteEssaiBL(idTest);
    }

    public void GetInfoEssai(Guid id, out int idTypeObject, out string name)
    {
      GrammarProvider.Instance.GetInfoObject(id, out name, out _, out idTypeObject);
    }

    public int GetNumEssai(Guid idTest)
    {
      return GrammarProvider.Instance.GetNumEssai(idTest);
    }

    public Conclusion GetConclusion(Guid testId)
    {
      return TestRepository.GetConclusion(testId);
    }

    // Extaction de l'action de type fonction
    public int GetActionFonction(int typeFonction)
    {
      return typeFonction & 0x0F;
    }

    // Extraction du type de signal
    public int GetTypeSignal(int typeFonction)
    {
      return (typeFonction >> 4) & 0x0F;
    }

    /// <summary>
    /// Recharge la liste des sous bandes depuis bat_data.
    /// Ajoute les sous-bandes créés
    /// Retire les sous-bandes retirées (non testé)
    /// </summary>
    /// <param name="test">l'essai à recharger</param>
    public void UpdateSubRanges(Test test)
    {
      var dataSR = this.GetSubRanges();
      var srToAdd = dataSR.Where(sr => !test.SubRangeList.Any(sb => sb.GuidSB == sr.GuidSB)).ToList();
      var srToRemove = test.SubRangeList.Where(sb => !dataSR.Any(sr => sb.GuidSB == sr.GuidSB)).ToList();

      // Inits srToAdd
      Instance.GetPrescans(srToAdd);

      Instance.GetSuspects(srToAdd);
      Instance.GetFinals(srToAdd);

      foreach (var sr in srToRemove)
      {
        test.SubRangeList.Remove(sr);
      }

      foreach (var sr in srToAdd)
      {
        test.SubRangeList.Add(sr);
      }
    }

    public List<Position> GetAllPositions()
    {
      var query = @"SELECT DISTINCT sDescription as Name , idCommande as IdScript
                    FROM g_DEFScriptPosition";
      var lst = new List<Position>();

      using (var reader = DataBase.Instance.ExecuteReader(query))
      {
        while (reader.Read())
        {
          int id = System.Convert.ToInt32(reader.GetInt32(1));
          if (id != this.CUSTOM_LINE)
          {
            lst.Add(new Position(reader.GetInt32(1), reader["Name"].ToString()));
          }
        }
      }

      // Load custom line names
      // SD_Ligne_Custom = 32767,
      // g_DEFScriptPosition : idTypeObjet : 137, 138, sTypePosition 'Line'
      string clPositionRequest = @"SELECT DISTINCT SBA.sNomCustomLine, OBJ1.idTypeObject
                                  FROM (g_ListeObjets OBJ
                                  INNER JOIN g_SousBandes SBA ON OBJ.guidObject = SBA.guidSousBande)
                                  LEFT OUTER JOIN g_ListeObjets OBJ1 ON OBJ.guidParent = OBJ1.guidObject
                                  WHERE iScriptPosition=32767";

      using (var reader = DataBase.Instance.ExecuteReader(clPositionRequest))
      {
        int id = this.CUSTOM_LINE;
        while (reader.Read())
        {
          lst.Add(new Position(id, reader["sNomCustomLine"].ToString()));
        }
      }

      return lst;
    }

    private void SetConclusion(Guid testId, Conclusion conclusion)
    {
      TestRepository.SetConclusion(testId, conclusion);
    }

    private void GetLabels(Test test)
    {
      test.FollowUp.Labels = FollowUpProvider.GetLabels(test.TestId, (int)test.TestType);
    }

    private void GetLabelValues(Test test)
    {
      FollowUpProvider.GetLabelValues(test.TestId, test.FollowUp);
    }

    private List<ExternalCurve> AjouterCourbeExt(int idCourbeExt)
    {
      // using (new NexioStopwatch($"{nameof(Provider)}.{nameof(this.AjouterCourbeExt)}"))
      {
        string sNomCourbe;
        int iPositionCourbe;
        int iTypeSignalCourbe;
        List<int> listeIdSB;
        double fFreq, fValeur;
        int iNbResultats;
        List<int> pListePos;
        List<string> pListeNomPos;
        List<ExternalCurve> tIndexInfoPhase1 = new List<ExternalCurve>();
        bool bPhase2 = false;

        // Récupère la liste des positions de l'essai
        this.BatData.GetListePositions(out var positions);
        var functions = pListePos = new List<int>();
        pListeNomPos = new List<string>();

        if (positions.All(tuple => tuple.Pos != 0))
        {
          positions.Add((0, "H/V"));
        }

        positions.ForEach(p => pListePos.Add(p.Pos));
        positions.ForEach(p => pListeNomPos.Add(p.Name));

        // Récupère les infos sur la courbe
        Guid guidEssaiSource;
        if (!this.BatData.GetInfoCourbeExt(idCourbeExt, out sNomCourbe, out iTypeSignalCourbe, out guidEssaiSource))
        {
          return null;
        }

        //////////////////////////////////////////////////////////////////////////////////
        // Phase 1: on crée les courbes pour les positions autres que aucune (s'il y en a)
        //////////////////////////////////////////////////////////////////////////////////

        // Création des courbes pour toutes les positions
        this.BatData.SelectPositionCourbeExt(idCourbeExt);

        while ((iPositionCourbe = this.BatData.GetNextPositionCourbeExt(out string posName)) != -1)
        {
          bool bFound = false;
          for (int i = 0; !bFound && i < pListePos.Count; i++)
          {
            if (pListePos[i] == iPositionCourbe)
            {
              if (iPositionCourbe != 32767 || pListeNomPos[i] == posName)
              {
                bFound = true;
              }
            }
          }

          var externalCurve = new ExternalCurve
          {
            Id = idCourbeExt,
            Name = sNomCourbe,
            GuidSource = guidEssaiSource,
            // Function = GetFunctionById(Fonction_Action.A_COURBEEXT, iTypeSignalCourbe),
          };

          // On ne traite la position 0 en phase 1 que si c'est une position de l'essai
          if (iPositionCourbe != 0 || bFound)
          {
            if (!pListePos.Contains(iPositionCourbe) || (iPositionCourbe == 32767 && !pListeNomPos.Contains(posName)))
            {
              string nom = posName;
              // INC0017706 MC 15/06/2022 -- On ajoute les noms de positions manquants pour éviter un crash
              if (!string.IsNullOrEmpty(nom))
              {
                pListeNomPos.Add(nom);
                pListePos.Add(iPositionCourbe);
              }
            }

            var tag = CurveTag.FromExternalCurve(externalCurve, iPositionCourbe, posName, pListeNomPos, pListePos, iTypeSignalCourbe);

            externalCurve.Tag = tag;

            // Crée la liste des SB qui correspondent à la position de la courbe
            listeIdSB = new List<int>();
            if (!this.BatData.SelectSB(-1, string.Empty, true))
            {
              this.BatData.UnSelectSB();
              this.BatData.SelectSB(-1, string.Empty, true);
            }

            listeIdSB.Clear();
            int idSB = this.BatData.GetSBSuivante();
            while (idSB != -1)
            {
              listeIdSB.Add(idSB);
              idSB = this.BatData.GetSBSuivante();
            }

            this.BatData.UnSelectSB();

            if (listeIdSB.Count >= 0)
            {
              // Sélectionne les résultats pour cette position
              iNbResultats = this.BatData.SelectValeursCourbeExt();

              if (iNbResultats > 0)
              {
                // Crée la courbe
                // La courbe sera visible pour les Sb qui correspondent à la position
                for (int i = 0; i < listeIdSB.Count; i++)
                {
                  externalCurve.ListIdSB.Add(listeIdSB[i]);
                }

                bool bNewVersion = false;

                double dLastFreq = -1, dPasFreq = 0, dLastPas = 0;

                // Ajout des données
                // Apparement, quand la fréquence vaut -1, c'est une discontinuité ?
                // Mais quand la fréquence suivante et très superieure à l'actuelle, c'est aussi une discontinuité ?
                // Il faut mettre idPoint à -10 pour signifier la discontinuité
                int idPoint = 0;
                while (this.BatData.GetNextValeurCourbeExt(out fFreq, out fValeur))
                {
                  if (fFreq == 0)
                  {
                    bNewVersion = true;
                  }
                  else
                  {
                    if (dLastFreq == -1)
                    {
                      dLastFreq = fFreq;
                    }

                    if (dPasFreq != 0)
                    {
                      dLastPas = dPasFreq;
                    }

                    dPasFreq = fFreq - dLastFreq;
                    if (((bNewVersion == true) && (fFreq <= 0)) || ((bNewVersion == false) && (fFreq < 0)))
                    {
                      externalCurve.AddPoint(-10, dLastFreq * 1e6, double.NaN);

                      // OR le 15/10/2009 il y a une discontinuite mais avec des frequences inferieurs
                      if (dPasFreq < 0)
                      {
                        dPasFreq = 0;
                      }

                      bNewVersion = false;
                    }
                    else
                    {
                      // OR le 15/10/2009 il y a une discontinuite mais avec des frequences inferieurs
                      // fFreq > dLastFreq*1.05 ça donne quoi avec une courbe importée mesurée sur une liste de fréquence discrètes ?
                      // AA le 29/04/2019 -- INC0012706 -- Modification de dLastFreq*1.05 en dLastFreq*1.2 pour augmenter le pas avant de de considérer une discontinuité
                      // à tester en profondeur si des problèmes sur la discontinuité sur les courbes importées apparaissent après cette modification
                      // AA le 10/05/2019 -- INC0012748 -- Modification de dLastFreq*1.2 en dLastFreq*1.25
                      // CM le 01/04/2021 -- INC0015163 -- Discontinuité constatée lors de faibles valeurs (90kHz => 180kHz)  => voir à changer la mécanique des discontinuités lors de l'import de courbes (mettre la freq avec undef_value)
                      if ((bNewVersion == false) && dLastPas != 0 && ((fFreq > (dLastFreq + (dPasFreq * 1.35))) || (dPasFreq < 0)))
                      {
                        externalCurve.AddPoint(-1, (dLastFreq + dLastPas) * 1e6, double.NaN);

                        // OR le 15/10/2009 il y a une discontinuité mais avec des fréquences inférieures
                        if (dPasFreq < 0)
                        {
                          dPasFreq = 0;
                        }
                      }

                      // Ajouter les points à la courbe
                      externalCurve.AddPoint(idPoint++, fFreq * 1e6, fValeur);
                      dLastFreq = fFreq;
                      bNewVersion = false;
                    }
                  }
                }

                tIndexInfoPhase1.Add(externalCurve);
              } // if iNbResultats
            } // if ListeIdSB
          }
          else
          { // La position 0 sera traitée en phase 2
            bPhase2 = true;
          }
        } // while iPosition

        //////////////////////////////////////////////////////////////////////////////////
        // Phase 2: on ajoute les courbes de la position 0 à celles créées en phase 1
        //////////////////////////////////////////////////////////////////////////////////
        ///// Pas de phase 2 si position 0 et si on a créé des courbes en phase 1
        if (bPhase2)
        {
          // Recherche des SB en position 0
          listeIdSB = new List<int>();
          if (!this.BatData.SelectSB(0, string.Empty, true))
          {
            this.BatData.UnSelectSB();
            this.BatData.SelectSB(0, string.Empty, true);
          }

          listeIdSB.Clear();
          int idSB = this.BatData.GetSBSuivante();
          while (idSB != -1)
          {
            listeIdSB.Add(idSB);
            idSB = this.BatData.GetSBSuivante();
          }

          this.BatData.UnSelectSB();

          if (listeIdSB.Count >= 0)
          {
            // Recherche de la position 0
            this.BatData.SelectPositionCourbeExt(idCourbeExt);
            while ((iPositionCourbe = this.BatData.GetNextPositionCourbeExt(out string posName)) != -1)
            {
              // On ne traite que la position 0 les autres on été créées en phase 1
              if (iPositionCourbe == 0)
              {
                // Sélectionne les résultats pour cette position
                iNbResultats = this.BatData.SelectValeursCourbeExt();
                if (iNbResultats > 0)
                {
                  int iCourbe;

                  // Création des courbes à partir des InfoCourbe de la phase 1
                  for (iCourbe = 0; iCourbe < tIndexInfoPhase1.Count; iCourbe++)
                  {
                    ExternalCurve externalCurve = tIndexInfoPhase1[iCourbe];

                    // La courbe sera visible pour les SB qui correspondent à la position
                    for (int i = 0; i < listeIdSB.Count; i++)
                    {
                      externalCurve.ListIdSB.Add(listeIdSB[i]);
                    }
                  }

                  double dLastFreq = -1, dPasFreq = 0, dLastPas = 0;

                  bool bNewVersion = false;

                  // Ajout des données
                  while (this.BatData.GetNextValeurCourbeExt(out fFreq, out fValeur))
                  {
                    if (fFreq == 0)
                    {
                      bNewVersion = true;
                    }
                    else
                    {
                      if (dLastFreq == -1)
                      {
                        dLastFreq = fFreq;
                      }

                      if (dPasFreq != 0)
                      {
                        dLastPas = dPasFreq;
                      }

                      dPasFreq = fFreq - dLastFreq;

                      // Apparemment, quand la fréquence vaut -1, c'est une discontinuité ?
                      // Mais quand la fréquence suivante et très superieure à l'actuelle, c'est aussi une discontinuité ?
                      // Il faut mettre idPoint à -10 pour signifier la discontinuité
                      for (iCourbe = 0; iCourbe < tIndexInfoPhase1.Count; iCourbe++)
                      {
                        ExternalCurve external = tIndexInfoPhase1[iCourbe];
                        if (((bNewVersion == true) && (fFreq <= 0)) || ((bNewVersion == false) && (fFreq < 0)))
                        {
                          external.AddPoint(-10, dLastFreq + dLastPas, double.NaN);

                          // OR le 15/10/2009 il y a une discontinuité mais avec des fréquences inférieurs
                          if (dPasFreq < 0)
                          {
                            dPasFreq = 0;
                          }

                          bNewVersion = false;
                        }
                        else
                        {
                          // OR le 27/01/2006 : l'idPoint -10 correspond au courbe importé mais avec une discontinuité
                          // dPasFreq > 2*dLastPas ça donne quoi avec une courbe importée mesurée sur une liste de fréquence discrètes ?
                          if ((bNewVersion == false) && dLastPas != 0 && dPasFreq > 2 * dLastPas)
                          {
                            external.AddPoint(-10, dLastFreq + dLastPas, double.NaN);

                            // OR le 15/10/2009 il y a une discontinuité mais avec des fréquences inférieurs
                            if (dPasFreq < 0)
                            {
                              dPasFreq = 0;
                            }
                          }

                          // Ajouter les points aux courbes
                          external.AddPoint(-1, fFreq * 1e6, fValeur);
                          dLastFreq = fFreq;
                          bNewVersion = false;
                        }
                      }
                    }
                  }
                } // if iNbResultats
              } // if iPositionCourbe==0
            } // while iPositionCourbe
          } // if ListeIdSB
        }

        return tIndexInfoPhase1;
      }
    }
  }
}