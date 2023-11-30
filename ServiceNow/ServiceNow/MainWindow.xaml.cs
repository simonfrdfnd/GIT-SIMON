using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServiceNow.Model;

namespace ServiceNow
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = new ViewModel.MainWindowViewModel(this);
    }

    public void DataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      DataGrid daatgrid = sender as DataGrid;
      Point pt = e.GetPosition(daatgrid);
      DataGridCell daatgridcell = null;
      //Do the hittest to find the DataGridCell
      VisualTreeHelper.HitTest(daatgrid, null, (result) =>
      {
        // Find the ancestor element form the hittested element
        // e.g., find the DataGridCell if we hittest on the inner TextBlock
        DataGridCell cell = FindVisualParent<DataGridCell>(result.VisualHit);
        if (cell != null)
        {
          daatgridcell = cell;
          return HitTestResultBehavior.Stop;
        }
        else
          return HitTestResultBehavior.Continue;
      },
      new PointHitTestParameters(pt));

      if (daatgridcell != null)
      {
        Console.WriteLine(daatgridcell.Content);
        if (daatgridcell.Content is TextBlock)
          Console.WriteLine((daatgridcell.Content as TextBlock).Text);
      }
    }

    private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject parentObject = VisualTreeHelper.GetParent(child);
      if (parentObject == null) return null;
      T parent = parentObject as T;
      if (parent != null)
        return parent;
      else
        return FindVisualParent<T>(parentObject);
    }
  }
}
