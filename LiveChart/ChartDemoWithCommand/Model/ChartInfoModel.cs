using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartDemoWithCommand.Model
{
    public partial class ChartInfoModel: INotifyPropertyChanged
    {
        private string _filePath = @"D:\UnoPrivate\C_Sharp\LiveChart\K2 FW MX SHEET (R_CH합본)_220628 (1).xlsx";
        public string FilePath { get => _filePath; set => _filePath = value; }
        


        private ChartValues<double> _firstValues;
        public ChartValues<double> FirstValues
        {
            get
            {
                return _firstValues;
            }
            set
            {
                _firstValues = value;
                OnPropertyChanged("FirstValues");
            }
        }

        private ChartValues<double> _secondValues;
        public ChartValues<double> SecondValues
        {
            get
            {
                return _secondValues;
            }
            set
            {
                _secondValues = value;
                
                OnPropertyChanged("SecondValues");
            }
        }


        private ChartValues<double> _thirdValues;
        public ChartValues<double> ThirdValues
        {
            get
            {
                return _thirdValues;
            }
            set
            {
                _thirdValues = value;
                OnPropertyChanged("ThirdValues");
            }
        }


        private ChartValues<double> _fourthValues;
        public ChartValues<double> FourthValues
        {
            get
            {
                return _fourthValues;
            }
            set
            {
                _fourthValues = value;
                OnPropertyChanged("FourthValues");
            }
        }

        private string _chartNemt;

        public string ChartName
        {
            get
            {
                return _chartNemt;
            }
            set
            {
                _chartNemt = value;
                OnPropertyChanged("ChartName");
            }
        }

        public int ChartIndexNumber;

        private List<string> _dataChartName;
        public List<string> DataChartName { get => _dataChartName; set => _dataChartName = value; }

        public List<List<string>> ChartNameList = new List<List<string>>() {
            new List<string>() { "RC1 0", "RC1 90" },
            new List<string>() { "RC1 0", "RC1 90" },
            new List<string>() { "RC2 0", "RC2 90"},
        };

        private List<string> _timeLabel_X;
        public List<string> TimeLabel_X
        {
            get => _timeLabel_X;
            set
            {
                _timeLabel_X = value;
                OnPropertyChanged("TimeLabel_X");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ChartInfoModel()
        {
            FirstValues = new ChartValues<double>();
            SecondValues = new ChartValues<double>();
            ThirdValues = new ChartValues<double>();
            FourthValues = new ChartValues<double>();
            DataChartName = new List<string>();
            TimeLabel_X = new List<string>();

        }



    }
}
