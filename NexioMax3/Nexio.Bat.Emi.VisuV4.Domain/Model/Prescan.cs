#pragma warning disable SA1618
namespace NexioMax3.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  public class Prescan
  {
    private Mutex dataPointMutex = new Mutex();
    private SortedList<int, (double x, double y)> dataPoint = new SortedList<int, (double x, double y)>();
    private SortedList<int, (double x, double y)> buffer = new SortedList<int, (double x, double y)>();
    private bool isBuffer = false;
    private Mutex bufferMutex = new Mutex(); // le verrou permet de bloquer les modifications de liste incongrues

    public Function Function { get; set; }

    public string Name { get; set; }

    /// <summary>
    /// Liste des points. La valeur x du point est en Hz
    /// Encapsulé dans un objet qui maintiens la liste de points dans la propriété Data.
    /// Cet objet gère un verrou sur la liste :
    /// Toujours utiliser
    /// using(var data = prescan.DataPoints) {...}
    /// </summary>
    public ExclusiveList<int, (double x, double y)> DataPoints()
    {
      if (this.isBuffer)
      {
        // on recopie le buffer dans data
        using (var buf = this.Buffer())
        {
          try
          {
            // on utilise directement le mutex da dataPoint pour pas faire d'appel recursif
            this.dataPointMutex.WaitOne();
            {
              foreach (var valueTuple in buf.Data)
              {
                if (!this.dataPoint.ContainsKey(valueTuple.Key))
                {
                  this.dataPoint.Add(valueTuple.Key, valueTuple.Value);
                }
              }
            }
          }
          finally
          {
            this.dataPointMutex.ReleaseMutex();
          }

          buf.Data.Clear();
        }

        this.isBuffer = false;
      }

      return new ExclusiveList<int, (double x, double y)>(this.dataPoint, this.dataPointMutex);
    }

    public void Clear()
    {
      using (var buf = this.Buffer())
      {
        buf.Data.Clear();
      }

      using (var dp = this.DataPoints())
      {
        dp.Data.Clear();
      }
    }

    /// <summary>
    /// Ajoute un point à la liste des points
    /// </summary>
    /// <param name="pt">Tuple du point. X doit être en Hz</param>
    /// <param name="idPoint">L'id du point</param>
    public void AddPoint((double x, double y) pt, int idPoint)
    {
      using (var buf = this.Buffer())
      {
        this.isBuffer = true;
        // utilisation d'un tampon pour eviter le changement du nombre d'éléments lors des ajouts
        buf.Data.Add(idPoint, pt);
      }
    }

    public void DeletePoints(int idFirstPt, int nNombre)
    {
      int idLastPt = idFirstPt + nNombre - 1;
      using (var dp = this.DataPoints())
      {
        if (dp.Data.Count > 0)
        {
          if (dp.Data.First().Key >= idFirstPt && dp.Data.Last().Key <= idLastPt)
          {
            dp.Data.Clear();
          }
          else
          {
            var toRemove = dp.Data.Where(pt => pt.Key >= idFirstPt && pt.Key <= idLastPt).ToList();
            foreach (var r in toRemove)
            {
              dp.Data.Remove(r.Key);
            }
          }
        }
      }
    }

    /// <summary>
    /// Requête linq protégé pour dataPoints.Values.Any(request);
    /// </summary>
    public bool ValuesAny(Func<(double x, double y), bool> request)
    {
      using (var dp = this.DataPoints())
      {
        return dp.Data.Values.Any(request);
      }
    }

    /// <summary>
    /// Requête linq protégé pour dataPoints.Any();
    /// </summary>
    public bool DataPointsAny()
    {
      using (var dp = this.DataPoints())
      {
        return dp.Data.Any();
      }
    }

    /// <summary>
    /// Requête linq protégé pour dataPoints.ContainsKey(idPoint);
    /// </summary>
    public bool DataPointsContainsKey(int idPoint)
    {
      using (var dp = this.DataPoints())
      {
        return dp.Data.ContainsKey(idPoint);
      }
    }

    /// <summary>
    /// Requête linq protégé pour dataPoints.Select(request);
    /// </summary>
    public IEnumerable<T> DataPointsSelect<T>(Func<KeyValuePair<int, (double x, double y)>, T> request)
    {
      using (var dp = this.DataPoints())
      {
        return dp.Data.Select(request);
      }
    }

    /// <summary>
    /// Accès sécurisé au buffer
    /// Cet objet gère un verrou sur la liste :
    /// Toujours utiliser
    /// using(var data = prescan.DataPoints) {...}
    protected ExclusiveList<int, (double x, double y)> Buffer()
    {
      return new ExclusiveList<int, (double x, double y)>(this.buffer, this.bufferMutex);
    }

    /// <summary>
    /// Gestion automatique du Mutex associé à la liste de valeur
    /// Utilisé à la fois pour le dataPoint et le buffer
    /// TOUJOURS UTILISER DANS UN USING()
    /// </summary>
    public class ExclusiveList<TKey, TValue> : IDisposable
    {
      private SortedList<TKey, TValue> data;
      private Mutex mutex;

      public ExclusiveList(SortedList<TKey, TValue> collection, Mutex mutex)
      {
        this.data = collection;
        this.mutex = mutex;
        this.mutex.WaitOne();
      }

      public SortedList<TKey, TValue> Data { get => this.data; }

      public void Dispose()
      {
        this.mutex.ReleaseMutex();
      }
    }
  }
}