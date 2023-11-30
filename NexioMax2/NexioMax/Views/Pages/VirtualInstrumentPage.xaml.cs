using System.Windows;
using System;
using Wpf.Ui.Common.Interfaces;
using System.Windows.Controls;
using System.Windows.Data;
using Wpf.Ui.Controls;
using System.Collections;
using NexioMax.ViewModels;
using System.ComponentModel;
using System.Windows.Media;
using FilterDataGrid;
using Wpf.Ui.Mvvm.Interfaces;
using System.Collections.ObjectModel;
using NexioMax.Models;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using DataGridTextColumn = System.Windows.Controls.DataGridTextColumn;
using System.Xml.Linq;

namespace NexioMax.Views.Pages
{
  /// <summary>
  /// Interaction logic for DataView.xaml
  /// </summary>
  public partial class VirtualInstrumentPage : INavigableView<ViewModels.VirtualInstrumentViewModel>
  {
    public ViewModels.VirtualInstrumentViewModel ViewModel
    {
      get;
    }

    public VirtualInstrumentPage(ViewModels.VirtualInstrumentViewModel viewModel)
    {
      ViewModel = viewModel;
      InitializeComponent();
      viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(VirtualInstrumentViewModel.CallRowsGroups))
      {
        foreach (var group in this.ViewModel.CallRowsGroups)
        {
          if (group.Count == 0)
          {
            continue;
          }

          // Créez une nouvelle ligne dans la grid
          RowDefinition row = new RowDefinition();
          row.Height = new GridLength(1, GridUnitType.Star);
          grid.RowDefinitions.Add(row);

          // Créez une nouvelle DataGrid
          System.Windows.Controls.DataGrid myDataGrid = new System.Windows.Controls.DataGrid();
          myDataGrid.EnableRowVirtualization = true;
          myDataGrid.EnableColumnVirtualization = true;
          myDataGrid.FontSize = 12;
          myDataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
          myDataGrid.HorizontalContentAlignment = HorizontalAlignment.Stretch;
          myDataGrid.BorderThickness = new Thickness(0);
          myDataGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
          myDataGrid.AutoGenerateColumns = false;

          // Style des headers
          Style style = new Style(typeof(DataGridColumnHeader));
          style.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));
          style.Setters.Add(new Setter(DataGridColumnHeader.BackgroundProperty, Brushes.DarkSlateGray));
          DataGridLength length = new DataGridLength(1, DataGridLengthUnitType.Star);
          // Header columns
          DataGridTextColumn colonneNumber = new DataGridTextColumn();
          colonneNumber.Header = "Number";
          colonneNumber.MinWidth = 50;
          colonneNumber.Width = new DataGridLength(50);
          colonneNumber.HeaderStyle = style;
          colonneNumber.Binding = new Binding("Number");
          DataGridTextColumn colonneDescription = new DataGridTextColumn();
          colonneDescription.Header = "Description";
          colonneDescription.MinWidth = 250;
          colonneDescription.Width = length;
          colonneDescription.HeaderStyle = style;
          colonneDescription.Binding = new Binding("Description");
          DataGridTextColumn colonneAnswer = new DataGridTextColumn();
          colonneAnswer.Header = "Answer";
          colonneAnswer.MinWidth = 250;
          colonneAnswer.Width = length;
          colonneAnswer.HeaderStyle = style;
          colonneAnswer.Binding = new Binding("Answer");
          DataGridTextColumn colonneAddress = new DataGridTextColumn();
          colonneAddress.Header = "Address";
          colonneAddress.MinWidth = 100;
          colonneAddress.Width = 200;
          colonneAddress.HeaderStyle = style;
          colonneAddress.Binding = new Binding("Address");
          myDataGrid.Columns.Add(colonneNumber);
          myDataGrid.Columns.Add(colonneDescription);
          myDataGrid.Columns.Add(colonneAnswer);
          myDataGrid.Columns.Add(colonneAddress);
          // ItemSource
          myDataGrid.ItemsSource = group;
          // Ajoutez la DataGrid à la nouvelle ligne
          grid.Children.Add(myDataGrid);
          Grid.SetRow(myDataGrid, grid.RowDefinitions.Count - 1);
        }
      }
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.address.Text = $"TCPIP0::127.0.0.1::{e.NewValue.ToString()}::SOCKET";
    }

    private void DataGridRow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

    }
  }
}
