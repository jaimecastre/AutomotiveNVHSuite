using SciChart.Charting.Visuals;
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
        Dictionary<int, SciChartSurface> _charts = new Dictionary<int, SciChartSurface>();

        public MainWindow()
        {
            InitializeComponent();
            ViewModel.Dispatcher = Dispatcher;

            SciChartSurface surface = new SciChartSurface();
            _panel.Children.Add(surface);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ViewModel.Graphs):
                    var graphsNew = ViewModel.Graphs.Where(x => !_charts.ContainsKey(x.Key)).ToList();
                    foreach (var graph in graphsNew)
                    {
                        _charts.Add(graph.Key, graph.Value);
                        _panel.Children.Add(graph.Value);
                    }
                    break;
            }
        }

        private CalculationsViewModel ViewModel => (DataContext as CalculationsViewModel);
    }
}
