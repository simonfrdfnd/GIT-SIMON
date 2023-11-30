using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using Wpf.Ui.Common.Interfaces;
using System.Collections.ObjectModel;
using NexioMax.Models;
using System.Collections.Generic;
using NexioMax.Helpers;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using NationalInstruments.Visa;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;

namespace NexioMax.ViewModels
{
  public partial class TraceAnalysisViewModel : ObservableObject, INavigationAware
  {
    private bool isInitialized = false;

    private List<TraceCall> traceCalls = new List<TraceCall>();

    private List<TraceCall> originalTraceCalls = new List<TraceCall>();

    private bool stopProgressBar = false;

    private List<MessageBasedSession> sessions = new List<MessageBasedSession>();

    private DataTable dataTable = new DataTable();

    Random rnd = new Random();

    [ObservableProperty]
    public DataGridCollectionView tableSource;

    [ObservableProperty]
    private ObservableCollection<TraceAnalysisCallRow> callRows = new ObservableCollection<TraceAnalysisCallRow>();

    [ObservableProperty]
    private float importProgression = 0;

    [ObservableProperty]
    private bool enableWrap = true;

    [ObservableProperty]
    private string fileName = "Trace path";

    [ObservableProperty]
    private bool isIoChecked;

    [ObservableProperty]
    private bool isBufferChecked;

    [ObservableProperty]
    private bool isGroupChecked;
    
    [ObservableProperty]
    private int selectedRow;

    public CallHelper.TypeFilter TypeFilter
    {
      get
      {
        if (isIoChecked)
          return CallHelper.TypeFilter.IoOnly;
        else
          return CallHelper.TypeFilter.None;
      }
    }

    public CallHelper.DescriptionFilter DescriptionFilter
    {
      get
      {
        if (isBufferChecked)
          return CallHelper.DescriptionFilter.BufferOnly;
        else
          return CallHelper.DescriptionFilter.None;
      }
    }

    public const int Count = 5000;

    [RelayCommand]
    private void OnReset()
    {
      this.IsBufferChecked = false;
      this.IsIoChecked = false;
      this.IsGroupChecked = false;
      this.traceCalls = this.originalTraceCalls.Select(item => (TraceCall)item.Clone()).ToList();
      this.CallRows = TraceCall2CallRow(this.DescriptionFilter, this.TypeFilter);
    }

    [RelayCommand]
    private async Task<int> OnImport()
    {
      try
      {
        this.EnableWrap = false;
        this.FileName = TraceHelper.OpenNiTrace();
        if (string.IsNullOrEmpty(this.FileName))
        {
          this.EnableWrap = true;
          return 0;
        }

        this.stopProgressBar = false;
        Task.Factory.StartNew(() => { this.traceCalls = TraceHelper.ReadCalls(this.FileName); this.stopProgressBar = true; }); //thread pour extraire les calls de la trace
        Thread.Sleep(1000);
        await Task.Factory.StartNew(() => { UpdateProgressBar(); }); //thread pour mettre à jour la progress bar
        this.originalTraceCalls = this.traceCalls.Select(item => (TraceCall)item.Clone()).ToList(); //on garde la liste originelle pour le reset

        //Si on assigne directement a this.CallRows ça plante, je ne sais pas pourquoi
        this.CallRows = TraceCall2CallRow(this.DescriptionFilter, this.TypeFilter);
        this.EnableWrap = true;
        return 1;
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
        return 0;
      }
    }

    [RelayCommand]
    private void OnIoCheck()
    {
      if (this.CallRows.Count == 0)
        return;

      if (IsGroupChecked)
        IsGroupChecked = false;

      this.CallRows = TraceCall2CallRow(this.DescriptionFilter, this.TypeFilter);
    }

    [RelayCommand]
    private void OnBufferCheck()
    {
      if (this.CallRows.Count == 0)
        return;

      if (IsGroupChecked)
        IsGroupChecked = false;

      this.CallRows = TraceCall2CallRow(this.DescriptionFilter, this.TypeFilter);
    }

    [RelayCommand]
    private void OnDisable()
    {
      if (this.CallRows.Count == 0)
        return;

      if (this.SelectedRow < 0)
        return;

      string address = this.CallRows[this.SelectedRow].Address;
      this.traceCalls = this.traceCalls.Where(x => x.Address != address).ToList();
      this.CallRows = TraceCall2CallRow(this.DescriptionFilter, this.TypeFilter);
    }

    [RelayCommand]
    private void OnPlay()
    {
      while (this.SelectedRow < this.CallRows.Count - 1)
      {
        OnPlayOne();
        Thread.Sleep(200);
      }
      OnPlayOne();
    }

