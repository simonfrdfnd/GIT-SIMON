namespace NexioMax3.Domain.Wrapper
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Text;
  using log4net;
  using Nexio.Tools;

  public class FunctionDLL
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(FunctionDLL));
    private static Dictionary<string, FunctionDLL> dlls = new Dictionary<string, FunctionDLL>();

    private IntPtr handleBatDataLibrary;

    private Func_HasProgressDlg hasProgressDlg = null;

    private Func_InitMembresReglageTab initMembresReglageTab;

    private Func_SetTypeProgression setTypeProgression;

    private Func_SetPas setPas;

    private Func_RegisterDialog registerDialog;

    protected FunctionDLL()
    {
    }

    /// <summary>
    /// Fonction Initialiser
    /// </summary>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_ExecuteListePoints(int e_nbPoints, int[] e_listePoints, int e_idFonction, int e_iModeExecution);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_ExecuteIntervalleFreq(double e_fFreqMin, double e_fFreqMax, int e_idFonction, int e_IdSB, int e_iModeExecution, bool initReglages);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_ExecuteIntervallePtFreq(int idFreqMin, int idFreqMax, int e_idFonction, int e_IdSB, int e_iModeExecution, bool initReglages);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_SetMainWnd(IntPtr hMainWnd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_GlobalExecuteTab(string pListeIdSBReq, double dStartFreq, double dStopFreq, bool bResume);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool Func_HasProgressDlg();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate double Func_GetFrequenceCourante();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_SetTypeProgression(IntPtr pvoid, int typeProgression);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_SetPas(IntPtr pvoid, double pas);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_GetBalayage();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_GetNbBalayages();

    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLGETAVANCEMENT(char*);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate int LPFNDLLGETIDFONCTIONS(const int**);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLTERMINER();
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLREPRENDRE();
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLSUSPENDRE();
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLGETETATSERVICE(int iService, int* iEtat);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate bool LPFNDLLINITEXECUTION();
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate bool LPFNDLLSETIDMESUREUR(int idMesureur);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate bool LPFNDLLSETIDMESUREURPRECEDENT(int idMesureur);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate bool LPFNDLLSETFREQMAXDOMAINE(double dFreqMaxDomaine);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate bool LPFNDLLSETMODEISOTROPIQUE(bool bModeIsotropique);
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate int LPFNDLLGETBALAYAGE();
    ////[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    ////public delegate void LPFNDLLSETRECALCMODE(bool bRecalc, double dOffset);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_FreeResources(IntPtr pvoid);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_Terminer();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_Suspendre();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_Reprendre();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_InitMembresReglageTab(IntPtr pvoid, StringBuilder pslValParam);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_RegisterDialog(IntPtr pvoid, String name, IntPtr dlg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_GetNbParam();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr Func_GetParam(int i);

    /// <summary>
    /// Initialise l'essai
    /// </summary>
    public Func_ExecuteListePoints ExecuteListePoints { get; private set; } = null;

    public Func_ExecuteIntervalleFreq ExecuteIntervalleFreq { get; private set; } = null;

    public Func_ExecuteIntervallePtFreq ExecuteIntervallePtFreq { get; private set; } = null;

    public Func_SetMainWnd SetMainWnd { get; private set; } = null;

    public Func_FreeResources FreeResources { get; private set; } = null;

    public Func_GetFrequenceCourante GetFrequenceCourante { get; private set; } = null;

    public Func_Terminer Terminer { get; private set; } = null;

    public Func_Suspendre Suspendre { get; private set; } = null;

    public Func_Reprendre Reprendre { get; private set; } = null;

    public Func_GetBalayage GetBalayage { get; private set; } = null;

    public Func_GetNbBalayages GetNbBalayages { get; private set; } = null;

    public Func_HasProgressDlg HasProgressDlg
    {
      get { return this.hasProgressDlg != null ? this.hasProgressDlg : () => false; }
      set { this.hasProgressDlg = value; }
    }

    public Func_GetNbParam GetNbParam { get; private set; } = null;

    public Func_GetParam GetParam { get; private set; } = null;

    public string DllPath { get; private set; }

    public bool IsFree { get; private set; }

    private Func_GlobalExecuteTab GlobalExecuteDll { get; set; } = null;

    public static FunctionDLL Get(string dllPath, IntPtr parentIntPtr, List<Service.IFunctionDialog> dialogs, out bool initSuccess)
    {
      initSuccess = true;
      if (!dlls.ContainsKey(dllPath))
      {
        var fct = new FunctionDLL();

        if (fct.InitialiserFonctionDLL(dllPath, parentIntPtr, dialogs))
        {
          dlls.Add(dllPath, fct);
        }
        else
        {
          initSuccess = false;
          return null;
        }
      }

      return dlls[dllPath];
    }

    public static void FreeDlls()
    {
      while (dlls.Count > 0)
      {
        var key = dlls.Keys.First();
        dlls[key].FreeLibrary();
        dlls.Remove(key);
      }
    }

    public bool IsGlobalExecute()
    {
      return this.GlobalExecuteDll != null;
    }

    public int GlobalExecute(int[] pListeIdSBReq, double dStartFreq, double dStopFreq, bool bResume)
    {
      try
      {
        using (new NexioStopwatch($"{nameof(FunctionDLL)}.{nameof(this.GlobalExecute)} ({Path.GetFileName(this.DllPath)})"))
        {
          if (this.GlobalExecuteDll != null)
          {
            // TODO : A quoi sert ce code ? on utilise pas la liste d'entier ni le pointeur derrière
            IntPtr intPtr = Marshal.AllocHGlobal(sizeof(int) * pListeIdSBReq.Length);
            Marshal.Copy(pListeIdSBReq, 0, intPtr, pListeIdSBReq.Length);

            string tab = string.Join("\t", pListeIdSBReq);

            return this.GlobalExecuteDll(tab, dStartFreq, dStopFreq, bResume);
          }
          else
          {
            return -1;
          }
        }
      }
      catch (Exception ex)
      {
        Log.Fatal(ex);

        throw;
      }
    }

    public List<string> GetParams()
    {
      var nbParam = this.GetNbParam();
      var result = new List<string>();
      for (int i = 0; i < nbParam; i++)
      {
        var ptr = this.GetParam(i);
        var str = Marshal.PtrToStringAnsi(ptr);
        result.Add(str);
      }

      return result;
    }

    public void SetTypeProgression(Model.TypesProgression progression)
    {
      this.setTypeProgression(IntPtr.Zero, (int)progression);
    }

    public void SetStep(double pas)
    {
      this.setPas(IntPtr.Zero, pas);
    }

    public void InitMembresReglageTab(List<string> settings)
    {
      var sb = new StringBuilder(256);
      foreach (var item in settings)
      {
        if (sb.Length > 0)
        {
          sb.Append("\t");
        }

        sb.Append(item);
      }

      this.initMembresReglageTab(IntPtr.Zero, sb);
    }

    public TDelegate AttachGenericFunction<TDelegate>(string cppFunctionName, bool optionnalFunction = false)
    {
      IntPtr ptrFunction = Win32.GetProcAddress(this.handleBatDataLibrary, cppFunctionName);
      if (ptrFunction == IntPtr.Zero)
      {
        if (optionnalFunction == false)
        {
          throw new Exception(string.Format(NexioMax3.Domain.Properties.Resources.CanNotLoad0Function1, this.DllPath, cppFunctionName));
        }
        else
        {
          return default(TDelegate);
        }
      }

      return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptrFunction);
    }

    protected bool InitialiserFonctionDLL(string dllPath, IntPtr parent, List<Service.IFunctionDialog> dialogs)
    {
      if (!this.IsFree)
      {
        if (dllPath != this.DllPath)
        {
          this.FreeLibrary();
        }
        else
        {
          return true;
        }
      }

      using (new NexioStopwatch($"{nameof(FunctionDLL)}.{nameof(this.InitialiserFonctionDLL)}:{dllPath}"))
      {
        string path = Path.GetDirectoryName(dllPath);
        Win32.SetDllDir(path);

        this.handleBatDataLibrary = Win32.LoadLibrary(dllPath);
        this.DllPath = dllPath;
        if (this.handleBatDataLibrary != IntPtr.Zero)
        {
          this.ExecuteListePoints = this.AttachGenericFunction<Func_ExecuteListePoints>("ExecuteListePoints");
          this.ExecuteIntervalleFreq = this.AttachGenericFunction<Func_ExecuteIntervalleFreq>("ExecuteIntervalleFreq");
          this.ExecuteIntervallePtFreq = this.AttachGenericFunction<Func_ExecuteIntervallePtFreq>("ExecuteIntervallePtFreq");

          this.GlobalExecuteDll = this.AttachGenericFunction<Func_GlobalExecuteTab>("GlobalExecuteTab", true);
          this.HasProgressDlg = this.AttachGenericFunction<Func_HasProgressDlg>("HasProgressDlg", true);

          this.Terminer = this.AttachGenericFunction<Func_Terminer>("Terminer");
          this.Suspendre = this.AttachGenericFunction<Func_Suspendre>("Suspendre");
          this.Reprendre = this.AttachGenericFunction<Func_Reprendre>("Reprendre");

          this.GetFrequenceCourante = this.AttachGenericFunction<Func_GetFrequenceCourante>("GetFrequenceCourante", true);

          this.GetBalayage = this.AttachGenericFunction<Func_GetBalayage>("GetBalayage");
          this.GetNbBalayages = this.AttachGenericFunction<Func_GetNbBalayages>("GetNbBalayages", true);

          this.SetMainWnd = this.AttachGenericFunction<Func_SetMainWnd>("SetMainWnd");
          this.FreeResources = this.AttachGenericFunction<Func_FreeResources>("FreeResources");
          this.initMembresReglageTab = this.AttachGenericFunction<Func_InitMembresReglageTab>("InitMembresReglageTab");
          this.setTypeProgression = this.AttachGenericFunction<Func_SetTypeProgression>("SetTypeProgression");
          this.setPas = this.AttachGenericFunction<Func_SetPas>("SetPas");
          this.registerDialog = this.AttachGenericFunction<Func_RegisterDialog>("RegisterDialog");
          this.GetNbParam = this.AttachGenericFunction<Func_GetNbParam>("GetNbParam");
          this.GetParam = this.AttachGenericFunction<Func_GetParam>("GetParam");
          if (this.SetMainWnd != null)
          {
            this.SetMainWnd(parent);
          }

          foreach (var dlg in dialogs)
          {
            this.registerDialog(IntPtr.Zero, dlg.Name, dlg.Native);
          }
        }
        else
        {
          Log.Fatal($"Load of {dllPath} failed");
          throw new Exception(NexioMax3.Domain.Properties.Resources.CanNotLoad + dllPath);
        }

        this.IsFree = false;
        return true;
      }
    }

    private void FreeLibrary()
    {
      if (this.FreeResources != null)
      {
        this.FreeResources(IntPtr.Zero);
      }

      this.ExecuteListePoints = null;
      this.ExecuteIntervalleFreq = null;
      this.ExecuteIntervallePtFreq = null;
      this.SetMainWnd = null;
      this.FreeResources = null;
      this.GetFrequenceCourante = null;
      this.Terminer = null;
      this.Suspendre = null;
      this.Reprendre = null;
      this.HasProgressDlg = null;
      this.GlobalExecuteDll = null;

      Win32.FreeLibrary(this.handleBatDataLibrary);
      this.IsFree = true;
    }
  }
}