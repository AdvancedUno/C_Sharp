using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using NvAPIWrapper.GPU;
using System.Management;

namespace Frism
{


    public class DnnSetClass
    {
        public delegate void MyEventHandelr(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide);
        public event MyEventHandelr setEvent;
        

        public void passDnnSet(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide)
        {
            setEvent?.Invoke(ThreadCnt, Width, Height, GpuNo, MinDefectNumTop, MinPValTop, MinDefectNumSide, MinPValSide);
        }
       
        
    };


    public class CntTime
    {

        public delegate void MyEventHandelr(long chechTime);
        public event MyEventHandelr cntInspTimeEvent;
        public event MyEventHandelr cntProcessTimeEvent;

        public void IncreaseInspTime(long inspTime)
        {
            cntInspTimeEvent?.Invoke(inspTime);
        }

        public void IncreaseProcessTime(long ProcessTime)
        {
            cntProcessTimeEvent?.Invoke(ProcessTime);
        }


    }

    public class CntNGClass
    {
        public delegate void MyEventHandelr();
        public event MyEventHandelr cntNGEvent;
        public event MyEventHandelr cntOKEvent;
        


        public void IncreaseNG()
        {
            cntNGEvent?.Invoke();
        }
        public void IncreaseOK()
        {
            cntOKEvent?.Invoke();
        }
        


    };


    public class ShowSignal
    {

        public delegate void MyEventHandelr();
        public event MyEventHandelr showCameraSignal;
        public event MyEventHandelr showBlowSignal;
        public event MyEventHandelr showEndCameraSignal;
        public event MyEventHandelr showEndBlowSignal;



        public void ShowCameraSignal()
        {
            showCameraSignal?.Invoke();
        }

        public void ShowBlowSignal()
        {
            showBlowSignal?.Invoke();
        }

        public void ShowEndCameraSignal()
        {
            showEndCameraSignal?.Invoke();
        }
        public void ShowEndBlowSignal()
        {
            showEndBlowSignal?.Invoke();
        }



    }





    public partial class MainWindow: Window, INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        System.Windows.Threading.DispatcherTimer timer;
        System.Windows.Threading.DispatcherTimer timerMain;
        private Stopwatch timeProcess = new Stopwatch();


        /////////////// New Windows /////////////
        public History viewHistory;
        public SelectCameras selectCameras;
        public LEDWindow ledSource; // = new LEDWindow();
        public DnnSetting dnnSetWindow;

        public Thread t5;

        public CntTime cntTime;         // = new CntTime();
        public DnnSetClass dnnSetClass; // = new DnnSetClass();
        public CntNGClass cntNGClass;   // = new CntNGClass();
        public ShowSignal showSigClass;

        private int iMaxThreadCnt;
        private int iMaxTileWidth;
        private int iMaxTileHeight;
        private int iGpuNo;
        private int iMinDefectNumTop;
        private float fMinPValTop;
        private int iMinDefectNumSide;
        private float fMinPValSide;


        private int NGCnt = 0;
        private int OKCnt = 0;

        private int NGYield = 0;
        private int OKYield = 0;


        private long InspTime = 0;
        private long ProcessTime = 0;

        private long avgInspTime = 0;
        private long avgProcessTime = 0;

        private int avgInspCnt = 0;
        private int avgProcessCnt = 0;

        private string sGpuTemp;

        public string showGpuTemp
        {
            get { return sGpuTemp; }
            set
            {
                sGpuTemp = value;
                OnPropertyChanged();
            }
        }
        
        private string brushGpuBackgroundColor = "Red";

        public string showBrushGpuBackgroundColor
        {
            get { return brushGpuBackgroundColor; }
            set
            {
                brushGpuBackgroundColor = value;
                OnPropertyChanged();
            }
        }


        private bool bCheckInspRun = false;


        private List<string> savedInfos = null;

        private bool bClaheBtn = false;
        

        public static Dictionary<ICameraInfo, String> CameraInfos;

