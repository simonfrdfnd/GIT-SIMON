namespace NexioMax3.Domain.Wrapper
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Text;
  using Microsoft.Win32;
  using NexioMax3.Domain.Model;

  public static class Win32
  {
    private static List<string> dllDirs = new List<string>();

    public static void SetDllDir(string path)
    {
      if (!string.IsNullOrEmpty(path) && !dllDirs.Contains(path))
      {
        AddDllDirectory(path);
        dllDirs.Add(path);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Utilisé pour charger une dll C++"),]
    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    public static extern IntPtr LoadLibrary(string dllPath);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Utilisé pour charger une dll C++"),]
    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hwnd, string procedureName);

    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Utilisé pour charger une dll C++"),]
    [DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    private static extern void AddDllDirectory(string lppathName);

    // 2 points qui peuvent entrainer des bugs à l'usage avec cette méthode :
    // https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setdlldirectorya
    // -To revert to the standard search path used by LoadLibrary and LoadLibraryEx, call SetDllDirectory with NULL.
    // -Each time the SetDllDirectory function is called, it replaces the directory specified in the previous SetDllDirectory call
    // On va lui preferer des appels à AddDllDirectory
    ////[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass", Justification = "Utilisé pour charger une dll C++"),]
    ////[DllImport("kernel32", CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true, SetLastError = true)]
    ////private static extern void SetDllDirectory(string lppathName);
  }
}