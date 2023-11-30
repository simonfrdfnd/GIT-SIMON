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

namespace NexioMax.Views.Pages
{
  /// <summary>
  /// Interaction logic for DataView.xaml
  /// </summary>
  public partial class TraceAnalysisPage : INavigableView<ViewModels.TraceAnalysisViewModel>
  {
    public ViewModels.TraceAnalysisViewModel ViewModel
    {
      get;
    }

    public TraceAnalysisPage(ViewModels.TraceAnalysisViewModel viewModel)
    {
      ViewModel = viewModel;
      InitializeComponent();
      //viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
      //ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.datagrid.ItemsSource);
      //if (collectionView != null && collectionView.CanGroup == true)
      //{
      //  collectionView.GroupDescriptions.Clear();
      //  collectionView.GroupDescriptions.Add(new PropertyGroupDescription("Description"));
      //}

      //GroupStyle groupStyle = new GroupStyle();
      ////// Définir le modèle d'en-tête de groupe
      //DataTemplate groupHeaderTemplate = new DataTemplate();
      //FrameworkElementFactory stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
      //FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
      //stackPanelFactory.AppendChild(textBlockFactory);
      //groupHeaderTemplate.VisualTree = stackPanelFactory;
      //groupStyle.HeaderTemplate = groupHeaderTemplate;
      ////// Ajouter des setters à votre GroupStyle
      //this.datagrid.GroupStyle.Add(groupStyle);
    }

    private void ToggleButton_UnChecked(object sender, RoutedEventArgs e)
    {
      //ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.datagrid.ItemsSource);
      //collectionView.GroupDescriptions.Clear();
      //this.datagrid.GroupStyle.Clear();
    }

    //private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //  DataGridRow dgRow;
    //  DataGridCell cell;
    //  int c = 0;
    //  try
    //  {
    //    if (e.PropertyName == nameof(TraceAnalysisViewModel.CallRows))
    //    {
    //      this.datagrid.UpdateLayout();
    //      for (int i = 0; i < this.datagrid.Items.Count; i++)
    //      {
    //        c++;
    //        dgRow = datagrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
    //        if (dgRow == null)
    //          return;
    //        //Duration
    //        cell = datagrid.Columns[3].GetCellContent(dgRow).Parent as DataGridCell;
    //        var brush = new LinearGradientBrush();
    //        brush.StartPoint = new Point(0, 0);
    //        brush.EndPoint = new Point(1, 0);
    //        brush.GradientStops.Add(new GradientStop(Colors.DarkOliveGreen, ((TraceAnalysisViewModel)sender).CallRows[i].DurationPercent));
    //        brush.GradientStops.Add(new GradientStop(Colors.Transparent, ((TraceAnalysisViewModel)sender).CallRows[i].DurationPercent));
    //        cell.Background = brush;

    //        dgRow.IsSelected = false;
    //      }
    //    }
    //  }
    //  catch (Exception ex)
    //  {
    //    System.Windows.MessageBox.Show(ex.Message);
    //  }
    //}
  }
}
