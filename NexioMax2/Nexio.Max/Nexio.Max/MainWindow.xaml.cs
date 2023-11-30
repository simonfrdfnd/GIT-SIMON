using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Nexio.Max.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Nexio.Max
{
  /// <summary>
  /// An empty window that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      navigationView.SelectedItem = navigationView.MenuItems[0]; // Sélectionnez le premier élément du menu au démarrage
      NavigateToPage(navigationView.SelectedItem as NavigationViewItem); // Naviguez vers la page correspondante
    }

    private void navigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
      if (args.SelectedItem is NavigationViewItem selectedItem)
      {
        NavigateToPage(selectedItem); // Naviguez vers la page correspondante lorsque la sélection change
      }
    }

    private void NavigateToPage(NavigationViewItem selectedItem)
    {
      if (selectedItem != null)
      {
        switch (selectedItem.Tag)
        {
          case "HomePage":
            contentFrame.Navigate(typeof(HomePage));
            break;
          case "IOPage":
            contentFrame.Navigate(typeof(IOPage));
            break;
          case "PageB":
            contentFrame.Navigate(typeof(PageB));
            break;
            // Ajoutez d'autres cas pour chaque page supplémentaire
        }
      }
    }
  }
}
