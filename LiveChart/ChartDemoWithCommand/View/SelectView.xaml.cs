using ChartDemoWithCommand.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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

namespace ChartDemoWithCommand.View
{
    /// <summary>
    /// SelectView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectView : UserControl, INotifyPropertyChanged
    {
        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;

                OnPropertyChanged("SelectedIndex");
            }
        }

        private List<string> _dataChartName;
        public List<string> DataChartName { 
            get => _dataChartName;
            set {
                _dataChartName = value;
                OnPropertyChanged("DataChartName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<ChartView> ChartHistory = new List<ChartView>()
        {
            new ChartView(1, 1, 0),
            new ChartView(1, 2, 0),
            new ChartView(3, 1, 0),
            new ChartView(3, 2, 0),
        };
        private List<ChartView> GrooveCount = new List<ChartView>()
        {
            new ChartView(1, 0, 1),
            new ChartView(1, 4, 1),
            new ChartView(1, 8, 1),
            new ChartView(1, 12, 1),
        };
        private List<ChartView> LineCut = new List<ChartView>()
        {
            //new ChartView(1, 0, 1),
            //new ChartView(1, 4, 1),
            //new ChartView(1, 8, 1),
            //new ChartView(1, 12, 1),

        };


        private List<List<ChartView>> ChartViewModelList = new List<List<ChartView>>();


        public SelectView()
        {
            InitializeComponent();
            ChartViewModelList.Add(ChartHistory);
            ChartViewModelList.Add(GrooveCount);
            DataChartName = new List<string>();
            DataChartName.Add("a");
            DataChartName.Add("b");
            DataChartName.Add("v");

            DataContext = this;
            
            content1.Content = ChartViewModelList[SelectedIndex][0]; //Activator.CreateInstance(null, "ChartView").Unwrap();
            content2.Content = ChartViewModelList[SelectedIndex][1]; //(ChartView)Activator.CreateInstance(null, $"ChartDemoWithCommand.ChartView").Unwrap();
            content3.Content = ChartViewModelList[SelectedIndex][2];
            content4.Content = ChartViewModelList[SelectedIndex][3];
        }

        private void ChangeTalbe_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = TableComboBox.SelectedIndex;
            content1.Content = ChartViewModelList[SelectedIndex][0];
            content2.Content = ChartViewModelList[SelectedIndex][1];
            content3.Content = ChartViewModelList[SelectedIndex][2];
            content4.Content = ChartViewModelList[SelectedIndex][3];
        }
    }
}
