namespace NexioMax3.Domain.Wrapper
{
  using System;
  using System.Runtime.InteropServices;
  using Nexio.Helper;

  public class BatScriptWrapper
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(BatScriptWrapper));

    private IntPtr handleBatScriptLibrary;

    public BatScriptWrapper(IntPtr parent)
    {
      this.Initialiser(parent);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Func_ExecuterScriptWpf(string filename);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_SetMainWnd(IntPtr hMainWnd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_SetParallelMode(Int32 bParallel);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Func_StopExecution();

    public Func_ExecuterScriptWpf ExecuterScript { get; private set; }

    public Func_SetMainWnd SetMainWnd { get; private set; }

    public Func_SetParallelMode SetParallelMode { get; private set; }

    public Func_StopExecution StopExecution { get; private set; }

    public string DllPath { get; private set; }

    public bool IsFree { get; private set; }

    public TDelegate AttachGenericFunction<TDelegate>(string cppFunctionName, bool optionnalFunction = false)
    {
      IntPtr ptrFunction = Win32.GetProcAddress(this.handleBatScriptLibrary, cppFunctionName);
      if (ptrFunction == IntPtr.Zero)
      {
        if (optionnalFunction == false)
        {
          throw new Exception(string.Format(NexioMax3.Domain.Properties.Resources.CanNotLoad0Function1, "Script.dll", cppFunctionName));
        }
        else
        {
          return default(TDelegate);
        }
      }

      return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptrFunction);
    }

    public Boolean Initialiser(IntPtr parent)
    {
      string pathEmi = ExecPathHelper.GetExecDirectory();
      string dllPath = System.IO.Path.Combine(pathEmi, "Script.dll");
      if (!System.IO.File.Exists(dllPath))
      {
        var ex = new Exception(string.Format("bat_data.dll not found ({0})", dllPath));
        Log.Error(ex);
        throw ex;
      }

      this.handleBatScriptLibrary = Win32.LoadLibrary(dllPath);
      this.DllPath = dllPath;

      if (this.handleBatScriptLibrary != IntPtr.Zero)
      {
        this.ExecuterScript = this.AttachGenericFunction<Func_ExecuterScriptWpf>("ExecuterScriptWpf");
        this.SetMainWnd = this.AttachGenericFunction<Func_SetMainWnd>("SetMainWnd");
        this.SetParallelMode = this.AttachGenericFunction<Func_SetParallelMode>("SetParallelMode");
        this.StopExecution = this.AttachGenericFunction<Func_StopExecution>("StopExecution");

        this.SetMainWnd?.Invoke(parent);
      }
      else
      {
        throw new Exception(NexioMax3.Domain.Properties.Resources.CanNotLoad + dllPath);
      }

      this.IsFree = false;
      return true;
    }

    public void FreeLibrary()
    {
      this.ExecuterScript = null;
      this.SetMainWnd = null;
      this.SetParallelMode = null;
      this.StopExecution = null;

      Win32.FreeLibrary(this.handleBatScriptLibrary);
      this.IsFree = true;
    }
  }
}