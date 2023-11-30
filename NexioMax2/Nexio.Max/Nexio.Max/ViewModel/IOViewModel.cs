using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NationalInstruments.Visa;
using NexioMax.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nexio.Max.ViewModel
{
  public partial class IOViewModel : ObservableObject, INotifyPropertyChanged
  {
    private const string DefaultAddress = "TCPIP0::127.0.0.1::13000::SOCKET";

    private const string DefaultEos = "\\n";

    [ObservableProperty]
    private string displayMode = "Ascii";

    [ObservableProperty]
    private ObservableCollection<string> addresses = new ObservableCollection<string> { "TCPIP0::127.0.0.1::13000::SOCKET", "TCPIP0::127.0.0.1::inst0::INSTR" };

    [ObservableProperty]
    private string selectedAddress = DefaultAddress;

    [ObservableProperty]
    private string selectedEosWrite = DefaultEos;

    [ObservableProperty]
    private string selectedEosRead = DefaultEos;

    [ObservableProperty]
    public int selectedTimeout = 3000;

    [ObservableProperty]
    private ObservableCollection<string> eosChars = new ObservableCollection<string> { "\\n", "\\r", "\\r\\n" };

    [ObservableProperty]
    private string enableConsole = "False";

    [ObservableProperty]
    private string enableAddress = "True";

    [ObservableProperty]
    private string enableEosRead = "True";

    [ObservableProperty]
    private string enableTimeout = "True";

    [ObservableProperty]
    private string enableOpen = "True";

    [ObservableProperty]
    private string enableClose = "False";

    [ObservableProperty]
    private string enableScan = "True";

    [ObservableProperty]
    private string turnColor = "False";

    [ObservableProperty]
    private ObservableCollection<string> console = new ObservableCollection<string>();

    private MessageBasedSession visaSession;

    public IOViewModel()
    {
      //Remise à défaut de la liste des adresses
      this.Addresses = new ObservableCollection<string> { DefaultAddress };
      this.SelectedAddress = DefaultAddress;

      //Création de la liste des addresses en mémoire de l'application si elle n'existe pas déjà
      ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
      if (localSettings.Values["Addresses"] == null)
      {
        localSettings.Values["Addresses"] = this.Addresses;
      }

      //Ajout des adresses mémoire à la liste des adresses à mettre dans la combobox
      this.Addresses = localSettings.Values["Addresses"] as ObservableCollection<string>;
    }

    [RelayCommand]
    private void OnOpen()
    {
      this.Console.Add("> Enter \"help\" to display the list of commands.");
      //Vérification des paramètres
      if (!VisaHelper.IsVisaFormat(this.SelectedAddress))
      {
        this.Console.Add($"> {this.SelectedAddress} is not a valid VISA address.\n");
        return;
      }

      //Ouvre la connection VISA
      this.Console.Add($"> Attempt to open visa session with {this.SelectedAddress} ...\n");
      Task.Run(() => OpenVisaCom());
    }

    //[RelayCommand]
    //private void OnClose()
    //{
    //  VisaHelper.CloseVisaCom(this.visaSession);
    //  this.Console.Add($"> Disconnected !\n");
    //  this.EnableConsole = "False";
    //  this.EnableAddress = "True";
    //  this.EnableEosRead = "True";
    //  this.EnableTimeout = "True";
    //  this.EnableOpen = "True";
    //  this.EnableClose = "False";
    //  this.EnableScan = "True";
    //}

    ////[RelayCommand]
    //////private void OnScan()
    //////{
    //////  List<string> devices = VisaHelper.ScanGPIB();
    //////  foreach (string device in devices)
    //////  {
    //////    //Ajout de la nouvelle adresse à l'IHM et en mémoire si elle n'est pas dans la liste
    //////    if (!this.Addresses.Contains(device))
    //////    {
    //////      this.Addresses.Insert(0, device);
    //////      Settings.Default.Addresses.Insert(0, device);
    //////      Settings.Default.Save();
    //////    }
    //////  }

    //////  this.SelectedAddress = this.Addresses.First();
    //////}

    //[RelayCommand]
    //private void OnLineEntered(string line)
    //{
    //  if (ConsoleHelper.IsACommand(line))
    //  {
    //    OnConsoleCommandEntered(line);
    //  }
    //  else
    //  {
    //    OnInstrumentCommandEntered(line);
    //  }
    //}

    //private void OnConsoleCommandEntered(string command)
    //{
    //  if (command == "help")
    //  {
    //    this.Console.Add($"> {command}\n");
    //    foreach (string s in ConsoleHelper.helpText)
    //      this.Console.Add(s);
    //    return;
    //  }

    //  if (command == "clear" || command == "cls")
    //  {
    //    this.Console.Add($"> {command}\n");
    //    this.Console.Clear();
    //    return;
    //  }

    //  if (command == "read")
    //  {
    //    ReadVisa();
    //    return;
    //  }
    //}

    //private void OnInstrumentCommandEntered(string command)
    //{
    //  string fullCommand = command;
    //  string cleanCommand = ConsoleHelper.CleanCommand(command, this.SelectedEosWrite);
    //  bool sendHex = fullCommand.Contains("0x");

    //  //Envoie de la commande SCPI
    //  try
    //  {
    //    if (ConsoleHelper.IsHex(command))
    //      VisaHelper.WriteByte(this.visaSession, VisaHelper.HexStringToByte(cleanCommand), this.SelectedEosWrite);
    //    else
    //      VisaHelper.Write(this.visaSession, cleanCommand, this.SelectedEosWrite);
    //  }
    //  catch (Exception ex)
    //  {
    //    this.Console.Add($"> {ex.Message}\n");
    //  }

    //  //Mise à jour de la console
    //  if (this.DisplayMode.Contains("Hex"))
    //  {
    //    if (sendHex)
    //      this.Console.Add($"> 0x{cleanCommand}\n");
    //    else
    //      this.Console.Add($"> {cleanCommand}\n");
    //    if (!sendHex) //l'ulistateur est en train d'envoyer de l'hexa, ça n'aurait pas de sens de convertir la représentation ascii de l'hexa
    //      this.Console.Add($"> 0x{VisaHelper.AsciiToHexString(cleanCommand)}\n");
    //  }
    //  else
    //    this.Console.Add($"> {cleanCommand}\n");

    //  //Read si besoin et si pas inhibé par -noread
    //  if (ConsoleHelper.IsAQuestion(cleanCommand) && ConsoleHelper.IsReadAuthorized(fullCommand))
    //  {
    //    try
    //    {
    //      if (ConsoleHelper.IsReadCount(fullCommand))
    //        Task.Run(() => ReadVisa(ConsoleHelper.GetReadCount(fullCommand)));
    //      else
    //        Task.Run(() => ReadVisa());
    //    }
    //    catch (Exception ex)
    //    {
    //      this.Console.Add($"> {ex.Message}\n");
    //      this.EnableConsole = "False";
    //    }
    //  }
    //}

    ///// <summary>
    ///// Recharge la la liste des adresses VISA à partir de celles qui sont en mémoire
    ///// </summary>
    ////private void UpdateAddressesFromMemory()
    ////{
    ////  //Remise à défaut de la liste des adresses
    ////  this.Addresses = new ObservableCollection<string> { DefaultAddress };
    ////  this.SelectedAddress = DefaultAddress;

    ////  //Création de la liste des addresses en mémoire de l'application si elle n'existe pas déjà
    ////  if (Settings.Default.Addresses == null)
    ////  {
    ////    Settings.Default.Addresses = new System.Collections.Specialized.StringCollection();
    ////  }

    ////  //Ajout des adresses mémoire à la liste des adresses à mettre dans la combobox
    ////  string[] addr = new string[Settings.Default.Addresses.Count];
    ////  Settings.Default.Addresses.CopyTo(addr, 0);
    ////  this.Addresses = new ObservableCollection<string>(this.Addresses.Union(addr));
    ////}

    public void OpenVisaCom()
    {
      try
      {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        this.visaSession = VisaHelper.OpenVisaCom(this.SelectedAddress, this.SelectedTimeout, this.SelectedEosRead);

        //Ajout de la nouvelle adresse à l'IHM et en mémoire si elle n'est pas dans la liste
        if (!this.Addresses.Contains(this.SelectedAddress))
        {
          this.Addresses.Insert(0, this.SelectedAddress);

          // Save a setting locally on the device
          localSettings.Values["Addresses"] = this.Addresses;
        }

        //Mise à jour de la console
        this.EnableConsole = "True";
        this.Console.Add($"> Connected !\n");
        this.EnableAddress = "False";
        this.EnableEosRead = "False";
        this.EnableTimeout = "False";
        this.EnableOpen = "False";
        this.EnableClose = "True";
        this.EnableScan = "False";
      }
      catch (Exception ex)
      {
        this.Console.Add($"> {ex.Message.ToString()}\n");
        this.EnableConsole = "False";
      }

    }

    ////public int ReadVisa(int byteCount = 0)
    ////{
    ////  Application.Current.Dispatcher.Invoke(new Action(() =>
    ////  {
    ////    this.EnableConsole = "False";
    ////    this.TurnColor = "True";
    ////  }));

    ////  string answer = VisaHelper.Read(this.visaSession, byteCount).Replace(Regex.Unescape(this.SelectedEosRead), "");
    ////  //Mise à jour de la console
    ////  Application.Current.Dispatcher.Invoke(new Action(() =>
    ////  {
    ////    this.EnableConsole = "True";
    ////    if (this.DisplayMode.Contains("Hex") && !answer.Contains("Timeout"))
    ////    {
    ////      this.Console.Add($"> {answer}\n");
    ////      this.Console.Add($"> 0x{Convert.ToHexString(Encoding.UTF8.GetBytes(answer))}\n");
    ////    }
    ////    else
    ////      this.Console.Add($"> {answer}\n");

    ////    this.TurnColor = "False";
    ////  }));
    ////  return 0;
    ////}
  }
}
