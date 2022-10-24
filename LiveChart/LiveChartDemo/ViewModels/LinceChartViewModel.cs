using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using Microsoft.Office.Interop.Excel;

namespace LiveChartDemo.ViewModels
{
    public partial class LinceChartViewModel: INotifyPropertyChanged
    {
        string filePath = @"C:\Users\sales\Downloads\K2 FW MX SHEET (R_CH합본)_220628.xlsx";
        private ChartValues<double> _values;

        public LinceChartViewModel()
        {
            Values = new ChartValues<double> { };
            OpenFile();
        }

        public ChartValues<double> Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                OnPropertyChanged("Values");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenFile()
        {
            if (filePath != "")
            {
                double[] keepValue = new double[] { };
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Open(Filename: @filePath);
                Worksheet worksheet1 = workbook.Worksheets.get_Item(5);

                application.Visible = false;
               
                int row = worksheet1.UsedRange.EntireRow.Count;
                Console.WriteLine(row);
                //Range rng = worksheet1.Range[ws.Cells[1, 1], ws.Cells[row, numOfColumn]];
                
                Range startRange = worksheet1.Cells[5, 3];
                Range endRange = worksheet1.Cells[100, 3];

                Range range = worksheet1.get_Range(startRange, endRange);
                object[,] rawData = range.Value;

                String data = "";

                Console.WriteLine(rawData.GetLength(0));
                Console.WriteLine(rawData.GetLength(1));

                for (int i = 1; i <= rawData.GetLength(0); ++i)
                {
                    for (int j = 1; j <= rawData.GetLength(1); ++j)
                    {
                        if (rawData[i, j] == null) continue;
                        //keepValue = keepValue.Concat(new double[] { (double)rawData[i, j] }).ToArray();
                        data += (rawData[i, j].ToString() + " ");
                        Values.Add((double)rawData[i, j]);
                    }
                    //Console.WriteLine(data);
                    data += "\n";
                }
            }
        }

        void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    // 객체 메모리 해제
                    Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                obj = null;
                throw ex;
            }
            finally
            {
                GC.Collect();   // 가비지 수집
            }
        }
    }
}
