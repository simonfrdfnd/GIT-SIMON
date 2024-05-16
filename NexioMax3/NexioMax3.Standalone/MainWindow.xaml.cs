using System.Windows;

namespace NexioMax3.Standalone
{
  using NexioMax3.Definition.ViewModel;

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