        public static Stopwatch timeS; // = new Stopwatch();

        const int c_maxCamerasToUse = 2;

        private bool bSnapshotMode = false;


        private bool bFirstInit = true;


        public string CamID;

        

        public List<string> caminfo = new List<string>(4);
        public List<string> camExTimeinfo = new List<string>(4);
        public List<string> LEDVal = new List<string>(4);
        public List<string> DnnFilePath = new List<string>(4);
        public List<string> CropInfo = new List<string>(4);

        public bool m_bCamOpened = false;

        List<ICameraInfo> allDeviceInfos = SelectCameras.selectedCameras;


        

        private int m_iCamOpenedCheck = 0;


        private bool bStartCheckingGPUTemp = false;


        public object lockCheckCam = new object();
        public void InitVariables()
        {
            try
            {
                ledSource = new LEDWindow();
                cntTime = new CntTime();
                dnnSetClass = new DnnSetClass();
                cntNGClass = new CntNGClass();
                showSigClass = new ShowSignal();
                timerMain = new DispatcherTimer();

                timeS = new Stopwatch();

                timer = new DispatcherTimer();
                Program.setBlowDevice();


            }
            catch(Exception e)
            {
                Logger.Error(e.Message + " InitVariables()");
                throw e;
            }
        }

        public MainWindow()
        {
            Logger.Debug("Start mainwindow");


            InitializeComponent();



            InitVariables();

            this.DataContext = this;

            UpdateDeviceList();
            UpdateCameraList();

            FirstView.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            FirstView.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            FirstView.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            SecondView.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            SecondView.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            SecondView.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            ThirdView.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            ThirdView.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            ThirdView.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            FourthView.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            FourthView.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            FourthView.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            timer.Tick += FirstView.GetUnoCamera().GiveImageFile;
            timer.Tick += SecondView.GetUnoCamera().GiveImageFile;
            timer.Tick += ThirdView.GetUnoCamera().GiveImageFile;
            timer.Tick += FourthView.GetUnoCamera().GiveImageFile;
            timer.Interval = TimeSpan.FromMilliseconds(150);

            timerMain.Tick += timer_Tick;
            timerMain.Interval = TimeSpan.FromMilliseconds(150);


            LEDVal.Add("0");
            LEDVal.Add("0");
            LEDVal.Add("0");
            LEDVal.Add("0");




            dnnSetClass.setEvent += DnnSetEvent;
            cntNGClass.cntOKEvent += IncreaseOKEvent;
            cntNGClass.cntNGEvent += IncreaseNGEvent;
            cntTime.cntInspTimeEvent += IncreaseInspTimeEvent;
            cntTime.cntProcessTimeEvent += IncreaseProcessEvent;
            showSigClass.showBlowSignal += ShowBlowSignal;
            showSigClass.showCameraSignal += ShowCameraSignal;
            showSigClass.showEndCameraSignal += ShowEndCameraSignal;
            showSigClass.showEndBlowSignal += ShowEndBlowSignal;
            
            

            DBAcess.CreateDB();




            // 로딩이 완료되면 화면을 앞으로 가져온다

            this.Activate();
            this.Focus();


            List<string> basicInfo = DBAcess.GiveBasicSettings("0");
            if (basicInfo != null && basicInfo.Count > 7)
            {   
                DnnSetEvent(Int32.Parse(basicInfo[0]), Int32.Parse(basicInfo[1]), Int32.Parse(basicInfo[2]), Int32.Parse(basicInfo[3]),
                Int32.Parse(basicInfo[4]), float.Parse(basicInfo[5]), Int32.Parse(basicInfo[6]), float.Parse(basicInfo[7]));
            }
            else
            {
                DnnSetEvent(1, 1600, 800, 4, 10, 0.5f, 10, 0.5f);
            }


            Thread gpuTempCheckThread = new Thread(new ThreadStart(StartCheckingGPUTemp));
            
            gpuTempCheckThread.Start();
            
            bStartCheckingGPUTemp = true;
            //StartCheckingGPUTemp();


        }

