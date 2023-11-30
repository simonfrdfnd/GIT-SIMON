using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace NexioMax.ViewModels
{
  public partial class MainWindowViewModel : ObservableObject
  {
    private bool _isInitialized = false;

    [ObservableProperty]
    private string _applicationTitle = String.Empty;

    [ObservableProperty]
    private ObservableCollection<INavigationControl> _navigationItems = new();

    [ObservableProperty]
    private ObservableCollection<INavigationControl> _navigationFooter = new();

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new();

    public MainWindowViewModel(INavigationService navigationService)
    {
      if (!_isInitialized)
        InitializeViewModel();
    }

    private void InitializeViewModel()
    {
      ApplicationTitle = "NexioMax";

      NavigationItems = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Home",
                    PageTag = "dashboard",
                    Icon = SymbolRegular.Home24,
                    PageType = typeof(Views.Pages.DashboardPage)
                },
                new NavigationItem()
                {
                    Content = "Interactive I/O",
                    PageTag = "io",
                    Icon = SymbolRegular.PlugConnected20,
                    PageType = typeof(Views.Pages.IoPage)
                },
                new NavigationItem()
                {
                    Content = "Trace Analysis",
                    PageTag = "analysis",
                    Icon = SymbolRegular.Microscope24,
                    PageType = typeof(Views.Pages.TraceAnalysisPage)
                },
                new NavigationItem()
                {
                    Content = "Virtual Instrument",
                    PageTag = "virtual",
                    Icon = SymbolRegular.Communication16,
                    PageType = typeof(Views.Pages.VirtualInstrumentPage)
                },
                new NavigationItem()
                {
                    Content = "Virtual Immunity Setup",
                    PageTag = "data",
                    Icon = SymbolRegular.Pulse20,
                    PageType = typeof(Views.Pages.DataPage)
                }
            };

      NavigationFooter = new ObservableCollection<INavigationControl>
            {
                new NavigationItem()
                {
                    Content = "Settings",
                    PageTag = "settings",
                    Icon = SymbolRegular.Settings24,
                    PageType = typeof(Views.Pages.SettingsPage)
                }
            };

      TrayMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem
                {
                    Header = "Home",
                    Tag = "tray_home"
                }
            };

      _isInitialized = true;
    }
  }
}
