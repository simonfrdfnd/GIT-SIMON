using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
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

namespace RepeaterTCP
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public Repeater Repeater { get; set; }
    public MainWindow()
    {
      InitializeComponent();
      IPAddress sender = IPAddress.Parse(this.sender.Text.Split(':')[0]);
      int portSender = Convert.ToInt32(this.sender.Text.Split(':')[1]);
      IPAddress recipient = IPAddress.Parse(this.recipient.Text.Split(':')[0]);
      int portRecipient = Convert.ToInt32(this.recipient.Text.Split(':')[1]);
      IPEndPoint ipSender = new IPEndPoint(sender, portSender);
      IPEndPoint ipRecipient = new IPEndPoint(recipient, portRecipient);
      this.Repeater = new Repeater(ipSender, ipRecipient);
      this.Repeater.Listener.DataReceived += OnDataReceived;
    }
    private void OnDataReceived(object sender, EventArgs e)
    {
      DataReceivedEventArgs eData = (DataReceivedEventArgs)e;
      Application.Current.Dispatcher.BeginInvoke(new Action(() =>
      {
        this.received.Text = CleanMessage(eData.Data);
      }));
      Sender.Send(this.Repeater.IpRecipient, eData.Data);
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
