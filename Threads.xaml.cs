using System;
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
    /// Interaction logic for Threads.xaml
    /// </summary>
    public partial class Threads : UserControl
    {
        Process process { get; set; }

        public Threads()
        {
            InitializeComponent();
        }

        public Threads(Process p)
        {
            this.process = p;
            InitializeComponent();
            this.ThreadView.ItemsSource = process.Threads;
        }
    }
}
