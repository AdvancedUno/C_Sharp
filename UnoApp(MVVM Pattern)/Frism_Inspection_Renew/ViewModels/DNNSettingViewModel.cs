using Frism_Inspection_Renew.Commands;
using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;



namespace Frism_Inspection_Renew.ViewModels
{
    public class DNNSettingViewModel : ViewModelBase
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand ApplySettingBtn { get; }
        public ICommand ChangeFolderPathBtn { get; }

        private List<string> _basicInfo;
        public List<string> BasicInfo { get => _basicInfo; set => _basicInfo = value; }
        

        private string _maxTileWidthTxt;
        public string MaxTileWidthTxt {
            get => _maxTileWidthTxt;
            set
            {
                _maxTileWidthTxt = value;
                OnPropertyChanged(MaxTileWidthTxt);
            }
        }

        private string _maxTileHeightTxt;
        public string MaxTileHeightTxt
        {
            get => _maxTileHeightTxt;
            set
            {
                _maxTileHeightTxt = value;
                OnPropertyChanged(MaxTileHeightTxt);
            }
        }


        private string _gpuNumberTxt;
        public string GpuNumberTxt
        {
            get => _gpuNumberTxt;
            set
            {
                _gpuNumberTxt = value;
                OnPropertyChanged(GpuNumberTxt);
            }
        }

        private string _minDefectNumTxtBoxTopTxt;
        public string MinDefectNumTxtBoxTopTxt
        {
            get => _minDefectNumTxtBoxTopTxt;
            set
            {
                _minDefectNumTxtBoxTopTxt = value;
                OnPropertyChanged(MinDefectNumTxtBoxTopTxt);
            }
        }

        private string _minPValueTxtBoxTopTxt;
        public string MinPValueTxtBoxTopTxt
        {
            get => _minPValueTxtBoxTopTxt;
            set
            {
                _minPValueTxtBoxTopTxt = value;
                OnPropertyChanged(MinPValueTxtBoxTopTxt);
            }
        }


        private string _minDefectNumTxtBoxSideTxt;
        public string MinDefectNumTxtBoxSideTxt
        {
            get => _minDefectNumTxtBoxSideTxt;
            set
            {
                _minDefectNumTxtBoxSideTxt = value;
                OnPropertyChanged(MinDefectNumTxtBoxSideTxt);
            }
        }        
        
        private string _minPValueTxtBoxSideTxt;
        public string MinPValueTxtBoxSideTxt
        {
            get => _minPValueTxtBoxSideTxt;
            set
            {
                _minPValueTxtBoxSideTxt = value;
                OnPropertyChanged(MinPValueTxtBoxSideTxt);
            }
        }


        private string _folderPathTxt;
        public string FolderPathTxt
        {
            get => _folderPathTxt;
            set
            {
                _folderPathTxt = value;
                OnPropertyChanged(FolderPathTxt);
            }
        }

        public SideBarViewModel SideBarViewModel { get; }
        public DNNSettingViewModel(SideBarViewModel sideBarViewModel)
        {
            //NavigateHomeCommand = new NavigateCommand<MainViewModel>
            //    (new NavigationService<MainViewModel>
            //    (navigationStore, () => new MainViewModel(navigationStore)
            //    ));
            SideBarViewModel = sideBarViewModel;
            

            ApplySettingBtn = new Command(ApplySettingBtnRun, CanExecute_func);
            ChangeFolderPathBtn = new Command(ChangeFolderPathBtnRun, CanExecute_func);

            InitVariables();

           
        }

        private void InitVariables()
        {
            try
            {
                BasicInfo = DBAcess.GiveBasicSettings("0");

                if (BasicInfo.Count() > 0)
                {
                    //MaxThreadCountTxtBox.Text = basicInfo[0];
                    MaxTileWidthTxt = BasicInfo[1];
                    MaxTileHeightTxt = BasicInfo[2];
                    GpuNumberTxt = BasicInfo[3];
                    MinDefectNumTxtBoxTopTxt = BasicInfo[4];
                    MinPValueTxtBoxTopTxt = BasicInfo[5];
                    MinDefectNumTxtBoxSideTxt = BasicInfo[6];
                    MinPValueTxtBoxSideTxt = BasicInfo[7];
                }

                string tempPath = DBAcess.GiveFilePath("0");
                if (tempPath != null)
                {
                    FolderPathTxt = "저장 경로: " + tempPath;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " InitVariables");
            }
            
        }

        private bool CanExecute_func(object obj)
        {
            return true;
        }


        private void ApplySettingBtnRun(object obj)
        {


            Console.WriteLine("ApplySettingBtnRun");

            try
            {

                if (MaxTileWidthTxt != null && MaxTileHeightTxt != null && GpuNumberTxt != null)
                {
                    BasicInfo = new List<string>();
                    BasicInfo.Add("4");
                    BasicInfo.Add(MaxTileWidthTxt);
                    BasicInfo.Add(MaxTileHeightTxt);
                    BasicInfo.Add(GpuNumberTxt);
                    BasicInfo.Add(MinDefectNumTxtBoxTopTxt);
                    BasicInfo.Add(MinPValueTxtBoxTopTxt);
                    BasicInfo.Add(MinDefectNumTxtBoxSideTxt);
                    BasicInfo.Add(MinPValueTxtBoxSideTxt);
                    int iCheckFileExist = DBAcess.InsertBasicSet("0", BasicInfo);

                    if (iCheckFileExist < 1)
                    {

                        DBAcess.UpdateDataBaseBasic("0", BasicInfo);
                    }

                }
                else
                {

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " ApplySettingBtnRun"); 
                //ShowException(exception);
                //Logger.Error(exception.Message + " DnnApplyBtn_Click");
            }




            //NavigateHomeCommand.Execute(obj);   
        }        
        private void ChangeFolderPathBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("ApplySettingBtnRun");
                FolderBrowserDialog folderPath = new FolderBrowserDialog();
                string folderPathTemp = DBAcess.GiveFilePath("0");

                folderPath.SelectedPath = folderPathTemp;
                folderPath.ShowDialog();
                if (folderPath.SelectedPath != "")
                {
                    //Program.saveFolderPath = folderPath.SelectedPath;
                    //FolderPathTxt.Text = "Folder Path: " + folderPath.SelectedPath;



                    int iCheck = DBAcess.InsertFilePath("0", folderPath.SelectedPath); ;


                    if (iCheck < 1)
                    {
                        DBAcess.UpdateDataFilePath("0", folderPath.SelectedPath);
                    }




                    //if (!Directory.Exists(Program.saveFolderPath))
                    //{
                    //    Directory.CreateDirectory(Program.saveFolderPath);
                    //}

                }
                else
                {

                }

                //if (Program.saveFolderPath == "" || Program.saveFolderPath == null)
                //{
                //    System.Windows.Forms.MessageBox.Show("Please Select a Folder.");
                //    return;
                //}
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " ChangeFolderPathBtnRun");
            }

           

        }

    }
}
