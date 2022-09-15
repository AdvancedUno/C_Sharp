
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Windows.Controls;
using ISeries = LiveChartsCore.ISeries;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

namespace ExampleChart
{
    public class ViewModel
    {
        string filePath = @"C:\Users\sales\Downloads\K2 FW MX SHEET (R_CH합본)_220628.xlsx";

        public ViewModel()
        {
            OpenFile();
        }






        public ISeries[] SeriesVal { get; set; } =
        {
            new LineSeries<double>
            {
                Values =  new double[] {4, 6, 5, 3, -3, -1, 2},
                Fill = null
            }
        };



        //public List<ISeries> SeriesVal { get; set; }
        //    = new List<ISeries>();

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
                LineSeries<double> test = new LineSeries<double>{
                    Values = new double[] {},
                    Fill = null
                };



                Range startRange = worksheet1.Cells[5, 3];
                Range endRange = worksheet1.Cells[50, 3];

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
                        keepValue = keepValue.Concat(new double[] { (double)rawData[i, j] }).ToArray();
                        //test.Values.Append((double)rawData[i, j]);
                        data += (rawData[i, j].ToString() + " ");
                    }
                    //Console.WriteLine(data);
                    data += "\n";
                }

                

                test.Values = keepValue;

                ISeries temp = test;

                SeriesVal[0] = temp;
                

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
