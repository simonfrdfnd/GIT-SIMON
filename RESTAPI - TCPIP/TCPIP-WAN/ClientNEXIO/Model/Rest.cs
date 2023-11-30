using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientNEXIO.Model
{
  static class Rest
  {
    public static string Get(string uri, string requestParam)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(uri);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      var requestUri = uri + requestParam;
      var response = client.GetAsync(requestUri).Result;
      if (response.IsSuccessStatusCode)
      {
        // Parse the response body
        var dataObjects = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(dataObjects);
        return dataObjects;
      }
      else
      {
        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
        return response.ReasonPhrase;
      }
    }

    public static void Post(string uri, string requestParam)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(uri);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      var requestUri = uri + requestParam;
      var response = client.PostAsync(requestUri, null);
    }

    public static void Delete(string uri, string requestParam)
    {
      var client = new HttpClient();
      client.BaseAddress = new Uri(uri);
      client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      var requestUri = uri + requestParam;
      var response = client.DeleteAsync(requestUri);
    }
  }
}
