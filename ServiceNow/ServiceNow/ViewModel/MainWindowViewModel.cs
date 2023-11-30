using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ServiceNow.ViewModel
{
  internal class MainWindowViewModel : ViewModelBase
  {
    public MainWindowViewModel(MainWindow parentView)
    {
      Model.ServiceNow serviceNow = new Model.ServiceNow("simon.froidefond@nexiogroup.com", "Nexio171296SF");
      serviceNow.GetMyIncidents();
      this.Collection = new ObservableCollection<GridRow>();
      this.Array = new GridArray();

      //met les ticket en assigned si le client a repondu
      foreach (Model.Ticket row in serviceNow.ListTickets)
      {
        if (row.UpdatedBy != "simon.froidefond@nexiogroup.com")
        {
          serviceNow.UpdateIncidentStatus(row.Id, "Assigned");
        }
      }

      foreach (Model.Ticket row in serviceNow.ListTickets)
      {
        this.Array.Add(new GridRow() { Number = row.Number, UpdatedBy = row.UpdatedBy });
      }
      Application.Current.Dispatcher.Invoke(new Action(() =>
      {
        this.Collection.Clear();
        foreach (var p in this.Array.GridRowList)
        {
          Collection.Add(p);
        }
      }));
    }

    public MainWindow ParentView { get; internal set; }

    public ObservableCollection<GridRow> Collection { get; set; }

    public GridArray Array { get; set; }
  }

  public class GridArray : ViewModelBase
  {
    public GridArray()
    {
      this.GridRowList = new List<GridRow>();
    }

    public List<GridRow> GridRowList { get; set; }

    public void ClearRow(int i)
    {
      this.GridRowList[i].Number = "";
      this.GridRowList[i].UpdatedBy = "";
    }

    public void ClearColumn(string columnName)
    {
      foreach (GridRow gr in this.GridRowList)
      {
        switch (columnName)
        {
          case "Number":
            gr.Number = "";
            break;

          case "Worknote":
            gr.UpdatedBy = "";
            break;
        }
      }
    }

    public void Clear()
    {
      foreach (GridRow gr in this.GridRowList)
      {
        gr.Number = "";
        gr.UpdatedBy = "";
      }
    }

    public void Add(GridRow gr)
    {
      this.GridRowList.Add(gr);
    }
  }

  public class GridRow : ViewModelBase
  {
    public string Number { get; set; }

    public string UpdatedBy { get; set; }

    public Brush CellColorCurrent { get; set; }

    private bool isChecked = true;

    public bool IsChecked
    {
      get
      {
        return this.isChecked;
      }

      set
      {
        if (this.isChecked != value)
        {
          this.isChecked = value;
          this.OnPropertyChanged("IsChecked");
        }
      }
    }
  }
}
