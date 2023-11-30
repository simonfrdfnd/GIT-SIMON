using Wpf.Ui.Common.Interfaces;

namespace NexioMax.Views.Pages
{
  /// <summary>
  /// Interaction logic for DataView.xaml
  /// </summary>
  public partial class IoPage : INavigableView<ViewModels.IoViewModel>
  {
    public ViewModels.IoViewModel ViewModel
    {
      get;
    }

    public IoPage(ViewModels.IoViewModel viewModel)
    {
      ViewModel = viewModel;

      InitializeComponent();
    }

    void Focus(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      this.TerminalOutput.Focus();
    }
  }
}