    [RelayCommand]
    private void OnPlayOne()
    {
      if (!this.IsIoChecked)
      {
        this.IsIoChecked = true;
        OnIoCheck();
      }

      if (!this.IsBufferChecked)
      {
        this.IsBufferChecked = true;
        OnBufferCheck();
      }

      if (this.CallRows.Count == 0)
        return;

      if (this.SelectedRow < 0)
      {
        this.SelectedRow++;
        return;
      }

      TraceAnalysisCallRow row = this.CallRows[this.SelectedRow];
      TraceCall call = this.traceCalls.First(x => x.Number == row.Number);

      if (TraceHelper.IsCallWrite(call))
      {
        try
        {
          if (!this.sessions.Any(x => x.ResourceName == call.Address))
          {
            //pour ouvrir la session il faut connaitre le eosRead donc on cherche un read avec la même adresse
            string eosRead = this.traceCalls.First(x => (x.Address == call.Address) && TraceHelper.IsCallRead(x)).BufferAsciiString.Last().ToString();
            this.sessions.Add(VisaHelper.OpenVisaCom(call.Address, 1000, eosRead));
          }
          string eosWrite = call.BufferAsciiString.Last().ToString();
          VisaHelper.Write(this.sessions.First(x => x.ResourceName == call.Address), row.Description, eosWrite);
        }
        catch (Exception ex) 
        {
          MessageBox.Show(ex.Message);
        }
      }

      if (TraceHelper.IsCallRead(call))
      {
        try
        {
          string answer = VisaHelper.Read(this.sessions.First(x => x.ResourceName == call.Address), 0);
          row.Description = answer.Replace("\n", "");
          int index = this.SelectedRow;
          this.CallRows.Remove(row);
          this.CallRows.Insert(index, row);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }

      if (this.SelectedRow == this.CallRows.Count - 1)
        this.SelectedRow = -1; 
      else
        this.SelectedRow++;
    }

    public void OnNavigatedTo()
    {
      if (!this.isInitialized)
        InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
      this.isInitialized = false;
    }

    private void InitializeViewModel()
    {
      this.isInitialized = true;

      dataTable.Columns.Add("Column 1");
      dataTable.Columns.Add("Column 2");
      dataTable.Columns.Add("Column 3");

      DataGridCollectionView dataGridCollectionView = new DataGridCollectionView(dataTable);
      dataGridCollectionView.ItemsCount += OnItemsCount;
      dataGridCollectionView.ItemsRequest += OnItemsRequest;

      TableSource = dataGridCollectionView;
    }

    private void UpdateProgressBar()
    {
      while (this.ImportProgression < 100 || !stopProgressBar)
      {
        this.ImportProgression = TraceHelper.ImportationProgress;
        Thread.Sleep(100);
        AllowUIToUpdate();
      }
      this.ImportProgression = 0;
    }

    private ObservableCollection<TraceAnalysisCallRow> TraceCall2CallRow(CallHelper.DescriptionFilter descriptionFilter, CallHelper.TypeFilter callFilter)
    {
      if (traceCalls.Count == 0)
        return null;

      TimeSpan traceDuration = TraceHelper.GetTraceDuration(this.traceCalls);
      TimeSpan sumOfDurations = TraceHelper.GetSumOfDurations(this.traceCalls);
      DateTime firstTimeStamp = this.traceCalls.First().Timestamp;
      ObservableCollection<TraceAnalysisCallRow> list = new ObservableCollection<TraceAnalysisCallRow>();
      if (callFilter == CallHelper.TypeFilter.None)
        foreach (TraceCall tc in this.traceCalls)
          if (descriptionFilter == CallHelper.DescriptionFilter.BufferOnly)
            list.Add(new TraceAnalysisCallRow(tc.Number, tc.BufferAsciiString, tc.Timestamp, (tc.Timestamp - firstTimeStamp) / traceDuration, tc.Duration, tc.Duration / sumOfDurations, tc.Address, tc.IsEnabled));
          else
            list.Add(new TraceAnalysisCallRow(tc.Number, tc.Description, tc.Timestamp, (tc.Timestamp - firstTimeStamp) / traceDuration, tc.Duration, tc.Duration / sumOfDurations, tc.Address, tc.IsEnabled));
      else
        foreach (TraceCall tc in this.traceCalls)
          if (TraceHelper.IsCallIO(tc))
            if (descriptionFilter == CallHelper.DescriptionFilter.BufferOnly)
              list.Add(new TraceAnalysisCallRow(tc.Number, tc.BufferAsciiString, tc.Timestamp, (tc.Timestamp - firstTimeStamp) / traceDuration, tc.Duration, tc.Duration / sumOfDurations, tc.Address, tc.IsEnabled));
            else
              list.Add(new TraceAnalysisCallRow(tc.Number, tc.Description, tc.Timestamp, (tc.Timestamp - firstTimeStamp) / traceDuration, tc.Duration, tc.Duration / sumOfDurations, tc.Address, tc.IsEnabled));

      return list;
    }
    private void AllowUIToUpdate()
    {
      DispatcherFrame frame = new DispatcherFrame();
      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
      {
        frame.Continue = false;
        return null;
      }), null);

      Dispatcher.PushFrame(frame);
      //EDIT:
      Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                    new Action(delegate { }));
    }
    private void OnItemsCount(DataGridCollectionView arg1, CountEventArgs arg2)
    {
      arg2.Count = Count;
    }

    private async void OnItemsRequest(DataGridCollectionView arg1, ItemsEventArgs arg2)
    {
      int startIndex = arg2.StartIndex;
      int count = arg2.RequestedItemsCount;

      List<object> items = new List<object>();

      for (int i = startIndex; i < startIndex + count; i++)
      {
        DataRow row = dataTable.NewRow();
        row[0] = this.CallRows[i].Number;
        row[1] = this.CallRows[i].Description;
        row[2] = this.CallRows[i].Timestamp;
        items.Add(row);
      }

      arg2.SetItems(items);
    }
  }
}
