namespace NexioMax3.Domain.Wrapper
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.InteropServices;
  using System.Text;
  using Nexio.Helper;

  public class MesureManuelleMaxHold : FunctionDLL
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MesureManuelleMaxHold));

    private readonly Native native;

    public MesureManuelleMaxHold()
    {
      var dlgs = new List<Service.IFunctionDialog>();
      this.InitialiserFonctionDLL(Path.Combine(ExecPathHelper.GetExecDirectory(), Native.DllName), IntPtr.Zero, dlgs);
      this.native = new Native();
      this.native.InitialiserFonctionDLL();
    }

    public void ResetMaxHoldValues()
    {
      this.native.ResetMaxHoldValues();
    }

    public bool GetIdPointFrequence(double e_Frequence, out int s_ident, out double s_freq, out bool s_bSuperieur)
    {
      return this.native.GetIdPointFrequence(e_Frequence, out s_ident, out s_freq, out s_bSuperieur);
    }

    public void SetScriptPosition(int pos)
    {
      this.native.SetScriptPosition(pos);
    }

    public void ReglerMatPlateau(int action, double value, bool showProgress)
    {
      this.native.ReglerMatPlateau(action, value, showProgress);
    }

    public void SetModeMesurePoint(int mode)
    {
      this.native.SetModeMesurePoint(mode);
    }

    public bool StartMesManMaxHold(int m_idPoint, double m_dSpan, int[] tDetecteurs, double m_dTemps, bool bPeakSearch, ref double m_dFreq, int iScriptPosition)
    {
      return this.native.StartMesManMaxHold(m_idPoint, m_dSpan, tDetecteurs, tDetecteurs.Length, m_dTemps, bPeakSearch, ref m_dFreq, iScriptPosition);
    }

    public void UpdateMesureur()
    {
      this.native.UpdateMesureur();
    }

    public void InitListeSuspects()
    {
      this.native.InitListeSuspects();
    }

    public int GetScriptPosition()
    {
      return this.native.GetScriptPosition();
    }

    public void AjouterIdFonctionModifiee(int idFonction)
    {
      this.native.AjouterIdFonctionModifiee(idFonction);
    }

    public void AfficherResultatsWpf(int idFirstPt, int nNombre)
    {
      this.native.AfficherResultatsWpf(idFirstPt, nNombre);
    }

    public double ReadHauteurCourante()
    {
      return this.native.ReadHauteurCourante();
    }

    public double ReadAngleCourant()
    {
      return this.native.ReadAngleCourant();
    }

    public double ReadAngle2Courant()
    {
      return this.native.ReadAngle2Courant();
    }

    public void ReadMaxHoldValuesMultiDetector(int[] tDetecteur, double dFrequence, double dSpan, double dTemps, ref double dFreqPic, out string ptMesures, out string ptMesuresMax)
    {
      StringBuilder meas = new StringBuilder(1024);
      StringBuilder measMax = new StringBuilder(1024);
      this.native.ReadMaxHoldValuesMultiDetector(tDetecteur, tDetecteur.Length, dFrequence, dSpan, dTemps, ref dFreqPic, meas, measMax);
      ptMesures = meas.ToString();
      ptMesuresMax = measMax.ToString();
    }

    public void SetModeExecution(int mode)
    {
      this.native.SetModeExecution(mode);
    }

    public void InvalideReglage()
    {
      this.native.InvalideReglage();
    }

    public void InitDomaine(int idPoint)
    {
      this.native.InitDomaine(idPoint);
    }

    public void InitIdFonctions(int idFonction)
    {
      this.native.InitIdFonctions(idFonction);
    }

    public bool InitExecution()
    {
      return this.native.InitExecution();
    }

    public void InitMembresReglage()
    {
      this.native.InitMembresReglage();
    }

    public void Arreter()
    {
      this.native.Arreter();
    }

    private class Native
    {
      public static readonly string DllName = "MesureManuelleMaxHold.dll";

      private IntPtr handleBatDataLibrary;

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_ResetMaxHoldValues();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_GetIdPointFrequence(double e_Frequence, out int s_ident, out double s_freq, out bool s_bSuperieur);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetScriptPosition(int pos);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate void Func_ReglerMatPlateau(int action, double value, bool showProgress);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetModeMesurePoint(int mode);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_StartMesManMaxHold(int m_idPoint, double m_dSpan, int[] tDetecteurs, int nbrDetecteurs, double m_dTemps, bool bPeakSearch, ref double m_dFreq, int iScriptPosition);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_ExecuteListePoints(int e_nbPoints, int[] e_listePoints, int e_idFonction, int e_iModeExecution);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_UpdateMesureur();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitListeSuspects();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate int Func_GetScriptPosition();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_AjouterIdFonctionModifiee(int idFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_AfficherResultatsWpf(int idFirstPt, int nNombre);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_ReadHauteurCourante();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_ReadAngleCourant();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate double Func_ReadAngle2Courant();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_ReadMaxHoldValuesMultiDetector(int[] tDetecteur, int nbrDetecteurs, double dFrequence, double dSpan, double dTemps, ref double dFreqPic, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder ptMesures, [MarshalAs(UnmanagedType.LPStr), Out] StringBuilder ptMesuresMax);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitialiserDataDLL();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitialiserPiloteDLL();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      [return: MarshalAs(UnmanagedType.I1)]
      public delegate bool Func_InitExecution();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetModeExecution(int mode);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitIdFonctions(int idFonction);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InvalideReglage();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitMembresReglage();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_SetMainWnd(IntPtr hMainWnd);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_Terminer();

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_InitDomaine(int idPoint);

      [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
      public delegate void Func_Arreter();

      public Func_SetMainWnd SetMainWnd { get; private set; } = null;

      public Func_ResetMaxHoldValues ResetMaxHoldValues { get; private set; }

      public Func_InitialiserDataDLL InitialiserDataDLL { get; private set; }

      public Func_InitialiserPiloteDLL InitialiserPiloteDLL { get; private set; }

      public Func_GetIdPointFrequence GetIdPointFrequence { get; private set; }

      public Func_SetScriptPosition SetScriptPosition { get; private set; }

      public Func_ReglerMatPlateau ReglerMatPlateau { get; private set; }

      public Func_SetModeMesurePoint SetModeMesurePoint { get; private set; }

      public Func_StartMesManMaxHold StartMesManMaxHold { get; private set; }

      public Func_Terminer Terminer { get; private set; }

      public Func_ExecuteListePoints ExecuteListePoints { get; private set; }

      public Func_UpdateMesureur UpdateMesureur { get; private set; }

      public Func_InitListeSuspects InitListeSuspects { get; private set; }

      public Func_GetScriptPosition GetScriptPosition { get; private set; }

      public Func_AjouterIdFonctionModifiee AjouterIdFonctionModifiee { get; private set; }

      public Func_AfficherResultatsWpf AfficherResultatsWpf { get; private set; }

      public Func_ReadHauteurCourante ReadHauteurCourante { get; private set; }

      public Func_ReadAngleCourant ReadAngleCourant { get; private set; }

      public Func_ReadAngle2Courant ReadAngle2Courant { get; private set; }

      public Func_ReadMaxHoldValuesMultiDetector ReadMaxHoldValuesMultiDetector { get; private set; }

      public Func_InitExecution InitExecution { get; private set; }

      public Func_SetModeExecution SetModeExecution { get; private set; }

      public Func_InitIdFonctions InitIdFonctions { get; private set; }

      public Func_InvalideReglage InvalideReglage { get; private set; }

      public Func_InitMembresReglage InitMembresReglage { get; private set; }

      public Func_InitDomaine InitDomaine { get; set; }

      public Func_Arreter Arreter { get; private set; }

      public TDelegate AttachGenericFunction<TDelegate>(string cppFunctionName, bool optionnalFunction = false)
      {
        IntPtr ptrFunction = Win32.GetProcAddress(this.handleBatDataLibrary, cppFunctionName);
        if (ptrFunction == IntPtr.Zero)
        {
          if (optionnalFunction == false)
          {
            throw new Exception(string.Format(NexioMax3.Domain.Properties.Resources.CanNotLoad0Function1, "MesureManuelleMaxHold.dll", cppFunctionName));
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
        var dllPath = System.IO.Path.Combine(pathEmi, DllName);
        if (!System.IO.File.Exists(dllPath))
        {
          var ex = new Exception(string.Format("bat_data.dll not found ({0})", dllPath));
          Log.Error(ex);
          throw ex;
        }

        this.handleBatDataLibrary = Win32.LoadLibrary(dllPath);

        if (this.handleBatDataLibrary != IntPtr.Zero)
        {
          this.ResetMaxHoldValues = this.AttachGenericFunction<Func_ResetMaxHoldValues>("ResetMaxHoldValues");

          this.GetIdPointFrequence = this.AttachGenericFunction<Func_GetIdPointFrequence>("GetIdPointFrequence");

          this.SetScriptPosition = this.AttachGenericFunction<Func_SetScriptPosition>("SetScriptPosition");

          this.ReglerMatPlateau = this.AttachGenericFunction<Func_ReglerMatPlateau>("ReglerMatPlateau");

          this.SetModeMesurePoint = this.AttachGenericFunction<Func_SetModeMesurePoint>("SetModeMesurePoint");

          this.StartMesManMaxHold = this.AttachGenericFunction<Func_StartMesManMaxHold>("StartMesManMaxHold");

          this.ExecuteListePoints = this.AttachGenericFunction<Func_ExecuteListePoints>("ExecuteListePoints");

          this.UpdateMesureur = this.AttachGenericFunction<Func_UpdateMesureur>("UpdateMesureur");

          this.InitListeSuspects = this.AttachGenericFunction<Func_InitListeSuspects>("InitListeSuspects");

          this.GetScriptPosition = this.AttachGenericFunction<Func_GetScriptPosition>("GetScriptPosition");

          this.AjouterIdFonctionModifiee = this.AttachGenericFunction<Func_AjouterIdFonctionModifiee>("AjouterIdFonctionModifiee");

          this.AfficherResultatsWpf = this.AttachGenericFunction<Func_AfficherResultatsWpf>("AfficherResultatsWpf");

          this.ReadHauteurCourante = this.AttachGenericFunction<Func_ReadHauteurCourante>("ReadHauteurCourante");

          this.ReadAngleCourant = this.AttachGenericFunction<Func_ReadAngleCourant>("ReadAngleCourant");

          this.ReadAngle2Courant = this.AttachGenericFunction<Func_ReadAngle2Courant>("ReadAngle2Courant");

          this.ReadMaxHoldValuesMultiDetector = this.AttachGenericFunction<Func_ReadMaxHoldValuesMultiDetector>("ReadMaxHoldValuesMultiDetector");

          this.InitExecution = this.AttachGenericFunction<Func_InitExecution>("InitExecution");

          this.SetModeExecution = this.AttachGenericFunction<Func_SetModeExecution>("SetModeExecution");

          this.InitIdFonctions = this.AttachGenericFunction<Func_InitIdFonctions>("InitIdFonctions");

          this.InvalideReglage = this.AttachGenericFunction<Func_InvalideReglage>("InvalideReglage");

          this.InitMembresReglage = this.AttachGenericFunction<Func_InitMembresReglage>("InitMembresReglage");

          this.SetMainWnd = this.AttachGenericFunction<Func_SetMainWnd>("SetMainWnd");

          this.InitialiserDataDLL = this.AttachGenericFunction<Func_InitialiserDataDLL>("InitialiserDataDLL");
          this.InitialiserDataDLL?.Invoke();

          this.InitialiserPiloteDLL = this.AttachGenericFunction<Func_InitialiserPiloteDLL>("InitialiserPiloteDLL");

          this.Terminer = this.AttachGenericFunction<Func_Terminer>("Terminer");
          this.InitDomaine = this.AttachGenericFunction<Func_InitDomaine>("InitDomaine");

          this.Arreter = this.AttachGenericFunction<Func_Arreter>("Arreter");

          this.InitialiserPiloteDLL?.Invoke();
        }
        else
        {
          throw new Exception(NexioMax3.Domain.Properties.Resources.CanNotLoad + dllPath);
        }

        return true;
      }

      internal void DechargerFonctionDLL()
      {
        this.ResetMaxHoldValues = null;

        this.GetIdPointFrequence = null;

        this.SetScriptPosition = null;

        this.ReglerMatPlateau = null;

        this.SetModeMesurePoint = null;

        this.StartMesManMaxHold = null;

        this.ExecuteListePoints = null;

        this.UpdateMesureur = null;

        this.InitListeSuspects = null;

        this.GetScriptPosition = null;

        this.AjouterIdFonctionModifiee = null;

        this.AfficherResultatsWpf = null;

        this.ReadHauteurCourante = null;

        this.ReadAngleCourant = null;

        this.ReadAngle2Courant = null;

        this.ReadMaxHoldValuesMultiDetector = null;

        this.InitExecution = null;

        this.SetModeExecution = null;

        this.InitIdFonctions = null;

        this.InvalideReglage = null;

        this.InitMembresReglage = null;

        this.SetMainWnd = null;

        this.InitialiserDataDLL = null;

        this.InitialiserPiloteDLL = null;

        this.Arreter = null;

        if (!Win32.FreeLibrary(this.handleBatDataLibrary))
        {
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
      }
    }
  }
}