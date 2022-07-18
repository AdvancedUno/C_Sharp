using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frism
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DnnSetting : Window
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        DnnSetClass dnnSetClass = null;
        MainWindow mainWindow = null;
        private bool bSnapshotCheck = false;



        List<string> basicInfo;

        public DnnSetting(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            #if DEBUG
            InspectCheckBox.Visibility = Visibility.Visible;
            SnapShotCheckBox.Visibility = Visibility.Visible;

            #endif



        basicInfo = DBAcess.GiveBasicSettings("0");

            if (basicInfo.Count() >0)
            {
                //MaxThreadCountTxtBox.Text = basicInfo[0];
                MaxTileWidthTxtBox.Text = basicInfo[1];
                MaxTileHeightTxtBox.Text = basicInfo[2];
                GpuNumberTxtBox.Text = basicInfo[3];
                MinDefectNumTxtBoxTop.Text = basicInfo[4];
                MinPValueTxtBoxTop.Text = basicInfo[5];
                MinDefectNumTxtBoxSide.Text = basicInfo[6];
                MinPValueTxtBoxSide.Text = basicInfo[7];
            }

            string tempPath = DBAcess.GiveFilePath("0");
            if(tempPath != null)
            {
                FolderPathTxt.Text = "저장 경로: " + tempPath;
            }
            

        }

        public void SetClass(DnnSetClass ClasInfo)
        {
            dnnSetClass = ClasInfo;
        }


        private void ShowException(Exception exception)
        {
            Logger.Error(exception.Message);

            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        //////////////////  DNN 설정 정보를 MainWindow 에 전달 ////////////////////

        private void DnnApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if ( MaxTileWidthTxtBox.Text != null && MaxTileHeightTxtBox.Text != null && GpuNumberTxtBox.Text != null)
                {
                    dnnSetClass.passDnnSet(4, Int32.Parse(MaxTileWidthTxtBox.Text),
                    Int32.Parse(MaxTileHeightTxtBox.Text), Int32.Parse(GpuNumberTxtBox.Text), Int32.Parse(MinDefectNumTxtBoxTop.Text), 
                    float.Parse(MinPValueTxtBoxTop.Text), Int32.Parse(MinDefectNumTxtBoxSide.Text), float.Parse(MinPValueTxtBoxSide.Text));
                    basicInfo = new List<string>();
                    basicInfo.Add("4");
                    basicInfo.Add(MaxTileWidthTxtBox.Text);
                    basicInfo.Add(MaxTileHeightTxtBox.Text);
                    basicInfo.Add(GpuNumberTxtBox.Text);
                    basicInfo.Add(MinDefectNumTxtBoxTop.Text);
                    basicInfo.Add(MinPValueTxtBoxTop.Text);
                    basicInfo.Add(MinDefectNumTxtBoxSide.Text);
                    basicInfo.Add(MinPValueTxtBoxSide.Text);
                    int iCheckFileExist = DBAcess.InsertBasicSet("0", basicInfo);
                    
                    if(iCheckFileExist < 1)
                    {
                        
                        DBAcess.UpdateDataBaseBasic("0", basicInfo);
                    }

                }
                else
                {

                }
            }
            catch (Exception exception)
            {
                //ShowException(exception);
                Logger.Error(exception.Message + " DnnApplyBtn_Click");
            }


            this.Close();
        }


        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderPath = new FolderBrowserDialog();
            string folderPathTemp = DBAcess.GiveFilePath("0");

            folderPath.SelectedPath = folderPathTemp;
            folderPath.ShowDialog();
            if (folderPath.SelectedPath != "")
            {
                Program.saveFolderPath = folderPath.SelectedPath;
                FolderPathTxt.Text = "Folder Path: " + folderPath.SelectedPath;



                int iCheck = DBAcess.InsertFilePath("0", folderPath.SelectedPath); ;


                if (iCheck < 1)
                {
                    DBAcess.UpdateDataFilePath("0", folderPath.SelectedPath);
                }




                if (!Directory.Exists(Program.saveFolderPath))
                {
                    Directory.CreateDirectory(Program.saveFolderPath);
                }

            }
            else
            {

            }

            if (Program.saveFolderPath == "" || Program.saveFolderPath == null)
            {
                System.Windows.Forms.MessageBox.Show("Please Select a Folder.");
                return;
            }
        }


        private void InspectCheckBox_Checked(object sender, RoutedEventArgs e)
        {

            if (InspectCheckBox.IsChecked.Value)
            {
                Program.bInspectBtn = true;
            }
            
        }



        private void InspectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (InspectCheckBox.IsChecked.Value == false)
            {
                Program.bInspectBtn = false;


            }

        }




        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void SnapShotCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (bSnapshotCheck)
            {
                bSnapshotCheck = false;
                mainWindow.ShowSnapShotBtn();
            }
                
        }





        private void SnapShotCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!bSnapshotCheck)
            {
                bSnapshotCheck = true;
                mainWindow.ShowSnapShotBtn();
            }
            
        }
    }
}
