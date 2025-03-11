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

namespace Comissar2
{
    /// <summary>
    /// Interaction logic for YouHaveNoRight.xaml
    /// </summary>
    public partial class YouHaveNoRight : UserControl
    {
        public YouHaveNoRight()
        {
            InitializeComponent();
            var parent = this.Parent as Window; if (parent != null)
            {
                parent.SizeToContent = SizeToContent.WidthAndHeight;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Window; if (parent != null) { parent.DialogResult = true; parent.Close(); }
        }
    }
}
