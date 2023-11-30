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
using System.Windows.Documents;

namespace NexioMax.ViewModels
{
  public partial class VirtualInstrumentViewModel : ObservableObject, INavigationAware
  {
    private bool isInitialized = false;

    private List<TraceCall> traceCalls = new List<TraceCall>();

    private VisaServer server;

    private bool stopProgressBar = false;

    [ObservableProperty]
    private int port = 13000;

    [ObservableProperty]
    private ObservableCollection<ObservableCollection<VirtualInstrumentCallRow>> callRowsGroups = new ObservableCollection<ObservableCollection<VirtualInstrumentCallRow>>();

    [ObservableProperty]
    private float importProgression = 0;

    [ObservableProperty]
    private bool enableWrap = true;

    [ObservableProperty]
    private string fileName = "Trace path";

    [RelayCommand]
    private void OnServer()
    {
      this.server = new VisaServer(this.Port);
      this.server.listener.ClientConnected += OnClientConnected;
      this.server.listener.ClientDisconnected += OnClientDisconnected;
      this.server.listener.CommandReceived += OnCommandReceived;
    }

    [RelayCommand]
    private async void OnImport()
    {
      try
      {
        this.EnableWrap = false;
        this.FileName = TraceHelper.OpenNiTrace();
        if (string.IsNullOrEmpty(this.FileName))
        {
          this.EnableWrap = true;
          return;
        }

        this.stopProgressBar = false;
        Task.Factory.StartNew(() => { this.traceCalls = TraceHelper.ReadCalls(this.FileName); this.stopProgressBar = true; }); //thread pour extraire les calls de la trace
        Thread.Sleep(1000);
        await Task.Factory.StartNew(() => { UpdateProgressBar(); }); //thread pour mettre à jour la progress bar

        //Conversion des calls en groupes
        this.CallRowsGroups = TraceCall2CallRow();
        this.EnableWrap = true;
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
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
    }

    private void OnClientDisconnected(object? sender, EventArgs e)
    {
      this.EnableWrap = true;
    }

    private void OnCommandReceived(object? sender, EventArgs e)
    {
      try
      {
        CommandReceivedEventArgs eCmd = (CommandReceivedEventArgs)e;
        foreach (string cmd in CommandHelper.GetCommands(eCmd.Command))
        {
          TraceCall call = this.traceCalls.First(x => x.BufferAsciiString.Contains(cmd));
          
          if (!ConsoleHelper.IsAQuestion(call.BufferAsciiString))
            return;

          string answer = TraceHelper.GetCallAnswer(this.traceCalls, call);
          this.server.CommandTreatment(answer);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void OnClientConnected(object? sender, EventArgs e)
    {
      this.EnableWrap = false;
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

    private ObservableCollection<ObservableCollection<VirtualInstrumentCallRow>> TraceCall2CallRow()
    {
      if (traceCalls.Count == 0)
        return null;

      ObservableCollection<ObservableCollection<VirtualInstrumentCallRow>> list = new ObservableCollection<ObservableCollection<VirtualInstrumentCallRow>>();

      var groups = this.traceCalls.GroupBy(x => x.BufferAsciiString).Select(y => y.First()).GroupBy(z => z.Address);
      foreach (var group in groups)
      {
        ObservableCollection<VirtualInstrumentCallRow> groupOfRows = new ObservableCollection<VirtualInstrumentCallRow>();
        foreach (var call in group.Where(x => TraceHelper.IsCallWrite(x))) 
        {
          groupOfRows.Add(new VirtualInstrumentCallRow(call.Number, call.BufferAsciiString, TraceHelper.GetCallAnswer(this.traceCalls, call), call.Address));
        }
        list.Add(groupOfRows);
      }
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
  }
}
