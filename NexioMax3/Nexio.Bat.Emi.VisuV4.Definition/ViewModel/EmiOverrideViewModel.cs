namespace NexioMax3.Definition.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Drawing;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Forms;
  using Nexio.Bat.Common.Domain.ATDB.Service;
  using Nexio.Bat.Common.Domain.Infrastructure.Service;
  using NexioMax3.Domain.Configuration.Model;
  using NexioMax3.Domain.Configuration.Model.EMIOverrides2;
  using NexioMax3.Domain.Model;
  using NexioMax3.Domain.Service;
  using Nexio.Helper;
  using Nexio.Tools;
  using Nexio.Wpf.Base;
  using Nexio.Wpf.Command;
  using Application = System.Windows.Application;
  using MessageBox = System.Windows.MessageBox;
  using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
  using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

  public class EmiOverrideViewModel : ViewModelBase
  {
    private RelayObjectCommand<PointOverrideViewModel> removePointCommand;

    private RelayObjectCommand<CurveOverrideViewModel> removeCurveCommand;

    private RelayObjectCommand<ExternalCurveOverrideViewModel> removeExternalCurveCommand;

    private RelayCommand importCommand;

    private RelayCommand exportCommand;

    private RelayCommand addCommand;

    private RelayCommand saveCommand;

    private RelayCommand cancelCommand;

    private RelayCommand orderByDetectorCommand;

    private RelayCommand orderCurvesByPositionCommand;

    private RelayCommand orderBySourceCommand;

    private RelayCommand orderPointsByPositionCommand;

    private RelayCommand orderByColumnCommand;

    private RelayCommand orderByCurveNameCommand;

    private RelayCommand orderByCurveProjectCommand;

    private RelayCommand orderExternalCurvesByPositionCommand;

    private EmiOverrideGroup selectedGroup;

    private OrderBy orderByDetector;

    private OrderBy orderByPositionCurves;

    private OrderBy orderByPositionPoints;

    private OrderBy orderBySource;

    private OrderBy orderByColumn;

    private OrderBy orderByCurveName;

    private OrderBy orderByProject;

    private OrderBy orderByPositionExternalCurve;

    private string invalidExportMessage;

    private bool areAllStylesValid;

    private RelayCommand goToHelpCommand;

    // Permet de stocker les infos des sources ainsi que les colonnes qui leurs sont liées
    private Dictionary<string, DatasCache> sourceDic = new Dictionary<string, DatasCache>();

    private RelayCommand clearCacheCommand;

    public EmiOverrideViewModel()
    {
      using (new NexioStopwatch($"{nameof(EmiOverrideViewModel)}.ctor"))
      {
        this.Positions.Add(NexioMax3.Definition.Properties.Resources.All);
        //Provider.Instance.GetAllPositions().Select(position => position.Name).Distinct().Where(s => !string.IsNullOrWhiteSpace(s)).OrderBy(s => s).ToList().ForEach(s => this.Positions.Add(s));

        this.sourceDic = SourcesCache.GetSources();

        foreach (var source in this.sourceDic)
        {
          this.Sources.Add(source.Key);
        }

        var listProject = ATDBProvider.Instance.GetListProjects().ToList();

        this.Projects.Add(ExternalCurveOverrideViewModel.AllProject);
        this.Projects.Add(ExternalCurveOverrideViewModel.OldProject);
        this.Projects.Add(ExternalCurveOverrideViewModel.ImportedFromFile);

        foreach (var atdbRoot in listProject)
        {
          this.Projects.Add(atdbRoot.Name);
        }

        this.SaveDirectory = OptionProvider.GetOptionPath(OptionProvider.Options.EmiOverrideDirectory);
        string jsonFilePath = null;
        if (string.IsNullOrWhiteSpace(this.SaveDirectory))
        {
          this.SaveDirectory = ExecPathHelper.GetExecDirectory();
          jsonFilePath = Path.Combine(this.SaveDirectory, EmiOverrideData.OverrideFile);
        }
        else
        {
          if (!Directory.Exists(this.SaveDirectory))
          {
            MessageBox.Show(NexioMax3.Definition.Properties.Resources.UnableToLocateEMIOverrideSFileSDirectory);
          }
          else
          {
            jsonFilePath = Path.Combine(this.SaveDirectory, EmiOverrideData.OverrideFile);
          }
        }

        if (File.Exists(jsonFilePath))
        {
          this.LoadFromJson(jsonFilePath);
        }

        UpdateValidities(this.Prescans);
        UpdateValidities(this.Limits);
        UpdateValidities(this.Finals);
        UpdateValidities(this.Suspects);
        UpdateValidities(this.Externals);
        this.AreAllStylesValid = !this.HasInvalidStyle();

        this.PrescansView = CollectionViewSource.GetDefaultView(this.Prescans);
        this.LimitsView = CollectionViewSource.GetDefaultView(this.Limits);
        this.FinalsView = CollectionViewSource.GetDefaultView(this.Finals);
        this.SuspectsView = CollectionViewSource.GetDefaultView(this.Suspects);
        this.ExternalsView = CollectionViewSource.GetDefaultView(this.Externals);
      }
    }

    public event RoutedEventHandler CloseRequested;

    public string SaveDirectory { get; }

    public ICollectionView SuspectsView { get; }

    public ICollectionView FinalsView { get; }

    public ICollectionView LimitsView { get; }

    public ICollectionView PrescansView { get; }

    public ICollectionView ExternalsView { get; }

    public ObservableCollection<CurveOverrideViewModel> Prescans { get; } = new ObservableCollection<CurveOverrideViewModel>();

    public ObservableCollection<CurveOverrideViewModel> Limits { get; } = new ObservableCollection<CurveOverrideViewModel>();

    public ObservableCollection<ExternalCurveOverrideViewModel> Externals { get; } = new ObservableCollection<ExternalCurveOverrideViewModel>();

    public ObservableCollection<PointOverrideViewModel> Suspects { get; } = new ObservableCollection<PointOverrideViewModel>();

    public ObservableCollection<PointOverrideViewModel> Finals { get; } = new ObservableCollection<PointOverrideViewModel>();

    public EmiOverrideGroup SelectedGroup
    {
      get => this.selectedGroup;
      set => this.Set(nameof(this.SelectedGroup), ref this.selectedGroup, value);
    }

    public OrderBy OrderByDetector
    {
      get => this.orderByDetector;
      set => this.Set(nameof(this.OrderByDetector), ref this.orderByDetector, value);
    }

    public OrderBy OrderByPositionCurves
    {
      get => this.orderByPositionCurves;
      set => this.Set(nameof(this.OrderByPositionCurves), ref this.orderByPositionCurves, value);
    }

    public OrderBy OrderByPositionPoints
    {
      get => this.orderByPositionPoints;
      set => this.Set(nameof(this.OrderByPositionPoints), ref this.orderByPositionPoints, value);
    }

    public OrderBy OrderBySource
    {
      get => this.orderBySource;
      set => this.Set(nameof(this.OrderBySource), ref this.orderBySource, value);
    }

    public OrderBy OrderByColumn
    {
      get => this.orderByColumn;
      set => this.Set(nameof(this.OrderByColumn), ref this.orderByColumn, value);
    }

    public OrderBy OrderByCurveName
    {
      get => this.orderByCurveName;
      set => this.Set(nameof(this.OrderByCurveName), ref this.orderByCurveName, value);
    }

    public OrderBy OrderByProject
    {
      get => this.orderByProject;
      set => this.Set(nameof(this.OrderByProject), ref this.orderByProject, value);
    }

    public OrderBy OrderByPositionExternalCurve
    {
      get => this.orderByPositionExternalCurve;
      set => this.Set(nameof(this.OrderByPositionExternalCurve), ref this.orderByPositionExternalCurve, value);
    }

    public ObservableCollection<string> Positions { get; } = new ObservableCollection<string>();

    public ObservableCollection<string> Sources { get; } = new ObservableCollection<string>();

    public ObservableCollection<string> Columns { get; } = new ObservableCollection<string>();

    public ObservableCollection<string> Projects { get; } = new ObservableCollection<string>();

    public ObservableCollection<double> Sizes { get; } = new ObservableCollection<double>() { 1, 1.5, 2, 2.5, 3, 4, 5 };

    public ObservableCollection<double> PointSizes { get; } = new ObservableCollection<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public List<Symbol> AvailableSymbols { get; } = new List<Symbol>(new[]
                                                                     {
                                                                       new Symbol() { Data = "M0,1A1,1,180,1,1,2,1A1,1,180,1,1,0,1", Name = "Circle", Id = 4 },
                                                                       new Symbol() { Data = "M0,0L1,0L1,1L0,1Z", Name = "Square", Id = 5 } ,
                                                                       new Symbol() { Data = "M1,0L2,1L1,2L0,1Z", Name = "Diamond", Id = 1, } ,
                                                                       new Symbol() { Data = "M0,2L1,0L2,2Z", Name = "Triangle", Id = 10 , } ,
                                                                       new Symbol() { Data = "M0,0L1,1M1,0L0,1", Name = "Cross", Id = 8, } ,
                                                                       new Symbol() { Data = "M1,0L1,2M0,1L2,1", Name = "Plus", Id = 3, } ,
                                                                       new Symbol() { Data = "M0,1A1,1,180,1,1,2,1A1,1,180,1,1,0,1", Name = "CircleFilled", Id = 6 },
                                                                       new Symbol() { Data = "M0,0L1,0L1,1L0,1Z", Name = "SquareFilled", Id = 7 },
                                                                       new Symbol() { Data = "M1,0L2,1L1,2L0,1Z", Name = "DiamondFilled", Id = 2 },
                                                                       new Symbol() { Data = "M0,2L1,0L2,2Z", Name = "TriangleFilled", Id = 15 },
                                                                     });

    public RelayObjectCommand<PointOverrideViewModel> RemovePointCommand => this.removePointCommand ?? (this.removePointCommand = new RelayObjectCommand<PointOverrideViewModel>(this.RemovePoint));

    public RelayObjectCommand<CurveOverrideViewModel> RemoveCurveCommand => this.removeCurveCommand ?? (this.removeCurveCommand = new RelayObjectCommand<CurveOverrideViewModel>(this.RemoveCurve));

    public RelayObjectCommand<ExternalCurveOverrideViewModel> RemoveExternalCurveCommand => this.removeExternalCurveCommand ?? (this.removeExternalCurveCommand = new RelayObjectCommand<ExternalCurveOverrideViewModel>(this.RemoveExternalCurve));

    public RelayCommand AddCommand => this.addCommand ?? (this.addCommand = new RelayCommand(this.Add));

    public RelayCommand ImportCommand => this.importCommand ?? (this.importCommand = new RelayCommand(this.Import));

    public RelayCommand ExportCommand => this.exportCommand ?? (this.exportCommand = new RelayCommand(this.Export));

    public RelayCommand SaveCommand => this.saveCommand ?? (this.saveCommand = new RelayCommand(this.Save));

    public RelayCommand CancelCommand => this.cancelCommand ?? (this.cancelCommand = new RelayCommand(this.Cancel));

    public RelayCommand OrderByDetectorCommand => this.orderByDetectorCommand ?? (this.orderByDetectorCommand = new RelayCommand(this.OrderByDetectorAction));

    public RelayCommand OrderCurvesByPositionCommand => this.orderCurvesByPositionCommand ?? (this.orderCurvesByPositionCommand = new RelayCommand(this.OrderCurvesByPosition));

    public RelayCommand OrderBySourceCommand => this.orderBySourceCommand ?? (this.orderBySourceCommand = new RelayCommand(this.OrderBySourceAction));

    public RelayCommand OrderByColumnCommand => this.orderByColumnCommand ?? (this.orderByColumnCommand = new RelayCommand(this.OrderByColumnAction));

    public RelayCommand OrderPointsByPositionCommand => this.orderPointsByPositionCommand ?? (this.orderPointsByPositionCommand = new RelayCommand(this.OrderPointsByPosition));

    public RelayCommand OrderByCurveNameCommand => this.orderByCurveNameCommand ?? (this.orderByCurveNameCommand = new RelayCommand(this.OrderExternalCurvesByName));

    public RelayCommand OrderByCurveProjectCommand => this.orderByCurveProjectCommand ?? (this.orderByCurveProjectCommand = new RelayCommand(this.OrderByCurveProject));

    public RelayCommand OrderExternalCurvesByPositionCommand => this.orderExternalCurvesByPositionCommand ?? (this.orderExternalCurvesByPositionCommand = new RelayCommand(this.OrderExternalCurvesByPosition));

    public RelayCommand GoToHelpCommand => this.goToHelpCommand ?? (this.goToHelpCommand = new RelayCommand(this.GoToHelp));

    public RelayCommand ClearCacheCommand => this.clearCacheCommand ?? (this.clearCacheCommand = new RelayCommand(this.ClearCache));

    public string InvalidExportMessage
    {
      get => this.invalidExportMessage;
      set => this.Set(nameof(this.InvalidExportMessage), ref this.invalidExportMessage, value);
    }

    public bool AreAllStylesValid
    {
      get => this.areAllStylesValid;
      set => this.Set(nameof(this.AreAllStylesValid), ref this.areAllStylesValid, value);
    }

    public static OrderBy GetNext(OrderBy orderBy)
    {
      switch (orderBy)
      {
        case OrderBy.ASC:
          return OrderBy.DESC;

        case OrderBy.NONE:
        case OrderBy.DESC:
          return OrderBy.ASC;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static void Notify(string title, string message)
    {
      using (var notifyIcon = new NotifyIcon())
      {
        notifyIcon.Visible = true;
        notifyIcon.Icon = SystemIcons.Application;
        if (title != null)
        {
          notifyIcon.BalloonTipTitle = title;
        }

        if (message != null)
        {
          notifyIcon.BalloonTipText = message;
        }

        notifyIcon.ShowBalloonTip(5 * 1000);
      }
    }

    private static Detector GetAssumedDetector(string name)
    {
      var split = name.Split('/');

      foreach (var s in split)
      {
        var lowered = s.ToLower();
        if (/*lowered.Contains("qpeak") || lowered.Contains("q-peak") ||*/ lowered.Contains("quasi") || lowered.Contains("qp") || lowered.Contains("q-p"))
        {
          return Detector.QPEAK;
        }

        if (lowered.Contains("peak") || lowered.Contains("pk"))
        {
          return Detector.PEAK;
        }

        if (lowered.Contains("average") || lowered.Contains("avg"))
        {
          if (lowered.Contains("cispr") || lowered.Contains("cavg") || lowered.Contains("c_avg"))
          {
            return Detector.CISPR_AVERAGE;
          }

          return Detector.AVERAGE;
        }

        if (lowered.Contains("rms"))
        {
          if (lowered.Contains("cispr") || lowered.Contains("crms"))
          {
            return Detector.CISPR_RMS;
          }

          return Detector.RMS;
        }
      }

      return Detector.AVERAGE;
    }

    private static void UpdateValidities(ICollection<CurveOverrideViewModel> curveOverrideViewModels)
    {
      foreach (var curveOverrideViewModel in curveOverrideViewModels)
      {
        curveOverrideViewModel.UpdateValidity(curveOverrideViewModels);
      }
    }

    private static void UpdateValidities(ICollection<PointOverrideViewModel> pointOverrideViewModels)
    {
      foreach (var curveOverrideViewModel in pointOverrideViewModels)
      {
        curveOverrideViewModel.UpdateValidity(pointOverrideViewModels);
      }
    }

    private static void UpdateValidities(ICollection<ExternalCurveOverrideViewModel> externalCurveOverrideViewModels)
    {
      foreach (var curveOverrideViewModel in externalCurveOverrideViewModels)
      {
        curveOverrideViewModel.UpdateValidity(externalCurveOverrideViewModels);
      }
    }

    private static void LogInteraction(string message)
    {
      (Application.Current.Properties["InteractionLogger"] as UserInteractionLogger)?.Log(message);
    }

    private void ClearCache()
    {
      if (MessageBox.Show(NexioMax3.Definition.Properties.Resources.AreYouSureToDeleteTheCache, NexioMax3.Definition.Properties.Resources.DeleteCache, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        SourcesCache.Clear();
        this.Save();
      }
    }

    private void Add()
    {
      switch (this.SelectedGroup)
      {
        case EmiOverrideGroup.Prescans:
          this.AddPrescan();
          break;

        case EmiOverrideGroup.Limits:
          this.AddLimit();
          break;

        case EmiOverrideGroup.Suspects:
          this.AddSuspect();
          break;

        case EmiOverrideGroup.Finals:
          this.AddFinal();
          break;

        case EmiOverrideGroup.Externals:
          this.AddExternal();
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void AddLimit()
    {
      var limit = new CurveOverrideViewModel() { IsLimit = true, Position = null };
      limit.PropertyChanged += this.LimitPropertyChanged;
      this.Limits.Add(limit);
      UpdateValidities(this.Limits);
    }

    private void AddPrescan()
    {
      var prescan = new CurveOverrideViewModel();
      prescan.PropertyChanged += this.PrescanPropertyChanged;
      this.Prescans.Add(prescan);
      UpdateValidities(this.Prescans);
    }

    private void AddFinal()
    {
      var final = new PointOverrideViewModel();
      // On ajoute le dictionnaire des sources à la création d'un nouveau finaux dans emi override pour qu'il est les bonnes infos dès le début
      final.SourceDic = this.sourceDic;
      final.SourceName = this.sourceDic.FirstOrDefault().Key;
      final.PropertyChanged += this.FinalPropertyChanged;
      this.Finals.Add(final);
      UpdateValidities(this.Finals);
    }

    private void AddSuspect()
    {
      var suspect = new PointOverrideViewModel();
      // On ajoute le dictionnaire des sources à la création d'un nouveau suspect dans emi override pour qu'il est les bonnes infos dès le début
      suspect.SourceDic = this.sourceDic;
      suspect.SourceName = this.sourceDic.FirstOrDefault().Key;
      suspect.PropertyChanged += this.SuspectPropertyChanged;
      this.Suspects.Add(suspect);
      UpdateValidities(this.Suspects);
    }

    private void AddExternal()
    {
      var suspect = new ExternalCurveOverrideViewModel();
      suspect.PropertyChanged += this.ExternalPropertyChanged;
      this.Externals.Add(suspect);
      UpdateValidities(this.Externals);
    }

    private void Export()
    {
      if (this.HasInvalidStyle())
      {
        return;
      }

      var sfd = new SaveFileDialog()
      {
        Filter = @"Json File (*.json)|*.json;|All Files (*.*)|*.*",
        AddExtension = true,
      };

      if (sfd.ShowDialog() != true || string.IsNullOrWhiteSpace(sfd.FileName))
      {
        return;
      }

      var filePath = sfd.FileName;

      LogInteraction("Exporting EmiOverride style");
      this.SaveDatas(filePath);

      Notify(NexioMax3.Definition.Properties.Resources.ExportComplete, NexioMax3.Definition.Properties.Resources.StylesExportComplete);
    }

    private bool HasInvalidStyle()
    {
      this.InvalidExportMessage = null;

      var hasSuspectsErrors = this.Suspects.Any(model => !model.IsValid);
      var hasFinalsErrors = this.Finals.Any(model => !model.IsValid);
      var hasPrescansErrors = this.Prescans.Any(model => !model.IsValid);
      var hasLimitsErrors = this.Limits.Any(model => !model.IsValid);
      var hasExternalsErrors = this.Externals.Any(model => !model.IsValid);

      if (hasLimitsErrors || hasFinalsErrors || hasPrescansErrors || hasSuspectsErrors || hasExternalsErrors)
      {
        var lst = new List<string>();

        if (hasLimitsErrors)
        {
          lst.Add(nameof(this.Limits));
        }

        if (hasPrescansErrors)
        {
          lst.Add(nameof(this.Prescans));
        }

        if (hasSuspectsErrors)
        {
          lst.Add(nameof(this.Suspects));
        }

        if (hasFinalsErrors)
        {
          lst.Add(nameof(this.Finals));
        }

        if (hasExternalsErrors)
        {
          lst.Add(nameof(this.Externals));
        }

        this.InvalidExportMessage = $"You have incompatible styles in : {string.Join(", ", lst)}";

        return true;
      }

      return false;
    }

    private void OrderPointsByPosition()
    {
      this.OrderBySource = OrderBy.NONE;
      this.OrderByColumn = OrderBy.NONE;
      this.OrderByPositionPoints = GetNext(this.OrderByPositionPoints);

      this.RefreshPoints();
    }

    private void OrderBySourceAction()
    {
      this.OrderByPositionPoints = OrderBy.NONE;
      this.OrderByColumn = OrderBy.NONE;
      this.OrderBySource = GetNext(this.OrderBySource);

      this.RefreshPoints();
    }

    private void OrderByColumnAction()
    {
      this.OrderByPositionPoints = OrderBy.NONE;
      this.OrderBySource = OrderBy.NONE;
      this.OrderByColumn = GetNext(this.OrderByColumn);

      this.RefreshPoints();
    }

    private void OrderCurvesByPosition()
    {
      this.OrderByDetector = OrderBy.NONE;
      this.OrderByPositionCurves = GetNext(this.OrderByPositionCurves);

      this.RefreshCurves();
    }

    private void OrderByDetectorAction()
    {
      this.OrderByPositionCurves = OrderBy.NONE;

      this.OrderByDetector = GetNext(this.OrderByDetector);

      this.RefreshCurves();
    }

    private void OrderByCurveProject()
    {
      this.OrderByCurveName = OrderBy.NONE;
      this.OrderByPositionExternalCurve = OrderBy.NONE;
      this.OrderByProject = GetNext(this.OrderByProject);

      this.RefreshExternalCurves();
    }

    private void OrderExternalCurvesByName()
    {
      this.OrderByProject = OrderBy.NONE;
      this.OrderByPositionExternalCurve = OrderBy.NONE;
      this.OrderByCurveName = GetNext(this.OrderByCurveName);

      this.RefreshExternalCurves();
    }

    private void OrderExternalCurvesByPosition()
    {
      this.OrderByCurveName = OrderBy.NONE;
      this.OrderByProject = OrderBy.NONE;
      this.OrderByPositionExternalCurve = GetNext(this.OrderByPositionExternalCurve);

      this.RefreshExternalCurves();
    }

    private void RefreshCurves()
    {
      this.PrescansView.SortDescriptions.Clear();
      this.LimitsView.SortDescriptions.Clear();
      var orderByDirection = this.OrderByDetector != OrderBy.NONE ? this.OrderByDetector : this.OrderByPositionCurves;
      var sortDescription = new SortDescription(this.OrderByDetector != OrderBy.NONE ? nameof(CurveOverrideViewModel.Detector) : nameof(CurveOverrideViewModel.Position),
                                                orderByDirection == OrderBy.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending);
      this.PrescansView.SortDescriptions.Add(sortDescription);
      this.LimitsView.SortDescriptions.Add(sortDescription);
    }

    private void RefreshPoints()
    {
      this.SuspectsView.SortDescriptions.Clear();
      this.FinalsView.SortDescriptions.Clear();
      var orderByDirection = this.OrderBySource != OrderBy.NONE ? this.OrderBySource : this.OrderByPositionPoints;
      var sortDescription = new SortDescription(this.OrderBySource != OrderBy.NONE ? nameof(PointOverrideViewModel.SourceName) : nameof(PointOverrideViewModel.Position),
                                                orderByDirection == OrderBy.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending);
      this.SuspectsView.SortDescriptions.Add(sortDescription);
      this.FinalsView.SortDescriptions.Add(sortDescription);
    }

    private void RefreshExternalCurves()
    {
      this.ExternalsView.SortDescriptions.Clear();
      var orderByDirection = this.OrderByCurveName != OrderBy.NONE ? this.OrderByCurveName : this.OrderByProject;
      var sortDescription = new SortDescription(this.OrderByCurveName != OrderBy.NONE ? nameof(ExternalCurveOverrideViewModel.Name) : nameof(ExternalCurveOverrideViewModel.Project),
                                                orderByDirection == OrderBy.ASC ? ListSortDirection.Ascending : ListSortDirection.Descending);
      this.ExternalsView.SortDescriptions.Add(sortDescription);
    }

    private void SaveDatas(string filePath)
    {
      var datas = new EmiOverrideData()
      {
        Suspects = this.Suspects.Select(model => model.ToData()).ToList(),
        Finals = this.Finals.Select(model => model.ToData()).ToList(),
        Prescans = this.Prescans.Select(model => model.ToData()).ToList(),
        Limits = this.Limits.Select(model => model.ToData()).ToList(),
        Externals = this.Externals.Select(model => model.ToData()).ToList(),
      };

      datas.Save(filePath);
    }

    private void Import()
    {
      var dialog = new OpenFileDialog()
      {
        Filter = @"Json File (*.json)|*.json;|Old Configuration File (*.ini)|*.ini;|All Files (*.*)|*.*",
        AddExtension = true,
        CheckFileExists = true,
        Title = NexioMax3.Definition.Properties.Resources.SelectEMIOverrideConfigurationToImport,
      };

      var res = dialog.ShowDialog();

      if (res != true)
      {
        return;
      }

      if (string.IsNullOrWhiteSpace(dialog.FileName) || !File.Exists(dialog.FileName))
      {
        return;
      }

      var filePath = dialog.FileName;
      switch (Path.GetExtension(filePath))
      {
        case ".json":
          {
            this.LoadFromJson(filePath);
            break;
          }

        case ".ini":
          {
            this.LoadFromIni(filePath);

            break;
          }
      }

      UpdateValidities(this.Prescans);
      UpdateValidities(this.Limits);
      UpdateValidities(this.Finals);
      UpdateValidities(this.Suspects);
      UpdateValidities(this.Externals);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void LoadFromIni(string filePath)
    {
      EMIOverrides.Instance.SetCustomFile(filePath);

      try
      {
        this.Prescans.Clear();

        foreach (var overrideCurveDef in EMIOverrides.Instance.OverridePrescan)
        {
          var detector = overrideCurveDef.Key;
          var def = overrideCurveDef.Value;
          var curve = CurveOverrideViewModel.FromCurveDefinition(detector, def);
          curve.PropertyChanged += this.PrescanPropertyChanged;
          this.Prescans.Add(curve);
        }

        this.Limits.Clear();

        foreach (var overrideCurveDef in EMIOverrides.Instance.OverrideLimit)
        {
          var detector = overrideCurveDef.Key;
          var def = overrideCurveDef.Value;
          var curve = CurveOverrideViewModel.FromCurveDefinition(detector, def);
          curve.IsLimit = true;
          curve.Position = null;
          curve.PropertyChanged += this.LimitPropertyChanged;
          this.Limits.Add(curve);
        }

        foreach (var overrideCurveDef in EMIOverrides.Instance.OverrideOther)
        {
          var detector = GetAssumedDetector(overrideCurveDef.Key);
          var source = overrideCurveDef.Key.Replace("()", string.Empty);
          var position = this.GetPosition(overrideCurveDef.Key);

          source = source.Replace($"({position})", string.Empty).Trim();

          while (source.Contains("  "))
          {
            source = source.Replace("  ", " ");
          }

          var iniSuspect =
            PointOverrideViewModel.FromPointDefinition(detector, position, source, overrideCurveDef.Value);
          var iniFinal = PointOverrideViewModel.FromPointDefinition(detector, position, source, overrideCurveDef.Value);

          if (this.AvailableSymbols.All(symbol => symbol.Id != iniSuspect.SymbolId))
          {
            iniSuspect.SymbolId = this.AvailableSymbols.Min(symbol => symbol.Id);
          }

          if (this.AvailableSymbols.All(symbol => symbol.Id != iniFinal.SymbolId))
          {
            iniFinal.SymbolId = this.AvailableSymbols.Min(symbol => symbol.Id);
          }

          iniSuspect.PropertyChanged += this.SuspectPropertyChanged;
          iniFinal.PropertyChanged += this.FinalPropertyChanged;

          this.Suspects.Add(iniSuspect);
          this.Finals.Add(iniFinal);
        }
      }
      finally
      {
        EMIOverrides.Instance.ResetCustomFile();
      }
    }

    private string GetPosition(string fullName)
    {
      foreach (var position in this.Positions)
      {
        if (fullName.ToLower().Contains(position.ToLower()))
        {
          return position;
        }
      }

      return "All";
    }

    private void LoadFromJson(string filePath)
    {
      var data = EmiOverrideData.Load(filePath);

      this.Suspects.Clear();

      foreach (var emiOverridePoint in data.Suspects)
      {
        var point = PointOverrideViewModel.FromData(emiOverridePoint, this.sourceDic, this.Positions);

        if (this.AvailableSymbols.All(symbol => symbol.Id != point.SymbolId))
        {
          point.SymbolId = this.AvailableSymbols.Min(symbol => symbol.Id);
        }

        point.PropertyChanged += this.SuspectPropertyChanged;
        this.Suspects.Add(point);
      }

      this.Finals.Clear();

      foreach (var emiOverridePoint in data.Finals)
      {
        var point = PointOverrideViewModel.FromData(emiOverridePoint, this.sourceDic, this.Positions);
        if (this.AvailableSymbols.All(symbol => symbol.Id != point.SymbolId))
        {
          point.SymbolId = this.AvailableSymbols.Min(symbol => symbol.Id);
        }

        point.PropertyChanged += this.FinalPropertyChanged;
        this.Finals.Add(point);
      }

      this.Prescans.Clear();

      foreach (var emiOverrideCurve in data.Prescans)
      {
        var curve = CurveOverrideViewModel.FromData(emiOverrideCurve, this.Positions);
        curve.PropertyChanged += this.PrescanPropertyChanged;
        this.Prescans.Add(curve);
      }

      this.Limits.Clear();

      foreach (var emiOverrideCurve in data.Limits)
      {
        var curve = CurveOverrideViewModel.FromData(emiOverrideCurve, this.Positions);
        curve.IsLimit = true;
        curve.Position = null;
        curve.PropertyChanged += this.LimitPropertyChanged;
        this.Limits.Add(curve);
      }

      this.Externals.Clear();

      foreach (var emiOverrideExternalCurve in data.Externals)
      {
        var external = ExternalCurveOverrideViewModel.FromData(emiOverrideExternalCurve);
        external.PropertyChanged += this.ExternalPropertyChanged;
        this.Externals.Add(external);
      }
    }

    private void RemoveExternalCurve(ExternalCurveOverrideViewModel obj)
    {
      if (this.Externals.Contains(obj))
      {
        this.Externals.Remove(obj);
        UpdateValidities(this.Externals);
      }
    }

    private void RemoveCurve(CurveOverrideViewModel obj)
    {
      if (this.Prescans.Contains(obj))
      {
        this.Prescans.Remove(obj);
        obj.PropertyChanged -= this.PrescanPropertyChanged;

        UpdateValidities(this.Prescans);
      }

      if (this.Limits.Contains(obj))
      {
        this.Limits.Remove(obj);
        obj.PropertyChanged -= this.LimitPropertyChanged;

        UpdateValidities(this.Limits);
      }

      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void RemovePoint(PointOverrideViewModel obj)
    {
      if (this.Suspects.Contains(obj))
      {
        this.Suspects.Remove(obj);
        obj.PropertyChanged -= this.SuspectPropertyChanged;
        UpdateValidities(this.Suspects);
      }

      if (this.Finals.Contains(obj))
      {
        this.Finals.Remove(obj);
        obj.PropertyChanged -= this.FinalPropertyChanged;
        UpdateValidities(this.Finals);
      }

      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void GoToHelp()
    {
      var path = Path.Combine(ExecPathHelper.GetExecDirectory(), "Help_BAT-EMC_EN.chm");

      if (!File.Exists(path))
      {
        System.Diagnostics.Debugger.Break();
      }

      const string entryId = "11017";

      Help.ShowHelp(Control.FromHandle(new System.Windows.Interop.WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle),
                    $"file://{path}",
                    HelpNavigator.TopicId, entryId);
    }

    private void SuspectPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(PointOverrideViewModel.IsValid))
      {
        return;
      }

      if (!(sender is PointOverrideViewModel))
      {
        return;
      }

      UpdateValidities(this.Suspects);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void FinalPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(PointOverrideViewModel.IsValid))
      {
        return;
      }

      if (!(sender is PointOverrideViewModel))
      {
        return;
      }

      UpdateValidities(this.Finals);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void PrescanPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(CurveOverrideViewModel.IsValid))
      {
        return;
      }

      if (!(sender is CurveOverrideViewModel))
      {
        return;
      }

      UpdateValidities(this.Prescans);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void LimitPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(CurveOverrideViewModel.IsValid))
      {
        return;
      }

      if (!(sender is CurveOverrideViewModel))
      {
        return;
      }

      UpdateValidities(this.Limits);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void ExternalPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ExternalCurveOverrideViewModel.IsValid))
      {
        return;
      }

      if (!(sender is ExternalCurveOverrideViewModel))
      {
        return;
      }

      UpdateValidities(this.Externals);
      this.AreAllStylesValid = !this.HasInvalidStyle();
    }

    private void Save()
    {
      // var iniDirectory = RegistryHelper.ReadLMProfileString("BAT-EMC", "Directories", "BAT-EMI", @"C:\bat-emc\BAT-EMI");
      if (!Directory.Exists(this.SaveDirectory))
      {
        if (
          MessageBox.Show(NexioMax3.Definition.Properties.Resources.EMIOverrideSDefaultSaveDirectoryIsNotAccessible,
                          NexioMax3.Definition.Properties.Resources.DirectoryNotFound, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        {
          return;
        }

        LogInteraction("Saving redirected to export due to inaccessible saving directory");
        this.Export();
        return;
      }

      LogInteraction("Saving EmiOverride");

      var jsonFilePath = Path.Combine(this.SaveDirectory, EmiOverrideData.OverrideFile);
      this.SaveDatas(jsonFilePath);

      this.CloseRequested?.Invoke(this, new RoutedEventArgs());
    }

    private void Cancel()
    {
      this.CloseRequested?.Invoke(this, new RoutedEventArgs());
    }
  }
}