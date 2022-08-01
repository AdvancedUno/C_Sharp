using Basler.Pylon;
using NLog;
using System;
using System.Diagnostics;

using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace Frism
{

    public class ngClass
    {
        public delegate void MyEventHandelr(object sender, EventArgs e);
        public event MyEventHandelr ngEvent;
        public event MyEventHandelr okEvent; 

        public void imgNG()
        {
            if (ngEvent != null)
            {
                ngEvent(this, new EventArgs());
            }
        }

        public void imgOK()
        {
            if (okEvent != null)
            {
                okEvent(this, new EventArgs());
            }
        }
    };

    public partial class MainViewWindow : System.Windows.Controls.UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public ngClass ngResult = new ngClass();

        //public IVisionCamera camera = new FileCamera(@"D:\TB\Image\Top");
        private IVisionCamera camera = new UnoCamera();
        public static Stopwatch timeS = new Stopwatch();


        private int iMaxThreadCnt;
        private int iMaxTileWidth;
        private int iMaxTileHeight;
        private int iGpuNo;
        private int iMinDefectNumTop;
        private float fMinPValTop;
        private int iMinDefectNumSide;
        private float fMinPValSide;

        public MainViewWindow()
        {

            

            InitializeComponent();





            ngResult.ngEvent += NGEvent;
            ngResult.okEvent += OKEvent;

            WindowImage.SetCamera(camera);  /////// Screen Window 로 Camera 정보 전달
            WindowImage.GetNG(ngResult);  /////// Screen Window 로 Delegate  Class 정보 전달

            camera.GuiCameraConnectionToCameraLost += OnDeviceRemoved;
            camera.GuiCameraOpenedCamera += OnCameraOpened;
            camera.GuiCameraClosedCamera += OnCameraClosed;
            camera.GuiCameraGrabStarted += OnGrabStarted;
            camera.GuiCameraGrabStopped += OnGrabStopped;



           

        }


        private void ShowException(Exception exception)
        {
            Logger.Error(exception.Message);

            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public void NGEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Cam1.Text = "NG";
                CamBorder1.Background = Brushes.Red;
            }));
            
        }

        public void OKEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                Cam1.Text = "OK";
                CamBorder1.Background = Brushes.Green;
            }));

        }


        //////////////////  DNN 설정 정보 MainViewWindow에 저장 ////////////////////
        public void SetDnnParameters(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide)
        {
            try
            {
                iMaxThreadCnt = ThreadCnt;
                iMaxTileHeight = Height;
                iMaxTileWidth = Width;
                iGpuNo = GpuNo;
                iMinDefectNumTop = MinDefectNumTop;
                fMinPValTop = MinPValTop;
                iMinDefectNumSide = MinDefectNumSide;
                fMinPValSide = MinPValSide;
                //WindowImage.InferDLL(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo, iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);
           
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " SetDnnParameters");
                //ShowException(exception);
            }
           
        }



        public IVisionCamera GetUnoCamera()
        {
            return camera;
        }
        public void OnDeviceRemoved(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnDeviceRemoved), sender, e);
                return;
            }
            camera.DestroyCamera();
        }
        public void OnCameraOpened(Object sender, EventArgs e)
        {
            
            if (!Dispatcher.CheckAccess())
            {
                
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            
            }
            try
            {
                //////////////////  DNN 설정 정보를 Screen Window 로 전달 및 DNN 설정 세팅 ////////////////////
                
                
                //camera.SetCamParameter(PLCamera.TriggerMode, PLCamera.TriggerSource, "On", "Software");
                camera.SetCamParameter(PLCamera.TriggerMode, PLCamera.TriggerSource, "On", "Line1");
                
                //camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);


                //camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Line1); ////// Trigger 를 Line 1 으로 부터 받음

                //camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Software); ///// Trigger 를 Software 신호로 받음

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " OnCameraOpened");
                //ShowException(ex);
            }
           




        }
        public void OnCameraClosed(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

            

            WindowImage.Clear();
            WindowImage.Destory();

        }
        public void OnGrabStarted(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }
        }
        public void OnGrabStopped(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStopped), sender, e);
                return;
            }
            
        }
        public void StartBtn()
        {
            if (camera.IsOpened())
            {
                camera.SoftwareTrigger();
                timeS.Start();
            }
        }
        public void ContinueGrab()
        {

            if (camera.IsOpened())
            {

                camera.SetMainMode();
                camera.StartContinuousShotGrabbing();
                //WindowImage.InitThread();
                

            }

        }




        public void InitializeDLL()
        {
            if (camera.IsOpened())
            {

                Console.WriteLine("Thead ID : _________ " + Thread.CurrentThread.ManagedThreadId);
                WindowImage.InferDLL(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo, iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);

            }
            else
            {
                Logger.Info("Cam is not opened + Cannoot infer dll");
            }

        }


        public void Trigger()
        {
            // camera.Parameters[PLCamera.TriggerSoftware].Execute();
            camera.ExecuteSoftwareTrigger(PLCamera.TriggerSoftware);
        }
        public void StopBtn()
        {
            if (camera.IsOpened())
            {
                camera.StopGrabbing();
            }   
        }

       

        public void FirstCreateCamera(ICameraInfo selectedCameraInfo)
        {
            
            Logger.Debug("FirstCreateCamera");
            if(selectedCameraInfo != null)
            {
                camera.CreateByCameraInfo(selectedCameraInfo);
                camera.OpenCamera();
            }
            //Console.WriteLine("Thead ID : _________ " + Thread.CurrentThread.ManagedThreadId);
            //WindowImage.InferDLL(iMaxThreadCnt, iMaxTileWidth, iMaxTileHeight, iGpuNo, iMinDefectNumTop, fMinPValTop, iMinDefectNumSide, fMinPValSide);

        }






    }
}
