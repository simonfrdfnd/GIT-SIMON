namespace Nexio.Bat.Emi.VisuV4.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Linq;
  using Nexio.Bat.Common.Domain.ATDB.Model;
  using Nexio.Bat.Common.Domain.TestDefinition.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public class Test
  {
    public int NumEssai { get; set; }

    public string ResultPath { get; set; }

    public string Name { get; set; }

    public string Unit { get; set; }

    public string UnitBL { get; set; }

    public List<SubRange> SubRangeList { get; set; } = new List<SubRange>();

    public ObjectType TestType { get; set; }

    public Guid TestId { get; set; }

    public FollowUp FollowUp { get; set; } = new FollowUp();

    public List<ExternalCurve> ExternalCurves { get; internal set; } = new List<ExternalCurve>();

    public List<Marker> Markers { get; internal set; } = new List<Marker>();

    public TestResult Result { get; internal set; }

    public Conclusion Conclusion { get; set; } = Conclusion.INCONCLUSIVE;
  }
}