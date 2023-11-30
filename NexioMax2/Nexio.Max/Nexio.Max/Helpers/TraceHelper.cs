using Microsoft.Win32;
using NexioMax.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace NexioMax.Helpers
{
  public static class TraceHelper
  {
    public static float ImportationProgress { get; internal set; }

    public static async Task<string> OpenNiTraceAsync()
    {
      //OpenFileDialog openFileDialog = new OpenFileDialog();
      //openFileDialog.Title = "Open Trace Capture File";
      //openFileDialog.Filter = "Capture Files|*.xml;*.txt";
      //openFileDialog.ShowDialog();
      //string fileName = openFileDialog.FileName;
      //return fileName;
      FileOpenPicker openPicker = new FileOpenPicker();
      openPicker.ViewMode = PickerViewMode.Thumbnail;
      openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
      openPicker.FileTypeFilter.Add(".jpg");
      openPicker.FileTypeFilter.Add(".jpeg");
      openPicker.FileTypeFilter.Add(".png");

      StorageFile file = await openPicker.PickSingleFileAsync();
      if (file != null)
      {
        // Application now has read/write access to the picked file
      }
      else
      {
      }

      return string.Empty;
    }

    public static List<TraceCall> ReadCalls(string path)
    {
      if (string.IsNullOrEmpty(path))
        throw new Exception("The file path is incorrect.");

      if (Path.GetExtension(path) == ".txt")//cas du .txt
        return ReadCallsTxt(path);
      else if (Path.GetExtension(path) == ".xml")
        return ReadCallsXml(path);
      else
        throw new Exception("File type should be .txt or .xml");
    }

    public static List<TraceCall> ReadCallsTxt(string path)
    {
      List<TraceCall> traceCalls = new List<TraceCall>();
      using (StreamReader file = new StreamReader(path, Encoding.UTF8))
      {
        string line = file.ReadLine();
        while (line != null)
        {
          List<string> callItems = new List<string>();
          callItems.Add(line);
          callItems.Add(file.ReadLine());
          callItems.Add(file.ReadLine());
          line = file.ReadLine();
          while (line != "")
          {
            callItems.Add(line);
            line = file.ReadLine();
          }
          TraceCallTxt tc = new TraceCallTxt(callItems);
          traceCalls.Add(tc);
          line = file.ReadLine();
        }
      }
      return traceCalls;
    }
    public static List<TraceCall> ReadCallsXml(string path)
    {
      List<TraceCall> traceCalls = new List<TraceCall>();
      using (StreamReader file = new StreamReader(path, Encoding.UTF8))
      {
        try
        {
          string trace = file.ReadToEnd();
          XElement elt = XElement.Parse(trace);
          IEnumerable<XNode> nodes = elt.Nodes();
          ImportationProgress = 0;
          float count = 0;
          foreach (XNode node in nodes)
          {
            TraceCallXml tc = new TraceCallXml(node, node.ElementsBeforeSelf().Count() + 1);
            traceCalls.Add(tc);
            count++;
            ImportationProgress = count / nodes.Count() * 100;
          }
        }
        catch (Exception e)
        {
          // Create the message dialog and set its content
          var messageDialog = new MessageDialog(e.Message);
          // Show the message dialog
          _ = messageDialog.ShowAsync();
        }
      }
      return traceCalls;
    }

    public static bool IsCallWrite(TraceCall call)
    {
      if (call.Description.Contains("viWrite") || call.Description.Contains("Envoyer") || (call.Description.Contains("Send") && !call.Description.Contains("SendIFC")))
        return true;
      else
        return false;
    }

    public static bool IsCallRead(TraceCall call)
    {
      if (call.Description.Contains("viRead") || call.Description.Contains("Receive"))
        return true;
      else
        return false;
    }

    public static bool IsCallIO(TraceCall call)
    {
      if (IsCallWrite(call) || IsCallRead(call))
        return true;
      else
        return false;
    }

    public static TimeSpan GetTraceDuration(List<TraceCall> traceCalls)
    {
      if (traceCalls.Count == 0)
        return TimeSpan.Zero;
      TimeSpan totalDuration = (traceCalls.Last().Timestamp + traceCalls.Last().Duration) - traceCalls.First().Timestamp;
      return totalDuration;
    }

    public static TimeSpan GetSumOfDurations(List<TraceCall> traceCalls)
    {
      TimeSpan sumOfDurations = TimeSpan.Zero;
      foreach (TraceCall call in traceCalls)
        sumOfDurations += call.Duration;
      return sumOfDurations;
    }

    public static string GetCallAnswer(List<TraceCall> traceCalls, TraceCall call)
    {
      if (!CommandHelper.IsQuestion(call.BufferAsciiString))
        return string.Empty;
      else
      {
        TraceCall callAnswer = traceCalls.First(x => (x.Number > call.Number) && (IsCallRead(x)));
        return callAnswer.BufferAsciiString;
      }
    }
    public static List<string> GetAddresses(List<TraceCall> traceCalls)
    {
      return traceCalls.Where(w => w.Address != string.Empty).GroupBy(x => x.Address).Select(y => y.First().Address).ToList();
    }

    public static int GetNumberOfGroups(List<TraceCall> traceCalls)
    {
      return traceCalls.Where(x => TraceHelper.IsCallWrite(x)).GroupBy(y => y.BufferAsciiString).Select(z => z.First()).Count();
    }
    public static List<int> GetIndexesOfGroups(List<TraceCall> traceCalls)
    {
      return (List<int>)traceCalls.Where(x => TraceHelper.IsCallWrite(x)).GroupBy(y => y.BufferAsciiString).Select(z => z.First()).Select(w => w.Number).ToList();
    }

    public static List<TraceCall> GetGroup(List<TraceCall> traceCalls, TraceCall call)
    {
      List<TraceCall> list = new List<TraceCall>();
      var groups = traceCalls.Where(x => TraceHelper.IsCallWrite(x)).GroupBy(y => y.BufferAsciiString);
      var group = groups.First(x => x.Key.Contains(call.BufferAsciiString));
      foreach (var obj in group)
      {
        list.Add((TraceCall)obj);
      }
      return list;
    }
  }
}
