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

namespace SenderVISA
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private RepeaterTCP.SenderVISA SenderVISA {get; set; }
    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
      if (this.SenderVISA == null) 
      {
        this.SenderVISA = new RepeaterTCP.SenderVISA(this.visaRessource.Text);
      }
      this.SenderVISA.Write(this.data.Text);
    }
  }
}
