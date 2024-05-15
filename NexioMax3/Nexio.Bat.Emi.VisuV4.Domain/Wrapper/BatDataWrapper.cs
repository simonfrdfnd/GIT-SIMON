namespace Nexio.Bat.Emi.VisuV4.Domain.Wrapper
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Text;
  using System.Threading;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Helper;
  using Nexio.Tools;

  public class BatDataWrapper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BatDataWrapper));

    private readonly Native native;

    public BatDataWrapper()
    {
#if DEBUG
      if (++RefCounter != 1)
      {
        System.Diagnostics.Debug.Assert(false, "trop  de références de BatData");
      }
#endif
      this.native = new Native();
      this.native.InitialiserFonctionDLL();
      this.native.LoadLogConfig("Bat-Emi_Log_Config.xml");
      this.native.SetIsV4(true);
    }

#if DEBUG
    public static int RefCounter { get; set; }
#endif

    public bool IsInitialized
    {
      get { return this.native.IsInitialized; }
    }

    public Test_State GetEtatEssai()
    {
      return this.native.GetEtatEssai();
    }

    public int GetNbLabelEquipementName()
    {
      return this.native.GetNbLabelEquipementName();
    }

    public int GetIdFonctionSuivante()
    {
      return this.native.GetIdFonctionSuivante();
    }

    public bool IsFonctionUtilisee(int idFct)
    {
      return this.native.IsFonctionUtilisee(idFct);
    }

    public void GetNomDllMesureur(int idMesureur, out string nomDllMesureur)
    {
      StringBuilder sznomDllMesureur = new StringBuilder(256);
      this.native.GetNomDllMesureur(idMesureur, sznomDllMesureur);
      nomDllMesureur = sznomDllMesureur.ToString();
    }

    public bool Terminer(Test_State e_Etat)
    {
      return this.native.Terminer((int)e_Etat);
    }

    public bool GetExecData(ref int idSB, ref int idFonction, ref int idPtFirstPt, ref int nbrePts)
    {
      using (new NexioStopwatch($"{nameof(BatDataWrapper)}.{nameof(this.GetExecData)}"))
      {
        var b = this.native.GetExecData(ref idSB, ref idFonction, ref idPtFirstPt, ref nbrePts);
        return b;
      }
    }

    public bool SelectFonction(Fonction_Action e_TypeFonction)
    {
      return this.native.SelectFonction(e_TypeFonction);
    }

    public bool GetParametresReglage(int e_idReglage, out int s_NbReglage, out List<string> s_ListeValeurParametres)
    {
      StringBuilder listBuilder = new StringBuilder(1024);
      var ret = this.native.GetParametresReglage(e_idReglage, out s_NbReglage, listBuilder);

      s_ListeValeurParametres = new List<string>();
      s_ListeValeurParametres.AddRange(listBuilder.ToString().Split(new[] { '\t' }));

      return ret;
    }

    public Guid GetGuidMontageSB(int idSB, int iSegmentSize)
    {
      var guid = this.native.GetGuidMontageSB(idSB, iSegmentSize);
      string guidStr = Marshal.PtrToStringAnsi(guid);
      if (!string.IsNullOrEmpty(guidStr))
      {
        return Guid.Parse(guidStr);
      }
      else
      {
        return Guid.Empty;
      }
    }

    public bool IsChangeDomaineSB()
    {
      return this.native.IsChangeDomaineSB();
    }

    public int GetEtatSB(int idSB)
    {
      return this.native.GetEtatSB(idSB);
    }

    public bool SelectFonctionWpfImport(Fonction_Action e_TypeFonction)
    {
      return this.native.SelectFonctionWpfImport(e_TypeFonction);
    }

    public string GetUniteEssai(int iTypeSignal)
    {
      var sb = new StringBuilder(256);
      this.native.GetUniteEssaiUnicode(sb, iTypeSignal);
      return sb.ToString();
    }

    public int GetSignatureFonction(int idFonction)
    {
      return this.native.GetSignatureFonction(idFonction);
    }

    public int GetSignatureFonctionForAnyEssai(int idFonction, string idEssai, string idProjet)
    {
      return this.native.GetSignatureFonctionForAnyEssai(idFonction, idEssai, idProjet);
    }

    public int GetFonctionSuivante(out string s_Intitule, out int s_TypeFonction, out bool bEssaiUsed)
    {
      StringBuilder intituleSB = new StringBuilder(256);
      s_TypeFonction = 0;
      bEssaiUsed = false;
      int ret;

      ret = this.native.GetFonctionSuivante(intituleSB, out s_TypeFonction, out bEssaiUsed);
      s_Intitule = intituleSB.ToString();

      return ret;
    }

    public int GetFonctionSuivanteWpfImport(out string s_Intitule, out int s_TypeFonction, out bool bEssaiUsed)
    {
      StringBuilder intituleSB = new StringBuilder(256);
      s_TypeFonction = 0;
      bEssaiUsed = false;
      int ret;

      ret = this.native.GetFonctionSuivanteWpfImport(intituleSB, out s_TypeFonction, out bEssaiUsed);
      s_Intitule = intituleSB.ToString();

      return ret;
    }

    public string GetScriptDebutSB(int iSBCourante)
    {
      var chPtr = this.native.GetScriptDebutSB(iSBCourante);
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public string GetScriptFinSB(int iSBCourante)
    {
      var chPtr = this.native.GetScriptFinSB(iSBCourante);
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public string GetScriptEndPrescan()
    {
      var chPtr = this.native.GetScriptEndPrescan();
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public string GetScriptEndTest()
    {
      var chPtr = this.native.GetScriptEndTest();
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public bool Initialiser(string e_sEssai, bool bUseDSN, int iEssai, string sPathResult, bool bUpdateFromDatabase)
    {
      using (new NexioStopwatch($"{nameof(BatDataWrapper)}.{nameof(this.Initialiser)}"))
      {
        return this.native.Initialiser(e_sEssai, bUseDSN, iEssai, sPathResult, bUpdateFromDatabase);
      }
    }

    public bool InitialiserFromFile(string path)
    {
      return this.native.InitFromFile(path);
    }

    /// <summary>
    /// Récupère les infos du point grace à une fréquence proche
    /// </summary>
    /// <param name="idSousBande">Sous bande du point</param>
    /// <param name="frequence">Fréquence de recherche en Mhz</param>
    /// <param name="idPoint">Id du point trouvé</param>
    /// <param name="frequenceProche">Fréquence la plus proche en Mhz</param>
    /// <param name="isSuperieur">est suppérieur</param>
    /// <returns>true si le point est trouvé</returns>
    public bool GetIdPointFrequence(int idSousBande, double frequence, out int idPoint, out double frequenceProche, out bool isSuperieur)
    {
      return this.native.GetIdPointFrequence(idSousBande, frequence, out idPoint, out frequenceProche, out isSuperieur);
    }

    public bool InitialiserWpfImport(string e_sEssai, string e_sProject, int iEssai, string sPathResult)
    {
      return this.native.InitialiserWpfImport(e_sEssai, e_sProject, iEssai, sPathResult);
    }

    public int GetActionFonction(int idFonction)
    {
      return this.native.GetActionFonction(idFonction);
    }

    public bool SauvegarderResultatsFonctionAuto(int idFct, string name, out int[] tabIdFonction, out int[] tabIdCourbes)
    {
      IntPtr ptrIdFonction, ptrIdCourbes;
      var res = this.native.SauvegarderResultatsFonctionAuto(idFct, name, out ptrIdFonction, out ptrIdCourbes, out int nbrIdFonction, out int nbrIdCourbes);
      if (res)
      {
        tabIdFonction = new int[nbrIdFonction];
        Marshal.Copy(ptrIdFonction, tabIdFonction, 0, nbrIdFonction);
        tabIdCourbes = new int[nbrIdCourbes];
        Marshal.Copy(ptrIdCourbes, tabIdCourbes, 0, nbrIdCourbes);
      }
      else
      {
        tabIdFonction = new int[0];
        tabIdCourbes = new int[0];
      }

      return res;
    }

    public bool GetGlobalProcedure(int m_idSBAuto, double m_dRangeFreqMin, double m_dRangeFreqMax, ref int idAction, out string szNomDLL)
    {
      StringBuilder nomDLL = new StringBuilder(256);
      var ret = this.native.GetGlobalProcedure(m_idSBAuto, m_dRangeFreqMin, m_dRangeFreqMax, ref idAction, nomDLL);
      szNomDLL = Convert.ToString(nomDLL);
      return ret;
    }

    public bool GetPositionInitialeSB(int e_IdSousBande, out double pfDistance, out int piScriptPosition, out bool s_pbHauteurAuto, out double s_pfHauteur, out bool s_pbAngleAuto, out double s_pfAngle)
    {
      return this.native.GetPositionInitialeSB(e_IdSousBande, out pfDistance, out piScriptPosition, out s_pbHauteurAuto, out s_pfHauteur, out s_pbAngleAuto, out s_pfAngle, out _);
    }

    public bool SelectSB(int e_iPosition, string posName, bool bIncNoPos)
    {
      return this.native.SelectSB(e_iPosition, posName, bIncNoPos);
    }

    public void UnSelectSB()
    {
      this.native.UnSelectSB();
    }

    public void UnSelectFonction()
    {
      this.native.UnSelectFonction();
    }

    public bool GetNextValeurCourbeExt(out double s_pFreq, out double s_pfValeur)
    {
      return this.native.GetNextValeurCourbeExt(out s_pFreq, out s_pfValeur);
    }

    /// <summary>
    /// Récupération des infos d'une sous-bande
    /// </summary>
    public int GetInfosSB(int e_IdSB, out double s_fMin, out double s_fMax, out double s_fPas, out TypesProgression typeProgress, out int s_nbPoint, out string s_NomScriptPosition, out string s_NomTypePosition)
    {
      int ret = 0;
      StringBuilder nomScriptPositionsb = new StringBuilder(256);
      StringBuilder nomTypePosition = new StringBuilder(256);
      ret = this.native.GetInfosSB(e_IdSB, out s_fMin, out s_fMax, out s_fPas, out typeProgress, out s_nbPoint, nomScriptPositionsb, nomTypePosition);
      s_NomScriptPosition = nomScriptPositionsb.ToString();
      s_NomTypePosition = nomTypePosition.ToString();

      return ret;
    }

    public bool GetTraitementSuivant(ref int pidSB, out int pidFirstPt, out int pidLastPt, out string szNomDLL, double rangeFreqMin, double rangeFreqMax)
    {
      StringBuilder nomDLL = new StringBuilder(256);
      var ret = this.native.GetTraitementSuivant(ref pidSB, out pidFirstPt, out pidLastPt, nomDLL, rangeFreqMin, rangeFreqMax);
      szNomDLL = Convert.ToString(nomDLL);
      return ret;
    }

    public void ResetEventAffiche()
    {
      this.native.ResetEventAffiche();
    }

    public bool Sauvegarder(bool bIntermediaire)
    {
      using (new NexioStopwatch($"{nameof(BatDataWrapper)}.{nameof(this.Sauvegarder)}"))
      {
        return this.native.Sauvegarder(bIntermediaire);
      }
    }

    public bool EssaiModifie()
    {
      using (new NexioStopwatch($"{nameof(BatDataWrapper)}.{nameof(this.EssaiModifie)}"))
      {
        return this.native.EssaiModifie();
      }
    }

    public void SetEventAffiche()
    {
      this.native.SetEventAffiche();
    }

    public int GetNbResultatsFonction(int idFonction, int idSB)
    {
      return this.native.GetNbResultatsFonction(idFonction, idSB);
    }

    public double GetFreqMinSB(int idSB)
    {
      return this.native.GetFreqMinSB(idSB);
    }

    public double GetFreqMaxSB(int idSB)
    {
      return this.native.GetFreqMaxSB(idSB);
    }

    public int GetInfoColumnFonction(int idfonction, int icol, out TypeCol piColType, out string s_ColName)
    {
      StringBuilder colNameSB = new StringBuilder(256);
      int ret;

      ret = this.native.GetInfoColumnFonction(idfonction, icol, out piColType, colNameSB);

      s_ColName = colNameSB.ToString();

      return ret;
    }

    public int GetCompleteInfoColumnFonction(int idfonction, int icol, out TypeCol piColType, out string s_ColName, out int iParam1, out int iParam2, out double dOffset)
    {
      StringBuilder colNameSB = new StringBuilder(256);

      var ret = this.native.GetCompleteInfoColumnFonction(idfonction, icol, out piColType, colNameSB, out iParam1, out iParam2, out dOffset);

      s_ColName = colNameSB.ToString();

      return ret;
    }

    public void GetListePositions(out List<(int Pos, string Name)> positions)
    {
      StringBuilder listPos = new StringBuilder(string.Empty, 256);
      StringBuilder listPosName = new StringBuilder(string.Empty, 256);
      this.native.GetListePositionsTab(listPos, listPosName);
      positions = new List<(int Pos, string Name)>();
      if (string.IsNullOrEmpty(listPos.ToString()))
      {
        positions = new List<(int Pos, string Name)>();
        return;
      }

      var posTable = listPos.ToString().Split('\t');
      var namesTable = listPosName.ToString().Split('\t');
      for (int i = 0; i < posTable.Length; i++)
      {
        positions.Add((int.Parse(posTable[i]), namesTable[i]));
      }
    }

    public bool GetInfosResultatsFonction(int idfonction, out int nbrValeurs, out string[] colNames, out bool[] colUsed)
    {
      StringBuilder names = new StringBuilder(256);
      var used = new StringBuilder(256);
      bool ret = this.native.GetInfosResultatsFonctionTab(idfonction, out nbrValeurs, names, used);
      colNames = names.ToString().Split('\t');
      colUsed = used.ToString().Split('\t').Select(u => u == "1").ToArray();

      return ret;
    }

    public int GetSBSuivante()
    {
      return this.native.GetSBSuivante();
    }

    public Guid GetGuidSB(int e_IdSB)
    {
      IntPtr guidPtr = this.native.GetGuidSB(e_IdSB);
      string guidStr = Marshal.PtrToStringAnsi(guidPtr);

      if (!string.IsNullOrEmpty(guidStr))
      {
        return Guid.Parse(guidStr);
      }
      else
      {
        return Guid.Empty;
      }
    }

    public string GetNomSB(int e_IdSB)
    {
      IntPtr commentPtr = this.native.GetCommentSB(e_IdSB);
      string comment = Marshal.PtrToStringAnsi(commentPtr);

      if (string.IsNullOrEmpty(comment))
      {
        IntPtr namePtr = this.native.GetNomSB(e_IdSB);
        return Marshal.PtrToStringAnsi(namePtr);
      }
      else
      {
        return comment;
      }
    }

    public int GetPositionSB(int e_IdSB)
    {
      return this.native.GetPositionSB(e_IdSB, out _);
    }

    public bool GetIdFreqSousbande(int e_idSousBande, out int s_idFreqMin, out int s_Nombre)
    {
      return this.native.GetIdFreqSousbande(e_idSousBande, out s_idFreqMin, out s_Nombre);
    }

    public int GetIdFonction(int e_MaskSignature, int e_Action, bool e_bOnlyUsed = false)
    {
      return this.native.GetIdFonction(e_MaskSignature, e_Action, e_bOnlyUsed);
    }

    public int GetIdSousBande(int e_idPoint)
    {
      return this.native.GetIdSousBande(e_idPoint);
    }

    public int GetIdTypeEssai()
    {
      return this.native.GetIdTypeEssai();
    }

    public bool SelectDLLFonction(Fonction_Action action)
    {
      return this.native.SelectDLLFonction((int)action);
    }

    public void SelectPositionCourbeExt(int idCourbe)
    {
      this.native.SelectPositionCourbeExt(idCourbe);
    }

    public int GetNextPositionCourbeExt(out string posName)
    {
      StringBuilder posBuilder = new StringBuilder(256);
      var result = this.native.GetNextPositionCourbeExt(posBuilder);
      posName = posBuilder.ToString();
      return result;
    }

    public void SelectCourbesExt()
    {
      this.native.SelectCourbesExt();
    }

    public int GetCourbeExtSuivante()
    {
      return this.native.GetCourbeExtSuivante();
    }

    public void SetResultat(int e_idPoint, int e_idFonction, int e_NbValeurs, double[] e_tValeurs, bool e_bEtat, double dCorrection = -1e10)
    {
      this.native.SetResultat(e_idPoint, e_idFonction, e_NbValeurs, e_tValeurs, e_bEtat, dCorrection);
    }

    public bool GetCommentaireResultat(int idPoint, int function, out string comment)
    {
      StringBuilder commentBuilder = new StringBuilder(256);
      var result = this.native.GetCommentaireResultat(idPoint, function, commentBuilder);
      comment = commentBuilder.ToString();
      return result;
    }

    public void SetFrequenceMesure(int e_idPoint, int e_idFonction, double dFrequence)
    {
      this.native.SetFrequenceMesure(e_idPoint, e_idFonction, dFrequence);
    }

    public void SetCommentaireResultat(int e_idPoint, int e_idFonction, string e_Commentaire)
    {
      this.native.SetCommentaireResultat(e_idPoint, e_idFonction, e_Commentaire);
    }

    public void SetColumnInfoFonction(int idFonction, int iCol, int iType, int iParam1 = 0, int iParam2 = 0, double dOffset = 0)
    {
      this.native.SetColumnInfoFonction(idFonction, iCol, iType, iParam1, iParam2, dOffset);
    }

    public int GetIdFonctionName(string e_sName, int e_Action, int iNbValeurs, string e_sColonne)
    {
      return this.native.GetIdFonctionName(e_sName, e_Action, iNbValeurs, e_sColonne);
    }

    /// <summary>
    /// Ce service permet d’obtenir le résultat d’une
    /// fonction pour Un point de  fréquence donné.
    /// </summary>
    /// <param name="e_IdPointFreq">identificateur du Point de fréquence concerné</param>
    /// <param name="e_IdFonction">identificateur de la fonction</param>
    /// <returns>table des valeurs résultat de la fonction ou null.</returns>
    public double[] GetResultat(int e_IdPointFreq, int e_IdFonction)
    {
      int ret = this.native.GetResultat(e_IdPointFreq, e_IdFonction, out IntPtr valuePtr, out bool s_bEtat);

      if (ret > 0)
      {
        double[] resultat = new double[ret];
        Marshal.Copy(valuePtr, resultat, 0, ret);
        return resultat;
      }
      else
      {
        return null;
      }
    }

    public bool GetDLLFonctionSuivante(out string s_Intitule, out string s_NomDLL)
    {
      StringBuilder intitule = new StringBuilder(256);
      StringBuilder nomDLL = new StringBuilder(256);
      bool ret = this.native.GetDLLFonctionSuivante(intitule, nomDLL);
      s_Intitule = Convert.ToString(intitule);
      s_NomDLL = Convert.ToString(nomDLL);
      return ret;
    }

    public string GetResultatString(int idPointFreq, int idFonction, int iRes)
    {
      bool ret = this.native.GetResultatString(idPointFreq, idFonction, iRes, out IntPtr text);

      return Marshal.PtrToStringAnsi(text);
    }

    public double GetResultatDouble(int idPointFreq, int idFonction, int iRes)
    {
      bool ret = this.native.GetResultatDouble(idPointFreq, idFonction, iRes, out double value);

      return value;
    }

    public bool FastSelectResultatsSBFonction(int e_idFonction, int e_idSousBande)
    {
      return this.native.FastSelectResultatsSBFonction(e_idFonction, e_idSousBande);
    }

    public int FastGetIdResultatSuivant()
    {
      return this.native.FastGetIdResultatSuivant();
    }

    /// <summary>
    /// Récupère la fréquence dun point
    /// </summary>
    /// <param name="e_idPointFreq">Id du point</param>
    /// <returns>La fréquence du point en Hz</returns>
    /// <remarks>Renvoie du Hz</remarks>
    public double GetFrequence(int e_idPointFreq)
    {
      // Bat Data renvoi du Mhz donc on passe ça en Hz pour Visu V4
      return ToHz(this.native.GetFrequence(e_idPointFreq));
    }

    /// <summary>
    /// Récupère la fréquence d'une mesure spécifique
    /// </summary>
    /// <param name="e_idPointFreq">L'id du point dont on veut l'info</param>
    /// <param name="e_idFonction">Fonction utilisé pour la mesure</param>
    /// <returns>La fréquence du point en Hz</returns>
    /// <remarks>Renvoi du Hz</remarks>
    public double GetFrequenceMesure(int e_idPointFreq, int e_idFonction)
    {
      // Bat Data renvoi du Mhz donc on passe ça en Hz pour Visu V4
      return ToHz(this.native.GetFrequenceMesure(e_idPointFreq, e_idFonction));
    }

    public int GetNbVisuLimite()
    {
      return this.native.GetNbVisuLimite();
    }

    public int SelectValeursCourbeExt()
    {
      return this.native.SelectValeursCourbeExt();
    }

    public bool GetInfoCourbeExt(int idCourbeExt, out string nom, out int s_piTypeSignal, out Guid s_guidEssaiSource)
    {
      StringBuilder sNomCourbe = new StringBuilder(256);
      StringBuilder sGuidEssaiSource = new StringBuilder(256);

      bool ret = this.native.GetInfoCourbeExt(idCourbeExt, sNomCourbe, out s_piTypeSignal, sGuidEssaiSource);
      nom = Convert.ToString(sNomCourbe);

      string guidStr = Convert.ToString(sGuidEssaiSource);

      if (!string.IsNullOrEmpty(guidStr))
      {
        s_guidEssaiSource = Guid.Parse(guidStr);
      }
      else
      {
        s_guidEssaiSource = Guid.Empty;
      }

      return ret;
    }

    public bool ExporterResultatWpf(string exportFile, List<int> ids, bool isExternalCurves)
    {
      return this.native.ExporterResultatsFonctionWpf(exportFile, ids.ToArray(), ids.Count, !isExternalCurves, !isExternalCurves, !isExternalCurves, isExternalCurves);
    }

    public bool ImporterCourbeExtFromFileWpf(string e_sNomCourbe, int iTypeEssai, string e_sFileName,
      ushort[] s_pTabIdCourbes)
    {
      return this.native.ImporterCourbeExtFromFileWpf(e_sNomCourbe, iTypeEssai, e_sFileName, s_pTabIdCourbes);
    }

    public int ImporterMaxCourbesExtWpf(int[] t_iEssaisSources, string s_idProjet, int elementsCount, int idFonction, string szNomCourbe, bool bOnlyMaxOfPositions)
    {
      return this.native.ImporterMaxCourbesExtWpf(t_iEssaisSources, s_idProjet, elementsCount, idFonction, szNomCourbe, bOnlyMaxOfPositions);
    }

    public bool GetInfosEssai(out string s_sNomEssai, out string s_sTypeEssai, out double s_pfFreqMin, out double s_pfFreqMax,
            out string s_sNomLimite, out int s_piClasse, out string sEquipement, out string sVersion,
            out string sRefHard, out string sRefSoft, out string sModifications)
    {
      StringBuilder sNomEssai = new StringBuilder(256);
      StringBuilder sTypeEssai = new StringBuilder(256);
      StringBuilder sNomLimite = new StringBuilder(256);
      StringBuilder equipement = new StringBuilder(256);
      StringBuilder version = new StringBuilder(256);
      StringBuilder refHard = new StringBuilder(256);
      StringBuilder refSoft = new StringBuilder(256);
      StringBuilder modifications = new StringBuilder(256);

      bool ret = this.native.GetInfosEssai(sNomEssai, sTypeEssai, out double pfFreqMin, out double pfFreqMax, sNomLimite, out int piClasse, equipement, version, refHard, refSoft, modifications);

      s_sNomEssai = Convert.ToString(sNomEssai);
      s_sTypeEssai = Convert.ToString(sTypeEssai);
      s_pfFreqMin = pfFreqMin;
      s_pfFreqMax = pfFreqMax;
      s_sNomLimite = Convert.ToString(sNomLimite);
      s_piClasse = piClasse;
      sEquipement = Convert.ToString(equipement);
      sVersion = Convert.ToString(version);
      sRefHard = Convert.ToString(refHard);
      sRefSoft = Convert.ToString(refSoft);
      sModifications = Convert.ToString(modifications);
      return ret;
    }

    public bool GetInfoVisuLimite(int indexLimite, out string szNom, out string szClasse, out int iSignature, out double fDistance, out int iTypeSignal, out string guidLimite, out bool bVisible, out int iTypeInterpolation)
    {
      StringBuilder szNomSB = new StringBuilder(256);
      StringBuilder szClasseSB = new StringBuilder(256);
      StringBuilder guidLimiteSB = new StringBuilder(256);
      var res = this.native.GetInfoVisuLimite(indexLimite, szNomSB, szClasseSB, out iSignature, out fDistance, out iTypeSignal, guidLimiteSB, out bVisible, out iTypeInterpolation);
      szNom = szNomSB.ToString();
      szClasse = szClasseSB.ToString();
      guidLimite = guidLimiteSB.ToString();
      return res;
    }

    public bool GetInfoVisuLimiteDesc(int indexLimite, out string szNom, out string szClasse, out int iSignature, out double fDistance, out int iTypeSignal, out string guidLimite, out bool bVisible, out int iTypeInterpolation, out string szDesc)
    {
      StringBuilder szNomSB = new StringBuilder(256);
      StringBuilder szClasseSB = new StringBuilder(256);
      StringBuilder guidLimiteSB = new StringBuilder(256);
      StringBuilder szDescSB = new StringBuilder(256);
      var res = this.native.GetInfoVisuLimiteDesc(indexLimite, szNomSB, szClasseSB, out iSignature, out fDistance, out iTypeSignal, guidLimiteSB, out bVisible, out iTypeInterpolation, szDescSB);
      szNom = szNomSB.ToString();
      szClasse = szClasseSB.ToString();
      guidLimite = guidLimiteSB.ToString();
      szDesc = szDescSB.ToString();
      return res;
    }

    public bool SetExecData(int e_idSB, int e_idFonction, int e_idPtFirstPt, int e_NbrePts)
    {
      return this.native.SetExecData(e_idSB, e_idFonction, e_idPtFirstPt, e_NbrePts);
    }

    public bool GetVisuLimiteNiveaux(int indexLimite, out int iNbVal, out double[] fFreq, out double[] fNiveaux)
    {
      bool ret = this.native.GetVisuLimiteNiveaux(indexLimite, out iNbVal, out IntPtr fFreqPtr, out IntPtr fNiveauxPtr);

      if (ret && iNbVal > 0)
      {
        fFreq = new double[iNbVal];
        Marshal.Copy(fFreqPtr, fFreq, 0, iNbVal);
        fNiveaux = new double[iNbVal];
        Marshal.Copy(fNiveauxPtr, fNiveaux, 0, iNbVal);
      }
      else
      {
        fFreq = new double[0];
        fNiveaux = new double[0];
      }

      return ret;
    }

    public bool GetLimiteNiveau(int e_iSigDetecteur, int e_iTypeSignal, int e_idPointFreq, out double s_pfNiveau, bool bUseVisibleLimits = false)
    {
      return this.native.GetLimiteNiveau(e_iSigDetecteur, e_iTypeSignal, e_idPointFreq, out s_pfNiveau, bUseVisibleLimits);
    }

    public bool IsGlobalProcedure()
    {
      bool ret = this.native.IsGlobalProcedure();
      return ret;
    }

    public int ImporterCourbeExtWpf(string idProject, int idEssai, int idFonction, string nomCourbe)
    {
      return this.native.ImporterCourbeExtWpf(idProject, idEssai, idFonction, nomCourbe);
    }

    public bool SupprimerCourbeExt(int e_IdCoubeExt)
    {
      return this.native.SupprimerCourbeExt(e_IdCoubeExt);
    }

    public void SetResultatEtat(int e_IdPoint, int e_IdFonction, bool e_bEtat)
    {
      this.native.SetResultatEtat(e_IdPoint, e_IdFonction, e_bEtat);
    }

    public void DeleteEssaiImport()
    {
      this.native.DeleteEssaiImport();
    }

    public void InitialiserFonctionDLL()
    {
      this.native.InitialiserFonctionDLL();
      this.native.SetIsV4(true);
    }

    public void DechargerFonctionDLL()
    {
#if DEBUG
      --RefCounter;
      System.Diagnostics.Debug.Assert(RefCounter >= 0, "trop de déchargement de BatData"); // message si refcounter est négatif
      System.Diagnostics.Debug.Assert(RefCounter <= 0, "trop de références de BatData"); // message is refcounter est positif
#endif
      this.native.DechargerFonctionDLL();
    }

    public int GetNombreSB()
    {
      return this.native.GetNombreSB();
    }

    public void FinFonctionSB(int e_IdSb, int e_idFonction)
    {
      this.native.FinFonctionSB(e_IdSb, e_idFonction);
    }

    public string GetPathResult()
    {
      var pathPtr = this.native.GetPathResult();
      return Marshal.PtrToStringAnsi(pathPtr);
    }

    public int GetIDEssai()
    {
      return this.native.GetIDEssai();
    }

    public string GetConfEM()
    {
      var chPtr = this.native.GetConfEM();
      return Marshal.PtrToStringAnsi(chPtr);
    }

    public void SetLimiteVisible(int e_IndexVisuLimite, bool e_bVisu, bool modified)
    {
      this.native.SetLimiteVisible(e_IndexVisuLimite, e_bVisu, modified);
    }

    public void RemoveLimiteVisibleFromDB(int e_IndexVisuLimite)
    {
      this.native.RemoveLimiteVisibleFromDB(e_IndexVisuLimite);
    }

    public bool IsLimitVisible(int e_IndexVisuLimite)
    {
      return this.native.IsLimitVisible(e_IndexVisuLimite);
    }

    public Langue GetLangue()
    {
      return (Langue)this.native.RecupererCodeLangue();
    }

    public bool IsMDSPrescan()
    {
      return this.native.IsMDSPrescan();
    }

    public bool IsMDSFinal()
    {
      return this.native.IsMDSFinal();
    }

    public void MDSGetAngles1(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur, out List<double> ppPosList, out List<double> ppValList, int iScriptPosition = -1, double dHeight = Service.Provider.UndefVal, double dAngle2 = Service.Provider.UndefVal)
    {
      IntPtr ppPosTable = IntPtr.Zero;
      IntPtr ppValTable = IntPtr.Zero;
      this.native.MDSGetAngles1((int)iTypeMeas, idPoint, (int)iDetecteur, ref ppPosTable, ref ppValTable, out int pNbValueTable, iScriptPosition, dHeight, dAngle2);

      double[] posRes = new double[pNbValueTable];
      double[] valRes = new double[pNbValueTable];
      if (pNbValueTable > 0)
      {
        Marshal.Copy(ppPosTable, posRes, 0, pNbValueTable);
        Marshal.Copy(ppValTable, valRes, 0, pNbValueTable);
      }

      ppPosList = new List<double>(posRes);
      ppValList = new List<double>(valRes);
    }

    public void MDSGetAngles2(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur, out List<double> ppPosList, out List<double> ppValList, int iScriptPosition = -1, double dAngle1 = Service.Provider.UndefVal, double dHeight = Service.Provider.UndefVal)
    {
      IntPtr ppPosTable = IntPtr.Zero;
      IntPtr ppValTable = IntPtr.Zero;
      this.native.MDSGetAngles2((int)iTypeMeas, idPoint, (int)iDetecteur, ref ppPosTable, ref ppValTable, out int pNbValueTable, iScriptPosition, dAngle1, dHeight);

      double[] posRes = new double[pNbValueTable];
      double[] valRes = new double[pNbValueTable];

      if (pNbValueTable > 0)
      {
        Marshal.Copy(ppPosTable, posRes, 0, pNbValueTable);
        Marshal.Copy(ppValTable, valRes, 0, pNbValueTable);
      }

      ppPosList = new List<double>(posRes);
      ppValList = new List<double>(valRes);
    }

    public void MDSGetHeights(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur, out List<double> ppPosList, out List<double> ppValList, int iScriptPosition = -1, double dAngle1 = Service.Provider.UndefVal, double dAngle2 = Service.Provider.UndefVal)
    {
      IntPtr ppPosTable = IntPtr.Zero;
      IntPtr ppValTable = IntPtr.Zero;
      this.native.MDSGetHeights((int)iTypeMeas, idPoint, (int)iDetecteur, ref ppPosTable, ref ppValTable, out int pNbValueTable, iScriptPosition, dAngle1, dAngle2);

      double[] posRes = new double[pNbValueTable];
      double[] valRes = new double[pNbValueTable];
      if (pNbValueTable > 0)
      {
        Marshal.Copy(ppPosTable, posRes, 0, pNbValueTable);
        Marshal.Copy(ppValTable, valRes, 0, pNbValueTable);
      }

      ppPosList = new List<double>(posRes);
      ppValList = new List<double>(valRes);
    }

    public void MDSGetPositions(MDS_Types iTypeMeas, int idPoint, Detector iDetecteur,
      int iScriptPosition,
      out List<double> ppAngle1List, out List<double> ppHeightList, out List<double> ppAngle2List,
      out List<double> ppValList)
    {
      IntPtr ppAngle1Table = IntPtr.Zero;
      IntPtr ppAngle2Table = IntPtr.Zero;
      IntPtr ppHeightTable = IntPtr.Zero;
      IntPtr ppValTable = IntPtr.Zero;
      this.native.MDSGetPositions((int)iTypeMeas, idPoint, (int)iDetecteur, iScriptPosition, ref ppAngle1Table, ref ppHeightTable, ref ppAngle2Table, ref ppValTable, out int pNbValueTable);

      double[] angle1Res = new double[pNbValueTable];
      double[] angle2Res = new double[pNbValueTable];
      double[] heightRes = new double[pNbValueTable];
      double[] valRes = new double[pNbValueTable];
      if (pNbValueTable > 0)
      {
        Marshal.Copy(ppAngle1Table, angle1Res, 0, pNbValueTable);
        Marshal.Copy(ppAngle2Table, angle2Res, 0, pNbValueTable);
        Marshal.Copy(ppHeightTable, heightRes, 0, pNbValueTable);
        Marshal.Copy(ppValTable, valRes, 0, pNbValueTable);
      }

      ppAngle1List = new List<double>(angle1Res);
      ppAngle2List = new List<double>(angle2Res);
      ppHeightList = new List<double>(heightRes);
      ppValList = new List<double>(valRes);
    }

    public void MDSGetDetectors(MDS_Types iTypeMeas, int idPoint,
      int iScriptPosition,
      out List<Detector> ppDetectorList)
    {
      IntPtr ppDetectors = IntPtr.Zero;
      this.native.MDSGetDetectors((int)iTypeMeas, idPoint, iScriptPosition, ref ppDetectors, out int pCount);
      int[] detRes = new int[pCount];

      if (pCount > 0)
      {
        Marshal.Copy(ppDetectors, detRes, 0, pCount);
      }

      ppDetectorList = new List<Detector>();
      foreach (var item in detRes)
      {
        ppDetectorList.Add((Detector)item);
      }
    }

    public bool ApplyNewCorrectionOnSB(int idSB)
    {
      return this.native.ApplyNewCorrectionOnSB(idSB);
    }

    public bool CanReadPipe()
    {
      using (new NexioStopwatch($"{nameof(BatDataWrapper)}.{nameof(this.CanReadPipe)}"))
      {
        return this.native.CanReadPipe();
      }
    }

    public bool SetEtatSB(int subRangeId, int etatSousBande)
    {
      return this.native.SetEtatSB(subRangeId, etatSousBande);
    }

    public int GetNbLimiteImportee()
    {
      return this.native.GetNbLimiteImportee();
    }

    public bool AjouterLimiteImportee(int e_iIndexLimite)
    {
      return this.native.AjouterLimiteImportee(e_iIndexLimite);
    }

    public void RetirerLimiteImportee(int e_iIndexLimite)
    {
      this.native.RetirerLimiteImportee(e_iIndexLimite);
    }

    public bool GetLimiteImporteeInfo(int indexLimit, out string s_sGuidLimit, out int s_iDetector, out double s_dDistanceEUT, out int s_iClasse, out int s_iInterpolation, out int s_iTypeSignal)
    {
      var guidBuilder = new StringBuilder(256);
      var classeBuilder = new StringBuilder(256);
      var r = this.native.GetLimitesImporteeInfos(indexLimit, guidBuilder, out s_iDetector, out s_dDistanceEUT, out s_iClasse, out s_iInterpolation, out s_iTypeSignal);
      s_sGuidLimit = guidBuilder.ToString();

      return r;
    }

    public string GetPositionName(int iPositionCourbe)
    {
      IntPtr ptr = this.native.GetPositionName(iPositionCourbe);
      return Marshal.PtrToStringAnsi(ptr);
    }

    public bool GetLimiteClasseId(int indexLimite, out int classeId)
    {
      return this.native.GetLimiteClasseId(indexLimite, out classeId);
    }

    public string GetEssaiGuid()
    {
      var s_sGuidEssai = new StringBuilder(256);
      this.native.GetGuidEssai(s_sGuidEssai);

      return s_sGuidEssai.ToString();
    }

    public void SetWindowForPilote(IntPtr parent)
    {
      this.native.SetWindowForPilote(parent);
    }

    public bool HasFunctionAngle2Prescan(Fonction_Action fonctionAction, int fonctionId)
    {
      return this.native.HasFunctionAngle2Prescan(fonctionAction, fonctionId);
    }

    public bool HasFunctionAnglePrescan(Fonction_Action fonctionAction, int fonctionId)
    {
      return this.native.HasFunctionAnglePrescan(fonctionAction, fonctionId);
    }

    public bool HasFunctionHauteurPrescan(Fonction_Action fonctionAction, int fonctionId)
    {
      return this.native.HasFunctionHauteurPrescan(fonctionAction, fonctionId);
    }

    public void DemanderMontage(bool b)
    {
      this.native.DemanderMontage(b);
    }

    public string GetTexteInfoReglage(int idSb)
    {
      var text = new StringBuilder(256);
      this.native.GetTexteInfoReglage(idSb, text);

      return text.ToString();
    }

    public void GetInfosCommentaireEssai(out string comment, out string conclusion, out string dateExecution)
    {
      var sbComment = new StringBuilder(256);
      var sbConclusion = new StringBuilder(256);
      var sbDateExecution = new StringBuilder(256);

      this.native.GetInfosCommentaireEssai(sbComment, sbConclusion, sbDateExecution);

      comment = Convert.ToString(sbComment);
      conclusion = Convert.ToString(sbConclusion);
      dateExecution = Convert.ToString(sbDateExecution);
    }

    public bool CanReadLog()
    {
      return this.native.CanReadLog();
    }

    public bool ReadLogData(out int logLevel, out string message, out int idSb)
    {
      var sb = new StringBuilder(256);
      var ret = this.native.ReadLogData(out logLevel, sb, out idSb);
      message = Convert.ToString(sb);

      return ret;
    }

#pragma warning disable SA1313 // Parameter names must begin with lower-case letter
    private static double ToHz(double MHz)
#pragma warning restore SA1313 // Parameter names must begin with lower-case letter
    {
      return MHz * 1e6;
    }

    private class Native
    {
      private IntPtr handleBatDataLibrary;

      /// <summary>
      /// Ce service permet d'obtenir le nom de label equipement
      /// </summary>
      /// <param name="iNum">numero de label</param>
      /// <returns>label</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetLabelName(int iNum);

      /// <summary>
      /// Ce service permet d'obtenir le nombre de label essai
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelName();

      /// <summary>
      /// Ce service permet d'obtenir le nombre de label essai
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelValue();

      /// <summary>
      /// Ce service permet d'obtenir le nombre de label equipement
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelEquipementValue();

      /// <summary>
      /// Ce service permet d'obtenir le nom de label equipement
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetLabelEquipementName(int iNum);

      /// <summary>
      ///  Ce service permet d'obtenir la valeur de label equipement
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetLabelEquipementValue(int iNum);

      /// <summary>
      ///  Ce service permet d'obtenir le nombre de label vehicule
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelVehiculeName();

      /// <summary>
      ///  Ce service permet d'obtenir le nombre de label vehicule
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelVehiculeValue();

      /// <summary>
      ///  Ce service permet d'obtenir le nom de label vehicule
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetLabelVehiculeName(int iNum);

      /// <summary>
      ///  Ce service permet d'obtenir la valeur de label vehicule
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate string Func_GetLabelVehiculeValue(int iNum);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_InitFromFile(string sPath);

      /// <summary>
      ///  Ce service retourne des informations sur toutes les
      ///                  fonctions qui ont été sélectionnées à l’aide du service
      ///                  SelectFonctions.
      ///                  Service utilisé par : VISU FONCTION
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdFonctionSuivante();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsFonctionUtilisee(int idFct);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdFonction(int e_MaskSignature, int e_Action, bool e_bOnlyUsed);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetColumnInfoFonction(int idFonction, int iCol, int iType, int iParam1, int iParam2, double dOffset);

      /// <summary>
      /// Selection des resultats d'une action sur une sous-bande
      /// </summary>
      /// <param name="e_Action">type d'action qui a généré le résultat</param>
      /// <param name="e_idSB">identificateur de la sous-bande</param>
      /// <returns>true if OK</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_FastSelectResultatsSBAction(Fonction_Action e_Action, int e_idSB);

      /// <summary>
      /// Lecture de l'ID du resultat suivany
      /// Les resultats doivent avoir été selectionné au préalable
      /// </summary>
      /// <returns>identificateur ou EOF s'il n'y a plus de résultat</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_FastGetIdResultatSuivant();

      /// <summary>
      /// Ce service permet de retourner les identificateurs
      ///                  des sous bandes qui ont été sélectionnées avec le
      ///                  service SelectSB.
      /// </summary>
      /// <returns>
      /// Identificateur de la sous bande sinon -1 (EOF)
      ///                  s’il n’y a plus de sous-bande.
      /// </returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNombreSB();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_UnSelectSB();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetNomDllMesureur(int idMesureur, StringBuilder szNomDLL);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbResultatsFonction(int idFonction, int idSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetTraitementSuivant(ref int pidSB, out int pidFirstPt, out int pidLastPt, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szNomDLL, double rangeFreqMin, double rangeFreqMax);

      /// <summary>
      /// Fonction qui retourne la valeur de la fréquence d’un
      ///                point  de  fréquence mesurée par une fonction (finaux) .
      ///                Par défaut retourne la fréquence du point
      /// </summary>
      /// <param name="e_idPointFreq">identificateur du Point de  fréquence</param>
      /// <param name="e_idFonction">identificateur de la fonction</param>
      /// <returns>La fréquence du point en Mhz</returns>
      /// <remarks>Renvoi du Mhz</remarks>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_GetFrequenceMesure(int e_idPointFreq, int e_idFonction);

      /// <summary>
      /// Ce service permet d’obtenir le type d'action effectué
      /// par une fonction à partir de son identificateur
      /// </summary>
      /// <param name="e_idFonction">e_idFonction</param>
      /// <returns>signature de la fonction., -1 si erreur</returns>
      /// <remarks>Renvoi du Mhz</remarks>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetActionFonction(int e_idFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SauvegarderResultatsFonctionAuto(int e_idFonction, string sName, out IntPtr ptrIdFonction, out IntPtr ptrIdCourbes, out int nbrIdFonction, out int nbrIdCourbe);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_UnSelectFonction();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdFonctionName(string sName, int e_Action, int iNbValeurs, string sColonne);

      /// <summary>
      /// Ce service permet de signaler la fin d’exécution d’une
      ///                  fonction sur une sous-bande.
      /// </summary>
      /// <param name="e_idSB">état de la sous bande</param>
      /// <param name="e_idFonction">identificateur de la fonction</param>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_FinFonctionSB(int e_idSB, int e_idFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate Test_State Func_GetEtatEssai();

      /// <summary>
      /// Ce service permet d'obtenir le nombre de label equipement
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLabelEquipementName();

      /// <summary>
      ///  Ce service permet de sauvegarder toutes la données
      ///    d’un essai et de modifier son état.
      ///
      ///    Le module DATA est réinitialisé, toute la mémoire allouée est libérée.
      ///
      ///    Ce service est appelé automatiquement si nécessaire lors du déchargement
      ///    de la DLL (ExitInstance) avec l’état ‘EnCours’
      /// </summary>
      /// <param name="e_Etat"> état final de l’essai (En cours, Terminé)</param>
      /// <returns>True si le traitement s’est déroulé correctement</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_Terminer(int e_Etat);

      /// <summary>
      /// Ce service permet de sélectionner toutes les fonctions
      /// d’analyse dont le type correspond à celui passé en parametre.
      /// </summary>
      /// <param name="e_TypeFonction">e_TypeFonction type de fonction ou A_ALL pour toutes</param>
      /// <returns>FALSE si selection impossible car active</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SelectFonction(Fonction_Action e_TypeFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetGuidMontageSB(int idSB, int iSegmentSize);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsChangeDomaineSB();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetEtatSB(int idSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SelectFonctionWpfImport(Fonction_Action e_TypeFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate void Func_SelectPositionCourbeExt(int idCourbe);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNextPositionCourbeExt([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder nomPosition);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SelectCourbesExt();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetCourbeExtSuivante();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetInfoCourbeExt(int e_IdCourbeExt, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_Nom, out int s_piTypeSignal, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_GuidEssaiSource);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_SelectValeursCourbeExt();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetNextValeurCourbeExt(out double s_pFreq, out double s_pfValeur);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetUniteEssaiUnicode([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder s_Unite, int iTypeSignal);

      /// <summary>
      ///  Ce service retourne des informations sur toutes les
      ///  fonctions qui ont été sélectionnées à l’aide du service
      ///  SelectFonctions.
      ///  Service utilisé par : VISU
      /// </summary>
      /// <param name="s_Intitule">nom de la fonction</param>
      /// <param name="s_TypeFonction">type de la fonction</param>
      /// <param name="bEssaiUsed">TRUE si utilisée dans l'essai</param>
      /// <returns>Identificateur de la fonction sinon -1 (EOF</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetFonctionSuivante([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_Intitule, out int s_TypeFonction, out bool bEssaiUsed);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetFonctionSuivanteWpfImport([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_Intitule, out int s_TypeFonction, out bool bEssaiUsed);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSignatureFonction(int e_idFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSignatureFonctionForAnyEssai(int i_idFonction, string s_idEssai, string s_idProjet);

      /// <summary>
      /// Initialise l'essai
      /// </summary>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_Initialiser(string e_sEssai, bool bUseDSN, int iEssai, string sPathResult, bool bUpdateFromDatabase);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_InitialiserWpfImport(string e_sEssai, string e_sProject, int iEssai, string sPathResult);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)] // TODO Finir l'appel
      public delegate bool Func_GetIdPointFrequence(int e_idSousBande, double e_Frequence, out int s_ident, out double s_freq, out bool s_bSuperieur);

      /// <summary>
      /// Ce service permet de sélectionner toutes les sous
      /// bandes qui correspondent au réglage passé en
      /// parametre et qui ont l’état passé en parametre.
      /// </summary>
      /// <param name="e_iPosition">position recherchée</param>
      /// <param name="bIncNoPos">FALSE pour exclure les SB sans position</param>
      /// <returns>true if OK</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SelectSB(int e_iPosition, string posName, bool bIncNoPos);

      /// <summary>
      /// Ce service permet de retourner les informations
      ///                  relative à la sous bande.
      /// </summary>
      /// <param name="e_IdSB">identificateur de la sous bande</param>
      /// <param name="s_fMin">frequence min</param>
      /// <param name="s_fMax">frequence max</param>
      /// <param name="s_fPas">frequency step</param>
      /// <param name="typeProgress">type de progression TODO remplacer par un enum</param>
      /// <param name="s_nbPoint">nombre de point dans la sous bande</param>
      /// <param name="s_NomScriptPosition">TODO ??</param>
      /// <param name="s_NomTypePosition">TODO ???</param>
      /// <returns>true if OK</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetInfosSB(int e_IdSB,
      out double s_fMin, out double s_fMax, out double s_fPas,
      out TypesProgression typeProgress, out int s_nbPoint,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_NomScriptPosition,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_NomTypePosition);

      /// <summary>
      /// Ce service permet de retourner les identificateurs
      ///                  des sous bandes qui ont été sélectionnées avec le
      ///                  service SelectSB.
      /// </summary>
      /// <returns>
      /// Identificateur de la sous bande sinon -1 (EOF)
      ///                  s’il n’y a plus de sous-bande.
      /// </returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetSBSuivante();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetGuidSB(int e_IdSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetNomSB(int e_IdSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetCommentSB(int e_IdSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetPositionSB(int e_IdSB, out IntPtr text);

      /// <summary>
      /// Description : Service de lecture du premier identificateur et du nombre
      ///   de points de fréquence d’une sous-bande.
      ///
      /// Remarque : Ce service peut être utilisé par une DLL pour stocker les
      ///     résultats plus rapidement en incrémentant directement l’ID apres chaque
      ///     traitement d’un point  de  fréquence (cas d’un préscan sur une sous-bande).
      ///
      /// </summary>
      /// <param name="e_idSousBande">identificateur de la sous-bande</param>
      /// <param name="s_idFreqMin">identificateur du 1° point fréquence
      ///                de la sous-bande (les autres sont consécutifs).</param>
      /// <param name="s_Nombre">Nombre de Point de  fréquence de la sous-bande</param>
      /// <returns>true if OK</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetIdFreqSousbande(int e_idSousBande, out int s_idFreqMin, out int s_Nombre);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdSousBande(int e_idPoint);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIdTypeEssai();

      /// <summary>
      /// Ce service permet d’obtenir le résultat d’une
      /// fonction pour Un point de  fréquence donné.
      /// </summary>
      /// <param name="e_IdPointFreq">identificateur du Point de fréquence concerné</param>
      /// <param name="e_IdFonction">identificateur de la fonction</param>
      /// <param name="s_tValeurs">table des valeurs résultat de la fonction</param>
      /// <param name="s_bEtat">etat</param>
      /// <returns>TRUE si le résultat est disponible.</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetResultat(int e_IdPointFreq, int e_IdFonction,
        out IntPtr s_tValeurs,
        out bool s_bEtat);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetResultatString(int idPointFreq, int idFonction, int iRes, out IntPtr text);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_GetResultatDouble(int idPointFreq, int idFonction, int iRes, out double value);

      /// <summary>
      /// Ce service permet de selectionner rapidement la première frequence d'une fonction pour la sous-bande donnée
      /// </summary>
      /// <param name="e_idFonction">The e identifier fonction.</param>
      /// <param name="e_idSousBande">The e identifier sous bande.</param>
      /// <returns>true if OK</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_FastSelectResultatsSBFonction(int e_idFonction, int e_idSousBande);

      /// <summary>
      /// Fonction qui retourne la valeur de la fréquence d’un
      /// point  de  fréquence.
      /// </summary>
      /// <param name="e_idPointFreq">identificateur du Point de  fréquence</param>
      /// <returns>double valeur de la fréquence</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_GetFrequence(int e_idPointFreq);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetInfosEssai([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sNomEssai,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sTypeEssai,
          out double s_pfFreqMin, out double s_pfFreqMax,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sNomLimite,
          out int s_piClasse,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder sEquipement,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder sVersion,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder sRefHard,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder sRefSoft,
          [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder sModifications);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetInfoColumnFonction(int idFonction, int iCol, out TypeCol piColType, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_ColName);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetCompleteInfoColumnFonction(int idFonction, int iCol, out TypeCol piColType, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_ColName, out int iParam1, out int iParam2, out double dOffset);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetInfosResultatsFonctionTab(
      int e_idFonction,
      out int s_NbValeurs,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_paColNames,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_paColUsed);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_GetCommentaireResultat(int e_idPoint, int e_idFonction, [MarshalAs(UnmanagedType.LPStr)] StringBuilder s_Commentaire);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetListePositionsTab(
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_pListePos,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_pListeNomPos);

      /// <summary>
      /// Retourne le nombre de limites a afficher par VISU
      ///    Les index de limites sont compris entre 0 et cette
      ///     valeur
      /// </summary>
      /// <returns>le nombre de limites a afficher par VISU</returns>
      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbVisuLimite();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetInfoVisuLimite(
      int indexLimite,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szNom,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szClasse,
      out int iSignature,
      out double fDistance,
      out int iTypeSignal,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guidLimite,
      out bool bVisible,
      out int iTypeInterpolation);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetInfoVisuLimiteDesc(
      int indexLimite,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szNom,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szClasse,
      out int iSignature,
      out double fDistance,
      out int iTypeSignal,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder guidLimite,
      out bool bVisible,
      out int iTypeInterpolation,
      [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder szDesc);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetVisuLimiteNiveaux(
      int e_IndexVisuLimite,
      out int s_NbValeurs,
      out IntPtr s_Freq,
      out IntPtr s_Niveaux);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetLimiteNiveau(int e_iSigDetecteur, int e_iTypeSignal, int e_idPointFreq, out double s_pfNiveau, bool bUseVisibleLimits);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetScriptDebutSB(int iSBCourante);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetScriptFinSB(int iSBCourante);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_GetFreqMinSB(int idSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_GetFreqMaxSB(int idSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SelectDLLFonction(int action);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetDLLFonctionSuivante([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_Intitule, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_NomDLL);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_ResetEventAffiche();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_Sauvegarder(bool bIntermediaire);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_EssaiModifie();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetEventAffiche();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetExecData(ref int idSB, ref int idFonction, ref int idPtFirstPt, ref int nbrePts);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SetExecData(int e_idSB, int e_idFonction, int e_idPtFirstPt, int e_NbrePts);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsGlobalProcedure();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetGlobalProcedure(int m_idSBAuto, double m_dRangeFreqMin, double m_dRangeFreqMax, ref int idAction, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_NomDLL);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetPositionInitialeSB(int e_IdSousBande, out double pfDistance, out int piScriptPosition, out bool s_pbHauteurAuto, out double s_pfHauteur, out bool s_pbAngleAuto, out double s_pfAngle, out IntPtr text);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetScriptEndPrescan();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetScriptEndTest();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_ExporterResultatsFonctionWpf(string szFileName, int[] i_tIds, int count, bool bNewFormat, bool bExportColonne, bool bFilter, bool bExternalCurves);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_ImporterCourbeExtWpf(string idProject, int idEssai, int idFunction, string nomCourbe);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_ImporterCourbeExtFromFileWpf(string e_sNomCourbe, int iTypeEssai, string e_sFileName, ushort[] s_pTabIdCourbes);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetResultat(int e_idPoint, int e_idFonction, int e_NbValeurs, double[] e_tValeurs, bool e_bEtat, double dCorrection);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_ImporterMaxCourbesExtWpf(int[] t_iEssaisSources, string s_idProjet, int elementsCount, int idFonction, string szNomCourbe, bool bOnlyMaxOfPositions);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetFrequenceMesure(int e_idPoint, int e_idFonction, double dFrequence);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetCommentaireResultat(int e_idPoint, int e_idFonction, string e_Commentaire);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_GetParametresReglage(int e_idReglage, out int s_NbReglage, StringBuilder listBuilder);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_SupprimerCourbeExt(int e_IdCourbeExt);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetResultatEtat(int e_IdPoint, int e_idFonction, bool e_bEtat);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_DeleteEssaiImport();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_LoadLogConfig(string confFile);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetPathResult();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetIDEssai();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetConfEM();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetLimiteVisible(int e_IndexVisuLimite, bool e_bVisu, bool modified);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_RemoveLimiteVisibleFromDB(int e_IndexVisuLimite);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_IsLimitVisible(int e_IndexVisuLimite);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_RecupererCodeLangue();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsMDSPrescan();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_IsMDSFinal();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_MDSGetAngles1(int iTypeMeas, int idPoint, int iDetecteur, ref IntPtr ppPosTable, ref IntPtr ppValTable, out int pNbValueTable, int iScriptPosition, double dHeight, double dAngle2);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_MDSGetAngles2(int iTypeMeas, int idPoint, int iDetecteur, ref IntPtr ppPosTable, ref IntPtr ppValTable, out int pNbValueTable, int iScriptPosition, double dAngle1, double dHeight);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_MDSGetHeights(int iTypeMeas, int idPoint, int iDetecteur, ref IntPtr ppPosTable, ref IntPtr ppValTable, out int pNbValueTable, int iScriptPosition, double dAngle1, double dAngle2);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_MDSGetPositions(
        int iTypeMeas,
        int idPoint,
        int iDetecteur,
        int iScriptPosition,
        ref IntPtr pAngle1Table,
        ref IntPtr pHauteurTable,
        ref IntPtr pAngle2Table,
        ref IntPtr ppValTable,
        out int pNbValueTable);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_MDSGetDetectors(
        int iTypeMeas,
        int idPoint,
        int iScriptPosition,
        ref IntPtr ppDetectors,
        out int pCount);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_ApplyNewCorrectionOnSB(int idSB);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_CanReadPipe();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_SetEtatSB(int idSB, int e_iEtat);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetNbLimiteImportee();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_AjouterLimiteImportee(int e_iIndexLimite);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate void Func_RetirerLimiteImportee(int e_iIndexLimite);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetLimitesImporteeInfos(int limiteIndex, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sGuidLimit, out int s_iDetector, out double s_dDistanceEUT, out int s_iClasse, out int s_iInterpolation, out int s_iTypeSignal);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetLimiteClasseId(int e_iIndexLimite, out int s_iClasse);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate void Func_SetWindowForPilote(IntPtr parent);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetGuidEssai([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sGuidEssai);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_HasFunctionAngle2Prescan(Fonction_Action e_iFonctionAction, int fonctionId);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_HasFunctionAnglePrescan(Fonction_Action e_iFonctionAction, int fonctionId);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_HasFunctionHauteurPrescan(Fonction_Action e_iFonctionAction, int fonctionId);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_DemanderMontage(bool e_bDemander);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetTexteInfoReglage(int e_idReglage, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_TexteReglage);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate IntPtr Func_GetPositionName(int idPositionType);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_GetInfosCommentaireEssai([MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sCommentaire,
        [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sConclusion, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sDateExecution);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_ReadLogData(out int s_iLogLevel, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder s_sMessage, out int s_iIdSb);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_CanReadLog();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate bool Func_SetIsV4(bool bIsV4);

      public Func_SetIsV4 SetIsV4 { get; private set; }

      public Func_SetResultatEtat SetResultatEtat { get; private set; }

      public Func_ImporterMaxCourbesExtWpf ImporterMaxCourbesExtWpf { get; private set; }

      public Func_FastGetIdResultatSuivant FastGetIdResultatSuivant { get; private set; }

      public Func_FastSelectResultatsSBAction FastSelectResultatsSBAction { get; private set; }

      public Func_FastSelectResultatsSBFonction FastSelectResultatsSBFonction { get; private set; }

      public Func_FinFonctionSB FinFonctionSB { get; private set; }

      public Func_GetActionFonction GetActionFonction { get; private set; }

      public Func_SauvegarderResultatsFonctionAuto SauvegarderResultatsFonctionAuto { get; private set; }

      public Func_GetCommentSB GetCommentSB { get; private set; }

      public Func_GetPositionSB GetPositionSB { get; private set; }

      public Func_GetCourbeExtSuivante GetCourbeExtSuivante { get; private set; }

      public Func_GetDLLFonctionSuivante GetDLLFonctionSuivante { get; private set; }

      public Func_GetEtatEssai GetEtatEssai { get; private set; }

      public Func_GetExecData GetExecData { get; private set; }

      public Func_GetFonctionSuivante GetFonctionSuivante { get; private set; }

      public Func_GetFonctionSuivanteWpfImport GetFonctionSuivanteWpfImport { get; private set; }

      public Func_GetFreqMaxSB GetFreqMaxSB { get; private set; }

      public Func_GetFreqMinSB GetFreqMinSB { get; private set; }

      public Func_GetFrequence GetFrequence { get; private set; }

      public Func_GetFrequenceMesure GetFrequenceMesure { get; private set; }

      public Func_GetGlobalProcedure GetGlobalProcedure { get; private set; }

      public Func_GetGuidSB GetGuidSB { get; private set; }

      public Func_GetIdFonction GetIdFonction { get; private set; }

      public Func_GetIdFonctionName GetIdFonctionName { get; private set; }

      public Func_GetIdFonctionSuivante GetIdFonctionSuivante { get; private set; }

      public Func_IsFonctionUtilisee IsFonctionUtilisee { get; private set; }

      public Func_GetIdFreqSousbande GetIdFreqSousbande { get; private set; }

      public Func_GetIdSousBande GetIdSousBande { get; private set; }

      public Func_GetIdTypeEssai GetIdTypeEssai { get; private set; }

      public Func_GetInfoColumnFonction GetInfoColumnFonction { get; private set; }

      public Func_GetCompleteInfoColumnFonction GetCompleteInfoColumnFonction { get; private set; }

      public Func_GetInfoCourbeExt GetInfoCourbeExt { get; private set; }

      public Func_GetInfosEssai GetInfosEssai { get; private set; }

      public Func_GetInfosResultatsFonctionTab GetInfosResultatsFonctionTab { get; private set; }

      public Func_GetCommentaireResultat GetCommentaireResultat { get; private set; }

      public Func_GetInfosSB GetInfosSB { get; private set; }

      public Func_GetInfoVisuLimite GetInfoVisuLimite { get; private set; }

      public Func_GetInfoVisuLimiteDesc GetInfoVisuLimiteDesc { get; private set; }

      public Func_GetLabelEquipementName GetLabelEquipementName { get; private set; }

      public Func_GetLabelEquipementValue GetLabelEquipementValue { get; private set; }

      public Func_GetLabelName GetLabelName { get; private set; }

      public Func_GetLabelVehiculeName GetLabelVehiculeName { get; private set; }

      public Func_GetLabelVehiculeValue GetLabelVehiculeValue { get; private set; }

      public Func_GetLimiteNiveau GetLimiteNiveau { get; private set; }

      public Func_GetListePositionsTab GetListePositionsTab { get; private set; }

      public Func_GetNbLabelEquipementName GetNbLabelEquipementName { get; private set; }

      public Func_GetNbLabelEquipementValue GetNbLabelEquipementValue { get; private set; }

      public Func_GetNbLabelName GetNbLabelName { get; private set; }

      public Func_GetNbLabelValue GetNbLabelValue { get; private set; }

      public Func_GetNbLabelVehiculeName GetNbLabelVehiculeName { get; private set; }

      public Func_GetNbLabelVehiculeValue GetNbLabelVehiculeValue { get; private set; }

      public Func_GetNbResultatsFonction GetNbResultatsFonction { get; private set; }

      public Func_GetNbVisuLimite GetNbVisuLimite { get; private set; }

      public Func_GetNextPositionCourbeExt GetNextPositionCourbeExt { get; private set; }

      public Func_GetNextValeurCourbeExt GetNextValeurCourbeExt { get; private set; }

      public Func_GetNombreSB GetNombreSB { get; private set; }

      public Func_GetNomSB GetNomSB { get; private set; }

      public Func_GetPositionInitialeSB GetPositionInitialeSB { get; private set; }

      public Func_GetResultat GetResultat { get; private set; }

      public Func_GetResultatString GetResultatString { get; private set; }

      public Func_GetResultatDouble GetResultatDouble { get; private set; }

      public Func_GetSBSuivante GetSBSuivante { get; private set; }

      public Func_GetScriptDebutSB GetScriptDebutSB { get; private set; }

      public Func_GetScriptEndPrescan GetScriptEndPrescan { get; private set; }

      public Func_GetScriptEndTest GetScriptEndTest { get; private set; }

      public Func_GetScriptFinSB GetScriptFinSB { get; private set; }

      public Func_GetSignatureFonction GetSignatureFonction { get; private set; }

      public Func_GetSignatureFonctionForAnyEssai GetSignatureFonctionForAnyEssai { get; private set; }

      public Func_GetTraitementSuivant GetTraitementSuivant { get; private set; }

      public Func_GetUniteEssaiUnicode GetUniteEssaiUnicode { get; private set; }

      public Func_GetVisuLimiteNiveaux GetVisuLimiteNiveaux { get; private set; }

      public Func_ExporterResultatsFonctionWpf ExporterResultatsFonctionWpf { get; private set; }

      public Func_ImporterCourbeExtFromFileWpf ImporterCourbeExtFromFileWpf { get; private set; }

      public Func_GetIdPointFrequence GetIdPointFrequence { get; private set; }

      public Func_ImporterCourbeExtWpf ImporterCourbeExtWpf { get; private set; }

      public Func_InitFromFile InitFromFile { get; private set; }

      public Func_Initialiser Initialiser { get; private set; }

      public Func_InitialiserWpfImport InitialiserWpfImport { get; private set; }

      public Func_IsGlobalProcedure IsGlobalProcedure { get; private set; }

      public Func_ResetEventAffiche ResetEventAffiche { get; private set; }

      public Func_Sauvegarder Sauvegarder { get; private set; }

      public Func_EssaiModifie EssaiModifie { get; private set; }

      public Func_SelectCourbesExt SelectCourbesExt { get; private set; }

      public Func_SelectDLLFonction SelectDLLFonction { get; private set; }

      public Func_SelectFonction SelectFonction { get; private set; }

      public Func_GetGuidMontageSB GetGuidMontageSB { get; private set; }

      public Func_IsChangeDomaineSB IsChangeDomaineSB { get; private set; }

      public Func_GetEtatSB GetEtatSB { get; private set; }

      public Func_SelectFonctionWpfImport SelectFonctionWpfImport { get; private set; }

      public Func_SelectPositionCourbeExt SelectPositionCourbeExt { get; private set; }

      public Func_SelectSB SelectSB { get; private set; }

      public Func_SelectValeursCourbeExt SelectValeursCourbeExt { get; private set; }

      public Func_SetColumnInfoFonction SetColumnInfoFonction { get; private set; }

      public Func_SetCommentaireResultat SetCommentaireResultat { get; private set; }

      public Func_SetEventAffiche SetEventAffiche { get; private set; }

      public Func_SetExecData SetExecData { get; private set; }

      public Func_SetFrequenceMesure SetFrequenceMesure { get; private set; }

      public Func_SetResultat SetResultat { get; private set; }

      public Func_Terminer Terminer { get; private set; }

      public Func_UnSelectFonction UnSelectFonction { get; private set; }

      public Func_UnSelectSB UnSelectSB { get; private set; }

      public Func_GetNomDllMesureur GetNomDllMesureur { get; private set; }

      public Func_GetParametresReglage GetParametresReglage { get; set; }

      public Func_SupprimerCourbeExt SupprimerCourbeExt { get; private set; }

      public Func_DeleteEssaiImport DeleteEssaiImport { get; private set; }

      public Func_LoadLogConfig LoadLogConfig { get; private set; }

      public Func_GetPathResult GetPathResult { get; private set; }

      public Func_GetIDEssai GetIDEssai { get; private set; }

      public Func_GetConfEM GetConfEM { get; private set; }

      public Func_SetLimiteVisible SetLimiteVisible { get; private set; }

      public Func_RemoveLimiteVisibleFromDB RemoveLimiteVisibleFromDB { get; private set; }

      public Func_IsLimitVisible IsLimitVisible { get; private set; }

      public Func_RecupererCodeLangue RecupererCodeLangue { get; private set; }

      public Func_IsMDSPrescan IsMDSPrescan { get; private set; }

      public Func_IsMDSFinal IsMDSFinal { get; private set; }

      public Func_MDSGetAngles1 MDSGetAngles1 { get; private set; }

      public Func_MDSGetAngles2 MDSGetAngles2 { get; private set; }

      public Func_MDSGetHeights MDSGetHeights { get; private set; }

      public Func_MDSGetPositions MDSGetPositions { get; private set; }

      public Func_MDSGetDetectors MDSGetDetectors { get; private set; }

      public Func_ApplyNewCorrectionOnSB ApplyNewCorrectionOnSB { get; private set; }

      public Func_CanReadPipe CanReadPipe { get; private set; }

      public Func_SetEtatSB SetEtatSB { get; private set; }

      public Func_GetNbLimiteImportee GetNbLimiteImportee { get; private set; }

      public Func_AjouterLimiteImportee AjouterLimiteImportee { get; private set; }

      public Func_RetirerLimiteImportee RetirerLimiteImportee { get; private set; }

      public Func_GetLimitesImporteeInfos GetLimitesImporteeInfos { get; private set; }

      public Func_GetLimiteClasseId GetLimiteClasseId { get; private set; }

      public Func_SetWindowForPilote SetWindowForPilote { get; private set; }

      public Func_GetGuidEssai GetGuidEssai { get; private set; }

      public Func_HasFunctionAngle2Prescan HasFunctionAngle2Prescan { get; private set; }

      public Func_HasFunctionAnglePrescan HasFunctionAnglePrescan { get; private set; }

      public Func_HasFunctionHauteurPrescan HasFunctionHauteurPrescan { get; private set; }

      public Func_DemanderMontage DemanderMontage { get; private set; }

      public Func_GetTexteInfoReglage GetTexteInfoReglage { get; private set; }

      public Func_GetPositionName GetPositionName { get; private set; }

      public Func_GetInfosCommentaireEssai GetInfosCommentaireEssai { get; private set; }

      public Func_CanReadLog CanReadLog { get; private set; }

      public Func_ReadLogData ReadLogData { get; private set; }

      public bool IsInitialized { get; set; } = false;

      public TDelegate AttachGenericFunction<TDelegate>(string cppFunctionName, bool optionnalFunction = false)
      {
        IntPtr ptrFunction = Win32.GetProcAddress(this.handleBatDataLibrary, cppFunctionName);
        if (ptrFunction == IntPtr.Zero)
        {
          if (optionnalFunction == false)
          {
            throw new Exception(string.Format(Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.CanNotLoad0Function1, "Bat_data.dll", cppFunctionName));
          }
          else
          {
            return default(TDelegate);
          }
        }

        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptrFunction);
      }

      internal bool InitialiserFonctionDLL()
      {
        string pathEmi = ExecPathHelper.GetExecDirectory();
        var dllPath = System.IO.Path.Combine(pathEmi, "bat_data.dll");
        if (!System.IO.File.Exists(dllPath))
        {
          var ex = new Exception(string.Format("bat_data.dll not found ({0})", dllPath));
          Log.Error(ex);
          throw ex;
        }

        this.handleBatDataLibrary = Win32.LoadLibrary(dllPath);

        if (this.handleBatDataLibrary != IntPtr.Zero)
        {
          this.FastGetIdResultatSuivant = this.AttachGenericFunction<Func_FastGetIdResultatSuivant>("FastGetIdResultatSuivant");
          this.FastSelectResultatsSBAction = this.AttachGenericFunction<Func_FastSelectResultatsSBAction>("FastSelectResultatsSBAction");
          this.FastSelectResultatsSBFonction = this.AttachGenericFunction<Func_FastSelectResultatsSBFonction>("FastSelectResultatsSBFonction");
          this.FinFonctionSB = this.AttachGenericFunction<Func_FinFonctionSB>("FinFonctionSB");
          this.GetActionFonction = this.AttachGenericFunction<Func_GetActionFonction>("GetActionFonction");
          this.SauvegarderResultatsFonctionAuto = this.AttachGenericFunction<Func_SauvegarderResultatsFonctionAuto>("SauvegarderResultatsFonctionAutoInt");
          this.GetCommentSB = this.AttachGenericFunction<Func_GetCommentSB>("GetCommentSB");
          this.GetCourbeExtSuivante = this.AttachGenericFunction<Func_GetCourbeExtSuivante>("GetCourbeExtSuivante");
          this.GetDLLFonctionSuivante = this.AttachGenericFunction<Func_GetDLLFonctionSuivante>("GetDLLFonctionSuivante");
          this.GetEtatEssai = this.AttachGenericFunction<Func_GetEtatEssai>("GetEtatEssai");
          this.GetExecData = this.AttachGenericFunction<Func_GetExecData>("GetExecData");
          this.GetFonctionSuivante = this.AttachGenericFunction<Func_GetFonctionSuivante>("GetFonctionSuivante");
          this.GetFonctionSuivanteWpfImport = this.AttachGenericFunction<Func_GetFonctionSuivanteWpfImport>("GetFonctionSuivanteWpfImport");
          this.GetFreqMaxSB = this.AttachGenericFunction<Func_GetFreqMaxSB>("GetFreqMaxSB");
          this.GetFreqMinSB = this.AttachGenericFunction<Func_GetFreqMinSB>("GetFreqMinSB");
          this.GetFrequence = this.AttachGenericFunction<Func_GetFrequence>("GetFrequence");
          this.GetFrequenceMesure = this.AttachGenericFunction<Func_GetFrequenceMesure>("GetFrequenceMesure");
          this.GetGlobalProcedure = this.AttachGenericFunction<Func_GetGlobalProcedure>("GetGlobalProcedure");
          this.GetGuidSB = this.AttachGenericFunction<Func_GetGuidSB>("GetGuidSB");
          this.GetGuidMontageSB = this.AttachGenericFunction<Func_GetGuidMontageSB>("GetGuidMontageSB");
          this.GetIdFonction = this.AttachGenericFunction<Func_GetIdFonction>("GetIdFonction");
          this.GetIdFonctionName = this.AttachGenericFunction<Func_GetIdFonctionName>("GetIdFonctionName");
          this.GetIdFonctionSuivante = this.AttachGenericFunction<Func_GetIdFonctionSuivante>("GetIdFonctionSuivante");
          this.IsFonctionUtilisee = this.AttachGenericFunction<Func_IsFonctionUtilisee>("IsFonctionUtilisee");
          this.GetIdFreqSousbande = this.AttachGenericFunction<Func_GetIdFreqSousbande>("GetIdFreqSousbande");
          this.GetIdPointFrequence = this.AttachGenericFunction<Func_GetIdPointFrequence>("GetIdPointFrequence");
          this.GetIdSousBande = this.AttachGenericFunction<Func_GetIdSousBande>("GetIdSousBande");
          this.GetIdTypeEssai = this.AttachGenericFunction<Func_GetIdTypeEssai>("GetIdTypeEssai");
          this.GetInfoColumnFonction = this.AttachGenericFunction<Func_GetInfoColumnFonction>("GetInfoColumnFonction");
          this.GetCompleteInfoColumnFonction = this.AttachGenericFunction<Func_GetCompleteInfoColumnFonction>("GetCompleteInfoColumnFonction");
          this.GetInfoCourbeExt = this.AttachGenericFunction<Func_GetInfoCourbeExt>("GetInfoCourbeExt");
          this.GetInfosEssai = this.AttachGenericFunction<Func_GetInfosEssai>("GetInfosEssai");
          this.GetInfosResultatsFonctionTab = this.AttachGenericFunction<Func_GetInfosResultatsFonctionTab>("GetInfosResultatsFonctionTab");
          this.GetCommentaireResultat = this.AttachGenericFunction<Func_GetCommentaireResultat>("GetCommentaireResultat");
          this.GetInfosSB = this.AttachGenericFunction<Func_GetInfosSB>("GetInfosSB");
          this.GetInfoVisuLimite = this.AttachGenericFunction<Func_GetInfoVisuLimite>("GetInfoVisuLimite");
          this.GetInfoVisuLimiteDesc = this.AttachGenericFunction<Func_GetInfoVisuLimiteDesc>("GetInfoVisuLimiteDesc");
          this.GetLabelEquipementName = this.AttachGenericFunction<Func_GetLabelEquipementName>("GetLabelEquipementName");
          this.GetLabelEquipementValue = this.AttachGenericFunction<Func_GetLabelEquipementValue>("GetLabelEquipementValue");
          this.GetLabelName = this.AttachGenericFunction<Func_GetLabelName>("GetLabelName");
          this.GetLabelVehiculeName = this.AttachGenericFunction<Func_GetLabelVehiculeName>("GetLabelVehiculeName");
          this.GetLabelVehiculeValue = this.AttachGenericFunction<Func_GetLabelVehiculeValue>("GetLabelVehiculeValue");
          this.GetLimiteNiveau = this.AttachGenericFunction<Func_GetLimiteNiveau>("GetLimiteNiveau");
          this.GetListePositionsTab = this.AttachGenericFunction<Func_GetListePositionsTab>("GetListePositionsTab");
          this.GetNbLabelEquipementName = this.AttachGenericFunction<Func_GetNbLabelEquipementName>("GetNbLabelEquipementName");
          this.GetNbLabelEquipementValue = this.AttachGenericFunction<Func_GetNbLabelEquipementValue>("GetNbLabelEquipementValue");
          this.GetNbLabelName = this.AttachGenericFunction<Func_GetNbLabelName>("GetNbLabelName");
          this.GetNbLabelValue = this.AttachGenericFunction<Func_GetNbLabelValue>("GetNbLabelValue");
          this.GetNbLabelVehiculeName = this.AttachGenericFunction<Func_GetNbLabelVehiculeName>("GetNbLabelVehiculeName");
          this.GetNbLabelVehiculeValue = this.AttachGenericFunction<Func_GetNbLabelVehiculeValue>("GetNbLabelVehiculeValue");
          this.GetNbResultatsFonction = this.AttachGenericFunction<Func_GetNbResultatsFonction>("GetNbResultatsFonction");
          this.GetNbVisuLimite = this.AttachGenericFunction<Func_GetNbVisuLimite>("GetNbVisuLimite");
          this.GetNextPositionCourbeExt = this.AttachGenericFunction<Func_GetNextPositionCourbeExt>("GetNextPositionCourbeExt");
          this.GetNextValeurCourbeExt = this.AttachGenericFunction<Func_GetNextValeurCourbeExt>("GetNextValeurCourbeExt");
          this.GetNombreSB = this.AttachGenericFunction<Func_GetNombreSB>("GetNombreSB");
          this.GetNomSB = this.AttachGenericFunction<Func_GetNomSB>("GetNomSB");
          this.GetPositionSB = this.AttachGenericFunction<Func_GetPositionSB>("GetPositionSB");
          this.GetPositionInitialeSB = this.AttachGenericFunction<Func_GetPositionInitialeSB>("GetPositionInitialeSB");
          this.GetResultat = this.AttachGenericFunction<Func_GetResultat>("GetResultat");
          this.GetResultatString = this.AttachGenericFunction<Func_GetResultatString>("GetResultatString");
          this.GetSBSuivante = this.AttachGenericFunction<Func_GetSBSuivante>("GetSBSuivante");
          this.GetScriptDebutSB = this.AttachGenericFunction<Func_GetScriptDebutSB>("GetScriptDebutSB");
          this.GetScriptEndPrescan = this.AttachGenericFunction<Func_GetScriptEndPrescan>("GetScriptEndPrescan");
          this.GetScriptEndTest = this.AttachGenericFunction<Func_GetScriptEndTest>("GetScriptEndTest");
          this.GetScriptFinSB = this.AttachGenericFunction<Func_GetScriptFinSB>("GetScriptFinSB");
          this.GetSignatureFonction = this.AttachGenericFunction<Func_GetSignatureFonction>("GetSignatureFonction");
          this.GetTraitementSuivant = this.AttachGenericFunction<Func_GetTraitementSuivant>("GetTraitementSuivant");
          this.GetUniteEssaiUnicode = this.AttachGenericFunction<Func_GetUniteEssaiUnicode>("GetUniteEssaiUnicode");
          this.GetVisuLimiteNiveaux = this.AttachGenericFunction<Func_GetVisuLimiteNiveaux>("GetVisuLimiteNiveaux");
          this.ExporterResultatsFonctionWpf = this.AttachGenericFunction<Func_ExporterResultatsFonctionWpf>("ExporterResultatsFonctionWpf");
          this.ImporterCourbeExtFromFileWpf = this.AttachGenericFunction<Func_ImporterCourbeExtFromFileWpf>("ImporterCourbeExtFromFileWpf");
          this.ImporterCourbeExtWpf = this.AttachGenericFunction<Func_ImporterCourbeExtWpf>("ImporterCourbeExtWpf");
          this.InitFromFile = this.AttachGenericFunction<Func_InitFromFile>("InitFromFile");
          this.Initialiser = this.AttachGenericFunction<Func_Initialiser>("Initialiser");
          this.InitialiserWpfImport = this.AttachGenericFunction<Func_InitialiserWpfImport>("InitialiserWpfImport");
          this.IsGlobalProcedure = this.AttachGenericFunction<Func_IsGlobalProcedure>("IsGlobalProcedure");
          this.ResetEventAffiche = this.AttachGenericFunction<Func_ResetEventAffiche>("ResetEventAffiche");
          this.Sauvegarder = this.AttachGenericFunction<Func_Sauvegarder>("Sauvegarder");
          this.EssaiModifie = this.AttachGenericFunction<Func_EssaiModifie>("EssaiModifie");
          this.SelectCourbesExt = this.AttachGenericFunction<Func_SelectCourbesExt>("SelectCourbesExt");
          this.SelectDLLFonction = this.AttachGenericFunction<Func_SelectDLLFonction>("SelectDLLFonction");
          this.SelectFonction = this.AttachGenericFunction<Func_SelectFonction>("SelectFonction");
          this.SelectFonctionWpfImport = this.AttachGenericFunction<Func_SelectFonctionWpfImport>("SelectFonctionWpfImport");
          this.SelectPositionCourbeExt = this.AttachGenericFunction<Func_SelectPositionCourbeExt>("SelectPositionCourbeExt");
          this.SelectSB = this.AttachGenericFunction<Func_SelectSB>("SelectSB");
          this.SelectValeursCourbeExt = this.AttachGenericFunction<Func_SelectValeursCourbeExt>("SelectValeursCourbeExt");
          this.SetColumnInfoFonction = this.AttachGenericFunction<Func_SetColumnInfoFonction>("SetColumnInfoFonction");
          this.SetCommentaireResultat = this.AttachGenericFunction<Func_SetCommentaireResultat>("SetCommentaireResultat");
          this.SetEventAffiche = this.AttachGenericFunction<Func_SetEventAffiche>("SetEventAffiche");
          this.SetExecData = this.AttachGenericFunction<Func_SetExecData>("SetExecData");
          this.SetFrequenceMesure = this.AttachGenericFunction<Func_SetFrequenceMesure>("SetFrequenceMesure");
          this.SetResultat = this.AttachGenericFunction<Func_SetResultat>("SetResultat");
          this.Terminer = this.AttachGenericFunction<Func_Terminer>("Terminer");
          this.UnSelectFonction = this.AttachGenericFunction<Func_UnSelectFonction>("UnSelectFonction");
          this.UnSelectSB = this.AttachGenericFunction<Func_UnSelectSB>("UnSelectSB");
          this.ImporterMaxCourbesExtWpf = this.AttachGenericFunction<Func_ImporterMaxCourbesExtWpf>("ImporterMaxCourbesExtWpf");
          this.GetSignatureFonctionForAnyEssai = this.AttachGenericFunction<Func_GetSignatureFonctionForAnyEssai>("GetSignatureFonctionForAnyEssai");
          this.IsChangeDomaineSB = this.AttachGenericFunction<Func_IsChangeDomaineSB>("IsChangeDomaineSB");
          this.GetEtatSB = this.AttachGenericFunction<Func_GetEtatSB>("GetEtatSB");
          this.GetNomDllMesureur = this.AttachGenericFunction<Func_GetNomDllMesureur>("GetNomDllMesureur");
          this.GetParametresReglage = this.AttachGenericFunction<Func_GetParametresReglage>("GetParametresReglageTab");
          this.SupprimerCourbeExt = this.AttachGenericFunction<Func_SupprimerCourbeExt>("SupprimerCourbeExt");
          this.SetResultatEtat = this.AttachGenericFunction<Func_SetResultatEtat>("SetResultatEtat");
          this.GetResultatDouble = this.AttachGenericFunction<Func_GetResultatDouble>("GetResultatDouble");
          this.DeleteEssaiImport = this.AttachGenericFunction<Func_DeleteEssaiImport>("DeleteEssaiImport");
          this.LoadLogConfig = this.AttachGenericFunction<Func_LoadLogConfig>("LoadLogConfig");
          this.GetPathResult = this.AttachGenericFunction<Func_GetPathResult>("GetPathResult");
          this.GetIDEssai = this.AttachGenericFunction<Func_GetIDEssai>("GetIDEssai");
          this.GetConfEM = this.AttachGenericFunction<Func_GetConfEM>("GetConfEM");
          this.SetLimiteVisible = this.AttachGenericFunction<Func_SetLimiteVisible>("SetLimiteVisible");
          this.RemoveLimiteVisibleFromDB = this.AttachGenericFunction<Func_RemoveLimiteVisibleFromDB>("RemoveLimiteVisibleFromDB");
          this.IsLimitVisible = this.AttachGenericFunction<Func_IsLimitVisible>("IsLimitVisible");
          this.RecupererCodeLangue = this.AttachGenericFunction<Func_RecupererCodeLangue>("RecupererCodeLangue");

          this.IsMDSPrescan = this.AttachGenericFunction<Func_IsMDSPrescan>("IsMDSPrescan");
          this.IsMDSFinal = this.AttachGenericFunction<Func_IsMDSFinal>("IsMDSFinal");
          this.MDSGetAngles1 = this.AttachGenericFunction<Func_MDSGetAngles1>("MDS_GetAngles1");
          this.MDSGetAngles2 = this.AttachGenericFunction<Func_MDSGetAngles2>("MDS_GetAngles2");
          this.MDSGetHeights = this.AttachGenericFunction<Func_MDSGetHeights>("MDS_GetHeights");
          this.MDSGetPositions = this.AttachGenericFunction<Func_MDSGetPositions>("MDS_GetPositions");
          this.MDSGetDetectors = this.AttachGenericFunction<Func_MDSGetDetectors>("MDS_GetDetectors");

          this.ApplyNewCorrectionOnSB = this.AttachGenericFunction<Func_ApplyNewCorrectionOnSB>("ApplyNewCorrectionOnSB");
          this.CanReadPipe = this.AttachGenericFunction<Func_CanReadPipe>("CanReadPipe");
          this.SetEtatSB = this.AttachGenericFunction<Func_SetEtatSB>("SetEtatSB");

          this.GetNbLimiteImportee = this.AttachGenericFunction<Func_GetNbLimiteImportee>("GetNbLimiteImportee");
          this.AjouterLimiteImportee = this.AttachGenericFunction<Func_AjouterLimiteImportee>("AjouterLimiteImportee");
          this.RetirerLimiteImportee = this.AttachGenericFunction<Func_RetirerLimiteImportee>("RetirerLimiteImportee");
          this.GetLimitesImporteeInfos = this.AttachGenericFunction<Func_GetLimitesImporteeInfos>("GetLimiteImporteeInfos");
          this.GetLimiteClasseId = this.AttachGenericFunction<Func_GetLimiteClasseId>("GetLimiteClasseId");

          this.SetWindowForPilote = this.AttachGenericFunction<Func_SetWindowForPilote>("SetWindowForPilote");

          this.GetGuidEssai = this.AttachGenericFunction<Func_GetGuidEssai>("GetGuidEssai");

          this.HasFunctionAngle2Prescan = this.AttachGenericFunction<Func_HasFunctionAngle2Prescan>("HasFunctionAngle2Prescan");
          this.HasFunctionAnglePrescan = this.AttachGenericFunction<Func_HasFunctionAnglePrescan>("HasFunctionAnglePrescan");
          this.HasFunctionHauteurPrescan = this.AttachGenericFunction<Func_HasFunctionHauteurPrescan>("HasFunctionHauteurPrescan");

          this.DemanderMontage = this.AttachGenericFunction<Func_DemanderMontage>("DemanderMontage");
          this.GetTexteInfoReglage = this.AttachGenericFunction<Func_GetTexteInfoReglage>("GetTexteInfoReglage");
          this.GetPositionName = this.AttachGenericFunction<Func_GetPositionName>("GetPositionName");
          this.GetInfosCommentaireEssai = this.AttachGenericFunction<Func_GetInfosCommentaireEssai>("GetInfosCommentaireEssai");

          this.CanReadLog = this.AttachGenericFunction<Func_CanReadLog>("CanReadLog");
          this.ReadLogData = this.AttachGenericFunction<Func_ReadLogData>("ReadLogData");
          this.SetIsV4 = this.AttachGenericFunction<Func_SetIsV4>("SetIsV4");
        }
        else
        {
          throw new Exception(Nexio.Bat.Emi.VisuV4.Domain.Properties.Resources.CanNotLoad + dllPath);
        }

        this.IsInitialized = true;
        return true;
      }

      internal void DechargerFonctionDLL()
      {
        this.FastGetIdResultatSuivant = null;
        this.FastSelectResultatsSBAction = null;
        this.FastSelectResultatsSBFonction = null;
        this.FinFonctionSB = null;
        this.GetActionFonction = null;
        this.GetCommentSB = null;
        this.GetCourbeExtSuivante = null;
        this.GetDLLFonctionSuivante = null;
        this.GetEtatEssai = null;
        this.GetExecData = null;
        this.GetFonctionSuivante = null;
        this.GetFonctionSuivanteWpfImport = null;
        this.GetFreqMaxSB = null;
        this.GetFreqMinSB = null;
        this.GetFrequence = null;
        this.GetCommentaireResultat = null;
        this.GetFrequenceMesure = null;
        this.GetGlobalProcedure = null;
        this.GetGuidSB = null;
        this.GetGuidMontageSB = null;
        this.GetIdFonction = null;
        this.GetIdFonctionName = null;
        this.GetIdFonctionSuivante = null;
        this.GetIdFreqSousbande = null;
        this.GetIdSousBande = null;
        this.GetIdTypeEssai = null;
        this.GetInfoColumnFonction = null;
        this.GetInfoCourbeExt = null;
        this.GetInfosEssai = null;
        this.GetInfosResultatsFonctionTab = null;
        this.GetInfosSB = null;
        this.GetInfoVisuLimite = null;
        this.GetInfoVisuLimiteDesc = null;
        this.GetLabelEquipementName = null;
        this.GetLabelEquipementValue = null;
        this.GetLabelName = null;
        this.GetLabelVehiculeName = null;
        this.GetLabelVehiculeValue = null;
        this.GetLimiteNiveau = null;
        this.GetListePositionsTab = null;
        this.GetNbLabelEquipementName = null;
        this.GetNbLabelEquipementValue = null;
        this.GetNbLabelName = null;
        this.GetNbLabelValue = null;
        this.GetNbLabelVehiculeName = null;
        this.GetNbLabelVehiculeValue = null;
        this.GetNbResultatsFonction = null;
        this.GetNbVisuLimite = null;
        this.GetNextPositionCourbeExt = null;
        this.GetNextValeurCourbeExt = null;
        this.GetNombreSB = null;
        this.GetNomSB = null;
        this.GetPositionSB = null;
        this.GetPositionInitialeSB = null;
        this.GetResultat = null;
        this.GetResultatString = null;
        this.GetSBSuivante = null;
        this.GetScriptDebutSB = null;
        this.GetScriptEndPrescan = null;
        this.GetScriptEndTest = null;
        this.GetScriptFinSB = null;
        this.GetSignatureFonction = null;
        this.GetSignatureFonctionForAnyEssai = null;
        this.GetTraitementSuivant = null;
        this.GetUniteEssaiUnicode = null;
        this.GetVisuLimiteNiveaux = null;
        this.ExporterResultatsFonctionWpf = null;
        this.ImporterCourbeExtFromFileWpf = null;
        this.ImporterCourbeExtWpf = null;
        this.InitFromFile = null;
        this.Initialiser = null;
        this.InitialiserWpfImport = null;
        this.IsGlobalProcedure = null;
        this.ResetEventAffiche = null;
        this.SelectCourbesExt = null;
        this.SelectDLLFonction = null;
        this.SelectFonction = null;
        this.SelectFonctionWpfImport = null;
        this.SelectPositionCourbeExt = null;
        this.SelectSB = null;
        this.SelectValeursCourbeExt = null;
        this.SetColumnInfoFonction = null;
        this.SetCommentaireResultat = null;
        this.SetEventAffiche = null;
        this.SetExecData = null;
        this.SetFrequenceMesure = null;
        this.SetResultat = null;
        this.Terminer = null;
        this.UnSelectFonction = null;
        this.UnSelectSB = null;

        this.SelectCourbesExt = null;
        this.GetCourbeExtSuivante = null;
        this.GetInfoCourbeExt = null;
        this.SelectValeursCourbeExt = null;
        this.ImporterCourbeExtFromFileWpf = null;
        this.ImporterCourbeExtWpf = null;
        this.ImporterMaxCourbesExtWpf = null;
        this.IsChangeDomaineSB = null;
        this.GetNomDllMesureur = null;
        this.GetParametresReglage = null;
        this.SupprimerCourbeExt = null;
        this.SetResultat = null;
        this.DeleteEssaiImport = null;
        this.GetIdPointFrequence = null;
        this.GetPathResult = null;
        this.GetIDEssai = null;
        this.GetLimiteClasseId = null;
        this.GetLimitesImporteeInfos = null;

        this.GetInfoColumnFonction = null;
        this.GetCompleteInfoColumnFonction = null;
        this.GetConfEM = null;
        this.SetLimiteVisible = null;
        this.RemoveLimiteVisibleFromDB = null;
        this.IsLimitVisible = null;
        this.ApplyNewCorrectionOnSB = null;
        this.CanReadPipe = null;

        this.IsMDSPrescan = null;
        this.IsMDSFinal = null;
        this.MDSGetAngles1 = null;
        this.MDSGetAngles2 = null;
        this.MDSGetHeights = null;
        this.MDSGetPositions = null;
        this.MDSGetDetectors = null;

        this.SetWindowForPilote = null;
        this.GetPositionName = null;
        this.GetInfosCommentaireEssai = null;

        this.CanReadLog = null;
        this.ReadLogData = null;

        if (!Win32.FreeLibrary(this.handleBatDataLibrary))
        {
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        this.IsInitialized = false;
      }
    }
  }
}