using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientNEXIO.Model;
using System.Net;

namespace ClientNEXIO
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ClientNEXIO.ServerTCPIP Server { get; set; }
    private string URL { get; set; }
    public MainWindow()
    {
      InitializeComponent();
    }
    private void OnCommandReceived(object sender, EventArgs e)
    {
      CommandReceivedEventArgs eCmd = (CommandReceivedEventArgs)e;
      string command = eCmd.Command;
      string result = "";

      Rest.Post(URL, $"/api/Values?value={"WRITE " + command}");
      
      if (command.Contains('?'))
      {
        int c = 0;
        //Attente de la réponse (10 essais)
        while ((!result.Contains("READ")) && (c < 10))
        {
          System.Threading.Thread.Sleep(500);
          result = Rest.Get(URL, $"/api/Values");
          c++;
        }

        if (c == 10)
        {
          this.textBoxContent.Text = "No answer";
        }
        else if (result.Contains("READ"))
        {
          this.Server.Answer = result.Substring(5).Trim('\"');
          this.Server.CommandTreated = true;
        }
      }
    }

  
    private void OnClick(object sender, EventArgs e)
    {
      if (this.button.Content.ToString() == "Go")
      {
        this.button.Content = "Stop";
        IPAddress address = IPAddress.Parse(this.textBoxVISA.Text.Split(':')[2]);
        int port = Convert.ToInt32(this.textBoxVISA.Text.Split(':')[4]);
        this.Server = new ServerTCPIP(new IPEndPoint(address, port));
        this.Server.Run(4096, new System.Threading.CancellationToken());
        this.Server.OnCommandReceived += OnCommandReceived;
        Rest.Delete(this.textboxURL.Text, "/api/Values");
        this.URL = this.textboxURL.Text;
      }
      else
      {
        this.button.Content = "Go";
      }
    }
  }
}
