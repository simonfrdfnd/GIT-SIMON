namespace Nexio.Bat.Emi.VisuV4.Domain.Model.Definition
{
  using System;
  using System.Collections.Generic;
  using System.Data.OleDb;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Service;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using Nexio.Bat.Emi.VisuV4.Domain.Engine;
  using Nexio.Bat.Emi.VisuV4.Domain.Repository;

  public class EMISousBandeFonction
  {
    public Guid GuidSousBande { get; set; }

    public int IdDLL { get; set; }

    public int NumOrdre { get; set; }

    public Guid GuidConfig { get; set; }
  }
}