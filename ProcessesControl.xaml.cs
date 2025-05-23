﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Comissar2
{
    /// <summary>
    /// Interaction logic for ProcessesControl.xaml
    /// </summary>
    public partial class ProcessesControl : UserControl
    {
        public ProcessesControl()
        {
            InitializeComponent();
            this.DataContext = new ProcessViewModel();

        }

        private void ProcessGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ((ProcessViewModel)this.DataContext).gridSortDirection = e.Column.SortDirection;
            ((ProcessViewModel)this.DataContext).gridSortColumn = e.Column.SortMemberPath;
        }
    }
}
