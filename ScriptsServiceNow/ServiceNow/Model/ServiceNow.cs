namespace ServiceNow.Model
{
  using Newtonsoft.Json;
  using RestSharp;
  using RestSharp.Authenticators;
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.Linq;

  public class ServiceNow
  {
    public RestClient Client { get; set; }

    public List<Ticket> ListTickets { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public ServiceNow(string login, string password)
    {
      this.ListTickets = new List<Ticket>();
      this.Login = login;
      this.Password = password;
      TryLogin(login, password);
    }

    public bool TryLogin(string login, string password)
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

    public static string GetApiUrl()
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

    public void GetMyIncidents()
    {
      var request = new RestRequest("now/table/incident?sysparm_query=active=true^assigned_to=0aac29c3dbd08c10295f320a689619b9", Method.GET);
      request.RequestFormat = RestSharp.DataFormat.Json;
      IRestResponse response = this.Client.Execute(request);
      dynamic jsonResponse = JsonConvert.DeserializeObject(response.Content);

      var nTickets = jsonResponse.result.Count;
      for (int i = 0; i < nTickets; i++)
      {
        var ticketContent = jsonResponse.result[i];
        var ticketNumber = ticketContent.number;
        var ticketId = ticketContent.sys_id;

        request = new RestRequest("now/table/incident/" + ticketId, Method.GET);
        request.RequestFormat = RestSharp.DataFormat.Json;
        response = this.Client.Execute(request);
        dynamic jsonResponse2 = JsonConvert.DeserializeObject(response.Content);

        var updatedBy = jsonResponse2.result.sys_updated_by;
        Ticket ticket = new Ticket(ticketNumber.Value, ticketId.Value, updatedBy.Value);
        this.ListTickets.Add(ticket);
      }
    }

    public void UpdateIncidentStatus(string ticketId, string status)
    {
      var request = new RestRequest("/now/table/incident/" + ticketId, Method.PATCH) { RequestFormat = DataFormat.Json };
      request.AddBody(new { u_incident_status = status });   // uses JsonSerializer
      request.AddBody(new { u_ticket_status_view = status });   // uses JsonSerializer
      var response = this.Client.Execute(request);
    }
  }
}