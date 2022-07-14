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



        


        public MainViewModel(Sub_MainView sub_MainView1, Sub_MainView sub_MainView2, Sub_MainView sub_MainView3, Sub_MainView sub_MainView4)
        {
            SetCameraBtn = new Command(SetCameraBtnRun, CanExecute_func);
            InitializeBtn = new Command(InitializeBtnRun, CanExecute_func);
            StopInspectBtn = new Command(StopInspectBtnRun, CanExecute_func);
            StartInspectBtn = new Command(StartInspectBtnRun, CanExecute_func);
            ShowHistoryBtn = new Command(ShowHistoryBtnRun, CanExecute_func);
            InspectionSettingBtn = new Command(InspectionSettingBtnRun, CanExecute_func);
            InspectCountResetBtn = new Command(InspectCountResetBtnRun, CanExecute_func);
            mainModel = new MainModel(sub_MainView1, sub_MainView2, sub_MainView3, sub_MainView4);
            InitVariables();



        }



        public void InitVariables()
        {
            try
            {


                mainModel.ledSource = new LEDWindow();
                mainModel.cntTime = new CntTime();
                mainModel.dnnSetClass = new DnnSetClass();
                mainModel.cntNGClass = new CntNGClass();
                mainModel.showSigClass = new ShowSignal();
                //mainModel.timerMain = new DispatcherTimer();

                //timeS = new Stopwatch();

                //timer = new DispatcherTimer();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " InitVariables()");
                throw e;
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ShowException(Exception exception)
        {
            Logger.Error(exception.Message);

            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        /// <summary>
        /// Open a Window for setting cameras
        /// </summary>
        /// <param name="obj"></param>
        private void SetCameraBtnRun(object obj)
        {
            
        }


        /// <summary>
        /// Start Initializing the program
        /// </summary>
        /// <param name="obj"></param>
        private void InitializeBtnRun(object obj)
        {

        }        


        /// <summary>
        /// Stop Inspection and stop cameras from capturing images, but do not close cameras
        /// </summary>
        /// <param name="obj"></param>
        private void StopInspectBtnRun(object obj)
        {

        }
        


        /// <summary>
        /// Start Inspection and turn on the LED Light to show the User that the program is ready to inspect objects
        /// </summary>
        /// <param name="obj"></param>
        private void StartInspectBtnRun(object obj)
        {

        }


        /// <summary>
        /// Open a History Window that will allow user to check the history of resulted images from the inspection.
        /// </summary>
        /// <param name="obj"></param>
        private void ShowHistoryBtnRun(object obj)
        {

        } 



        /// <summary>
        /// Open an InspectionSetting Window where Users can set parameters for the inspection.
        /// </summary>
        /// <param name="obj"></param>
        private void InspectionSettingBtnRun(object obj)
        {

        }
        

        /// <summary>
        /// When clicked, resets the Ok and NG image counts, and also makes the MainView Window to inspection read position
        /// </summary>
        /// <param name="obj"></param>
        private void InspectCountResetBtnRun(object obj)
        {

        }



        private bool CanExecute_func(object obj)
        {
            return true;
        }





        /// <summary>
        /// Update Camera infos and add them to the CameraInfosDictionary
        /// </summary>
        public void UpdateDeviceList()
        {
            Logger.Debug("Update Device List");

            try
            {

                List<ICameraInfo> cameraInfos = CameraFinder.Enumerate();

                if (cameraInfos.Count > 0)
                {

                    mainModel.CameraInfosDictionary = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in cameraInfos)
                    {

                        mainModel.CameraInfosDictionary.Add(cameraInfo, cameraInfo[CameraInfoKey.FriendlyName]);

                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " UpdateDeviceList MainW");

            }
        }



        public void UpdateCameraList()
        {
            Logger.Debug("Update Camera List");
            try
            {
                mainModel.SavedInfosList = DBAcess.CamInfos();

                CameraInfoBox.ItemsSource = savedInfos;
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

        }


        ///////////////////// 카메라 업데이트 및 노출 값 설정 ////////////////////////////
        private void UpdateCamera(List<string> camIDList, List<string> camExTime)
        {

            bool testDNN = true;
            Logger.Debug("Update Start");
            if (CamID == null)
            {
                Logger.Debug("CamID");
                System.Windows.Forms.MessageBox.Show("Please Select The Setting");
                return;
            }

            if (CameraInfos == null)
            {
                FirstView.FirstCreateCamera(null);
                SecondView.FirstCreateCamera(null);
                ThirdView.FirstCreateCamera(null);
                FourthView.FirstCreateCamera(null);
                Logger.Debug("CameraInfos");
                return;
            }
            Logger.Debug("Update and Open Cameras");





            try
            {

                foreach (ICameraInfo cameraInfo in CameraInfos.Keys)
                {
                    //Logger.Debug("Upp");
                    if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[0])
                    {
                        FirstView.FirstCreateCamera(cameraInfo);
                        if (camExTime[1] != null)
                        {


                            double exposureT = (double.Parse(camExTime[0]));
                            FirstView.WindowImage.GiveUno().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);


                        }


                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[1])
                    {
                        SecondView.FirstCreateCamera(cameraInfo);
                        if (camExTime[1] != null)
                        {
                            double exposureT = (double.Parse(camExTime[1]));
                            SecondView.WindowImage.GiveUno().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);

                        }


                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[2])
                    {
                        ThirdView.FirstCreateCamera(cameraInfo);
                        if (camExTime[2] != null)
                        {
                            double exposureT = (double.Parse(camExTime[2]));
                            ThirdView.WindowImage.GiveUno().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);

                        }
                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[3])
                    {
                        FourthView.FirstCreateCamera(cameraInfo);
                        if (camExTime[3] != null)
                        {
                            double exposureT = (double.Parse(camExTime[3]));
                            FourthView.WindowImage.GiveUno().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);

                        }
                    }

                }
                Logger.Debug("Update and Open Cameras Finished");


            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

        }


        ///////////////////// LED 조명 설정 및 실행 ////////////////////////////
        private void UpdateLED(List<string> LEDidList)
        {

            if (ledSource == null)
                ledSource = new LEDWindow();

            Logger.Debug("Update and Open LED");
            try
            {

                ledSource.MainCom_Open("COM9", 19200);



            }
            catch (Exception exception)
            {

                ShowException(exception);
            }


        }


        private void UpdateDnnFiles(List<string> DnnFileList)
        {

            try
            {

                Logger.Debug("Update and Assign DNN Files");
                FirstView.WindowImage.iThreadID = 0;
                SecondView.WindowImage.iThreadID = 1;
                ThirdView.WindowImage.iThreadID = 2;
                FourthView.WindowImage.iThreadID = 3;


                FirstView.WindowImage.sDnnPath = DnnFileList[0];
                SecondView.WindowImage.sDnnPath = DnnFileList[1];
                ThirdView.WindowImage.sDnnPath = DnnFileList[2];
                FourthView.WindowImage.sDnnPath = DnnFileList[3];

                DnnToptxt.Text = DnnFileList[0];
                DnnFirstTxt.Text = DnnFileList[1];
                DnnSecondTxt.Text = DnnFileList[2];
                DnnThirdTxt.Text = DnnFileList[3];

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDnnFiles MainW");
            }
        }

        public void OnDeviceRemoval(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnDeviceRemoval), sender, e);
                return;
            }
        }

        private void OnCameraOpened(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }
        }

        private void OnCameraClosed(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }
        }

        public void StartGrab()
        {

            if (Program.saveFolderPath == null)
            {
                Program.saveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                System.Windows.Forms.MessageBox.Show("Temporary Path has been Assigned. Please Select a Folder First. Path: " + Program.saveFolderPath);

            }

            string desiredName = String.Format(DateTime.Now.ToString("HHmmssfff"));
            string path = System.IO.Path.Combine(Program.saveFolderPath, desiredName);


            try
            {
                Action imageGrab = () =>
                {
                    FirstView.StartBtn();
                };
                Task.Factory.StartNew(imageGrab);

                Action imageGrab1 = () =>
                {
                    SecondView.StartBtn();
                };
                Task.Factory.StartNew(imageGrab1);

                Action imageGrab2 = () =>
                {
                    ThirdView.StartBtn();
                };
                Task.Factory.StartNew(imageGrab2);

                Action imageGrab3 = () =>
                {
                    FourthView.StartBtn();
                };
                Task.Factory.StartNew(imageGrab3);

            }
            catch (Exception exception)
            {
                ShowException(exception);
            }



        }


        private void Stop()
        {
            UpdateCameraList();
            try
            {
                timer.Stop();
                Thread.Sleep(10);

                if (ledSource.serialPort1.IsOpen)
                {
                    ledSource.Led_OnOff(0x00, 0);
                }

                FirstView.StopBtn();
                SecondView.StopBtn();
                ThirdView.StopBtn();
                FourthView.StopBtn();

            }
            catch (Exception ex)
            {
                ShowException(ex);
            }

        }


        private void DestroyCam()
        {
            try
            {
                FirstView.GetUnoCamera().DestroyCamera();
                SecondView.GetUnoCamera().DestroyCamera();
                ThirdView.GetUnoCamera().DestroyCamera();
                FourthView.GetUnoCamera().DestroyCamera();
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            try
            {
                if (ledSource.serialPort1.IsOpen)
                {
                    ledSource.Led_OnOff(0x00, 0);
                    ledSource.serialPort1.Close();
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }



            Program.checkSet = false;
        }






    }



}
