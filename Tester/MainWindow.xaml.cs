using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tester
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

        private TesterViewModel ViewModel => (DataContext as TesterViewModel);

        private void Button_LoadFile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommandLoadFile();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommandSettings();
        }

        private void Button_FileInfo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommandFileInfo();
        }

        private void Button_GetData_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CommandGetData();
        }
    }
}