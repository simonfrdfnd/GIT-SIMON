namespace NexioMax3.Model
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

  internal class GSousBandes
  {
    public Guid GuidSousBande { get; set; }

    public Guid GuidEssai { get; set; }

    public double FreqMin { get; set; }

    public double FreqMax { get; set; }

    public string Commentaire { get; set; }

    public double ValeurPasFreq { get; set; }

    public int TypeProgression { get; set; }

    public bool FMinInclue { get; set; }

    public bool FMaxInclue { get; set; }

    public double Distance { get; set; }

    public Guid GuidDomaine { get; set; }

    public Guid GuidDomaineFinal { get; set; }

    public int ScriptPosition { get; set; }

    public string ScriptNomCustomLine { get; set; } = String.Empty;

    public int EtatExecution { get; set; }

    public bool HauteurAuto { get; set; }

    public double Hauteur { get; set; }

    public bool AngleAuto { get; set; }

    public double Angle { get; set; }

    public string ScriptDebutSB { get; set; }

    public string ScriptFinSB { get; set; }

    public int? GlobalPhase { get; set; }

    public int? GlobalPhaseStep { get; set; }
  }
}