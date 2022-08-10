using Basler.Pylon;
using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Views;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace Frism_Inspection_Renew
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MainModel mainModel;

        public ICommand SetCameraBtn { get; set; }
        public ICommand InitializeBtn { get; set; }
        public ICommand StopInspectBtn { get; set; }
        public ICommand StartInspectBtn { get; set; }
        public ICommand ShowHistoryBtn { get; set; }
        public ICommand InspectionSettingBtn { get; set; }
        public ICommand InspectCountResetBtn { get; set; }
        public String outputTxt { get; set; }


        public String OutputTxt
        {
            get
            {
                return outputTxt;
            }
            set
            {
                outputTxt = value;
                OnPropertyChanged("OutputTxt");
            }
        }



        public MainViewModel()
        {

            InitVariables();
            InitCommands();
        }



        public void InitVariables()
        {
            try
            {
              
                mainModel = new MainModel();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void InitCommands()
        {
            try
            {
                SetCameraBtn = new Command(SetCameraBtnRun, CanExecute_func);
                InitializeBtn = new Command(InitializeBtnRun, CanExecute_func);
                StopInspectBtn = new Command(StopInspectBtnRun, CanExecute_func);
                StartInspectBtn = new Command(StartInspectBtnRun, CanExecute_func);
                ShowHistoryBtn = new Command(ShowHistoryBtnRun, CanExecute_func);
                InspectionSettingBtn = new Command(InspectionSettingBtnRun, CanExecute_func);
                InspectCountResetBtn = new Command(InspectCountResetBtnRun, CanExecute_func);
            }
            catch (Exception e)
            {
                throw e;
            }
        }




        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetCameraBtnRun(object obj)
        {
            // call  mainModel.SetCamera to open
            Console.WriteLine("SetCameraBtnRun");
        }

        private void InitializeBtnRun(object obj)
        {
            Console.WriteLine("InitializeBtnRun");
        }        

        private void StopInspectBtnRun(object obj)
        {
            Console.WriteLine("StopInspectBtnRun");
        }

        private void StartInspectBtnRun(object obj)
        {
            Console.WriteLine("Insp Start");
        }

        private void ShowHistoryBtnRun(object obj)
        {
            Console.WriteLine("ShowHistoryBtnRun");
        } 

        private void InspectionSettingBtnRun(object obj)
        {
            Console.WriteLine("InspectionSettingBtnRun");
        }
        
        private void InspectCountResetBtnRun(object obj)
        {
            Console.WriteLine("InspectCountResetBtnRun");
        }

        private bool CanExecute_func(object obj)
        {
            return true;
        }








    }



}
