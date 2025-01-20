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

namespace AutomotiveNVHSuite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonStartRemote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StartRemote();
        }

        private void ButtonStopRemote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopRemote();
        }

        private CalculationsViewModel ViewModel => (DataContext as CalculationsViewModel);
    }
}
