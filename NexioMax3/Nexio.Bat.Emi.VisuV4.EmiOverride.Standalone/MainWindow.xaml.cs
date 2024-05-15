using System.Windows;

namespace Nexio.Bat.Emi.VisuV4.EmiOverride.Standalone
{
  using Nexio.Bat.Emi.VisuV4.Definition.ViewModel;

  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      var vm = new EmiOverrideViewModel();
      vm.CloseRequested += this.VmOnCloseRequested;
      this.DataContext = vm;
    }

    private void VmOnCloseRequested(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
