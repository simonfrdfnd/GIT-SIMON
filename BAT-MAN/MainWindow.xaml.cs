using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Win32;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BAT_MAN
{
  /// <summary>
  /// An empty window that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainWindow : Window
  {
    private List<BranchRow> _branchRows;
    public MainWindow()
    {
      this.InitializeComponent();
      _branchRows = new List<BranchRow>();

      // Chargement des branches
      string devPath = GetDevPath();
      if (devPath != null)
      {
        List<string> branchesPaths = LoadBranches(devPath).ToList();
        CreateBranchRows(branchesPaths);
      }

      // Chargement des versions
      string installPath = GetInstallPath();
      if (installPath != null)
      {
        List<string> versionsPaths = LoadVersions(installPath).ToList();
        CreateVersionRows(versionsPaths);
      }

      CreateGrid(_branchRows);

      // Resize
      IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
      var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
      var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
      appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1000, Height = 200 + 75 * _branchRows.Count });
      appWindow.Title = "BAT-MAN";
      appWindow.SetIcon("WindowIcon.ico");
    }

    private void CreateGrid(List<BranchRow> branchRows)
    {
      Grid grid = new Grid();
      grid.Margin = new Thickness(0, 50, 0, 0);
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());
      grid.ColumnDefinitions.Add(new ColumnDefinition());

      foreach (var column in grid.ColumnDefinitions)
      {
        column.Width = new GridLength(1, GridUnitType.Auto);
      }

      for (int i = 0; i < branchRows.Count; i++)
      {
        grid.RowDefinitions.Add(new RowDefinition());
      }

      for (int i = 0; i < branchRows.Count; i++)
      {
        if (branchRows[i].HasSolution)
        {
          Button solution = new Button();
          solution.MaxHeight = 40;
          solution.MaxWidth = 40;
          var image = new Image();
          string relativeImagePath = "visual.png";
          string executablePath = AppDomain.CurrentDomain.BaseDirectory;
          string absoluteImagePath = Path.Combine(executablePath, relativeImagePath);
          image.Source = new BitmapImage(new Uri(absoluteImagePath));
          solution.Content = image;
          solution.Click += OpenSolution;
          Grid.SetColumn(solution, 0);
          Grid.SetRow(solution, i);
          grid.Children.Add(solution);
        }

        TextBlock branch = new TextBlock();
        branch.Text = $"{branchRows[i].BranchName}";
        branch.Margin = new Thickness(0, 5, 0, 0);
        Grid.SetColumn(branch, 1);
        Grid.SetRow(branch, i);
        grid.Children.Add(branch);

        TextBlock version = new TextBlock();
        version.Text = $"{branchRows[i].ExeVersion}";
        version.Margin = new Thickness(0, 5, 0, 0);
        Grid.SetColumn(version, 2);
        Grid.SetRow(version, i);
        grid.Children.Add(version);

        Button type = new Button();
        if (branchRows[i].BranchType == BranchType.Version)
        {
          type.Content = $"{branchRows[i].BranchType}";
        }
        else
        {
          type.Content = $"{branchRows[i].BranchType}";
        }
        type.HorizontalAlignment = HorizontalAlignment.Stretch;
        type.Click += OpenBAT;

        if (branchRows[i].HasExe)
        {
          type.IsEnabled = true;
        }
        else
        {
          type.IsEnabled = false;
        }

        if (branchRows[i].BranchType == BranchType.Debug)
        {
          type.Background = new SolidColorBrush(Colors.OrangeRed);
        }
        else if (branchRows[i].BranchType == BranchType.Release)
        {
          type.Background = new SolidColorBrush(Colors.DarkBlue);
        }
        else
        {
          type.Background = new SolidColorBrush(Colors.DarkGreen);
        }

        Grid.SetColumn(type, 3);
        Grid.SetRow(type, i);
        grid.Children.Add(type);

        if (branchRows[i].HasReferential)
        {
          Button referential = new Button();
          referential.Content = "Referential";
          referential.Click += OpenReferential;
          referential.Background = new SolidColorBrush(Colors.LightGray);
          referential.Foreground = new SolidColorBrush(Colors.Black);
          Grid.SetColumn(referential, 4);
          Grid.SetRow(referential, i);
          grid.Children.Add(referential);
        }

        if (branchRows[i].Databases.Count > 0)
        {
          //Combo
          ComboBox databasesCombo = new ComboBox();
          databasesCombo.ItemsSource = branchRows[i].Databases.ConvertAll(x => x = Path.GetFileName(x));
          databasesCombo.SelectedIndex = 0;
          databasesCombo.SelectionChanged += SelectedDatabaseChanged;

          Grid.SetColumn(databasesCombo, 5);
          Grid.SetRow(databasesCombo, i);
          grid.Children.Add(databasesCombo);
        }
      }

      grid.RowSpacing = 30;
      grid.ColumnSpacing = 30;
      grid.HorizontalAlignment = HorizontalAlignment.Center;
      panel.Children.Add(grid);
    }

    private void SelectedDatabaseChanged(object sender, SelectionChangedEventArgs e)
    {
      int rowIndex = Grid.GetRow(sender as ComboBox);
      _branchRows[rowIndex].DatabaseSelectedIndex = (sender as ComboBox).SelectedIndex;
    }

    private void OpenBAT(object sender, RoutedEventArgs e)
    {
      int rowIndex = Grid.GetRow(sender as Button);
      string filePath = _branchRows[rowIndex].InitPath;

      if (!File.Exists(filePath))
      {
        using (StreamWriter writetext = new StreamWriter(filePath))
        {
          writetext.WriteLine(_branchRows[rowIndex].GetIniFileContent());
        }

      }

      string[] lines = File.ReadAllLines(filePath);
      string databaseString = $"BAT-EMC3={_branchRows[rowIndex].Databases[_branchRows[rowIndex].DatabaseSelectedIndex]}";
      //si la base de donnée n'est pas celle choisie par l'utilsateur, on configure le .ini
      if (!lines[1].Contains(databaseString))
      {
        // Parcours des lignes et ajout des commentaires entre les balises
        for (int i = 1; i < lines.Length; i++)
        {
          if (lines[i] == "[Directories]")
          {
            break;
          }

          if (lines[i].Trim().First() != ';')
          {
            lines[i] = ";" + lines[i];
          }
        }

        List<string> linesList = new List<string>(lines);
        linesList.Insert(1, databaseString);
        lines = linesList.ToArray();
        File.WriteAllLines(filePath, lines);
      }
      Process.Start(_branchRows[rowIndex].ExePath);
    }

    private void OpenReferential(object sender, RoutedEventArgs e)
    {
      int rowIndex = Grid.GetRow(sender as Button);
      Process.Start("explorer.exe", _branchRows[rowIndex].ReferentialPath);
    }

    private async void OpenSolution(object sender, RoutedEventArgs e)
    {
      int rowIndex = Grid.GetRow(sender as Button);
      string solutionPath = _branchRows[rowIndex].SolutionPath;
      string devenvPath = GetVisualStudioPath();

      // Create a new ProcessStartInfo object
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
        FileName = devenvPath,
        Arguments = $"\"{solutionPath}\"",
        Verb = "runas" // Run the process as administrator
      };

      try
      {
        Process.Start(startInfo);
      }
      catch (Exception ex)
      {
        var cd = new ContentDialog
        {
          Title = "Error",
          Content = "This action requires administrator role.",
          CloseButtonText = "OK"
        };

        cd.XamlRoot = this.Content.XamlRoot;
        var result = await cd.ShowAsync();
      }
    }

    private string[] LoadBranches(string localValue)
    {
      // Recuperation de la liste de toutes les branches de dev
      this.devPath.Text = localValue;
      var maint = Directory.GetDirectories(localValue, "*MAINT");
      var trunk = Directory.GetDirectories(localValue, "*trunk");
      var branchesPaths = new string[maint.Length + trunk.Length];
      maint.CopyTo(branchesPaths, 0);
      trunk.CopyTo(branchesPaths, maint.Length);
      return branchesPaths;
    }

    private string[] LoadVersions(string localValue)
    {
      // Recuperation de la liste de toutes les branches de dev
      this.installPath.Text = localValue;
      var versionsPaths = Directory.GetDirectories(localValue, "BAT-EMC*");
      return versionsPaths;
    }

    private string GetDevPath()
    {
      ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
      String localValue = localSettings.Values["devPath"] as string;
      return localValue;
    }
    private string GetInstallPath()
    {
      ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
      String localValue = localSettings.Values["installPath"] as string;
      return localValue;
    }

    private void CreateBranchRows(List<string> branchesPaths)
    {
      foreach (var branchPath in branchesPaths) // Boucle sur toutes les branches de dev
      {
        foreach (BranchType branchType in Enum.GetValues(typeof(BranchType))) // Boucle sur bin/binDebug
        {
          if (branchType == BranchType.Version) // On gèrera plus tard les versions
          {
            continue;
          }

          string binPath = String.Empty;
          bool hasExe = false;
          string exePath = String.Empty;
          string exeName = String.Empty;
          string exeVersion = String.Empty;
          bool hasReferential = false;
          string referentialPath = String.Empty;
          bool hasSolution = false;
          string solutionPath = String.Empty;
          List<string> databases = new List<string>();

          if (File.Exists($"{branchPath}\\BAT_EMC.sln"))
          {
            hasSolution = true;
            solutionPath = $"{branchPath}\\BAT_EMC.sln";
          }

          if (Directory.Exists($"{branchPath}\\{BranchHelper.GetBinFolder(branchType)}")) // Si le dossier binPath existe
          {
            binPath = $"{branchPath}\\{BranchHelper.GetBinFolder(branchType)}";

            if (Directory.Exists($"{binPath}\\Referential")) // On cherche le dossier référentiel
            {
              hasReferential = true;
              referentialPath = $"{binPath}\\Referential";
            }

            if (Directory.Exists($"{binPath}\\Database")) // On cherche le dossier database
            {
              databases = Directory.GetFiles($"{binPath}\\Database", "*.accdb").ToList();
            }

            if (File.Exists($"{binPath}\\BAT_EMC3.exe"))
            {
              hasExe = true;
              exePath = $"{binPath}\\BAT_EMC3.exe";
              exeVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
            }
          }

          _branchRows.Add(new BranchRow(branchPath, Path.GetFileName(branchPath), branchType, binPath, hasExe, exeVersion, exePath, hasReferential, referentialPath, hasSolution, solutionPath, databases));
        }
      }
    }

    private void CreateVersionRows(List<string> versionsPaths)
    {
      foreach (var versionPath in versionsPaths) // Boucle sur toutes les branches de dev
      {
        string binPath = String.Empty;
        bool hasExe = false;
        string exePath = String.Empty;
        string exeName = String.Empty;
        string exeVersion = String.Empty;
        bool hasReferential = false;
        string referentialPath = String.Empty;
        bool hasSolution = false;
        string solutionPath = String.Empty;
        List<string> databases = new List<string>();

        if (Directory.Exists($"{versionPath}\\Referential")) // On cherche le dossier référentiel
        {
          hasReferential = true;
          referentialPath = $"{versionPath}\\Referential";
        }

        if (Directory.Exists($"{versionPath}\\Database")) // On cherche le dossier database
        {
          databases = Directory.GetFiles($"{versionPath}\\Database", "*.accdb").ToList();
        }

        if (File.Exists($"{versionPath}\\BAT_EMC3.exe"))
        {
          hasExe = true;
          exePath = $"{versionPath}\\BAT_EMC3.exe";
          exeVersion = FileVersionInfo.GetVersionInfo(exePath).FileVersion;
        }

        _branchRows.Add(new BranchRow(versionPath, Path.GetFileName(versionPath), BranchType.Version, binPath, hasExe, exeVersion, exePath, hasReferential, referentialPath, hasSolution, solutionPath, databases));
      }

    }
    private async void Button_Click1(object sender, RoutedEventArgs e)
    {
      try
      {
        var window = new Microsoft.UI.Xaml.Window();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var folderPicker = new Windows.Storage.Pickers.FolderPicker();
        folderPicker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder != null)
        {
          Windows.Storage.AccessCache.StorageApplicationPermissions.
          FutureAccessList.AddOrReplace("PickedFolderToken", folder);
          this.devPath.Text = folder.Path;
          ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
          localSettings.Values["devPath"] = folder.Path;

          AppRestartFailureReason restartError = AppInstance.Restart(null);
        }
        else
        {
          this.devPath.Text = "Operation cancelled";
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private async void Button_Click2(object sender, RoutedEventArgs e)
    {
      try
      {
        var window = new Microsoft.UI.Xaml.Window();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var folderPicker = new Windows.Storage.Pickers.FolderPicker();
        folderPicker.FileTypeFilter.Add("*");
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        var folder = await folderPicker.PickSingleFolderAsync();
        if (folder != null)
        {
          Windows.Storage.AccessCache.StorageApplicationPermissions.
          FutureAccessList.AddOrReplace("PickedFolderToken", folder);
          this.installPath.Text = folder.Path;
          ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
          localSettings.Values["installPath"] = folder.Path;

          AppRestartFailureReason restartError = AppInstance.Restart(null);
        }
        else
        {
          this.installPath.Text = "Operation cancelled";
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    static string GetVisualStudioPath()
    {
      return @"C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe";
    }
  }
}
