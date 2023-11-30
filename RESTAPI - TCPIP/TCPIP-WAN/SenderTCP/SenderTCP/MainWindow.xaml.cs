using RepeaterTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace SenderTCP
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
      IPAddress address = IPAddress.Parse(this.address.Text.Split(':')[0]);
      int port = Convert.ToInt32(this.address.Text.Split(':')[1]);
      IPEndPoint ipRecipient = new IPEndPoint(address, port); 
      Sender.Send(ipRecipient, Encoding.ASCII.GetBytes(this.data.Text));
    }
  }
}
