namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public class Function
  {
    public int ID { get; set; }

    public string Name { get; set; }

    public int FunctionType { get; set; }

    public int Action { get; set; }

    public int TypeSignal { get; set; }

    public DetectorSignature Signature { get; set; }

    public List<Column> Columns { get; } = new List<Column>();
  }
}