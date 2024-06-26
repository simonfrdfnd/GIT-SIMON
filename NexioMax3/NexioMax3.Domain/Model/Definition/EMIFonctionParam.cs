﻿namespace NexioMax3.Domain.Model.Definition
{
  using System;
  using System.Collections.Generic;
  using System.Data.OleDb;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Service;
  using Nexio.Bat.Common.Domain.Infrastructure;
  using Nexio.Bat.Common.Domain.Infrastructure.AccessDataBase;
  using NexioMax3.Domain.Engine;
  using NexioMax3.Domain.Repository;

  internal class EMIFonctionParam
  {
    public Guid GuidSousBande { get; set; }

    public int IdDLL { get; set; }

    public int IndexParam { get; set; }

    public string ValParam { get; set; }
  }
}