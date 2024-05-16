﻿using NexioMax3.Definition.ViewModel;
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

namespace NexioMax3.Definition.View
{
  /// <summary>
  /// Interaction logic for VisaConsoleView.xaml
  /// </summary>
  public partial class VisaConsoleView : UserControl
  {
    public VisaConsoleView()
    {
      this.InitializeComponent();
      var vm = new VisaConsoleViewModel();
      this.DataContext = vm;
    }
  }
}