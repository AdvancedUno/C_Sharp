using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ChartDemoWithCommand.ViewModel;
using LiveCharts;
using LiveCharts.Events;
using LiveCharts.Wpf;


namespace ChartDemoWithCommand.View
{
    /// <summary>
    /// ChartView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChartView : UserControl, INotifyPropertyChanged
    {
        //private int _selectedIndex = 0;
        //public int SelectedIndex { 
        //    get => _selectedIndex;
        //    set 
        //    {
        //        _selectedIndex = value;     
        //        OnPropertyChanged("SelectedIndex");
        //    } 
        //}

        public ChartView(int TableID, int ChartID, int ChartType)
        {
            InitializeComponent();

            if (ChartType == 0)
            {
                DataContext = new ChartViewModel(TableID, ChartID);
            }
            else if(ChartType == 1)
            {
                DataContext = new GrooveCountViewModel(TableID, ChartID);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChartOnDataClick(object sender, ChartPoint p)
        {
            var asPixels = Chart.ConvertToPixels(p.AsPoint());
            Console.WriteLine("[EVENT] You clicked (" + p.X + ", " + p.Y + ") in pixels (" +
                            asPixels.X + ", " + asPixels.Y + ")");
        }

        private void Chart_OnDataHover(object sender, ChartPoint p)
        {
            Console.WriteLine("[EVENT] you hovered over " + p.X + ", " + p.Y);
        }

        private void ChartOnUpdaterTick(object sender)
        {
            Console.WriteLine("[EVENT] chart was updated");
        }

        private void Axis_OnRangeChanged(RangeChangedEventArgs eventargs)
        {
            Console.WriteLine("[EVENT] axis range changed");
        }

        private void ChartMouseMove(object sender, MouseEventArgs e)
        {
            var point = Chart.ConvertToChartValues(e.GetPosition(Chart));

            //X.Text = point.X.ToString("N");
            //Y.Text = point.Y.ToString("N");
        }
    }
}