        #region PropertyChangedEventHandler 연동
        /////////////// Property Update Fnc /////////////
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void UpdateDeviceList()
        {
            Logger.Debug("Update Device List");

            try
            {
                
                List<ICameraInfo> cameraInfos = CameraFinder.Enumerate();

                if (cameraInfos.Count > 0)
                {

                    CameraInfos = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in cameraInfos)
                    {

                        CameraInfos.Add(cameraInfo, cameraInfo[CameraInfoKey.FriendlyName]);
                        
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
                savedInfos = DBAcess.CamInfos();
                
                CameraInfoBox.ItemsSource = savedInfos;
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

        }


        ///////////////////// 카메라 업데이트 및 노출 값 설정 ////////////////////////////
        private void UpdateCamera(List<string> camIDList, List<string>camExTime)
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
                        if(camExTime[1] != null)
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
            catch(Exception ex)
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
            CamOpendCheck();
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
                System.Windows.Forms.MessageBox.Show("Temporary Path has been Assigned. Please Select a Folder First. Path: "+ Program.saveFolderPath);

            }

            string desiredName = String.Format(DateTime.Now.ToString("HHmmssfff"));
            string path = System.IO.Path.Combine(Program.saveFolderPath, desiredName);


            try
            {
                //Action imageGrab = () =>
                //{
                    FirstView.StartBtn();
                //};
                //Task.Factory.StartNew(imageGrab);

                //Action imageGrab1 = () =>
                //{
                    SecondView.StartBtn();
                //};
                //Task.Factory.StartNew(imageGrab1);

                //Action imageGrab2 = () =>
                //{
                    ThirdView.StartBtn();
                //};
                //Task.Factory.StartNew(imageGrab2);

                //Action imageGrab3 = () =>
                //{
                    FourthView.StartBtn();
                //};
                //Task.Factory.StartNew(imageGrab3);

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
        


        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            bStartCheckingGPUTemp = false;
            
            if (bCheckInspRun)
            {
                System.Windows.Forms.MessageBox.Show("You Must Stop the Inspection First Before Closing the Window");
                e.Cancel = true;
                return;
            }

            Logger.Debug("Destroying mainwindow");

            DestroyCam();
            Logger.Debug("Destroyed Cam");

            if (ledSource != null)
                ledSource.Close();

            Logger.Debug("Destroyed Led");

            try
            {
                if (ledSource.serialPort1.IsOpen)
                {
                    ledSource.Led_OnOff(0x00, 0);
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            if (ledSource != null)
            {
                ledSource.Close();
            }

            if (viewHistory != null)
            {
                viewHistory.Close();
            }

            if (selectCameras != null)
            {
                selectCameras.Close();
            }

            if (dnnSetWindow != null)
            {
                dnnSetWindow.Close();
            }

            Program.bContinueBlowSignal = false;


            if (t5 != null)
            {
                // t5.Interrupt();
                t5.Join();
                
            }

            Logger.Debug("Finish destroyng mainWindow");


            //System.Environment.Exit(0);
            //System.Windows.Application.Current.Shutdown();
            Process.GetCurrentProcess().Kill();
        }

        private void SelectCamerasButton_Click(object sender, RoutedEventArgs e)
        {

            
            

            DestroyCam();

            ContinueButton.IsEnabled = false;
            StopButton.IsEnabled = false;
            SnapShotButton.IsEnabled = false;
            bCheckInspRun = false;

            selectCameras = new SelectCameras(this);

            selectCameras.SetTimer(timer);


            selectCameras.ShowDialog();
            //selectCameras.Show();
        }


        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {  
            try
            {
                viewHistory = new History();
                viewHistory.Show();
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }

        private void SetCamerasButton_Click(object sender, RoutedEventArgs e)
        {


            try
            {


                //this.MainWindow = splashScreen;





                Program.bSavingStop = true;

                if (CameraInfoBox.SelectedItem != null)
                    {
                    var splashScreen = new SplashScreenWindow();
                    splashScreen.mainImage.Visibility = Visibility.Hidden;
                    splashScreen.mainImage.Source = null;
                    splashScreen.mainText.Visibility = Visibility.Hidden;

                    splashScreen.MainLoding.Height = 100;
                    splashScreen.MainLoding.Width = 550;
                    splashScreen.progressBar.Visibility = Visibility.Hidden;
                    splashScreen.subText.Visibility = Visibility.Visible;
                    splashScreen.Show();

                    FirstView.WindowImage.Destory();
                    SecondView.WindowImage.Destory();
                    ThirdView.WindowImage.Destory();
                    FourthView.WindowImage.Destory();

                    DestroyCam();



                    Program.SetClass(cntNGClass);
                    Program.useCntTimeClass = cntTime;
                    Program.useShowSigClass = showSigClass;

                    if (Program.saveFolderPath == null)
                    {
                        Program.saveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INTELLIZ Corp\\Frism Inspection";
                        //System.Windows.Forms.MessageBox.Show("Temporary Path has been Assigned. Please Select a Folder First. Path: " + Program.saveFolderPath);



                        if (!Directory.Exists(Program.saveFolderPath))
                        {
                            Directory.CreateDirectory(Program.saveFolderPath);
                        }
                    }


                    try
                    {
                        FirstView.WindowImage.SetSavePath(Program.saveImagePath, 100);
                        SecondView.WindowImage.SetSavePath(Program.saveImagePath, 120);
                        ThirdView.WindowImage.SetSavePath(Program.saveImagePath, 240);
                        FourthView.WindowImage.SetSavePath(Program.saveImagePath, 0);
                    }
                    catch (Exception exception)
                    {
                        ShowException(exception);
                    }


                    string tempFilePath = DBAcess.GiveFilePath("0");
                    if (tempFilePath != null)
                    {
                        Program.saveFolderPath = tempFilePath;
                    }



                    ///////////////////// DB 에서 카메라 값 가져오기 ////////////////////////////
                    caminfo = DBAcess.GiveCamSettings(CamID);


                    ///////////////////// DB 에서 노출 값 가져오기 ////////////////////////////
                    camExTimeinfo = DBAcess.GiveExTimeSetting(CamID);


                    ///////////////////// DB 에서 LED 조명 값 가져오기 ////////////////////////////
                    LEDVal = DBAcess.GiveLEDValSetting(CamID);


                    ///////////////////// DB 에서 DNN 경로 가져오기 ////////////////////////////
                    DnnFilePath = DBAcess.GiveDNNFileSetting(CamID);

                    CropInfo = DBAcess.GiveCropInfoSetting(CamID);

                    ///////////////////// 초기 DNN 설정 실행 ////////////////////////////

                    FirstView.WindowImage.InitThread();
                   // SecondView.WindowImage.InitThread();
                    //ThirdView.WindowImage.InitThread();
                    //FourthView.WindowImage.InitThread();


                    Thread.Sleep(10);



                    ///////////////////// DNN 경로 설정 ////////////////////////////
                    UpdateDnnFiles(DnnFilePath);

                    ///////////////////// DNN 값 초기 값 설정  ////////////////////////////
                    UpdateDnnSetting();

                    ///////////////////// 카메라 업데이트 및 노출 값 설정 ////////////////////////////
                    UpdateCamera(caminfo, camExTimeinfo);


                    ResetBtn_Click(sender, e);

                    ///////////////////// LED 조명 설정 및 실행 ////////////////////////////
                    UpdateLED(LEDVal);

                    UpdateCropInfoSetting();
                    m_bCamOpened = true;
                    while (true)
                    {

                        Thread.Sleep(1);

                        if (m_bCamOpened)
                        {
                            Console.WriteLine("cam opened");
                            break;
                        }


                    }
                    m_bCamOpened = false;
                    Program.bSavingStop = false;


                    //StartLED(LEDVal);
                    Thread tSaveImageThread = new Thread(new ThreadStart(Program.SaveAllImagesThread));
                    tSaveImageThread.Start();
                    Thread.Sleep(10);

                    Thread tSaveTopThread = new Thread(new ThreadStart(Program.SaveImageThreadTop));
                    Thread tSaveFirstThread = new Thread(new ThreadStart(Program.SaveImageThread1));
                    Thread tSaveSecondThread = new Thread(new ThreadStart(Program.SaveImageThread2));
                    Thread tSaveThirdThread = new Thread(new ThreadStart(Program.SaveImageThread3));

                    tSaveTopThread.Start();
                    tSaveFirstThread.Start();
                    tSaveSecondThread.Start();
                    tSaveThirdThread.Start();

                    Thread.Sleep(10);


                    Thread t1 = new Thread(new ThreadStart(FirstView.InitializeDLL));
                    Thread t2 = new Thread(new ThreadStart(SecondView.InitializeDLL));
                    Thread t3 = new Thread(new ThreadStart(ThirdView.InitializeDLL));
                    Thread t4 = new Thread(new ThreadStart(FourthView.InitializeDLL));

                    t1.Start();
                    t2.Start();
                    t4.Start();
                    t3.Start();

                    bFirstInit = true;



                  



                    ContinueButton.IsEnabled = false;
                    SnapShotButton.IsEnabled = true;

                    while (true)
                    {
                        Thread.Sleep(2);

                        if (FirstView.WindowImage.m_bInferReadyFlag && SecondView.WindowImage.m_bInferReadyFlag
                            && ThirdView.WindowImage.m_bInferReadyFlag && FourthView.WindowImage.m_bInferReadyFlag)
                        {
                            break;
                        }
                    }
                   

                    for (int i = 0; i < 3; i++)
                    {
                        Console.WriteLine("blow : " + i);

                        Program.blow(10);
                        
                        Thread.Sleep(200);
                    }

                    //splashScreen.Close();
                        

                    ContinueButton.IsEnabled = true;
                    textInitial.Background = System.Windows.Media.Brushes.LimeGreen;
                    splashScreen.Close();



                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Please Select an Option.");
                    }
                
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " SetCamerasButton_Click MainW");
            }


        }



        ///////////////////// DNN 값 초기 값 설정  ////////////////////////////
        public void UpdateDnnSetting()
        {
            try
            {
                FirstView.SetDnnParameters(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo, iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);
                SecondView.SetDnnParameters(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo , iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);
                ThirdView.SetDnnParameters(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo , iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);
                FourthView.SetDnnParameters(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo, iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " UpdateDnnSetting");
            }
            
        }


        private void UpdateCropInfoSetting()
        {
            FirstView.WindowImage.SetCropVal(CropInfo[0]);
            SecondView.WindowImage.SetCropVal(CropInfo[1]);
            ThirdView.WindowImage.SetCropVal(CropInfo[2]);
            FourthView.WindowImage.SetCropVal(CropInfo[3]);

        }

        private void SnapShotButton_Click_1(object sender, RoutedEventArgs e)
        {
            //SnapShotButton.IsEnabled = false;
            StartGrab();
            //timeS.Reset();
        }


        void timer_Tick(object sender, EventArgs e)
        {
            if (!Program.m_bCam1Insp)
            {
                CameraSignalTxt.Background = System.Windows.Media.Brushes.Gray;
                BlowSignalTxt.Background = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                CameraSignalTxt.Background = System.Windows.Media.Brushes.Yellow;
                BlowSignalTxt.Background = System.Windows.Media.Brushes.Yellow;
            }
        }


        private void ShowException(Exception exception)
        {
            Logger.Error(exception.Message);

            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //MainWindow.timeS.Reset();
                Stop();
                SnapShotButton.IsEnabled = false;
                SelectCameraButton.IsEnabled = true;
                ContinueButton.IsEnabled = true;

                Program.checkSet = false;
                StopButton.IsEnabled = false;
                bCheckInspRun = false;
                timerMain.Stop();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " StopButton_Click");
                //ShowException(exception);
            }
           
        }

        //private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        //{
        //    FolderBrowserDialog folderPath = new FolderBrowserDialog();
        //    string folderPathTemp = DBAcess.GiveFilePath("0");

        //    folderPath.SelectedPath = folderPathTemp;
        //    folderPath.ShowDialog();
        //    if(folderPath.SelectedPath != "")
        //    {
        //        Program.saveFolderPath = folderPath.SelectedPath;



        //        int iCheck = DBAcess.InsertFilePath("0", folderPath.SelectedPath); ;
                   

        //        if (iCheck < 1)
        //        {
        //            DBAcess.UpdateDataFilePath("0", folderPath.SelectedPath);
        //        }


                

        //        if (!Directory.Exists(Program.saveFolderPath))
        //        {
        //            Directory.CreateDirectory(Program.saveFolderPath);
        //        }
               
        //    }
        //    else
        //    {
                
        //    }
            
        //    if (Program.saveFolderPath == "" || Program.saveFolderPath == null)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Please Select a Folder.");
        //        return;
        //    }
            
            
            


        //    #region  Train 위한 이미지 저장 
        //    /////////// Train 위한 이미지 저장 //////////////

        //    //string pathTop = System.IO.Path.Combine(Program.saveFolderPath, "Top");
        //    //string path1 = System.IO.Path.Combine(Program.saveFolderPath, "Side_1");
        //    //string path2 = System.IO.Path.Combine(Program.saveFolderPath, "Side_2");
        //    //string path3 = System.IO.Path.Combine(Program.saveFolderPath, "Side_3");



        //    //if (!Directory.Exists(pathTop))
        //    //{
        //    //    Directory.CreateDirectory(pathTop);

        //    //}

        //    //Program.saveImagePathTrainingTop = pathTop;

        //    //if (!Directory.Exists(path1))
        //    //{
        //    //    Directory.CreateDirectory(path1);

        //    //}

        //    //Program.saveImagePathTraining1 = path1;

        //    //if (!Directory.Exists(path2))
        //    //{
        //    //    Directory.CreateDirectory(path2);

        //    //}

        //    //Program.saveImagePathTraining2 = path2;

        //    //if (!Directory.Exists(path3))
        //    //{
        //    //    Directory.CreateDirectory(path3);

        //    //}

        //    //Program.saveImagePathTraining3 = path3;
        //    #endregion

        //}

        private void CameraInfoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CameraInfoBox.Items.Count > 0)
                {
                    if (CameraInfoBox.SelectedItem != null)
                    {
                        CamID = (string)CameraInfoBox.SelectedItem;
                    }
                    else
                    {
                        CamID = null;
                    }
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception.Message + " CameraInfoBox_SelectionChanged");
                //ShowException(exception);
            }
           
        }

        private void StartLED(List<string> LEDidList)
        {
            Logger.Debug("StartLED");
            try
            {
                int red = Int32.Parse(LEDidList[0]);
                if (red > 0)
                {
                    ledSource.Set_DimmingValue(1, red);
                }



                Thread.Sleep(100);
                int green = Int32.Parse(LEDidList[1]);
                if (green > 0)
                {
                    ledSource.Set_DimmingValue(2, green);
                }

                Thread.Sleep(100);
                int blue = Int32.Parse(LEDidList[2]);
                if (blue > 0)
                {
                    ledSource.Set_DimmingValue(3, blue);
                }


                Thread.Sleep(100);
                int white = Int32.Parse(LEDidList[3]);
                if (white > 0)
                {
                    ledSource.Set_DimmingValue(0, white);
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception.Message + " StartLED");
                //ShowException(exception);
            }
            
        }
        

        public void ShowSnapShotBtn()
        {
            if (bSnapshotMode)
            {
                bSnapshotMode = false;
                SnapShotButton.Visibility = Visibility.Hidden;
            }
            else
            {
                bSnapshotMode = true;
                SnapShotButton.Visibility = Visibility.Visible;
            }
            
        }


        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
        
            StartLED(LEDVal);


            FirstView.ContinueGrab();
            SecondView.ContinueGrab();
            ThirdView.ContinueGrab();
            FourthView.ContinueGrab();



            try
            {
                ////FirstView.camera.ChangeFilePath(@"D:\aaaaa");
                


                //Program.StartIOSig();

                for (int i = 0; i < 1; i++)
                {
                    Console.WriteLine("blow : " + i);
                    Program.blow(10);
                    Thread.Sleep(200);
                }


                Program.StartIOSig();

                //ContinueButton.IsEnabled = false;
                //SnapShotButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " ContinueButton_Click");
                //ShowException(exception);
            }





            try
            {
                Program.checkSet = true;

                //t5 = new Thread(new ThreadStart(Program.Saving));
                //Thread t5 = new Thread(new ThreadStart(Program.SavingForTrain)); /// Train 용 이미지 저장
                //t5.Start();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " ContinueButton_Click");
                //ShowException(exception);
            }
            bCheckInspRun = true;
            ContinueButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            Thread.Sleep(100);
            timerMain.Start();
            timer.Start();

        }






        //private void InspectCheckBox_Checked(object sender, RoutedEventArgs e)
        //{
            
        //    if (InspectCheckBox.IsChecked.Value)
        //    {
        //        Program.bInspectBtn = true;
                
                
        //    }
        //    ContinueButton.IsEnabled = false;


        //}

        //private void InspectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    if (InspectCheckBox.IsChecked.Value == false)
        //    {
        //        Program.bInspectBtn = false;
               

        //    }

        //}


        public void SettingOptionClose()
        {
            //dnnSetWindow.
        }

        public void SettingCameraWindowClosing()
        {
            UpdateCameraList();
        }


        public void DnnSetEvent(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {

                    iMaxThreadCnt = ThreadCnt;
                    iMaxTileHeight = Height;
                    iMaxTileWidth = Width;
                    iGpuNo = GpuNo;
                    iMinDefectNumTop = MinDefectNumTop;
                    fMinPValTop = MinPValTop;
                    iMinDefectNumSide = MinDefectNumSide;
                    fMinPValSide = MinPValSide;

                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " DnnSetEvent");
                //ShowException(exception);
            }
        }


        public void ShowCameraSignal()
        {
            //Console.WriteLine("yellowwwwwww");
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                CameraSignalTxt.Background = System.Windows.Media.Brushes.Yellow;
                //Thread.Sleep(1000);
                //CameraSignalTxt.Background = System.Windows.Media.Brushes.Gray;
            }));
        }

        public void ShowEndCameraSignal()
        {
            //Console.WriteLine("graaaaaaaaay");
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                CameraSignalTxt.Background = System.Windows.Media.Brushes.Gray;
                //Thread.Sleep(1000);
                //CameraSignalTxt.Background = System.Windows.Media.Brushes.Gray;
            }));
        }

        public void ShowEndBlowSignal()
        {
            //Console.WriteLine("graaaaaaaaay");
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                BlowSignalTxt.Background = System.Windows.Media.Brushes.Gray;
                //Thread.Sleep(1000);
                //CameraSignalTxt.Background = System.Windows.Media.Brushes.Gray;
            }));
        }



        public void ShowBlowSignal()
        {
            

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                BlowSignalTxt.Background = System.Windows.Media.Brushes.Yellow;
                
               // BlowSignalTxt.Background = System.Windows.Media.Brushes.Gray;
            }));
        }


        public void IncreaseOKEvent()
        {
            OKCnt++;
            OKYield = (OKCnt * 100) / (OKCnt + NGCnt);
            NGYield = 100 - OKYield;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {

                
                OKNumTxt.Text = String.Format("{0} %  [{1}]", OKYield, OKCnt);
                NGNumTxt.Text = String.Format("{0} %  [{1}]", NGYield, NGCnt);


            }));

        }

        public void IncreaseNGEvent()
        {
            NGCnt++;
            NGYield = (NGCnt * 100) / (OKCnt + NGCnt);
            OKYield = 100 - NGYield;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {

                OKNumTxt.Text = String.Format("{0} %  [{1}]", OKYield, OKCnt);
                NGNumTxt.Text = String.Format("{0} %  [{1}]", NGYield, NGCnt);

            }));

        }

        public void IncreaseInspTimeEvent(long inspTime)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    avgInspCnt++;
                    InspTime = inspTime;
                    InspTimeTxt.Text = InspTime.ToString() + " ms"; ;
                    avgInspTime += inspTime;
                    AvgInspTimeTxt.Text = (avgInspTime / avgInspCnt).ToString();
                    inspTime = 0;

                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " DnnSetEvent");
                //ShowException(exception);
            }
           

        }
        public void IncreaseProcessEvent(long processTime)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    avgProcessCnt++;
                    ProcessTime = processTime;
                    ProcessTimeTxt.Text = ProcessTime.ToString() + " ms";
                    avgProcessTime += processTime;
                    AvgProcessTimeTxt.Text = (avgProcessTime / avgProcessCnt).ToString();
                    ProcessTime = 0;



                }));
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " IncreaseProcessEvent");
                //ShowException(exception);
            }

        }

        private void DnnOptionBtn_Click(object sender, RoutedEventArgs e)
        {
            dnnSetWindow = new DnnSetting(this);
            dnnSetWindow.SetClass(dnnSetClass);
            dnnSetWindow.Show();


        }

        
        
        


        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {

            NGCnt = 0;
            NGYield = 0;
            OKCnt = 0;
            OKYield = 0;
            OKNumTxt.Text = String.Format("{0} %  [{1}]", OKYield, OKCnt);
            NGNumTxt.Text = String.Format("{0} %  [{1}]", NGYield, NGCnt);
            Program.imagePathTop.Clear();
            Program.imagePath1.Clear();
            Program.imagePath2.Clear();
            Program.imagePath3.Clear();

            if(Program.qSaveBlowSignal != null)
            {
                while (Program.qSaveBlowSignal.Count > 0)
                {
                    Program.qSaveBlowSignal.Take();
                }
            }
       
            


            FirstView.Cam1.Text = "Ready";
            FirstView.WindowImage.ImageSource = null;
            FirstView.CamBorder1.Background = System.Windows.Media.Brushes.Gray;

            SecondView.Cam1.Text = "Ready";
            SecondView.WindowImage.ImageSource = null;
            SecondView.CamBorder1.Background = System.Windows.Media.Brushes.Gray;

            ThirdView.Cam1.Text = "Ready";
            ThirdView.WindowImage.ImageSource = null;
            ThirdView.CamBorder1.Background = System.Windows.Media.Brushes.Gray;

            FourthView.Cam1.Text = "Ready";
            FourthView.WindowImage.ImageSource = null;
            FourthView.CamBorder1.Background = System.Windows.Media.Brushes.Gray;

        }

        private void FirstView_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void CamOpendCheck()
        {
            lock (lockCheckCam)
            {
                m_iCamOpenedCheck++;
                if (m_iCamOpenedCheck == 4)
                {
                    m_bCamOpened = true;
                }
            }
            

        }


        private void StartCheckingGPUTemp()
        {
            PhysicalGPU[] gpus = PhysicalGPU.GetPhysicalGPUs();
            

            while (true)
           {

                Thread.Sleep(5);
                if (bStartCheckingGPUTemp)
                {
                    foreach (GPUThermalSensor sensor in gpus[0].ThermalInformation.ThermalSensors)
                    {
                        showGpuTemp = String.Format("GPU 온도: {0} C", sensor.CurrentTemperature);
                        
                        if(sensor.CurrentTemperature > 65)
                        {
                            showBrushGpuBackgroundColor = "Red";
                        }
                        else
                        {
                            showBrushGpuBackgroundColor = "Green";
                        }
                        
                    }



                  
                }

            }


        }


    }

    
}
