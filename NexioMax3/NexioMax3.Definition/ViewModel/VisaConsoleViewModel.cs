﻿namespace NexioMax3.Definition.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using Nexio.Com.VISA;
  using Nexio.Wpf.Base;
  using Nexio.Wpf.Command;
  using NexioMax3.Domain.Configuration.Model.EMIOverrides2;
  using NexioMax3.Domain.Configuration.Model.NexioMax3;

  public class VisaConsoleViewModel : ViewModelBase
  {
    private string displayMode;

    private ObservableCollection<string> addresses;

    private ObservableCollection<string> eosChars;

    private string selectedAddress;

    private string selectedEosWrite;

    private string selectedEosRead;

    private int timeout;

    private int bufferSize;

    private bool enableClose;

    private bool enableOpen;

    private bool enableScan;

    private RelayCommand openCommand;

    public VisaConsoleViewModel()
    {
      this.displayMode = "Ascii";
      this.LoadAdresses();
      this.eosChars = new ObservableCollection<string> { "\\n", "\\r", "\\r\\n" };
      this.selectedEosWrite = this.eosChars.First();
      this.selectedEosRead = this.eosChars.First();
      this.timeout = 3000;
      this.bufferSize = 4096;
      this.enableClose = false;
      this.enableOpen = true;
      this.enableScan = true;
    }

    public string DisplayMode
    {
      get => this.displayMode;
      set => this.Set(nameof(this.DisplayMode), ref this.displayMode, value);
    }

    public ObservableCollection<string> Addresses
    {
      get => this.addresses;
      set => this.Set(nameof(this.Addresses), ref this.addresses, value);
    }

    public ObservableCollection<string> EosChars
    {
      get => this.eosChars;
      set => this.Set(nameof(this.EosChars), ref this.eosChars, value);
    }

    public string SelectedAddress
    {
      get => this.selectedAddress;
      set => this.Set(nameof(this.SelectedAddress), ref this.selectedAddress, value);
    }

    public string SelectedEosWrite
    {
      get => this.selectedEosWrite;
      set => this.Set(nameof(this.SelectedEosWrite), ref this.selectedEosWrite, value);
    }

    public string SelectedEosRead
    {
      get => this.selectedEosRead;
      set => this.Set(nameof(this.SelectedEosRead), ref this.selectedEosRead, value);
    }

    public int Timeout
    {
      get => this.timeout;
      set => this.Set(nameof(this.Timeout), ref this.timeout, value);
    }
    public int BufferSize
    {
      get => this.bufferSize;
      set => this.Set(nameof(this.BufferSize), ref this.bufferSize, value);
    }

    public bool EnableClose
    {
      get => this.enableClose;
      set => this.Set(nameof(this.EnableClose), ref this.enableClose, value);
    }

    public bool EnableOpen
    {
      get => this.enableOpen;
      set => this.Set(nameof(this.EnableOpen), ref this.enableOpen, value);
    }

    public bool EnableScan
    {
      get => this.enableScan;
      set => this.Set(nameof(this.EnableScan), ref this.enableScan, value);
    }

    public RelayCommand OpenCommand => this.openCommand ?? (this.openCommand = new RelayCommand(this.OpenAction));

    private void OpenAction()
    {
      this.SaveAddresses();
    }

    private void LoadAdresses()
    {
      this.addresses = new ObservableCollection<string> {};
      foreach (string address in NexioMax3Data.Load(NexioMax3Data.NexioMax3File).Addresses)
      {
        this.addresses.Add(address);
      }

      if (this.addresses.Count == 0)
      {
        this.addresses.Add("TCPIP0::127.0.0.1::13000::SOCKET");
        this.addresses.Add("TCPIP0::127.0.0.1::inst0::INSTR");
        this.addresses.Add("GPIB0::10::INSTR");
      }

      this.selectedAddress = this.addresses.First();
    }

    private void SaveAddresses()
    {
      if (this.Addresses.Contains(this.selectedAddress))
      {
        this.Addresses.Move(this.Addresses.IndexOf(this.selectedAddress), 0);
      }
      else
      {
        this.Addresses.Insert(0, this.selectedAddress);
      }

      var jsonFilePath = Path.Combine(NexioMax3Data.NexioMax3File);
      var datas = new NexioMax3Data
      {
        Addresses = this.addresses.ToList(),
      };

      datas.Save(jsonFilePath);
    }
  }
}