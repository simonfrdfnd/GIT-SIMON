using RepeaterTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace ListenerTCP
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    Listener Listener { get; set; }
    public MainWindow()
    {
      InitializeComponent();
      IPAddress address = IPAddress.Parse(this.ipSender.Text.Split(':')[0]);
      int port = Convert.ToInt32(this.ipSender.Text.Split(':')[1]);
      this.Listener = new Listener(new IPEndPoint(address, port));
      this.Listener.ListenerRun(4096, new System.Threading.CancellationToken());
      this.Listener.DataReceived += OnDataReceived;
    }
    private void OnDataReceived(object sender, EventArgs e)
    {
      try
      {
        DataReceivedEventArgs eData = (DataReceivedEventArgs)e;
        Application.Current.Dispatcher.BeginInvoke(new Action(() => 
        {
          this.received.Text = CleanMessage(eData.Data);
        }));
      }
      catch(Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }

    private static string CleanMessage(byte[] bytes)
    {
      string message = System.Text.Encoding.ASCII.GetString(bytes);

      string messageToPrint = null;
      foreach (var nullChar in message)
      {
        if (nullChar != '\0')
        {
          messageToPrint += nullChar;
        }
      }
      return messageToPrint;
    }
  }
}
