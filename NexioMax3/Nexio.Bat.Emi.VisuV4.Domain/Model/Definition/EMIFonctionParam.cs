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

  internal class EMIFonctionParam
  {
    public Guid GuidSousBande { get; set; }

    public int IdDLL { get; set; }

    public int IndexParam { get; set; }

    public string ValParam { get; set; }
  }
}