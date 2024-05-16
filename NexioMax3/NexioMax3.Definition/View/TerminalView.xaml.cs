namespace NexioMax3.Definition.View
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using NexioMax3.Definition.ViewModel;

  /// <summary>
  /// Interaction logic for TerminalView.xaml
  /// </summary>
  public partial class TerminalView : UserControl
  {
    private bool stopEvent;

    public TerminalView()
    {
      this.InitializeComponent();
      var vm = new TerminalViewModel();
      this.DataContext = vm;
      this.stopEvent = false;
    }

    private void TerminalTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (this.IsBackOrLeft(e.Key) && IsLineBegining())
      {
        e.Handled = true;
      }
      else if (e.Key == Key.Enter)
      {
        this.TerminalTextBox.Text += "\n> ";
        this.stopEvent = true;
        this.TerminalTextBox.CaretIndex = this.TerminalTextBox.Text.Length;
      }
    }

    private void TerminalTextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
      if (this.stopEvent)
      {
        this.stopEvent = false;
        return;
      }

      if (this.IsLineBegining())
      {
        this.stopEvent = true;
        if (this.TerminalTextBox.Text.LastIndexOf("\n> ") >= 0)
        {
          this.TerminalTextBox.CaretIndex = this.TerminalTextBox.Text.LastIndexOf("\n> ") + 3;
        }
        else
        {
          this.TerminalTextBox.CaretIndex = 2;
        }
      }
    }

    private bool IsBackOrLeft(Key key)
    {
      if (key is Key.Left || key is Key.Back)
      {
        return true;
      }

      return false;
    }

    private bool IsLineBegining()
    {
      int index = this.TerminalTextBox.CaretIndex;
      if (this.TerminalTextBox.CaretIndex <= 2)
      {
        return true;
      }
      else if (this.TerminalTextBox.Text.Substring(index - 3).Contains("\n> "))
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }
}
