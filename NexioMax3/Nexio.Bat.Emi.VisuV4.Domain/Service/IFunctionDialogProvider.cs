namespace NexioMax3.Domain.Service
{
  using System;
  using System.Collections.Generic;

  public interface IFunctionDialogProvider
  {
    List<IFunctionDialog> GetDialogs(string nomDllManu);
  }

  public interface IFunctionDialog
  {
    string Name { get; }

    IntPtr Native { get; }
  }
}
