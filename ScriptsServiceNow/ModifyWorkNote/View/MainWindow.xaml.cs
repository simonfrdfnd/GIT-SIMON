using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ModifyWorkNote
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    Model.ServiceNow ServiceNow { get; set; }
    public MainWindow()
    {
      InitializeComponent();
      this.ServiceNow = new Model.ServiceNow("simon.froidefond@nexiogroup.com", "Nexio171296SF");
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
      this.ServiceNow.GetWorknote("");
      //string comments = this.ServiceNow.GetIncidentComments(this.tb.Text);
    }
  }
}
