namespace Model
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using RestSharp;
  using RestSharp.Authenticators;
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.Linq;

  public class ServiceNow
  {
    public RestClient Client { get; set; }
    public string Login { get; set; }

    public string Password { get; set; }

    public ServiceNow(string login, string password)
    {
      this.Login = login;
      this.Password = password;
      TryLogin(login, password);
    }

    public string GetIncidentComments(string ticketNumber)
    {
      string resource = $"now/table/incident?number={ticketNumber}&sysparm_display_value=true";
      var request = new RestRequest(resource, Method.GET);
      request.RequestFormat = RestSharp.DataFormat.Json;
      IRestResponse response = this.Client.Execute(request);
      JObject joResponse = JObject.Parse(response.Content);
      var ticket = joResponse["result"].ToString();
      var ticket2 = ticket.Remove(0, 5);
      var ticket3 = ticket2.Remove(ticket2.Length - 2, 2);
      JObject joResponse2 = JObject.Parse(ticket3);
      var com = joResponse2["comments_and_work_notes"].ToString();
      return com;
    }

    public string GetWorknote(string ticketNumber)
    {
      string resource = $"now/table/sys_history_line?sys_id=7fbd10831bd3551043fda64fad4bcb6b";
      var request = new RestRequest(resource, Method.GET);
      request.RequestFormat = RestSharp.DataFormat.Json;
      IRestResponse response = this.Client.Execute(request);
      JObject joResponse = JObject.Parse(response.Content);
      var ticket = joResponse["result"].ToString();
      var ticket2 = ticket.Remove(0, 5);
      var ticket3 = ticket2.Remove(ticket2.Length - 2, 2);
      request.Method = Method.PATCH;
      request.Body = new RequestBody();
      return "";
    }

    public void ParseCommentSttring(string comments)
    {
      List<string> commentsList = new List<string>();
      commentsList = comments.Split().ToList();
    }

    private bool TryLogin(string login, string password)
    {
      string baseUrl = GetApiUrl();
      this.Client = new RestClient(baseUrl)
      {
        Authenticator = new HttpBasicAuthenticator(login, password),
      };

      var request = new RestRequest("/now/table/incident?sysparm_limit=15", Method.GET);
      request.AddParameter("sys_updated_on", DateTime.Now);
      request.RequestFormat = RestSharp.DataFormat.Json;
      IRestResponse responseLogin = this.Client.Execute(request);
      return responseLogin.StatusCode == System.Net.HttpStatusCode.OK;
    }

    private static string GetApiUrl()
    {
      string url = GetYourNexioUrl();
      if (url.Last() != '/')
      {
        url += "/";
      }

      url += "api";
      return url;
    }

    private static string GetYourNexioUrl()
    {
      string yournexio = ConfigurationManager.AppSettings["YourNexioServer"];
      if (string.IsNullOrEmpty(yournexio))
      {
        yournexio = @"https://nexioprod.service-now.com";
      }

      return yournexio;
    }
  }
}