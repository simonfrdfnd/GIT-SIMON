namespace NexioMax3.Definition.ViewModel
{
  using System.Windows;
  using System.Windows.Controls;
  using Nexio.Wpf.Base;
  using NexioMax3.Definition.View;

  public class Max3ViewModel : ViewModelBase
  {
    private Max3Group selectedGroup;
    private UserControl selectedUserControl;

    public Max3ViewModel()
    {
      this.SelectedGroup = Max3Group.VisaConsole;
    }

    public event RoutedEventHandler CloseRequested;

    public Max3Group SelectedGroup
    {
      get => this.selectedGroup;
      set
      {
        this.Set(nameof(this.SelectedGroup), ref this.selectedGroup, value);
        this.SelectedUserControl = GetCorrespondingUserControl(value);
      }
    }

    public UserControl SelectedUserControl
    {
      get => this.selectedUserControl;
      set => this.Set(nameof(this.SelectedUserControl), ref this.selectedUserControl, value);
    }
    private UserControl GetCorrespondingUserControl(Max3Group group)
    {
      switch (group)
      {
        case Max3Group.VisaConsole:
          return new VisaConsoleView();
        case Max3Group.VirtualImmunitySetup:
          return new VirtualImmunitySetupView();
        default:
          return null;
      }
    }
  }
}