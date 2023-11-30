using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;

namespace RESTAPI.Controllers
{
  public class ValuesController : ApiController
  {
    public string path = HttpContext.Current.Server.MapPath("~/App_Data/data.txt");
    public string Get()
    {
      string[] lines;
      try
      {
        lines = File.ReadLines(path).ToArray();
      }
      catch
      {
        lines = File.ReadLines(path).ToArray(); //retry au cas ou le fichier soit en cours d'utilisation
      }
      if (lines.Length == 0)
      {
        return "";
      }
      return lines[lines.Length - 1];
    }

    // POST api/values
    public void Post(string value)
    {
      File.AppendAllText(path, value + "\n");
    }

    // DELETE api/values/5
    public void Delete()
    {
      File.WriteAllText(path, "");
    }
  }
}
