namespace NexioMax3.Definition.ViewModel
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Nexio.Wpf.Base;
  using Nexio.Wpf.Command;
  using NexioMax3.Definition.View;

  public class TerminalViewModel : ViewModelBase
  {
    private string content;

    private RelayObjectCommand<KeyEventArgs> onEnterCommand;

    public TerminalViewModel()
    {
      this.Content = "> ";
    }

    public string Content
    {
      get => this.content;
      set => this.Set(nameof(this.Content), ref this.content, value);
    }

    public RelayObjectCommand<KeyEventArgs> OnEnterCommand => this.onEnterCommand ?? (this.onEnterCommand = new RelayObjectCommand<KeyEventArgs>(this.OnEnterAction));

    private void OnEnterAction(KeyEventArgs e)
    {
      Console.WriteLine("Do work...");
    }
  }
}
