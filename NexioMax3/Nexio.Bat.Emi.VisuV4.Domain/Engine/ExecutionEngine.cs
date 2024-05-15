namespace NexioMax3.Domain.Engine
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Timers;
  using System.Windows;
  using NexioMax3.Domain.Model;
  using NexioMax3.Domain.Model.Execution;
  using NexioMax3.Domain.Service;
  using NexioMax3.Domain.Wrapper;
  using Nexio.Helper;
  using Nexio;

  public enum EngineRunState
  {
    Stop,

    Pause,

    Running,
  }

  public class ExecutionEngine
  {
    public static readonly int VISU_ID_TOUS = 65535;

    public static readonly int VISU_ID_RANGE = VISU_ID_TOUS - 1;

    private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ExecutionEngine));

    private static UserInteractionLogger interactionLogger;

    private int idSBAuto = -1;

    private bool bTermineOperateur;

    private int idSousBande = -1;

    private double fFreqMin;

    private double fFreqMax;

    // Execution Range only
    private double dRangeFreqMin;

    private double dRangeFreqMax;

    private bool bFonctionActive = false;

    private bool bExecRange;

    private FunctionDLL pFonction;

    private string sMessage;

    private ProgressInfo currentProgressInfo;

    private IProgress<ProgressInfo> currentProgress;

    private EngineRunState runState = EngineRunState.Stop;

    private double freqMinExecManu;

    private double freqMaxExecManu;

    private string nomDllManu;

    private List<int> listeIdSB;

    private bool pauseRequest = false;

    private bool stopRequest = false;

    private IFunctionDialogProvider dialogProvider;

    public ExecutionEngine(IFunctionDialogProvider dialogProvider)
    {
      this.dialogProvider = dialogProvider;
      this.RunState = EngineRunState.Stop;
      this.SetModeExecAutomatique(VISU_ID_TOUS, 0, 0);
    }

    public event EventHandler<int> OnPause;

    public event EventHandler<int> OnResume;

    public event EventHandler<int> OnStop;

    public event EventHandler OnAutoExecFinished;

    public event EventHandler OnAutoExecStopped;

    public event EventHandler OnStartExecution;

    public event EventHandler OnManualExecFinished;

    public event EventHandler RunStateChanged;

    public Task RunningTask { get; set; }

    public EngineRunState RunState
    {
      get => this.runState;

      private set
      {
        if (this.runState == value)
        {
          return;
        }

        if (value == EngineRunState.Running)
        {
          this.bTermineOperateur = false;
        }

        this.runState = value;
        this.RaiseRunStateChanged();
      }
    }

    public bool FonctionActive => this.bFonctionActive;

    public async Task<bool> RunProcedureManualInterval(List<int> subrangesToExecute, List<SubRangeSettings> settings, IntPtr parent, bool isGlobalProcedure, bool newSubranges, System.IProgress<ProgressInfo> progress, CancellationToken cancel)
    {
      Provider.Instance.BatData.GetConfEM(); // force le OpenDatabase sur l'essai
      this.OnStartExecution?.Invoke(this, null);
      this.stopRequest = false;
      this.RunState = EngineRunState.Running;
      await this.ExecutionManuelleIntervalle(subrangesToExecute, settings, parent, newSubranges, progress);
      this.RunState = EngineRunState.Stop;
      var listSR = Provider.Instance.GetSubRanges().Where(sr => subrangesToExecute.Contains(sr.Id));
      return listSR.All(s => s.State == SB_State.SB_TERMINEE);
    }

    public async Task<bool> RunProcedureAutomatic(List<int> subrangesToExecute, IntPtr parent, bool isGlobalProcedure, System.IProgress<ProgressInfo> progress, CancellationToken cancel)
    {
      Provider.Instance.BatData.GetConfEM(); // force le OpenDatabase sur l'essai
      this.OnStartExecution?.Invoke(this, null);
      this.stopRequest = false;
      this.RunState = EngineRunState.Running;
      if (isGlobalProcedure)
      {
        await this.RunAutomatiqueGlobale(subrangesToExecute, parent, progress);
      }
      else
      {
        await this.RunAutomatique(subrangesToExecute, parent, progress);
      }

      this.RunState = EngineRunState.Stop;
      var listSR = Provider.Instance.GetSubRanges().Where(sr => subrangesToExecute.Contains(sr.Id));
      return listSR.All(s => s.State == SB_State.SB_TERMINEE);
    }

    public void Stop()
    {
      this.bTermineOperateur = true;
      this.stopRequest = true;
      if (this.pFonction != null)
      {
        this.pFonction?.Terminer();
      }

      this.OnStop?.Invoke(this, this.idSousBande);
    }

    public void Pause()
    {
      this.RunState = EngineRunState.Pause;
      this.pauseRequest = true;
      if (this.pFonction != null)
      {
        this.currentProgressInfo?.SetMessage("Pause");
        this.currentProgress?.Report(this.currentProgressInfo);
        this.pFonction?.Suspendre();
      }

      this.OnPause?.Invoke(this, this.idSousBande);
    }

    public void Resume()
    {
      this.RunState = EngineRunState.Running;
      this.pauseRequest = false;
      if (this.pFonction != null)
      {
        this.currentProgressInfo?.SetMessage("Resume");
        this.currentProgress?.Report(this.currentProgressInfo);
        this.pFonction?.Reprendre();
      }

      this.OnResume?.Invoke(this, this.idSousBande);
    }

    public void SetModeExecManuel(string prescanDll, double fmin, double fmax, IEnumerable<int> listeIdSB)
    {
      this.freqMinExecManu = fmin;
      this.freqMaxExecManu = fmax;
      this.nomDllManu = prescanDll;
      this.listeIdSB = listeIdSB.ToList();
    }

    public void SetValidationAuto(bool isValidationAuto)
    {
      if (isValidationAuto)
      {
        Provider.Instance.Pilote3.SetIsValidAuto(true);
        Provider.Instance.Pilote3.SetExecutionAuto(true);
        Provider.Instance.Pilote3.SetExecutionAutoFinaux(true);
      }
    }

    public void SetModeExecAutomatique(int idSBAuto, double dFreqMin, double dFreqMax)
    {
      this.idSBAuto = idSBAuto;
      this.dRangeFreqMin = dFreqMin;
      this.dRangeFreqMax = dFreqMax;
      this.bExecRange = idSBAuto == VISU_ID_RANGE;
    }

    private static void LogInteraction(string message)
    {
      (interactionLogger ?? (interactionLogger = Application.Current.Properties["InteractionLogger"] as UserInteractionLogger))?.Log(message);
    }

    private static bool CompareParams(List<string> params1, List<string> params2)
    {
      if (params1.Count != params2.Count)
      {
        return false;
      }

      for (int i = 0; i < params1.Count; i++)
      {
        if (params1[i] != params2[i])
        {
          return false;
        }
      }

      return true;
    }

    private void RaiseRunStateChanged()
    {
      this.RunStateChanged?.Invoke(this, new EventArgs());
    }

    private void RunScript(string sScript, IntPtr parent)
    {
      string sPathScript;

      if (!string.IsNullOrEmpty(sScript))
      {
        string pathEmi = ExecPathHelper.GetExecDirectory();
        sPathScript = string.Format("{0}\\scripts\\{1}", pathEmi, sScript);
        if (this.ExecuterScript(sPathScript, parent) != 1)
        {
          int i = 0;
          i++;
          // sMessage.Format("Erreur execution script: %s", sScript);
          // AfxMessageBox(sMessage);
        }
      }
    }

    private async Task ExecutionManuelleIntervalle(List<int> subrangesToExecute, List<SubRangeSettings> settings, IntPtr parent, bool additionalSR, System.IProgress<ProgressInfo> prog)
    {
      this.OnStartExecution?.Invoke(this, null);

      string pathEmi = ExecPathHelper.GetExecDirectory();
      string currentDllName = string.Empty;
      string sNomScript;
      int iModeExecution;
      int iNbPointsExecutes;

      var timer = new System.Timers.Timer(1000);
      timer.Elapsed += this.Timer_Elapsed;

      this.currentProgressInfo = new ProgressInfo();
      this.currentProgressInfo.Start();
      this.currentProgress = prog;
      this.currentProgress.Report(this.currentProgressInfo);

      Provider.Instance.BatData.ResetEventAffiche();

      // Charger la DLL
      // Pour indiquer à la BD d'affichage que les services
      // de la fonction ne peuvent pas être utilisées
      this.bFonctionActive = false;

      // m_ProgressDlg.SetFonction(null);
      // RAZDEL(m_pFonction);
      if (this.pFonction != null)
      {
        // this.pFonction.FreeLibrary();
        this.pFonction = null;
      }

      // this.pFonction = new FunctionDLL();

      // Sauvegarde du nom de la DLL chargée
      currentDllName = this.nomDllManu;

      // Charger la DLL
      var dlgs = this.dialogProvider.GetDialogs(this.nomDllManu);
      bool bRet = await Task.Run(() =>
                                 {
                                   this.pFonction =
                                            FunctionDLL.Get(Path.Combine(pathEmi, this.nomDllManu), parent, dlgs,
                                                            out var initialized);
                                   return initialized;
                                 });

      LogInteraction($"Loading function {currentDllName}");
      // Si le chargement s'est mal déroulé
      if (!bRet)
      {
        // Affichage d'un message dans la BD d'évolution
        // strFormat.LoadString(VISU_IDS_ERREUR_INIT_FONCTION);
        // m_sMessage.Format(strFormat, szNomDLL);
        // AfxMessageBox(m_sMessage);
        this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ErrorInitFunction0, this.nomDllManu));
        this.currentProgress.Report(this.currentProgressInfo);
        return;
      }

      // SINON DLL chargée correctement
      timer.Start();
      this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ExecuteFunction0, this.nomDllManu));
      this.currentProgress.Report(this.currentProgressInfo);

      // Paramètres à afficher sur l'exécution de la 1ère SB
      iModeExecution = 2; // Mode manuel, exécution de la 1ère SB
      if (Provider.Instance.BatData.IsGlobalProcedure() && this.pFonction.IsGlobalExecute())
      {
        // Utilisation de l'exécution globale si elle est gérée par la dll
        this.currentProgressInfo.IsProgression = !this.pFonction.HasProgressDlg();
        this.currentProgress.Report(this.currentProgressInfo);

        this.bFonctionActive = true;

        iNbPointsExecutes = this.pFonction.GlobalExecute(subrangesToExecute.ToArray(), this.freqMinExecManu, this.freqMaxExecManu, false);

        this.bFonctionActive = false;

        this.currentProgressInfo.SetMessage("Updating graph...");
        this.currentProgress.Report(this.currentProgressInfo);
      }
      else
      {
        // Sinon exécution SB par SB (ancien fonctionnement)
        for (int iSB = 0; (!this.bTermineOperateur) && (iSB < subrangesToExecute.Count); iSB++)
        {
          this.currentProgress.Report(this.currentProgressInfo);

          // Id de la sous bande pour l'affichage
          this.idSousBande = subrangesToExecute[iSB];
          var srSettings = settings[iSB];
          this.currentProgress.Report(this.currentProgressInfo);

          // Fréquences minimales et maximales de la sous bande
          this.fFreqMin = Provider.Instance.BatData.GetFreqMinSB(this.idSousBande);
          this.fFreqMax = Provider.Instance.BatData.GetFreqMaxSB(this.idSousBande);

          double freqMinToExecute = Math.Max(this.fFreqMin, this.freqMinExecManu);
          double freqMaxToExecute = Math.Min(this.fFreqMax, this.freqMaxExecManu);

          // Contrôle si les fréquences correspondent à la SB
          if ((freqMinToExecute < this.fFreqMax) && (freqMaxToExecute > this.fFreqMin))
          {
            this.bFonctionActive = true;

            this.currentProgressInfo.SetFreqRange(this.fFreqMin * 1e6, this.fFreqMax * 1e6);
            this.currentProgressInfo.SetFreqRangeTraitees(freqMinToExecute * 1e6, freqMaxToExecute * 1e6);
            this.currentProgressInfo.IsProgression = !this.pFonction.HasProgressDlg();
            this.currentProgress.Report(this.currentProgressInfo);

            // Exécution du script de début de SB
            // ====================================
            sNomScript = Provider.Instance.BatData.GetScriptDebutSB(this.idSousBande);
            this.RunScript(sNomScript, parent);

            // Exécution de la fonction
            // ==========================
            this.pFonction.SetTypeProgression(srSettings.TypeProgression);
            this.pFonction.SetTypeProgression(srSettings.TypeProgression);
            this.pFonction.SetStep(srSettings.Step);
            this.pFonction.InitMembresReglageTab(srSettings.Reglages);
            var beforeParams = this.pFonction.GetParams();
            iNbPointsExecutes = this.pFonction.ExecuteIntervalleFreq(freqMinToExecute, freqMaxToExecute, -1, this.idSousBande, iModeExecution, false);

            // Exécution du script de fin de SB
            // ====================================
            sNomScript = Provider.Instance.BatData.GetScriptFinSB(this.idSousBande);
            this.RunScript(sNomScript, parent);

            // Ne plus demander les paramètres pour les sous-bandes suivantes
            iModeExecution = 1;

            this.bFonctionActive = false;

            this.currentProgressInfo.SetMessage("Updating graph...");
            this.currentProgress.Report(this.currentProgressInfo);

            if (iNbPointsExecutes > 0)
            {
              if (additionalSR)
              {
                Provider.Instance.BatData.FinFonctionSB(this.idSousBande, -1); // idFonction est innutilisé
                var afterParams = this.pFonction.GetParams();
                if (!CompareParams(beforeParams, afterParams))
                {
                  var guidsb = Provider.Instance.BatData.GetGuidSB(this.idSousBande);
                  SubRangeDefinitionHelper.SaveFunctionParameters(this.nomDllManu, guidsb, afterParams);
                }
              }

              // Sélection de toutes les fonctions
              if (Provider.Instance.BatData.SelectDLLFonction(Fonction_Action.A_ALL))
              {
                bool idFct;
                string s_Intitule;
                string s_NomDLL;

                // Recherche de la fonction exécutée dans la DLL
                idFct = Provider.Instance.BatData.GetDLLFonctionSuivante(out s_Intitule, out s_NomDLL);
                while (idFct == true)
                {
                  // Si on a trouvé la fonction qui est executé
                  if (string.Compare(this.nomDllManu, s_NomDLL, true) == 0)
                  {
                    // Ecriture dans le fichier log
                    string strUniteMin, strUniteMax;

                    // Formate la borne min
                    double savFreqMin = this.freqMinExecManu;
                    this.FormatBorne(ref savFreqMin, out strUniteMin);

                    // Format la borne max
                    double savFreqMax = this.freqMaxExecManu;
                    this.FormatBorne(ref savFreqMax, out strUniteMax);

                    log.Debug(string.Format("Processing function:{0} between {1} {2} and {3} {4}\r\n", s_Intitule, savFreqMin, strUniteMin, savFreqMax, strUniteMax));
                  }

                  idFct = Provider.Instance.BatData.GetDLLFonctionSuivante(out s_Intitule, out s_NomDLL);
                }
              }

              // GD le 28/03/2007 : sauvegarde intermédiaire des résultats
              Provider.Instance.BatData.Terminer((Test_State)(-1));

              if (this.stopRequest)
              {
                // Arrêt demandé, on demande au driver de stopper la mesure et on sort
                this.pFonction?.Terminer();
                this.OnAutoExecStopped?.Invoke(this, null);
                timer.Stop();

                return;
              }
              else if (this.pauseRequest)
              {
                // Pause demandé
                // On demande au driver de suspendre la mesure et on attend la sortie de la pause ou l'arrêt
                this.pFonction?.Suspendre();
                this.currentProgressInfo.SetMessage("Pause");
                this.currentProgress.Report(this.currentProgressInfo);
                do
                {
                  System.Threading.Thread.Sleep(500);
                }
                while (!this.stopRequest && this.pauseRequest);

                if (this.stopRequest)
                {
                  // L'utilisateur abandonne => arrête la mesure et sort
                  this.currentProgressInfo.SetMessage("Stopped");
                  this.currentProgress.Report(this.currentProgressInfo);
                  this.pFonction?.Terminer();
                  this.OnAutoExecStopped?.Invoke(this, null);
                  timer.Stop();
                  return;
                }
                else
                {
                  // Reprise de la mesure
                  this.currentProgressInfo.SetMessage("Restart");
                  this.currentProgress.Report(this.currentProgressInfo);
                  this.pFonction.Reprendre();
                }
              }
            }

            // Si aucun point exécuté <=> annulation par l'utilisateur
            if (iNbPointsExecutes < 0)
            {
              if (additionalSR && iSB == 0)
              {
                // cas mesure additionelle pas encore commencé. On revert.
                var toRemove = subrangesToExecute.Select(id => Provider.Instance.BatData.GetGuidSB(id)).ToList();
                SubRangeDefinitionHelper.Rollback(toRemove);
              }

              break;
            }
          }
        }
      }

      this.currentProgressInfo.Finish();
      this.currentProgress.Report(this.currentProgressInfo);

      timer.Stop();

      this.OnManualExecFinished?.Invoke(this, null);
      log.Debug("Fin ExecutionManuel");

      if (this.pFonction != null)
      {
        // this.pFonction.FreeLibrary();
        this.pFonction = null;
      }
    }

    private void FormatBorne(ref double f, out string unite)
    {
      if (f < 0.001)
      {
        f *= 1e6f;
        unite = "Hz";
      }
      else if (f < 1)
      {
        f = f * 1e3f;
        unite = "KHz";
      }
      else if (f >= 1000)
      {
        f = f / 1e3f;
        unite = "GHz";
      }
      else
      {
        unite = "MHz";
      }
    }

    private int ExecuterScript(string scriptPath, IntPtr parent)
    {
      BatScriptWrapper script = new BatScriptWrapper(parent);
      return script.ExecuterScript(scriptPath);
    }

    private async Task RunAutomatiqueGlobale(List<int> subrangesToExecute, IntPtr parent, System.IProgress<ProgressInfo> progress)
    {
      Thread.CurrentThread.Name = "RunAutomaticGlobale";
      LogInteraction($"Start of globale procedure");
      log.Debug("Start RunAutomatiqueGlobale");
      string pathEmi = ExecPathHelper.GetExecDirectory();
      string currentDllName = string.Empty;
      string szNomDLL;
      bool bErreurExecution = false;
      List<int> listeSB = new List<int>();
      bool bContinue;

      var timer = new System.Timers.Timer(1000);
      timer.Elapsed += this.Timer_Elapsed;

      this.currentProgressInfo = new ProgressInfo();
      this.currentProgressInfo.Start();
      this.currentProgress = progress;
      this.currentProgress.Report(this.currentProgressInfo);

      Provider.Instance.BatData.ResetEventAffiche();

      // -1 = POS_ALL
      Provider.Instance.BatData.SelectSB(-1, string.Empty, true);
      this.fFreqMin = float.PositiveInfinity;
      this.fFreqMax = float.NegativeInfinity;

      List<(int idSb, string Dll)> execTracker = new List<(int idSb, string Dll)>();

      foreach (int idSB in subrangesToExecute)
      {
        if (this.idSBAuto == VISU_ID_TOUS || this.idSBAuto == VISU_ID_RANGE || idSB == this.idSBAuto)
        {
          listeSB.Add(idSB);
          int stateSB = Provider.Instance.BatData.GetInfosSB(idSB, out double fMin, out double fMax, out _, out _, out _, out _, out _);
          this.fFreqMin = (float)Math.Min(this.fFreqMin, fMin);
          this.fFreqMax = (float)Math.Max(this.fFreqMax, fMax);
        }
      }

      // while ((int idSB = Provider.Instance.BatData.GetSBSuivante()) != -1)
      // {
      //  if (this.idSBAuto == VISU_ID_TOUS || this.idSBAuto == VISU_ID_RANGE || idSB == this.idSBAuto)
      //  {
      //    listeSB.Add(idSB);
      //    int stateSB = Provider.Instance.BatData.GetInfosSB(idSB, out double fMin, out double fMax, out _, out _, out _, out _, out _);
      //    this.fFreqMin = (float)Math.Min(this.fFreqMin, fMin);
      //    this.fFreqMax = (float)Math.Max(this.fFreqMax, fMax);
      //  }
      // }

      // Recherche les traitement restant à exécuter en automatique (A_ALL => retourne traitement suivant)
      int idAction = (int)Fonction_Action.A_ALL;
      if (this.idSBAuto == VISU_ID_RANGE)
      {
        bContinue = Provider.Instance.BatData.GetGlobalProcedure(this.idSBAuto, this.dRangeFreqMin, this.dRangeFreqMax, ref idAction, out szNomDLL);
        this.fFreqMin = this.dRangeFreqMin;
        this.fFreqMax = this.dRangeFreqMax;
      }
      else
      {
        bContinue = Provider.Instance.BatData.GetGlobalProcedure(this.idSBAuto, 0, double.MaxValue, ref idAction, out szNomDLL);
      }

      LogInteraction($"Execution of {szNomDLL}, Range : {this.fFreqMin.ToStringUnit(3, "MHz")} - {this.fFreqMax.ToStringUnit(3, "MHz")}");

      bool loadDll = false;

      while (bContinue && !this.bTermineOperateur && !bErreurExecution)
      {
        // strFormat.LoadString(VISU_IDS_INIT_FONCTION);
        // m_sMessage.Format(strFormat, szNomDLL);
        // m_ProgressDlg.SetMessage(m_sMessage);
        this.currentProgressInfo.SetFreqRange(this.fFreqMin * 1e6, this.fFreqMax * 1e6);
        this.currentProgressInfo.SetFreqRangeTraitees(this.fFreqMin * 1e6, this.fFreqMax * 1e6);
        this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.InitFunction0, szNomDLL));
        this.currentProgress.Report(this.currentProgressInfo);

        // SI une nouvelle DLL doit être chargée
        if (string.Compare(currentDllName, szNomDLL, true) != 0)
        {
          // Pour indiquer à la BD d'affichage que les services
          // de la fonction ne peuvent pas être utilisées
          this.bFonctionActive = false;

          // m_ProgressDlg.SetFonction(null);
          if (this.pFonction != null)
          {
            // this.pFonction.FreeLibrary();
            this.pFonction = null;
          }

          loadDll = true; /* this.pFonction = new FunctionDLL();*/
        }

        if (this.pFonction != null || loadDll)
        {
          // Sauvegarde du nom de la DLL chargée
          currentDllName = szNomDLL;

          LogInteraction($"Loading function {currentDllName}");
          // Charger la DLL
          var dlgs = this.dialogProvider.GetDialogs(currentDllName);
          bool bRet = await Task.Run(() =>
                                     {
                                       Thread.CurrentThread.Name = "awaitInRunAutoGlobale";
                                       try
                                       {
                                         FunctionDLL.FreeDlls();
                                         this.pFonction = FunctionDLL.Get(Path.Combine(pathEmi, currentDllName), parent,
                                                                            dlgs, out var initialized);

                                         return initialized;
                                       }
                                       catch (Exception ex)
                                       {
                                         this.currentProgressInfo.SetMessage(ex.Message);
                                         this.currentProgress.Report(this.currentProgressInfo);
                                         log.Error(ex.Message);
                                         return false;
                                       }
                                     });

          // m_ProgressDlg.SetFonction(m_pFonction);

          // SI le chargement s'est mal déroulé
          if (!bRet)
          {
            // Affichage d'un message dans la BD d'évolution
            // strFormat.LoadString(VISU_IDS_ERREUR_INIT_FONCTION);
            // m_sMessage.Format(strFormat, szNomDLL);
            // AfxMessageBox(m_sMessage);
            // bErreurExecution = TRUE;
            bErreurExecution = true;
            log.ErrorFormat(NexioMax3.Domain.Properties.Resources.ErrorInitFunction0, szNomDLL);
            this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ErrorInitFunction0, szNomDLL));
            this.currentProgress.Report(this.currentProgressInfo);
          }
          else
          {
            // SINON DLL chargée correctement
            // strFormat.LoadString(VISU_IDS_EXECUTION_FONCTION);
            // m_sMessage.Format(strFormat, szNomDLL);
            // m_ProgressDlg.SetMessage(m_sMessage);
            timer.Start();
            this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ExecuteFunction0, szNomDLL));
            this.currentProgress.Report(this.currentProgressInfo);

            // Pour indiquer à la BD d'affichage que les services
            // de la fonction peuvent être utilisées
            this.bFonctionActive = true;

            // m_ProgressDlg.ShowProgress(this, !m_pFonction->HasProgressDlg());
            if ((this.dRangeFreqMin == 0) && (this.dRangeFreqMax == 0))
            {
              // Mode complet
              // Nombre de points de la sous bande
              int[] array = listeSB.Where(i => !execTracker.Any(tuple => tuple.idSb == i && tuple.Dll == szNomDLL)).ToArray();

              // si il n'y a pas de sous-bande qui reste à executé avec cette dll, on arrete l'execution
              if (array.Length == 0)
              {
                break;
              }

              if (this.pFonction.GlobalExecute(array, -double.MaxValue, double.MaxValue, true) <= 0)
              {
                bErreurExecution = true;
              }
            }
            else
            {
              // Mode Bande de Frequence
              // Nombre de points de la sous bande
              int[] array = listeSB.Where(i => !execTracker.Any(tuple => tuple.idSb == i && tuple.Dll == szNomDLL)).ToArray();

              // si il n'y a pas de sous-bande qui reste à executé avec cette dll, on arrete l'execution
              if (array.Length == 0)
              {
                break;
              }

              if (this.pFonction.GlobalExecute(array, this.dRangeFreqMin, this.dRangeFreqMax, true) <= 0)
              {
                bErreurExecution = true;
              }
            }

            this.bFonctionActive = false;

            this.currentProgressInfo.SetMessage("Update Graph");
            this.currentProgress.Report(this.currentProgressInfo);
          }

          this.sMessage = string.Empty;
        }

        // GD le 28/03/2007 : sauvegarde intermédiaire des résultats
        Provider.Instance.BatData.Terminer((Test_State)(-1));

        if (!bErreurExecution && idAction == (int)Fonction_Action.A_PRESCAN)
        {
          string sScript = Provider.Instance.BatData.GetScriptEndPrescan();
          if (!string.IsNullOrEmpty(sScript))
          {
            this.RunScript(sScript, parent);
          }
        }

        // Passer au traitement suivant
        idAction = (int)Fonction_Action.A_ALL;
        foreach (var i in listeSB)
        {
          if (((SB_State)Provider.Instance.BatData.GetEtatSB(i)) == SB_State.SB_TERMINEE)
          {
            execTracker.Add((i, szNomDLL));
          }
        }

        if (this.idSBAuto == VISU_ID_RANGE)
        {
          bContinue = Provider.Instance.BatData.GetGlobalProcedure(this.idSBAuto, this.dRangeFreqMin, this.dRangeFreqMax, ref idAction, out szNomDLL);
        }
        else
        {
          bContinue = Provider.Instance.BatData.GetGlobalProcedure(this.idSBAuto, 0, double.MaxValue, ref idAction, out szNomDLL);
        }

        log.Debug($"Global procedure continue={bContinue}");

        if (this.stopRequest)
        {
          // Arrêt demandé, on demande au driver de stopper la mesure et on sort
          this.pFonction?.Terminer();
          timer.Stop();

          return;
        }
        else if (this.pauseRequest)
        {
          // Pause demandé
          // On demande au driver de suspendre la mesure et on attend la sortie de la pause ou l'arrêt
          this.pFonction?.Suspendre();
          this.currentProgressInfo.SetMessage("Pause");
          this.currentProgress.Report(this.currentProgressInfo);
          do
          {
            System.Threading.Thread.Sleep(500);
          }
          while (!this.stopRequest && this.pauseRequest);

          if (this.stopRequest)
          {
            // L'utilisateur abandonne => arrête la mesure et sort
            this.currentProgressInfo.SetMessage("Stopped");
            this.currentProgress.Report(this.currentProgressInfo);
            this.pFonction?.Terminer();
            timer.Stop();

            return;
          }
          else
          {
            // Reprise de la mesure
            this.currentProgressInfo.SetMessage("Restart");
            this.currentProgress.Report(this.currentProgressInfo);
            this.pFonction?.Reprendre();
          }
        }
      }

      if (!bErreurExecution)
      {
        string sScript = Provider.Instance.BatData.GetScriptEndTest();
        if (!string.IsNullOrEmpty(sScript))
        {
          this.RunScript(sScript, parent);
        }
      }

      this.currentProgressInfo.Finish();
      this.currentProgress.Report(this.currentProgressInfo);

      timer.Stop();

      // m_ProgressDlg.SetFonction(NULL);
      // RAZDEL(m_pFonction);
      if (this.pFonction != null)
      {
        // this.pFonction.FreeLibrary();
        this.pFonction = null;
        loadDll = false;
      }

      if (!bErreurExecution)
      {
        this.OnAutoExecFinished?.Invoke(this, null);
      }
      else
      {
        this.OnAutoExecStopped?.Invoke(this, null);
      }

      log.Debug("End ExecutionGlobale");
    }

    private async Task RunAutomatique(List<int> subrangesToExecute, IntPtr parent, System.IProgress<ProgressInfo> prog)
    {
      log.Debug("Start RunAutomatique");
      LogInteraction($"Start of automatic procedure on {subrangesToExecute.Count} subranges");
      string pathEmi = ExecPathHelper.GetExecDirectory();
      string currentDllName = string.Empty;
      int idSB;
      bool bErreurExecution = false;
      int iSBCourante = -1;
      string sNomScript;

      var timer = new System.Timers.Timer(1000);
      timer.Elapsed += this.Timer_Elapsed;

      this.currentProgressInfo = new ProgressInfo();
      this.currentProgressInfo.Start();
      this.currentProgress = prog;
      this.currentProgress.Report(this.currentProgressInfo);

      Provider.Instance.BatData.ResetEventAffiche();
      var loadDll = false;
      bool bContinue = false;
      int curentSubrangePtr = 0;
      do
      {
        // Recherche les traitement restant à exécuter en automatique
        // m_idSBAuto contient l'id de la SB ou VISU_ID_TOUS si exécution complète
        idSB = subrangesToExecute[curentSubrangePtr];
        bContinue = Provider.Instance.BatData.GetTraitementSuivant(ref idSB, out int idPtFirst, out int idPtLast, out string szNomDLL, this.dRangeFreqMin, this.dRangeFreqMax);

        if (!bContinue)
        {
          curentSubrangePtr++;
          continue;
        }

        // Id de la sous bande pour l'affichage
        this.idSousBande = idSB;

        // Execution des scripts
        if (iSBCourante != this.idSousBande)
        {
          if (this.idSousBande > 0 && iSBCourante >= 0)
          {
            // si nouvelle SB
            // Execution du script de fin de SB
            sNomScript = Provider.Instance.BatData.GetScriptFinSB(iSBCourante);
            this.RunScript(sNomScript, parent);
          }

          // Execution du script de debut de SB suivante
          sNomScript = Provider.Instance.BatData.GetScriptDebutSB(this.idSousBande);
          this.RunScript(sNomScript, parent);

          iSBCourante = this.idSousBande;
        }

        this.currentProgress.Report(this.currentProgressInfo);

        // Affichage de la vue correspondant à la sous-bande
        // TODO : gerer les events messages vers l'UI
        // if (m_pParent != null)
        // {
        //    m_pParent->SendMessage(WM_AFFICHE_SB, IdSB);
        // }
        if (idSB == VISU_ID_RANGE)
        {
          // Fréquences du rang à executer
          this.fFreqMin = this.dRangeFreqMin;
          this.fFreqMax = this.dRangeFreqMax;
        }
        else
        {
          // Fréquences minimales et maximales de la sous bande
          this.fFreqMin = Provider.Instance.BatData.GetFreqMinSB(idSB);
          this.fFreqMax = Provider.Instance.BatData.GetFreqMaxSB(idSB);
        }

        // TODO : gestion du progress
        this.currentProgressInfo.SetFreqRange(this.fFreqMin * 1e6, this.fFreqMax * 1e6);
        this.currentProgressInfo.SetFreqRangeTraitees(this.fFreqMin * 1e6, this.fFreqMax * 1e6);
        this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.InitFunction0, szNomDLL));
        this.currentProgress.Report(this.currentProgressInfo);

        // SI une nouvelle DLL doit être chargée
        if (string.Compare(currentDllName, szNomDLL, true) != 0)
        {
          // Pour indiquer à la BD d'affichage que les services
          // de la fonction ne peuvent pas être utilisées
          this.bFonctionActive = false;

          // m_ProgressDlg.SetFonction(null);
          // RAZDEL(m_pFonction);
          if (this.pFonction != null)
          {
            // this.pFonction.FreeLibrary();
            this.pFonction = null;
          }

          loadDll = true; // this.pFonction = new FunctionDLL();
        }

        if (this.pFonction != null || loadDll)
        {
          // Sauvegarde du nom de la DLL chargée
          currentDllName = szNomDLL;

          // Charger la DLL
          var dlgs = this.dialogProvider.GetDialogs(currentDllName);
          LogInteraction($"Loading {currentDllName}");
          bool bRet = await Task.Run(() =>
                                     {
                                       this.pFonction = FunctionDLL.Get(Path.Combine(pathEmi, currentDllName), parent,
                                                                             dlgs, out var initSuccess);
                                       return initSuccess;
                                     });

          // m_ProgressDlg.SetFonction(m_pFonction);

          // SI le chargement s'est mal déroulé
          if (!bRet)
          {
            // Affichage d'un message dans la BD d'évolution
            // strFormat.LoadString(VISU_IDS_ERREUR_INIT_FONCTION);
            // m_sMessage.Format(strFormat, szNomDLL);
            // AfxMessageBox(m_sMessage);
            bErreurExecution = true;

            log.ErrorFormat(NexioMax3.Domain.Properties.Resources.ErrorInitFunction0, szNomDLL);
            this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ErrorInitFunction0, szNomDLL));
            this.currentProgress.Report(this.currentProgressInfo);
          }
          else
          {
            LogInteraction($"Execution of {szNomDLL}, Range : {this.fFreqMin.ToStringUnit(3, "MHz")} - {this.fFreqMax.ToStringUnit(3, "MHz")}");

            timer.Start();

            // SINON DLL chargée correctement
            this.currentProgressInfo.SetMessage(string.Format(NexioMax3.Domain.Properties.Resources.ExecuteFunction0, szNomDLL));
            this.currentProgress.Report(this.currentProgressInfo);

            // strFormat.LoadString(VISU_IDS_EXECUTION_FONCTION);
            // m_sMessage.Format(strFormat, szNomDLL);
            // m_ProgressDlg.SetMessage(m_sMessage);

            // Pour indiquer à la BD d'affichage que les services
            // de la fonction peuvent être utilisées
            this.bFonctionActive = true;

            // m_ProgressDlg.ShowProgress(this, !m_pFonction->HasProgressDlg());

            // Nombre de points de la sous bande
            if (this.pFonction.ExecuteIntervallePtFreq(idPtFirst, idPtLast, -1, idSB, 0, true) < 0)
            {
              bErreurExecution = true;
            }

            this.bFonctionActive = false;

            this.currentProgressInfo.SetMessage("Update Graph");
            this.currentProgress.Report(this.currentProgressInfo);

            // m_sMessage.LoadString(VISU_IDS_MAJGRAPHE);
            // m_ProgressDlg.SetMessage(m_sMessage);

            // m_ProgressDlg.ShowProgress(this, true);

            // Selection de toutes les fonctions
            if (Provider.Instance.BatData.SelectDLLFonction(Fonction_Action.A_ALL))
            {
              bool idFct;

              // Recherche de la fonction executer dans la DLL
              idFct = Provider.Instance.BatData.GetDLLFonctionSuivante(out string s_Intitule, out string s_NomDLL);
              while (idFct == true)
              {
                // Si on a trouve la fonction qui est executé
                if (szNomDLL == s_NomDLL)
                {
                  // TODO : log de freqmin freqmax
                  //// Ecriture dans le fichier log
                  // string strFonction;
                  // string strUniteMin, strUniteMax;

                  //// Formate la borne min
                  // double savFreqMin = this.m_fFreqMin;
                  // FormatBorne(&savFreqMin, &strUniteMin);

                  //// Format la borne max
                  // double savFreqMax = this.m_fFreqMax;
                  // FormatBorne(&savFreqMax, &strUniteMax);

                  // strFonction.Format(VISU_IDS_EXECUTION_FONCTION_LOG, s_Intitule, savFreqMin, strUniteMin, savFreqMax, strUniteMax);
                  // WriteFunctionToFileLog(strFonction);
                }

                idFct = Provider.Instance.BatData.GetDLLFonctionSuivante(out s_Intitule, out s_NomDLL);
              }
            }
          }
        }

        log.Debug("Traitement suivant ExecutionAutomatique");

        // GD le 28/03/2007 : sauvegarde intermédiaire des résultats
        Provider.Instance.BatData.Terminer((Test_State)(-1));

        if (this.stopRequest)
        {
          // Arrêt demandé, on demande au driver de stopper la mesure et on sort
          this.pFonction?.Terminer();
          this.OnAutoExecStopped?.Invoke(this, null);
          timer.Stop();

          return;
        }
        else if (this.pauseRequest)
        {
          // Pause demandé
          // On demande au driver de suspendre la mesure et on attend la sortie de la pause ou l'arrêt
          this.pFonction?.Suspendre();
          this.currentProgressInfo.SetMessage("Pause");
          this.currentProgress.Report(this.currentProgressInfo);
          do
          {
            System.Threading.Thread.Sleep(500);
          }
          while (!this.stopRequest && this.pauseRequest);

          if (this.stopRequest)
          {
            // L'utilisateur abandonne => arrête la mesure et sort
            this.currentProgressInfo.SetMessage("Stopped");
            this.currentProgress.Report(this.currentProgressInfo);
            this.pFonction?.Terminer();
            this.OnAutoExecStopped?.Invoke(this, null);
            timer.Stop();

            return;
          }
          else
          {
            // Reprise de la mesure
            this.currentProgressInfo.SetMessage("Restart");
            this.currentProgress.Report(this.currentProgressInfo);
            this.pFonction.Reprendre();
          }
        }
      }
      while (curentSubrangePtr < subrangesToExecute.Count && !this.bTermineOperateur && !bErreurExecution);

      // force la sauvegarde de l'état de la dernière SB
      Provider.Instance.BatData.Sauvegarder(false);

      // Exécution du script de fin de la dernière SB
      if (iSBCourante >= 0)
      {
        if (subrangesToExecute.Contains(iSBCourante))
        {
          sNomScript = Provider.Instance.BatData.GetScriptFinSB(iSBCourante);
          LogInteraction($"Running Script {sNomScript}");
          this.RunScript(sNomScript, parent);
        }
      }

      this.currentProgressInfo.Finish();
      this.currentProgress.Report(this.currentProgressInfo);

      timer.Stop();

      Provider.Instance.BatData.SetExecData(-1, 0, 0, -1);

      if (this.pFonction != null)
      {
        // this.pFonction.FreeLibrary();
        this.pFonction = null;
        loadDll = false;
      }

      if (bErreurExecution)
      {
        this.OnAutoExecStopped?.Invoke(this, null);
      }
      else
      {
        this.OnAutoExecFinished?.Invoke(this, null);
      }

      log.Debug("End RunAutomatique");
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (this.pFonction != null)
      {
        if (this.pFonction.GetFrequenceCourante != null)
        {
          var currentFreq = this.pFonction.GetFrequenceCourante();
          this.currentProgressInfo?.SetCurrentFreq(currentFreq * 1e6);
        }

        try
        {
          if (this.pFonction.GetNbBalayages != null)
          {
            var total = this.pFonction.GetNbBalayages();
            this.currentProgressInfo?.SetTotalBalayage(total);

            if (this.pFonction.GetBalayage != null)
            {
              var balayage = this.pFonction.GetBalayage();
              this.currentProgressInfo?.SetCurrentBalayage(balayage);
            }
          }
        }
        catch (Exception exception)
        {
          log.Error(exception);

          throw;
        }

        if (this.currentProgressInfo != null)
        {
          this.currentProgress.Report(this.currentProgressInfo);
        }
      }
    }
  }
}