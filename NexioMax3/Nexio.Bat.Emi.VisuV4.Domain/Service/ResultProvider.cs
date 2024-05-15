namespace NexioMax3.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using NexioMax3.Domain.Model;

  public class ResultProvider
  {
    private static ResultProvider intance;

    private ResultProvider()
    {
    }

    public static ResultProvider Instance
    {
      get
      {
        return intance ?? (intance = new ResultProvider());
      }
    }
  }
}
