namespace Nexio.Bat.Emi.VisuV4.Domain.Service
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Nexio.Bat.Emi.VisuV4.Domain.Model;

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
