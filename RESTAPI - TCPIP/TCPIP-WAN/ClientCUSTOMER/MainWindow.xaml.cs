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
using ClientCUSTOMER.Model;
using System.Net;
using System.Threading;

namespace ClientCUSTOMER
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private string URL { get; set; }

    private CancellationTokenSource TokenSource { get; set; }

    private SenderVISA SenderVISA { get; set; }
    private string Command
    {
      get { return command; }
      set 
      {
        command = value.Substring(6);
        this.SenderVISA.Write(command);
        Application.Current.Dispatcher.Invoke(new Action(() => { this.textBoxContent.Text = command; }));
        if (command.Contains('?'))
        {
          string res = this.SenderVISA.Read();
          if (res.Length > 0) 
          {
            Rest.Post(URL, $"/api/Values?value=READ {res}");
            Application.Current.Dispatcher.Invoke(new Action(() => { this.textBoxContent.Text = res; }));
          }
        }
      }
    }

    private string command;
    public MainWindow()
    {
      InitializeComponent();
    }
    private void OnClick(object sender, EventArgs e)
    {
      if (this.button.Content.ToString() == "Go")
      {
        this.TokenSource = new CancellationTokenSource();
        this.button.Content = "Stop";
        Rest.Delete(this.textboxURL.Text, "/api/Values");
        this.URL = this.textboxURL.Text;
        this.SenderVISA = new SenderVISA(this.textBoxVISA.Text);
        var task = new Task(() => GetCommand());
        task.Start();
      }
      else
      {
        this.button.Content = "Go";
      }
    }

    private void GetCommand()
    {
      string res;
      while (!this.TokenSource.IsCancellationRequested) 
      {
        res = Rest.Get(this.URL, "/api/Values");
        res = res.Trim('\"');
        if (res.Contains("WRITE")) 
        {
          Rest.Delete(URL, "/api/Values");
          this.Command = res;
          Console.WriteLine(res);  
        }
        else
        {
          Console.WriteLine("Nothing to read...");
        }
        Thread.Sleep(1000);
      }
    }
  }
}
