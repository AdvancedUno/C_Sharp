using ChartDemoWithCommand.Command;
using LiveCharts.Events;
using LiveCharts;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ChartDemoWithCommand.Model;

namespace ChartDemoWithCommand.ViewModel
{
    public class GrooveCountViewModel
    {
        string filePath = @"C:\Users\sales\Downloads\K2 FW MX SHEET (R_CH합본)_220628.xlsx";
        



        private List<ChartValues<double>> _saveChartsList;
        public List<ChartValues<double>> SaveChartsList { get => _saveChartsList; set => _saveChartsList = value; }

        public string[] Labels { get; set; }

        public MyCommand<ChartPoint> DataHoverCommand { get; set; }
        public MyCommand<ChartPoint> DataClickCommand { get; set; }
        public MyCommand<LiveCharts.Wpf.CartesianChart> UpdaterTickCommand { get; set; }
        public MyCommand<RangeChangedEventArgs> RangeChangedCommand { get; set; }

        public Func<double, string> XFormatter { get; set; }

        private ChartInfoModel _chartInfo;
        public ChartInfoModel ChartInfo
        {
            get => _chartInfo;
            set => _chartInfo = value;
        }

        public GrooveCountViewModel(int chartIndexNumber, int ColumNumber)
        {
            try
            {

                ChartInfo = new ChartInfoModel();
                SaveChartsList = new List<ChartValues<double>>();

                SaveChartsList.Add(ChartInfo.FirstValues);
                SaveChartsList.Add(ChartInfo.SecondValues);

                ChartInfo.ChartIndexNumber = chartIndexNumber;

                for (int i = 0; i < 2; i++)
                {
                    OpenFile(i, i * 2 + 13 + ColumNumber);
                }

                OpenFileName();

                //ChartName = ChartNameList[chartIndexNumber - 1][ColumNumber];

                Labels = new[] { "Chrome", "Mozilla", "Opera", "IE" };

                DataClickCommand = new MyCommand<ChartPoint>
                {
                    ExecuteDelegate = p => Console.WriteLine(
                        "[COMMAND] you clicked " + p.X + ", " + p.Y + "aaaaaaaaaaaa")
                };
                DataHoverCommand = new MyCommand<ChartPoint>
                {
                    ExecuteDelegate = p => Console.WriteLine(
                        "[COMMAND] you hovered over " + p.X + ", " + p.Y)
                };
                UpdaterTickCommand = new MyCommand<LiveCharts.Wpf.CartesianChart>
                {
                    ExecuteDelegate = c => Console.WriteLine("[COMMAND] Chart was updated!")
                };
                RangeChangedCommand = new MyCommand<RangeChangedEventArgs>
                {
                    ExecuteDelegate = e => Console.WriteLine("[COMMAND] Axis range changed")
                };

                XFormatter = val => new DateTime((long)val).ToString("dd MMM");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenFile(int chartIndex, int chartID)
        {
            if (filePath != "")
            {
                double[] keepValue = new double[] { };
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Open(Filename: @filePath);
                Worksheet worksheet1 = workbook.Worksheets.get_Item(ChartInfo.ChartIndexNumber);

                application.Visible = false;

                int row = worksheet1.UsedRange.EntireRow.Count;
                //Console.WriteLine(row);
                //Range rng = worksheet1.Range[ws.Cells[1, 1], ws.Cells[row, numOfColumn]];


                Range startRange = worksheet1.Cells[10, chartID];
                Range endRange = worksheet1.Cells[row, chartID];

                Range range = worksheet1.get_Range(startRange, endRange);
                object[,] rawData = range.Value;

                //Console.WriteLine(rawData.GetLength(0));
                //Console.WriteLine(rawData.GetLength(1));

                String data = "";
                for (int i = 1; i <= rawData.GetLength(0); ++i)
                {
                    for (int j = 1; j <= rawData.GetLength(1); ++j)
                    {
                        if (rawData[i, j] == null) continue;
                        //keepValue = keepValue.Concat(new double[] { (double)rawData[i, j] }).ToArray();
                        data += (rawData[i, j].ToString() + " ");
                        SaveChartsList[chartIndex].Add((double)rawData[i, j]);
                    }
                    //Console.WriteLine(data);
                    data += "\n";
                }

                workbook.Close();
                application.Quit();

                ReleaseObject(application);
                ReleaseObject(workbook);
                ReleaseObject(worksheet1);
                ReleaseObject(startRange);
                ReleaseObject(endRange);

            }
        }


        public void OpenFileName()
        {
            if (filePath != "")
            {
                double[] keepValue = new double[] { };
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Open(Filename: @filePath);
                Worksheet worksheet1 = workbook.Worksheets.get_Item(ChartInfo.ChartIndexNumber);

                application.Visible = false;

                int col = worksheet1.UsedRange.EntireColumn.Count;
                //int row = worksheet1.UsedRange.EntireRow.Count;
                //Console.WriteLine(row);
                //Range rng = worksheet1.Range[ws.Cells[1, 1], ws.Cells[row, numOfColumn]];

                Range startTableNameRange = worksheet1.Cells[4, 4];
                Range endTableNameRange = worksheet1.Cells[4, col];

                Range tableNameRange = worksheet1.get_Range(startTableNameRange, endTableNameRange);
                object[,] tableNameData = tableNameRange.Value;

                String tableData = "";

                for (int i = 1; i <= tableNameData.GetLength(0); ++i)
                {
                    for (int j = 1; j <= tableNameData.GetLength(1); ++j)
                    {
                        if (tableNameData[i, j] != null)
                        {
                            Console.WriteLine(tableNameData[i, j]);
                            tableData += (tableNameData[i, j].ToString() + " ");
                            ChartInfo.DataChartName.Add(tableNameData[i, j].ToString());
                        }
                    }
                    tableData += "\n";
                }
                workbook.Close();
                application.Quit();

                ReleaseObject(application);
                ReleaseObject(workbook);
                ReleaseObject(worksheet1);
                ReleaseObject(startTableNameRange);
                ReleaseObject(endTableNameRange);
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
