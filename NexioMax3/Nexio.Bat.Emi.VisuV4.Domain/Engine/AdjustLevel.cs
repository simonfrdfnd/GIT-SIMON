namespace Nexio.Bat.Emi.VisuV4.Domain.Engine
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;
  using Nexio.Bat.Emi.VisuV4.Domain.Service;

  public class AdjustLevel
  {
    public static AxisRange Get(TestResult result, IEnumerable<Limit> limits, IEnumerable<SubRange> subRanges, IEnumerable<ExternalCurve> externals, bool fromTestResult = true)
    {
      if (fromTestResult && !result.YLimits.AdjustLevel)
      {
        return result.YLimits.Range;
      }

      var range = AxisRange.None;

      if (subRanges != null)
      {
        // using for loop as subRanges / ValueList are supposed to be modified during RunPrescanProcess
        var prescList = subRanges.SelectMany(sr => sr.PrescanList).ToList();

        for (int i = 0; i < prescList.Count; i++)
        {
          var pre = prescList[i];
          using (var dp = pre.DataPoints())
          {
            for (int j = 0; j < dp.Data.Count(); j++)
            {
              if (dp.Data.Values.Count < j)
              {
                break;
              }

              var res = dp.Data.Values[j];
              if (!double.IsNaN(res.y))
              {
                range += res.y;
              }
            }
          }
        }

        var suspList = subRanges.SelectMany(sr => sr.SuspectList).ToList();
        for (int i = 0; i < suspList.Count; i++)
        {
          var sus = suspList[i];

          // On parcours toute les mesures
          var colUsed = sus.Function.Columns.Where(c => c.Used).ToList();
          for (int j = 0; j < colUsed.Count; j++)
          {
            if (colUsed[j].Type == TypeCol.TYPECOL_MESURE)
            {
              var pt = sus.Values[j];
              if (double.TryParse(pt, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double level))
              {
                range += level;
              }
            }
          }
        }

        var finoList = subRanges.SelectMany(sr => sr.FinalList).ToList();
        for (int i = 0; i < finoList.Count; i++)
        {
          var fino = finoList[i];

          // On parcours toute les mesures
          var colUsed = fino.Function.Columns.Where(c => c.Used).ToList();
          for (int j = 0; j < colUsed.Count; j++)
          {
            if (colUsed[j].Type == TypeCol.TYPECOL_MESURE)
            {
              var pt = fino.Values[j];
              if (double.TryParse(pt, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out double level))
              {
                range += level;
              }
            }
          }
        }
      }

      if (externals != null)
      {
        // using for loop as subRanges / ValueList are supposed to be modified during RunPrescanProcess
        foreach (var pre in externals)
        {
          for (int j = 0; j < pre.DataPoints.Count(); j++)
          {
            var res = pre.DataPoints.ElementAt(j);
            if (!double.IsNaN(res.y))
            {
              range += res.y;
            }
          }
        }
      }

      if (limits != null)
      {
        // create an anonymous method for range intersection (excluding bounds)
        var intersect = new Func<AxisRange, AxisRange, bool>((f1, f2) => (f2.Max > f1.Min) && (f2.Min < f1.Max));

        // Get List Band
        // TODO : filter limitbands used in test definition
        foreach (var limit in limits)
        {
          if (!limit.IsVisible)
          {
            continue;
          }

          var limitXRange = (limit.DataPoints.Select(p => p.x).Min() * 1e6, limit.DataPoints.Select(p => p.x).Max() * 1e6); // range en Hz
          if (subRanges.Any(s => intersect((s.FMin, s.FMax), limitXRange)))
          {
            foreach (var pt in limit.DataPoints)
            {
              range += pt.y;
            }
          }
        }
      }

      var levels = AxisRange.RoundMinMax(range);

      AxisRange refRange = AxisRange.None;
      var unit = Provider.Instance.GetUniteEssai(result.ID);
      var isTestdBm = unit.ToLower() == "dbm";
      foreach (var sr in subRanges)
      {
        Provider.Instance.BatData.GetParametresReglage(sr.Id, out int nbr, out List<string> reglages);

        // sur certains essais il n'y a pas de reflevel auto
        if (reglages.Count < ((int)ConstReglage.RefLevAuto + 1) || reglages[(int)ConstReglage.RefLevAuto] == "0")
        {
          double refLeveldBuV = reglages[(int)ConstReglage.NiveauRef].ToDouble(); // récupération du niveau de référence. Ce niveau est toujours stocké en dBµV
          double dynamic = reglages[(int)ConstReglage.Dynamique].ToDouble();
          double refLevel;

          if (!isTestdBm)
          {
            refLevel = refLeveldBuV;
          }
          else
          {
            refLevel = refLeveldBuV - 107;
          }

          refRange += refLevel;
          refRange += refLevel - dynamic;
        }
      }

      levels += refRange;

      return levels;
    }
  }
}