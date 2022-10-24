﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using LiveCharts;
using LiveCharts.Events;
using ChartDemoWithCommand.Command;
using Microsoft.Office.Interop.Excel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Diagnostics;
using LiveCharts.Charts;
using ChartDemoWithCommand.Model;

namespace ChartDemoWithCommand.ViewModel
{
    public partial class ChartViewModel:INotifyPropertyChanged
    {
        

        private List<ChartValues<double>> _saveChartsList;
        public List<ChartValues<double>> SaveChartsList { get => _saveChartsList; set => _saveChartsList = value; }

        public MyCommand<ChartPoint> DataHoverCommand { get; set; }
        public MyCommand<ChartPoint> DataClickCommand { get; set; }
        public MyCommand<LiveCharts.Wpf.CartesianChart> UpdaterTickCommand { get; set; }
        public MyCommand<RangeChangedEventArgs> RangeChangedCommand { get; set; }

        public Func<double, string> XFormatter { get; set; }

        private ChartInfoModel _chartInfo;
        public ChartInfoModel ChartInfo { 
            get => _chartInfo; 
            set => _chartInfo = value; 
        }

        public ChartViewModel(int chartIndexNumber, int ColumNumber)
        {
            try
            {
                ChartInfo = new ChartInfoModel();
                SaveChartsList = new List<ChartValues<double>>();
                SaveChartsList.Add(ChartInfo.FirstValues);
                SaveChartsList.Add(ChartInfo.SecondValues);
                SaveChartsList.Add(ChartInfo.ThirdValues);
                SaveChartsList.Add(ChartInfo.FourthValues);

                ChartInfo.ChartIndexNumber = chartIndexNumber;

                for (int i = 0; i < 4; i++)
                {
                    OpenFile(i, i + 4 * ColumNumber);
                }

                OpenFileName();

                ChartInfo.ChartName = ChartInfo.ChartNameList[chartIndexNumber - 1][ColumNumber - 1];


                

                DataClickCommand = new MyCommand<ChartPoint>
                {

                    ExecuteDelegate = p => Console.WriteLine(
                        "[COMMAND] you clicked " + p.X + ", " + p.Y)
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

                ChartInfo.TimeLabel_X.Add("aaa");


                XFormatter = val => new DateTime((long)val).ToString("dd MMM");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OpenFile(int chartIndex, int chartID)
        {
            if (ChartInfo.FilePath != "")
            {
                double[] keepValue = new double[] { };
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Open(Filename: @ChartInfo.FilePath);
                Worksheet worksheet1 = workbook.Worksheets.get_Item(ChartInfo.ChartIndexNumber);

                application.Visible = false;

                //int row = worksheet1.UsedRange.EntireRow.Count;
                //Console.WriteLine(row);
                //Range rng = worksheet1.Range[ws.Cells[1, 1], ws.Cells[row, numOfColumn]];

                Range startRange = worksheet1.Cells[15, chartID];
                Range endRange = worksheet1.Cells[50, chartID];

                Range range = worksheet1.get_Range(startRange, endRange);
                object[,] rawData = range.Value;

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
            if (ChartInfo.FilePath != "")
            {
                double[] keepValue = new double[] { };
                Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = application.Workbooks.Open(Filename: @ChartInfo.FilePath);
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
                    //Console.WriteLine(data);
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
