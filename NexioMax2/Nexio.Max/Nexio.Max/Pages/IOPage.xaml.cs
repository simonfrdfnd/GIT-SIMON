using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Windows.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Nexio.Max.Pages
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class IOPage : Page
  {
    public IOPage()
    {
      this.InitializeComponent();
      this.DataContext = new Nexio.Max.ViewModel.IOViewModel();
    }

    private void console_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
      if (e.Key.ToString() == "226") //on empêche les >
      {
        e.Handled = true;
        return;
      }

      
      if (e.Key == Windows.System.VirtualKey.Back) //on empêche de supprimer les >
      {
        if (console.Text.Substring(console.SelectionStart - 2, 2) == "> ")
        {
          e.Handled = true;
          return;
        }
      }

      if (e.Key == Windows.System.VirtualKey.Enter)
      {
        // Trouver l'index de la dernière occurrence de '\n' avant la position du curseur
        int lastNewLineIndex = console.Text.Length; 

        // Ajouter "> " au début de la ligne
        string newLine = Environment.NewLine + "> ";
        console.Text += newLine;

        // Déplacer le curseur à la fin de la nouvelle ligne
        console.SelectionStart = console.Text.Length;

        // Empêcher la saisie de retour à la ligne supplémentaire
        e.Handled = true;
      }
    }
  }
}
