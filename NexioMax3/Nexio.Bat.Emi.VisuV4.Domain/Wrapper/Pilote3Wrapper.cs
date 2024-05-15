namespace Nexio.Bat.Emi.VisuV4.Domain.Wrapper
{
  using System;
  using System.Runtime.InteropServices;
  using System.Text;
  using Nexio.Helper;

  public class Pilote3Wrapper
  {
    private readonly Native native;

    /// <summary>
    /// The this handle
    /// </summary>
    private GCHandle thisHandle;

    public Pilote3Wrapper()
    {
#if DEBUG
      System.Diagnostics.Debug.Assert(++RefCounter == 1, "trop  de références de Pilote3");
#endif
      this.native = new Native();
      this.native.MontageChanged += this.Native_MontageChanged;
      this.native.PositionChanged += this.Native_PositionChanged;
      this.native.InitialiserFonctionDLL();
      this.thisHandle = GCHandle.Alloc(this);
    }

    public event EventHandler MontageChanged;

    public event EventHandler PositionChanged;

#if DEBUG
    public static int RefCounter { get; set; }
#endif

    public void Terminer()
    {
      this.native.Terminer();
    }

    public string GetNameMontage()
    {
      var chPtr = this.native.GetNameMontage();
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public int GetPositionCourante()
    {
      return this.native.GetPositionCourante();
    }

    public long ChangerPosition(int iTypePos, double dPosP1, double dPosP2 = 0, bool bNow = true)
    {
      return this.native.ChangerPosition(iTypePos, dPosP1, dPosP2, bNow);
    }

    public long Motion_Start(int iScriptMes, double dValue, int iSpeed, int iMotionCtx = 0)
    {
      return this.native.Motion_Start(iScriptMes, dValue, false, iSpeed, iMotionCtx); // bShowProgress à false car on ne veut pas afficher la progression par le pilote
    }

    public bool GetMast(out Guid mastGuid, out string mastName)
    {
      StringBuilder sbName = new StringBuilder(256);
      StringBuilder sbGuid = new StringBuilder(256);
      bool res = this.native.GetMast(sbGuid, sbName);
      if (res)
      {
        string guidStr = Convert.ToString(sbGuid);
        mastName = Convert.ToString(sbName);
        mastGuid = Guid.Parse(guidStr);
      }
      else
      {
        mastGuid = Guid.Empty;
        mastName = string.Empty;
      }

      return res;
    }

    public bool IsMastUsed(out Guid mastGuid, out string mastName, int heightIndex)
    {
      StringBuilder sbName = new StringBuilder(256);
      StringBuilder sbGuid = new StringBuilder(256);
      bool res = this.native.IsMastIdUsed(heightIndex, sbGuid, sbName);
      if (res)
      {
        string guidStr = Convert.ToString(sbGuid);
        mastName = Convert.ToString(sbName);
        mastGuid = Guid.Parse(guidStr);
      }
      else
      {
        mastGuid = Guid.Empty;
        mastName = string.Empty;
      }

      return res;
    }

    public bool GetTable(out Guid tableGuid, out string tableName)
    {
      StringBuilder sbName = new StringBuilder(256);
      StringBuilder sbGuid = new StringBuilder(256);
      bool res = this.native.GetTable(sbGuid, sbName);
      if (res)
      {
        string guidStr = Convert.ToString(sbGuid);
        tableName = Convert.ToString(sbName);
        tableGuid = Guid.Parse(guidStr);
      }
      else
      {
        tableGuid = Guid.Empty;
        tableName = string.Empty;
      }

      return res;
    }

    public bool IsTurntableIdUsed(int id, out Guid tableGuid, out string tableName)
    {
      StringBuilder sbName = new StringBuilder(256);
      StringBuilder sbGuid = new StringBuilder(256);
      bool res = this.native.IsTurntableIdUsed(id, sbGuid, sbName);
      if (res)
      {
        string guidStr = Convert.ToString(sbGuid);
        tableName = Convert.ToString(sbName);
        tableGuid = Guid.Parse(guidStr);
      }
      else
      {
        tableGuid = Guid.Empty;
        tableName = string.Empty;
      }

      return res;
    }

    public bool IsMastIdUsed(int id, out Guid tableGuid, out string tableName)
    {
      StringBuilder sbName = new StringBuilder(256);
      StringBuilder sbGuid = new StringBuilder(256);
      bool res = this.native.IsMastIdUsed(id, sbGuid, sbName);
      if (res)
      {
        string guidStr = Convert.ToString(sbGuid);
        tableName = Convert.ToString(sbName);
        tableGuid = Guid.Parse(guidStr);
      }
      else
      {
        tableGuid = Guid.Empty;
        tableName = string.Empty;
      }

      return res;
    }

    public void MesurerPosition(int iTypePos, out double dNewPos)
    {
      this.native.MesurerPosition(iTypePos, out dNewPos);
    }

    public bool GetCurrentPosition(int iTypePos, out double dNewPos)
    {
      return this.native.GetCurrentPosition(iTypePos, out dNewPos);
    }

    public void GetSpeedPosition(int iTypePos, out double dSpeed)
    {
      this.native.GetSpeedPosition(iTypePos, out dSpeed);
    }

    public void SetResultPath(string path)
    {
      this.native.SetResultPath(path);
    }

    public Guid GetGuidMontage()
    {
      var chPtr = this.native.GetGuidMontage();
      string guidStr = Marshal.PtrToStringAnsi(chPtr);
      if (!string.IsNullOrEmpty(guidStr))
      {
        return Guid.Parse(guidStr);
      }
      else
      {
        return Guid.Empty;
      }
    }

    public long PreparerDomaine(double frequence, Guid guidTransducteur, bool bAskDomaineSB)
    {
      return this.native.PreparerDomaine(frequence, guidTransducteur, bAskDomaineSB);
    }

    public long PreparerDomaineCRBM(double frequence)
    {
      return this.native.PreparerDomaineCRBM(frequence);
    }

    public long PreparerDomaineTestEM()
    {
      return this.native.PreparerDomaineTestEM();
    }

    public long GetFacteurAntenne(double frequence, double pFacteur, int iChaine, int iPositionNSA = -1)
    {
      return this.native.GetFacteurAntenne(frequence, pFacteur, iChaine, iPositionNSA);
    }

    public int GetIdTypeTransducteur()
    {
      return this.native.GetIdTypeTransducteur();
    }

    public long Marche()
    {
      return this.native.Marche();
    }

    public long Arret()
    {
      return this.native.Arret();
    }

    public long LancerSweep(double dFreqStop, long lDuree)
    {
      return this.native.LancerSweep(dFreqStop, lDuree);
    }

    public long ArreterSweep()
    {
      return this.native.ArreterSweep();
    }

    public bool IsMesureAvecModulation()
    {
      return this.native.IsMesureAvecModulation();
    }

    public bool IsEmiMeasWithMovingAngle()
    {
      return this.native.IsEmiMeasWithMovingAngle();
    }

    public void SetEmiMeasWithMovingAngle(bool bMeasWithMovingAngle)
    {
      this.native.SetEmiMeasWithMovingAngle(bMeasWithMovingAngle);
    }

    public bool IsEmiGlobalPrescan()
    {
      return this.native.IsEmiGlobalPrescan();
    }

    public void SetEmiGlobalPrescan(bool bMeasWithMovingAngle)
    {
      this.native.SetEmiGlobalPrescan(bMeasWithMovingAngle);
    }

    public long InitSB_EMI(Guid guidSB, Guid guidMontage, ref double freqMin, ref double freqMax, out int pIdMesureur, bool bChaineAnnexe, bool bAskDomaineSB, int iEtatSB, int iPositionSB, bool useBackup = false, int idChaineReceiver = 10404)
    {
      var ret = this.native.InitSB_EMI(guidSB.ToString("B"), guidMontage.ToString("B"), ref freqMin, ref freqMax, out long lIdMesureur, bChaineAnnexe, bAskDomaineSB, iEtatSB, iPositionSB, useBackup, idChaineReceiver, IntPtr.Zero);
      pIdMesureur = (int)lIdMesureur;
      return ret;
    }

    public long VerifierRFout(double pdRFout, double dFmin, double dFmax, int iNbPts, int iTypeProgresion)
    {
      return this.native.VerifierRFout(pdRFout, dFmin, dFmax, iNbPts, iTypeProgresion);
    }

    public long CorrigerMesure(double frequence, double mesure, int iContext, int position = -1, int idChaine = 10404/*CD_MesEmission*/)
    {
      return this.native.CorrigerMesure(frequence, mesure, iContext, position, idChaine);
    }

    public long CorrigerMesureVectorielle(double dFreq, double pMesureReal, double pMesureImag)
    {
      return this.native.CorrigerMesureVectorielle(dFreq, pMesureReal, pMesureImag);
    }

    public long ExecuterScript(int iTypePos, double dPosP1, double dPosP2/*=0.*/, double dPosP3/*=0.*/)
    {
      return this.native.ExecuterScript(iTypePos, dPosP1, dPosP2, dPosP3);
    }

    public long ReglerMatPlateau(int iScriptMes, double dValue, bool bShowProgress)
    {
      return this.native.ReglerMatPlateau(iScriptMes, dValue, bShowProgress);
    }

    public long Motion_GetPos(out double pdPos, out bool pbMoving, int iMotionCtx = 0)
    {
      return this.native.Motion_GetPos(out pdPos, out pbMoving, iMotionCtx);
    }

    public int GetCurrentMotionScript()
    {
      return this.native.GetCurrentMotionScript();
    }

    public long Motion_Stop(int iMotionCtx = 0)
    {
      return this.native.Motion_Stop(iMotionCtx);
    }

    public string GetTexteErreur()
    {
      var chPtr = this.native.GetTexteErreur();
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public int GetAdresse(int typeMeta)
    {
      return this.native.GetAdresse(typeMeta);
    }

    public int GetGPIBInterfaceID(int typeMeta)
    {
      return this.native.GetGPIBInterfaceID(typeMeta);
    }

    public string GetAdresseText(int typeMeta)
    {
      return this.native.GetAdresseText(typeMeta);
    }

    public void SetMainWnd(IntPtr hMainWnd)
    {
      this.native.SetMainWnd(hMainWnd);
    }

    public int GetSerialSpeed(int typeMeta)
    {
      return this.native.GetSerialSpeed(typeMeta);
    }

    public int GetSerialParityBit(int typeMeta)
    {
      return this.native.GetSerialParityBit(typeMeta);
    }

    public int GetSerialStopBit(int typeMeta)
    {
      return this.native.GetSerialStopBit(typeMeta);
    }

    public int GetSerialDataBit(int typeMeta)
    {
      return this.native.GetSerialDataBit(typeMeta);
    }

    public bool GetSerialXonXoff(int typeMeta)
    {
      return this.native.GetSerialXonXoff(typeMeta);
    }

    public string GetEOSRead(int typeMeta)
    {
      return this.native.GetEOSRead(typeMeta);
    }

    public string GetEOSWrite(int typeMeta)
    {
      return this.native.GetEOSWrite(typeMeta);
    }

    public long GetFrequenceMinMontage(out double pFreq)
    {
      return this.native.GetFrequenceMinMontage(out pFreq);
    }

    public long GetFrequenceMaxMontage(out double pFreq)
    {
      return this.native.GetFrequenceMaxMontage(out pFreq);
    }

    public long GetFrequenceMinMontageEMI(out double pFreq)
    {
      return this.native.GetFrequenceMinMontageEMI(out pFreq);
    }

    public long GetFrequenceMaxMontageEMI(out double pFreq)
    {
      return this.native.GetFrequenceMaxMontageEMI(out pFreq);
    }

    public long GetGainChaine(double dFreq, int iChaine, double pPerteInsertion)
    {
      return this.native.GetGainChaine(dFreq, iChaine, pPerteInsertion);
    }

    public long GetNiveauMaxSortie(double dFreq, int iChaine, double pNiveauMaxSortie)
    {
      return this.native.GetNiveauMaxSortie(dFreq, iChaine, pNiveauMaxSortie);
    }

    public long InitDomaine(Guid guidEssai, long lTypeMontages, Guid guidMontage)
    {
      return this.native.InitDomaine(guidEssai.ToString("B"), lTypeMontages, guidMontage.ToString("B"));
    }

    public long Mesurer(double dFrequence, long iChaine, long iScript, double tValeurs, int iNbValeurs/*=1*/)
    {
      return this.native.Mesurer(dFrequence, iChaine, iScript, tValeurs, iNbValeurs);
    }

    public long ExecutionESD(Guid guidEssai, double dNiveau, int iEchantillon, int iTemps, Guid guidMontage, int iTypeInj)
    {
      return this.native.ExecutionESD(guidEssai, dNiveau, iEchantillon, iTemps, guidMontage, iTypeInj);
    }

    public void SetProtection(bool bProtect)
    {
      this.native.SetProtection(bProtect);
    }

    public void SetGuidEssaiCEME(Guid guidEssai)
    {
      this.native.SetGuidEssaiCEME(guidEssai);
    }

    public void SetExecutionAuto(bool bExecAuto/*=true*/)
    {
      this.native.SetExecutionAuto(bExecAuto);
    }

    public void SetExecutionAutoFinaux(bool bExecAuto/*=true*/)
    {
      this.native.SetExecutionAutoFinaux(bExecAuto);
    }

    public void TraceMesure(double dFreq, string sContexte)
    {
      this.native.TraceMesure(dFreq, sContexte);
    }

    public void RunScriptMateriel(Guid guidMateriel, int iIdScript, double dParam1, double dParam2, double dParam3)
    {
      this.native.RunScriptMateriel(guidMateriel, iIdScript, dParam1, dParam2, dParam3);
    }

    public int GetIScriptPostionCurrentSB()
    {
      return this.native.GetIScriptPostionCurrentSB();
    }

    public bool GetManuelMode()
    {
      return this.native.GetManuelMode();
    }

    public void SetManuelMode(bool bManuelMode)
    {
      this.native.SetManuelMode(bManuelMode);
    }

    public int GetIdReceiver(int idChaine = 10404/*CD_MesEmission*/)
    {
      return this.native.GetIdReceiver(idChaine);
    }

    public void SetIsValidAuto(bool isValidAuto)
    {
      this.native.SetIsValidAuto(isValidAuto);
    }

    public bool GetIsValidAuto()
    {
      return this.native.GetIsValidAuto();
    }

    public long InitSB_EMS(Guid guidSB, Guid guidMontage, int iRegulation, bool bConsAM, bool bCroissant, bool bMesPuissances, bool bMesPert, int nHarmoniques, int iEtatSB, bool bMesPertPuissCRBM, bool bMesPertPuiss)
    {
      return this.native.InitSB_EMS(guidSB.ToString("B"), guidMontage.ToString("B"), iRegulation, bConsAM, bCroissant, bMesPuissances, bMesPert, nHarmoniques, iEtatSB, bMesPertPuissCRBM, bMesPertPuiss);
    }

    public long ActiverFrequence(double frequence, double consigne, double seuilDepart, double dPGeneCalib, string modulation)
    {
      return this.native.ActiverFrequence(frequence, consigne, seuilDepart, dPGeneCalib, double.MaxValue, modulation);
    }

    public long GetMesuresPuissances(out double pPoutF, out double pPoutR, out double pPeqF, out double pPeqR, out double pGene, out double pPeqGene, bool bConsigne, bool bAllowAll)
    {
      return this.native.GetMesuresPuissances(out pPoutF, out pPoutR, out pPeqF, out pPeqR, out pGene, out pPeqGene, bConsigne, bAllowAll);
    }

    public void DechargerFonctionDLL()
    {
#if DEBUG
      System.Diagnostics.Debug.Assert(--RefCounter == 0, "trop  de références de Pilote3");
#endif
      this.native.DechargerFonctionDLL();
    }

    internal void InitialiserFonctionDLL()
    {
      this.native.InitialiserFonctionDLL();
    }

    protected void RaiseMontageChanged()
    {
      this.MontageChanged?.Invoke(this, new EventArgs());
    }

    protected void RaisePositionChanged()
    {
      this.PositionChanged?.Invoke(this, new EventArgs());
    }

    private void Native_PositionChanged(object sender, EventArgs e)
    {
      this.RaisePositionChanged();
    }

    private void Native_MontageChanged(object sender, EventArgs e)
    {
      this.RaiseMontageChanged();
    }

    public class Native
    {
      private IntPtr handlePilote3Library;

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void SetupChangeDelegate();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void PositionChangeDelegate();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetSetupChangeDelegate(IntPtr func);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetPositionChangeDelegate(IntPtr func);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      public delegate IntPtr Func_GetTexteErreur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_Terminer();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetNameMontage();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_InitSB_EMS(string guidSB, string guidMontage, int iRegulation, bool bConsAM, bool bCroissant, bool bMesPuissances, bool bMesPert, int nHarmoniques, int iEtatSB, bool bMesPertPuissCRBM, bool bMesPertPuiss);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_InitSB_EMS_CRBM(Guid guidSB, Guid guidMontage, bool bMesPertE, bool bMesPertP);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_PreparerDomaine(double frequence, Guid guidTransducteur, bool bAskDomaineSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_PreparerDomaineCRBM(double frequence);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_PreparerDomaineTestEM();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ActiverFrequence(double frequence, double consigne, double niveauDepart, double dPGeneCalib, double maxPeqF, string modulation);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_IncrementerNiveau(double dDeltaPeqGene, bool bMarcheOff/*=false*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesuresPuissances(out double pPoutF, out double pPoutR, out double pPeqF, out double pPeqR, out double pGene, out double pPeqGene, bool bConsigne, bool bAllowAll);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesuresPuissancesVSWR(double pPeqF, double pPeqR);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesuresSE(double pdPEut, double pdPRef_or_Field);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesuresHarmoniques(double ptdValues);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_GetNiveauGenerateur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesurePerturbation(double mesure, bool bAllowAllDevice);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesureCourant(double courant);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesurePerturbationPuissance(double mesure, bool bAllowAllDevice);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesureECRBM(double x, double y, double z, double t);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesurePCRBM(double pPuiss);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesuresPuissancesCRBM(double pPoutF, double pPoutR, double pPeqF, double pPeqR, double pGene);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetGainTransducteur(double frequence, double pGain);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetVSWR(double frequence, double pVswr);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetFacteurAntenne(double frequence, double pFacteur, int iChaine, int iPositionNSA);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate Guid Func_GetIdTransducteur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate Guid Func_GetIdTransducteurChaine(int idChaine);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdTypeTransducteur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Marche();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetPositionCourante();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetRawAxisFields(double x, double y, double z, double resultante);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetCorrectedAxisFields(double x, double y, double z, double resultante);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Arret();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_EtatGenerateur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_LancerSweep(double dFreqStop, long lDuree);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ArreterSweep();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsMesureAvecModulation();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsEmiMeasWithMovingAngle();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate void Func_SetEmiMeasWithMovingAngle(bool bMeasWithMovingAngle);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsEmiGlobalPrescan();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetEmiGlobalPrescan(bool bMeasWithMovingAngle);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_InitSB_EMI(string guidSB, string guidMontage, ref double freqMin, ref double freqMax, out long pIdMesureur, bool bChaineAnnexe, bool bAskDomaineSB, int iEtatSB, int iPositionSB, bool useBackup, int idChaineReceiver, IntPtr pidChainUtilise);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_VerifierRFout(double pdRFout, double dFmin, double dFmax, int iNbPts, int iTypeProgresion);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_CorrigerMesure(double frequence, double mesure, int iContext, int position, int idChaine);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_CorrigerMesureVectorielle(double dFreq, double pMesureReal, double pMesureImag);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ChangerPosition(int iTypePos, double dPosP1, double dPosP2/*=0.*/, bool bNow/*=true*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ExecuterScript(int iTypePos, double dPosP1, double dPosP2/*=0.*/, double dPosP3/*=0.*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_MesurerPosition(int iTypePos, out double dNewPos);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetCurrentPosition(int iTypePos, out double dNewPos);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetSpeedPosition(int iTypePos, out double dSpeed);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ReglerMatPlateau(int iScriptMes, double dValue, bool bShowProgress);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Motion_Start(int iScriptMes, double dValue, bool bShowProgress, int iSpeed, int iMotionCtx);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetMast([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guid, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder nom);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetTable([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guid, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder nom);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsTurntableIdUsed(int id, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guid, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder nom);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsMastIdUsed(int id, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guid, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder nom);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Motion_GetPos(out double pdPos, out bool pbMoving, int iMotionCtx);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Motion_Stop(int iMotionCtx);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetCurrentMotionScript();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetAdresse(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetGPIBInterfaceID(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetAdresseText(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetMainWnd(IntPtr hMainWnd);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSerialSpeed(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSerialParityBit(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSerialStopBit(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSerialDataBit(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetSerialXonXoff(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetEOSRead(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetEOSWrite(int typeMeta);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetFrequenceMinMontage(out double pFreq);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetFrequenceMaxMontage(out double pFreq);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetFrequenceMinMontageEMI(out double pFreq);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetFrequenceMaxMontageEMI(out double pFreq);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ReglerNiveauSUSB(int iNunero, double dGene);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetMesureNiveauSUSB(int iNumero, double pMesure);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Init55020(Guid guidEssai, Guid guidMontage);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetNiveauGeneSUSB(int iNumero, double pGene);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetPertePinceSUSB(double dFreq, double pPertePince);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetGainChaine(double dFreq, int iChaine, double pPerteInsertion);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_GetNiveauMaxSortie(double dFreq, int iChaine, double pNiveauMaxSortie);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_InitDomaine(string guidEssai, long lTypeMontages, string guidMontage);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_Mesurer(double dFrequence, long iChaine, long iScript, double tValeurs, int iNbValeurs/*=1*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate long Func_ExecutionESD(Guid guidEssai, double dNiveau, int iEchantillon, int iTemps, Guid guidMontage, int iTypeInj);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetProtection(bool bProtect);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetGuidEssaiCEME(Guid guidEssai);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetGuidMontage();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetExecutionAuto(bool bExecAuto/*=true*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetExecutionAutoFinaux(bool bExecAuto/*=true*/);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetResultPath(string pszPath);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_TraceMesure(double dFreq, string sContexte);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_RunScriptMateriel(Guid guidMateriel, int iIdScript, double dParam1, double dParam2, double dParam3);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIScriptPostionCurrentSB();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetManuelMode();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetManuelMode(bool bManuelMode);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdReceiver(int idChaine);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetIsValidAuto(bool isValidAuto);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_GetIsValidAuto();

      public event EventHandler MontageChanged;

      public event EventHandler PositionChanged;

      public Func_SetSetupChangeDelegate SetSetupChangeDelegate { get; private set; }

      public Func_SetPositionChangeDelegate SetPositionChangeDelegate { get; private set; }

      public Func_GetTexteErreur GetTexteErreur { get; private set; }

      public Func_Terminer Terminer { get; private set; }

      public Func_GetNameMontage GetNameMontage { get; private set; }

      public Func_InitSB_EMS InitSB_EMS { get; private set; }

      public Func_InitSB_EMS_CRBM InitSB_EMS_CRBM { get; private set; }

      public Func_PreparerDomaine PreparerDomaine { get; private set; }

      public Func_PreparerDomaineCRBM PreparerDomaineCRBM { get; private set; }

      public Func_PreparerDomaineTestEM PreparerDomaineTestEM { get; private set; }

      public Func_ActiverFrequence ActiverFrequence { get; private set; }

      public Func_IncrementerNiveau IncrementerNiveau { get; private set; }

      public Func_GetMesuresPuissances GetMesuresPuissances { get; private set; }

      public Func_GetMesuresPuissancesVSWR GetMesuresPuissancesVSWR { get; private set; }

      public Func_GetMesuresSE GetMesuresSE { get; private set; }

      public Func_GetMesuresHarmoniques GetMesuresHarmoniques { get; private set; }

      public Func_GetNiveauGenerateur GetNiveauGenerateur { get; private set; }

      public Func_GetMesurePerturbation GetMesurePerturbation { get; private set; }

      public Func_GetMesureCourant GetMesureCourant { get; private set; }

      public Func_GetMesurePerturbationPuissance GetMesurePerturbationPuissance { get; private set; }

      public Func_GetMesureECRBM GetMesureECRBM { get; private set; }

      public Func_GetMesurePCRBM GetMesurePCRBM { get; private set; }

      public Func_GetMesuresPuissancesCRBM GetMesuresPuissancesCRBM { get; private set; }

      public Func_GetGainTransducteur GetGainTransducteur { get; private set; }

      public Func_GetVSWR GetVSWR { get; private set; }

      public Func_GetFacteurAntenne GetFacteurAntenne { get; private set; }

      public Func_GetIdTransducteur GetIdTransducteur { get; private set; }

      public Func_GetIdTransducteurChaine GetIdTransducteurChaine { get; private set; }

      public Func_GetIdTypeTransducteur GetIdTypeTransducteur { get; private set; }

      public Func_Marche Marche { get; private set; }

      public Func_GetPositionCourante GetPositionCourante { get; private set; }

      public Func_GetRawAxisFields GetRawAxisFields { get; private set; }

      public Func_GetCorrectedAxisFields GetCorrectedAxisFields { get; private set; }

      public Func_Arret Arret { get; private set; }

      public Func_EtatGenerateur EtatGenerateur { get; private set; }

      public Func_LancerSweep LancerSweep { get; private set; }

      public Func_ArreterSweep ArreterSweep { get; private set; }

      public Func_IsMesureAvecModulation IsMesureAvecModulation { get; private set; }

      public Func_IsEmiMeasWithMovingAngle IsEmiMeasWithMovingAngle { get; private set; }

      public Func_SetEmiMeasWithMovingAngle SetEmiMeasWithMovingAngle { get; private set; }

      public Func_IsEmiGlobalPrescan IsEmiGlobalPrescan { get; private set; }

      public Func_SetEmiGlobalPrescan SetEmiGlobalPrescan { get; private set; }

      public Func_InitSB_EMI InitSB_EMI { get; private set; }

      public Func_VerifierRFout VerifierRFout { get; private set; }

      public Func_CorrigerMesure CorrigerMesure { get; private set; }

      public Func_CorrigerMesureVectorielle CorrigerMesureVectorielle { get; private set; }

      public Func_ChangerPosition ChangerPosition { get; private set; }

      public Func_ExecuterScript ExecuterScript { get; private set; }

      public Func_MesurerPosition MesurerPosition { get; private set; }

      public Func_GetCurrentPosition GetCurrentPosition { get; private set; }

      public Func_GetSpeedPosition GetSpeedPosition { get; private set; }

      public Func_ReglerMatPlateau ReglerMatPlateau { get; private set; }

      public Func_Motion_Start Motion_Start { get; private set; }

      public Func_GetMast GetMast { get; private set; }

      public Func_GetTable GetTable { get; private set; }

      public Func_IsTurntableIdUsed IsTurntableIdUsed { get; private set; }

      public Func_IsMastIdUsed IsMastIdUsed { get; private set; }

      public Func_Motion_GetPos Motion_GetPos { get; private set; }

      public Func_Motion_Stop Motion_Stop { get; private set; }

      public Func_GetCurrentMotionScript GetCurrentMotionScript { get; private set; }

      public Func_GetAdresse GetAdresse { get; private set; }

      public Func_GetGPIBInterfaceID GetGPIBInterfaceID { get; private set; }

      public Func_GetAdresseText GetAdresseText { get; private set; }

      public Func_SetMainWnd SetMainWnd { get; private set; }

      public Func_GetSerialSpeed GetSerialSpeed { get; private set; }

      public Func_GetSerialParityBit GetSerialParityBit { get; private set; }

      public Func_GetSerialStopBit GetSerialStopBit { get; private set; }

      public Func_GetSerialDataBit GetSerialDataBit { get; private set; }

      public Func_GetSerialXonXoff GetSerialXonXoff { get; private set; }

      public Func_GetEOSRead GetEOSRead { get; private set; }

      public Func_GetEOSWrite GetEOSWrite { get; private set; }

      public Func_GetFrequenceMinMontage GetFrequenceMinMontage { get; private set; }

      public Func_GetFrequenceMaxMontage GetFrequenceMaxMontage { get; private set; }

      public Func_GetFrequenceMinMontageEMI GetFrequenceMinMontageEMI { get; private set; }

      public Func_GetFrequenceMaxMontageEMI GetFrequenceMaxMontageEMI { get; private set; }

      public Func_ReglerNiveauSUSB ReglerNiveauSUSB { get; private set; }

      public Func_GetMesureNiveauSUSB GetMesureNiveauSUSB { get; private set; }

      public Func_Init55020 Init55020 { get; private set; }

      public Func_GetNiveauGeneSUSB GetNiveauGeneSUSB { get; private set; }

      public Func_GetPertePinceSUSB GetPertePinceSUSB { get; private set; }

      public Func_GetGainChaine GetGainChaine { get; private set; }

      public Func_GetNiveauMaxSortie GetNiveauMaxSortie { get; private set; }

      public Func_InitDomaine InitDomaine { get; private set; }

      public Func_Mesurer Mesurer { get; private set; }

      public Func_ExecutionESD ExecutionESD { get; private set; }

      public Func_SetProtection SetProtection { get; private set; }

      public Func_SetGuidEssaiCEME SetGuidEssaiCEME { get; private set; }

      public Func_GetGuidMontage GetGuidMontage { get; private set; }

      public Func_SetExecutionAuto SetExecutionAuto { get; private set; }

      public Func_SetExecutionAutoFinaux SetExecutionAutoFinaux { get; private set; }

      public Func_SetResultPath SetResultPath { get; private set; }

      public Func_TraceMesure TraceMesure { get; private set; }

      public Func_RunScriptMateriel RunScriptMateriel { get; private set; }

      public Func_GetIScriptPostionCurrentSB GetIScriptPostionCurrentSB { get; private set; }

      public Func_GetManuelMode GetManuelMode { get; private set; }

      public Func_SetManuelMode SetManuelMode { get; private set; }

      public Func_GetIdReceiver GetIdReceiver { get; private set; }

      public Func_GetIsValidAuto GetIsValidAuto { get; private set; }

      public Func_SetIsValidAuto SetIsValidAuto { get; private set; }

      public TDelegate AttachGenericFunction<TDelegate>(string cppFunctionName, bool optionnalFunction = false)
      {
        IntPtr ptrFunction = Win32.GetProcAddress(this.handlePilote3Library, cppFunctionName);
        if (ptrFunction == IntPtr.Zero)
        {
          if (optionnalFunction == false)
          {
            throw new Exception(string.Format(Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.CanNotLoad0Function1, "Pilote3.dll", cppFunctionName));
          }
          else
          {
            return default(TDelegate);
          }
        }

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptrFunction);
      }

      public void SetupChange()
      {
        this.MontageChanged?.Invoke(this, null);
      }

      public void PositionChange()
      {
        this.PositionChanged?.Invoke(this, null);
      }

      internal bool InitialiserFonctionDLL()
      {
        string pathEmi = ExecPathHelper.GetExecDirectory();
        string dllPath;
        if (Environment.Is64BitProcess)
        {
          string x64Path = System.IO.Path.Combine(pathEmi, "x64");
          Win32.SetDllDir(x64Path);
          dllPath = System.IO.Path.Combine(x64Path, "Pilote3.dll");
        }
        else
        {
          Win32.SetDllDir(pathEmi);
          dllPath = System.IO.Path.Combine(pathEmi, "Pilote3.dll");
        }

        this.handlePilote3Library = Win32.LoadLibrary(dllPath);

        if (this.handlePilote3Library != IntPtr.Zero)
        {
          this.SetSetupChangeDelegate = this.AttachGenericFunction<Func_SetSetupChangeDelegate>("SetSetupChangeDelegate");
          if (this.SetSetupChangeDelegate != null)
          {
            SetupChangeDelegate deleg = new SetupChangeDelegate(this.SetupChange);
            GCHandle prgHdl = GCHandle.Alloc(deleg, GCHandleType.Normal);
            this.SetSetupChangeDelegate(Marshal.GetFunctionPointerForDelegate(deleg));
          }

          this.SetPositionChangeDelegate = this.AttachGenericFunction<Func_SetPositionChangeDelegate>("SetPositionChangeDelegate");
          if (this.SetPositionChangeDelegate != null)
          {
            PositionChangeDelegate deleg = new PositionChangeDelegate(this.PositionChange);
            GCHandle prgHdl = GCHandle.Alloc(deleg, GCHandleType.Normal);
            this.SetPositionChangeDelegate(Marshal.GetFunctionPointerForDelegate(deleg));
          }

          this.GetTexteErreur = this.AttachGenericFunction<Func_GetTexteErreur>("GetTexteErreur");
          this.Terminer = this.AttachGenericFunction<Func_Terminer>("Terminer");
          this.GetNameMontage = this.AttachGenericFunction<Func_GetNameMontage>("GetNameMontage");
          this.InitSB_EMS = this.AttachGenericFunction<Func_InitSB_EMS>("InitSB_EMS");
          this.InitSB_EMS_CRBM = this.AttachGenericFunction<Func_InitSB_EMS_CRBM>("InitSB_EMS_CRBM");
          this.PreparerDomaine = this.AttachGenericFunction<Func_PreparerDomaine>("PreparerDomaine");
          this.PreparerDomaineCRBM = this.AttachGenericFunction<Func_PreparerDomaineCRBM>("PreparerDomaineCRBM");
          this.PreparerDomaineTestEM = this.AttachGenericFunction<Func_PreparerDomaineTestEM>("PreparerDomaineTestEM");
          this.ActiverFrequence = this.AttachGenericFunction<Func_ActiverFrequence>("ActiverFrequenceWpf");
          this.IncrementerNiveau = this.AttachGenericFunction<Func_IncrementerNiveau>("IncrementerNiveau");
          this.GetMesuresPuissances = this.AttachGenericFunction<Func_GetMesuresPuissances>("GetMesuresPuissances");
          this.GetMesuresPuissancesVSWR = this.AttachGenericFunction<Func_GetMesuresPuissancesVSWR>("GetMesuresPuissancesVSWR");
          this.GetMesuresSE = this.AttachGenericFunction<Func_GetMesuresSE>("GetMesuresSE");
          this.GetMesuresHarmoniques = this.AttachGenericFunction<Func_GetMesuresHarmoniques>("GetMesuresHarmoniques");
          this.GetNiveauGenerateur = this.AttachGenericFunction<Func_GetNiveauGenerateur>("GetNiveauGenerateur");
          this.GetMesurePerturbation = this.AttachGenericFunction<Func_GetMesurePerturbation>("GetMesurePerturbation");
          this.GetMesureCourant = this.AttachGenericFunction<Func_GetMesureCourant>("GetMesureCourant");
          this.GetMesurePerturbationPuissance = this.AttachGenericFunction<Func_GetMesurePerturbationPuissance>("GetMesurePerturbationPuissance");
          this.GetMesureECRBM = this.AttachGenericFunction<Func_GetMesureECRBM>("GetMesureECRBM");
          this.GetMesurePCRBM = this.AttachGenericFunction<Func_GetMesurePCRBM>("GetMesurePCRBM");
          this.GetMesuresPuissancesCRBM = this.AttachGenericFunction<Func_GetMesuresPuissancesCRBM>("GetMesuresPuissancesCRBM");
          this.GetGainTransducteur = this.AttachGenericFunction<Func_GetGainTransducteur>("GetGainTransducteur");
          this.GetVSWR = this.AttachGenericFunction<Func_GetVSWR>("GetVSWR");
          this.GetFacteurAntenne = this.AttachGenericFunction<Func_GetFacteurAntenne>("GetFacteurAntenne");
          this.GetIdTransducteur = this.AttachGenericFunction<Func_GetIdTransducteur>("GetIdTransducteur");
          this.GetIdTransducteurChaine = this.AttachGenericFunction<Func_GetIdTransducteurChaine>("GetIdTransducteurChaine");
          this.GetIdTypeTransducteur = this.AttachGenericFunction<Func_GetIdTypeTransducteur>("GetIdTypeTransducteur");
          this.Marche = this.AttachGenericFunction<Func_Marche>("Marche");
          this.GetPositionCourante = this.AttachGenericFunction<Func_GetPositionCourante>("GetPositionCourante");
          this.GetRawAxisFields = this.AttachGenericFunction<Func_GetRawAxisFields>("GetRawAxisFields");
          this.GetCorrectedAxisFields = this.AttachGenericFunction<Func_GetCorrectedAxisFields>("GetCorrectedAxisFields");
          this.Arret = this.AttachGenericFunction<Func_Arret>("Arret");
          this.EtatGenerateur = this.AttachGenericFunction<Func_EtatGenerateur>("EtatGenerateur");
          this.LancerSweep = this.AttachGenericFunction<Func_LancerSweep>("LancerSweep");
          this.ArreterSweep = this.AttachGenericFunction<Func_ArreterSweep>("ArreterSweep");
          this.IsMesureAvecModulation = this.AttachGenericFunction<Func_IsMesureAvecModulation>("IsMesureAvecModulation");
          this.IsEmiMeasWithMovingAngle = this.AttachGenericFunction<Func_IsEmiMeasWithMovingAngle>("IsEmiMeasWithMovingAngle");
          this.SetEmiMeasWithMovingAngle = this.AttachGenericFunction<Func_SetEmiMeasWithMovingAngle>("SetEmiMeasWithMovingAngle");
          this.IsEmiGlobalPrescan = this.AttachGenericFunction<Func_IsEmiGlobalPrescan>("IsEmiGlobalPrescan");
          this.SetEmiGlobalPrescan = this.AttachGenericFunction<Func_SetEmiGlobalPrescan>("SetEmiGlobalPrescan");
          this.InitSB_EMI = this.AttachGenericFunction<Func_InitSB_EMI>("InitSB_EMI");
          this.VerifierRFout = this.AttachGenericFunction<Func_VerifierRFout>("VerifierRFout");
          this.CorrigerMesure = this.AttachGenericFunction<Func_CorrigerMesure>("CorrigerMesure");
          this.CorrigerMesureVectorielle = this.AttachGenericFunction<Func_CorrigerMesureVectorielle>("CorrigerMesureVectorielle");
          this.ChangerPosition = this.AttachGenericFunction<Func_ChangerPosition>("ChangerPosition");
          this.ExecuterScript = this.AttachGenericFunction<Func_ExecuterScript>("ExecuterScript");
          this.MesurerPosition = this.AttachGenericFunction<Func_MesurerPosition>("MesurerPosition");
          this.GetCurrentPosition = this.AttachGenericFunction<Func_GetCurrentPosition>("GetCurrentPosition");
          this.GetSpeedPosition = this.AttachGenericFunction<Func_GetSpeedPosition>("GetSpeedPosition");
          this.ReglerMatPlateau = this.AttachGenericFunction<Func_ReglerMatPlateau>("ReglerMatPlateau");
          this.Motion_Start = this.AttachGenericFunction<Func_Motion_Start>("Motion_Start");
          this.Motion_GetPos = this.AttachGenericFunction<Func_Motion_GetPos>("Motion_GetPos");
          this.Motion_Stop = this.AttachGenericFunction<Func_Motion_Stop>("Motion_Stop");
          this.GetCurrentMotionScript = this.AttachGenericFunction<Func_GetCurrentMotionScript>("GetCurrentMotionScript");
          this.GetAdresse = this.AttachGenericFunction<Func_GetAdresse>("GetAdresse");
          this.GetGPIBInterfaceID = this.AttachGenericFunction<Func_GetGPIBInterfaceID>("GetGPIBInterfaceID");
          this.GetAdresseText = this.AttachGenericFunction<Func_GetAdresseText>("GetAdresseText");
          this.SetMainWnd = this.AttachGenericFunction<Func_SetMainWnd>("SetMainWnd");
          this.GetSerialSpeed = this.AttachGenericFunction<Func_GetSerialSpeed>("GetSerialSpeed");
          this.GetSerialParityBit = this.AttachGenericFunction<Func_GetSerialParityBit>("GetSerialParityBit");
          this.GetSerialStopBit = this.AttachGenericFunction<Func_GetSerialStopBit>("GetSerialStopBit");
          this.GetSerialDataBit = this.AttachGenericFunction<Func_GetSerialDataBit>("GetSerialDataBit");
          this.GetSerialXonXoff = this.AttachGenericFunction<Func_GetSerialXonXoff>("GetSerialXonXoff");
          this.GetEOSRead = this.AttachGenericFunction<Func_GetEOSRead>("GetEOSRead");
          this.GetEOSWrite = this.AttachGenericFunction<Func_GetEOSWrite>("GetEOSWrite");
          this.GetFrequenceMinMontage = this.AttachGenericFunction<Func_GetFrequenceMinMontage>("GetFrequenceMinMontage");
          this.GetFrequenceMaxMontage = this.AttachGenericFunction<Func_GetFrequenceMaxMontage>("GetFrequenceMaxMontage");
          this.GetFrequenceMinMontageEMI = this.AttachGenericFunction<Func_GetFrequenceMinMontageEMI>("GetFrequenceMinMontageEMI");
          this.GetFrequenceMaxMontageEMI = this.AttachGenericFunction<Func_GetFrequenceMaxMontageEMI>("GetFrequenceMaxMontageEMI");
          this.ReglerNiveauSUSB = this.AttachGenericFunction<Func_ReglerNiveauSUSB>("ReglerNiveauSUSB");
          this.GetMesureNiveauSUSB = this.AttachGenericFunction<Func_GetMesureNiveauSUSB>("GetMesureNiveauSUSB");
          this.Init55020 = this.AttachGenericFunction<Func_Init55020>("Init55020");
          this.GetNiveauGeneSUSB = this.AttachGenericFunction<Func_GetNiveauGeneSUSB>("GetNiveauGeneSUSB");
          this.GetPertePinceSUSB = this.AttachGenericFunction<Func_GetPertePinceSUSB>("GetPertePinceSUSB");
          this.GetGainChaine = this.AttachGenericFunction<Func_GetGainChaine>("GetGainChaine");
          this.GetNiveauMaxSortie = this.AttachGenericFunction<Func_GetNiveauMaxSortie>("GetNiveauMaxSortie");
          this.InitDomaine = this.AttachGenericFunction<Func_InitDomaine>("InitDomaine");
          this.Mesurer = this.AttachGenericFunction<Func_Mesurer>("Mesurer");
          this.ExecutionESD = this.AttachGenericFunction<Func_ExecutionESD>("ExecutionESD");
          this.SetProtection = this.AttachGenericFunction<Func_SetProtection>("SetProtection");
          this.SetGuidEssaiCEME = this.AttachGenericFunction<Func_SetGuidEssaiCEME>("SetGuidEssaiCEME");
          this.GetGuidMontage = this.AttachGenericFunction<Func_GetGuidMontage>("GetGuidMontage");
          this.SetExecutionAuto = this.AttachGenericFunction<Func_SetExecutionAuto>("SetExecutionAuto");
          this.SetExecutionAutoFinaux = this.AttachGenericFunction<Func_SetExecutionAutoFinaux>("SetExecutionAutoFinaux");
          this.SetResultPath = this.AttachGenericFunction<Func_SetResultPath>("SetResultPath");
          this.TraceMesure = this.AttachGenericFunction<Func_TraceMesure>("TraceMesure");
          this.RunScriptMateriel = this.AttachGenericFunction<Func_RunScriptMateriel>("RunScriptMateriel");
          this.GetIScriptPostionCurrentSB = this.AttachGenericFunction<Func_GetIScriptPostionCurrentSB>("GetIScriptPostionCurrentSB");
          this.GetManuelMode = this.AttachGenericFunction<Func_GetManuelMode>("GetManuelMode");
          this.SetManuelMode = this.AttachGenericFunction<Func_SetManuelMode>("SetManuelMode");
          this.GetIdReceiver = this.AttachGenericFunction<Func_GetIdReceiver>("GetIdReceiver");
          this.GetMast = this.AttachGenericFunction<Func_GetMast>("GetMast");
          this.GetTable = this.AttachGenericFunction<Func_GetTable>("GetTable");
          this.IsTurntableIdUsed = this.AttachGenericFunction<Func_IsTurntableIdUsed>("IsTurntableIdUsed");
          this.IsMastIdUsed = this.AttachGenericFunction<Func_IsMastIdUsed>("IsMastIdUsed");
          this.SetIsValidAuto = this.AttachGenericFunction<Func_SetIsValidAuto>("SetIsValidAuto");
          this.GetIsValidAuto = this.AttachGenericFunction<Func_GetIsValidAuto>("GetIsValidAuto");
        }
        else
        {
          throw new Exception(Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.CanNotLoad + dllPath);
        }

        return true;
      }

      internal void DechargerFonctionDLL()
      {
        this.GetTexteErreur = null;
        this.Terminer = null;
        this.GetNameMontage = null;
        this.InitSB_EMS = null;
        this.InitSB_EMS_CRBM = null;
        this.PreparerDomaine = null;
        this.PreparerDomaineCRBM = null;
        this.PreparerDomaineTestEM = null;
        this.IncrementerNiveau = null;
        this.GetMesuresPuissances = null;
        this.GetMesuresPuissancesVSWR = null;
        this.GetMesuresSE = null;
        this.GetMesuresHarmoniques = null;
        this.GetNiveauGenerateur = null;
        this.GetMesurePerturbation = null;
        this.GetMesureCourant = null;
        this.GetMesurePerturbationPuissance = null;
        this.GetMesureECRBM = null;
        this.GetMesurePCRBM = null;
        this.GetMesuresPuissancesCRBM = null;
        this.GetGainTransducteur = null;
        this.GetVSWR = null;
        this.GetFacteurAntenne = null;
        this.GetIdTransducteur = null;
        this.GetIdTransducteurChaine = null;
        this.GetIdTypeTransducteur = null;
        this.Marche = null;
        this.GetPositionCourante = null;
        this.GetRawAxisFields = null;
        this.GetCorrectedAxisFields = null;
        this.Arret = null;
        this.EtatGenerateur = null;
        this.LancerSweep = null;
        this.ArreterSweep = null;
        this.IsMesureAvecModulation = null;
        this.IsEmiMeasWithMovingAngle = null;
        this.SetEmiMeasWithMovingAngle = null;
        this.IsEmiGlobalPrescan = null;
        this.SetEmiGlobalPrescan = null;
        this.InitSB_EMI = null;
        this.VerifierRFout = null;
        this.CorrigerMesure = null;
        this.CorrigerMesureVectorielle = null;
        this.ChangerPosition = null;
        this.ExecuterScript = null;
        this.MesurerPosition = null;
        this.GetSpeedPosition = null;
        this.ReglerMatPlateau = null;
        this.Motion_Start = null;
        this.Motion_GetPos = null;
        this.Motion_Stop = null;
        this.GetCurrentMotionScript = null;
        this.GetAdresse = null;
        this.GetGPIBInterfaceID = null;
        this.GetAdresseText = null;
        this.SetMainWnd = null;
        this.GetSerialSpeed = null;
        this.GetSerialParityBit = null;
        this.GetSerialStopBit = null;
        this.GetSerialDataBit = null;
        this.GetSerialXonXoff = null;
        this.GetEOSRead = null;
        this.GetEOSWrite = null;
        this.GetFrequenceMinMontage = null;
        this.GetFrequenceMaxMontage = null;
        this.GetFrequenceMinMontageEMI = null;
        this.GetFrequenceMaxMontageEMI = null;
        this.ReglerNiveauSUSB = null;
        this.GetMesureNiveauSUSB = null;
        this.Init55020 = null;
        this.GetNiveauGeneSUSB = null;
        this.GetPertePinceSUSB = null;
        this.GetGainChaine = null;
        this.GetNiveauMaxSortie = null;
        this.InitDomaine = null;
        this.Mesurer = null;
        this.ExecutionESD = null;
        this.SetProtection = null;
        this.SetGuidEssaiCEME = null;
        this.GetGuidMontage = null;
        this.SetExecutionAuto = null;
        this.SetExecutionAutoFinaux = null;
        this.SetResultPath = null;
        this.TraceMesure = null;
        this.RunScriptMateriel = null;
        this.GetIScriptPostionCurrentSB = null;
        this.GetManuelMode = null;
        this.SetManuelMode = null;
        this.GetIdReceiver = null;
        this.GetMast = null;
        this.GetTable = null;
        this.IsTurntableIdUsed = null;
        this.IsMastIdUsed = null;
        this.SetIsValidAuto = null;
        this.GetIsValidAuto = null;
        if (!Win32.FreeLibrary(this.handlePilote3Library))
        {
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
      }
    }
  }
}