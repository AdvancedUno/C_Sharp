using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using NLog;
using System.Windows.Threading;
using Frism_Inspection_Renew.Events;
using System.Collections.Concurrent;

namespace Frism_Inspection_Renew.Models
{
    public class VisionCameraGroup
    {
        public delegate void GetImageDelegate(int i);

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //public IVisionCamera camera = new FileCamera(@"D:\TB\Image\Top");

        private List<IVisionCamera> _iVisionCameraGroup;
        public List<IVisionCamera> IVisionCameraGroup { get => _iVisionCameraGroup; set => _iVisionCameraGroup = value; }


        private IVisionCamera _visionCamera;
        public IVisionCamera VisionCamera { get => _visionCamera; set => _visionCamera = value; }

        private int _totalCameraNumber;
        public int TotalCameraNumber { get => _totalCameraNumber; set => _totalCameraNumber = value; }

        public GetImageDelegate cameraImageDelegate;


        private List<int> _capturedImageList;
        public List<int> CapturedImageList { get => _capturedImageList; set => _capturedImageList = value; }


        public event EventHandler<ImageCapturedEventArgs> ImageCapturedSignal;

        protected virtual void RaiseEventFrameCaptured(ImageCapturedEventArgs e)
        {
            EventHandler<ImageCapturedEventArgs> handler = ImageCapturedSignal;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        public VisionCameraGroup(int numCamera)
        {
            TotalCameraNumber = numCamera;
            IVisionCameraGroup = new List<IVisionCamera>();
            CapturedImageList = new List<int>();
            for (int i = 0; i < TotalCameraNumber; i++)
            {
                VisionCamera = new FileCamera(@"D:\TB\Image\Top", i);
                
                IVisionCameraGroup.Add(VisionCamera);
                cameraImageDelegate = new GetImageDelegate(CheckCapturedCamera);

                IVisionCameraGroup[i].GuiCameraConnectionToCameraLost += OnDeviceRemoved;
                IVisionCameraGroup[i].GuiCameraOpenedCamera += OnCameraOpened;
                IVisionCameraGroup[i].GuiCameraClosedCamera += OnCameraClosed;
                IVisionCameraGroup[i].GuiCameraGrabStarted += OnGrabStarted;
                IVisionCameraGroup[i].GuiCameraGrabStopped += OnGrabStopped;
                IVisionCameraGroup[i].GuiCameraFrameReadyForDisplay += OnImageReady;
                CapturedImageList.Add(0);


            }
        }

        
        public void CheckCapturedCamera(int number)
        {
            Console.WriteLine(number);

        }


        public void OnDeviceRemoved(Object sender, EventArgs e)
        {


            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {

                Dispatcher.CurrentDispatcher.BeginInvoke(new EventHandler<EventArgs>(OnDeviceRemoved), sender, e);
                return;
            }
            for (int i = 0; i < TotalCameraNumber; i++)
            {
                IVisionCameraGroup[i].DestroyCamera();
            }
            
        }
        public void OnCameraOpened(Object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }
            try
            {
                //////////////////  DNN 설정 정보를 Screen Window 로 전달 및 DNN 설정 세팅 ////////////////////
                for (int i = 0; i < TotalCameraNumber; i++)
                {
                    IVisionCameraGroup[i].SetCamParameter(PLCamera.TriggerMode, PLCamera.TriggerSource, "On", "Software");
                }
                //camera.SetCamParameter(PLCamera.TriggerMode, PLCamera.TriggerSource, "On", "Line1");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " OnCameraOpened");
                //ShowException(ex);
            }

        }
        public void OnCameraClosed(Object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

        }
        public void OnGrabStarted(Object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }
        }
        public void OnGrabStopped(Object sender, EventArgs e)
        {
            if (!Dispatcher.CurrentDispatcher.CheckAccess())
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStopped), sender, e);
                return;
            }
        }
        #region  "OnImageReady" - 이미지 받아오기 및 후 처리
        private void OnImageReady(Object sender, ImageCapturedEventArgs e)
        {
            try
            {
                
                CapturedImageList[e.cameraNumber] = 1;
                
                //Console.WriteLine("Thead ID OnImageReady : _________ " + Thread.CurrentThread.ManagedThreadId);
                for(int i = 0; i < TotalCameraNumber; i++)
                {
                    if(CapturedImageList[i] == 0)
                    {
                        return;
                    }
                }
                for (int i = 0; i < TotalCameraNumber; i++)
                {
                    CapturedImageList[i] = 0;
                }

                RaiseEventFrameCaptured(e);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + "OnImageReady");
            }
        }

        #endregion

        public List<IVisionCamera> GetIVisionCameraList()
        {
            return IVisionCameraGroup;
        }


    }
}
