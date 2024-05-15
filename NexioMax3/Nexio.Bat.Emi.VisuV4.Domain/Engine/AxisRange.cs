namespace Nexio.Bat.Emi.VisuV4.Domain.Engine
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Newtonsoft.Json;

  [JsonObject(MemberSerialization.OptIn)]
  public struct AxisRange
  {
    public static AxisRange None = new AxisRange(double.PositiveInfinity, double.NegativeInfinity);

    public AxisRange(double min, double max)
    {
      this.Min = min;
      this.Max = max;
    }

    [JsonProperty(PropertyName = "Min")]
    public double Min { get; set; }

    [JsonProperty(PropertyName = "Max")]
    public double Max { get; set; }

    /// <summary>
    /// User-defined conversion from tuple to AxisRange
    /// </summary>
    public static implicit operator AxisRange((double, double) r)
    {
      return new AxisRange(r.Item1, r.Item2);
    }

    public static implicit operator (double, double)(AxisRange r)
    {
      return (r.Min, r.Max);
    }

    public static AxisRange operator +(AxisRange l, double r)
    {
      return new AxisRange(Math.Min(l.Min, r), Math.Max(l.Max, r));
    }

    public static AxisRange operator +(AxisRange l, AxisRange r)
    {
      return new AxisRange(Math.Min(l.Min, r.Min), Math.Max(l.Max, r.Max));
    }

    public static bool operator ==(AxisRange x, AxisRange y)
    {
      return x.Min == y.Min && x.Max == y.Max;
    }

    public static bool operator !=(AxisRange x, AxisRange y)
    {
      return !(x == y);
    }

    public static AxisRange RoundMinMax(AxisRange r, bool extend = true)
    {
      if (r == AxisRange.None)
      {
        return r;
      }

      var range = Math.Abs(r.Max - r.Min);
      double interval;
      Func<double, double> exponent = x => Math.Ceiling(Math.Log(x, 10));
      Func<double, double> mantissa = x => x / Math.Pow(10, exponent(x) - 1);
      Func<double, double> removeNoise = x => double.Parse(x.ToString("e14"));
      if (Math.Abs(range) > double.Epsilon)
      {
        interval = Math.Pow(10, exponent(range));
        double intervalCandidate = interval;
        while (true)
        {
          var m = (int)mantissa(intervalCandidate);
          if (m == 5)
          {
            // reduce 5 to 2
            intervalCandidate = removeNoise(intervalCandidate / 2.5);
          }
          else if (m == 2 || m == 1 || m == 10)
          {
            // reduce 2 to 1, 10 to 5, 1 to 0.5
            intervalCandidate = removeNoise(intervalCandidate / 2.0);
          }
          else
          {
            intervalCandidate = removeNoise(intervalCandidate / 2.0);
          }

          if (range / intervalCandidate > 10)
          {
            break;
          }

          if (double.IsNaN(intervalCandidate) || double.IsInfinity(intervalCandidate))
          {
            break;
          }

          interval = intervalCandidate;
        }
      }
      else
      {
        // on est sur le cas min == max
        if (r.Max == 0)
        {
          interval = 1;
          return new AxisRange(-1, 1);
        }
        else
        {
          interval = r.Max;
          double intervalCandidate = interval;
          while (true)
          {
            var m = (int)mantissa(intervalCandidate);
            if (m == 5)
            {
              // reduce 5 to 2
              intervalCandidate = removeNoise(intervalCandidate / 2.5);
            }
            else if (m == 2 || m == 1 || m == 10)
            {
              // reduce 2 to 1, 10 to 5, 1 to 0.5
              intervalCandidate = removeNoise(intervalCandidate / 2.0);
            }
            else
            {
              intervalCandidate = removeNoise(intervalCandidate / 2.0);
            }

            if (r.Max / intervalCandidate > 10)
            {
              break;
            }

            if (double.IsNaN(intervalCandidate) || double.IsInfinity(intervalCandidate))
            {
              break;
            }

            interval = intervalCandidate;
          }
        }
      }

      var roundMin = Math.Floor(r.Min / interval) * interval;
      var roundMax = Math.Ceiling(r.Max / interval) * interval;

      if (extend)
      {
        if (roundMin == r.Min)
        {
          roundMin -= interval;
        }

        if (roundMax == r.Max)
        {
          roundMax += interval;
        }
      }

      return new AxisRange(roundMin, roundMax);
    }

    public static AxisRange Combine(IEnumerable<AxisRange> ranges)
    {
      var res = AxisRange.None;
      foreach (var r in ranges)
      {
        res += r;
      }

      return res;
    }

    /// <summary>
    /// Retourne les ranges non définis par l'ensemble defined sur l'intervale range
    /// </summary>
    /// <param name="range">l'intervale inspecté</param>
    /// <param name="defined">les ranges définis</param>
    /// <returns>les ranges non définis</returns>
    public static IEnumerable<AxisRange> UndefinedRanges(AxisRange range, IEnumerable<AxisRange> defined)
    {
      if (range == AxisRange.None || (range.Max - range.Min) <= 0)
      {
        yield break;
      }

      var ordered = defined.OrderBy(fr => fr.Max)
            .OrderBy(fr => fr.Min)
            .ToList();

      if (!ordered.Any())
      {
        yield return range;
        yield break;
      }

      if (ordered.First().Min < range.Min)
      {
        yield return new AxisRange(range.Min, ordered.First().Min);
      }

      for (int i = 1; i < ordered.Count; i++)
      {
        var r0 = ordered[i - 1];
        var r1 = ordered[i];
        if (r1.Min > r0.Max)
        {
          yield return new AxisRange(r0.Max, r1.Min);
        }
      }

      if (ordered.Last().Max < range.Max)
      {
        yield return new AxisRange(ordered.Last().Max, range.Max);
      }
    }

    public override bool Equals(Object obj)
    {
      return obj is AxisRange && this == (AxisRange)obj;
    }

    public override int GetHashCode()
    {
      return this.Min.GetHashCode() ^ this.Max.GetHashCode();
    }
  }
}