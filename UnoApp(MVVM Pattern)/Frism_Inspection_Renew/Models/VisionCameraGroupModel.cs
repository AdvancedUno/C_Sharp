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
using System.Windows.Media.Media3D;

namespace Frism_Inspection_Renew.Models
{
    public class VisionCameraGroupModel
    {
        public delegate void GetImageDelegate(int i);
        public GetImageDelegate cameraImageDelegate;


        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public object lockObject = new object();


        private List<IVisionCamera> _iVisionCameraGroup;
        public List<IVisionCamera> IVisionCameraGroup { get => _iVisionCameraGroup; set => _iVisionCameraGroup = value; }


        private IVisionCamera _visionCamera;
        public IVisionCamera VisionCamera { get => _visionCamera; set => _visionCamera = value; }

        private int _totalCameraNumber;
        public int TotalCameraNumber { get => _totalCameraNumber; set => _totalCameraNumber = value; }


        private int camID = -1;

        private List<int> _capturedImageList;
        public List<int> CapturedImageList { get => _capturedImageList; set => _capturedImageList = value; }

        private Dictionary<ICameraInfo, string> _cameraInfos;
        public Dictionary<ICameraInfo, string> CameraInfos { get => _cameraInfos; set => _cameraInfos = value; }

        private Dictionary<ICameraInfo, int> _cameraCheckDictionary; // = new Dictionary<ICameraInfo, int>();
        public Dictionary<ICameraInfo, int> CameraCheckDictionary { get => _cameraCheckDictionary; set => _cameraCheckDictionary = value; }

        private List<string> _camExtimeInfoList;
        public List<string> CamExtimeInfoList { get => _camExtimeInfoList; set => _camExtimeInfoList = value; }
        
        private bool _checkFileCamera;
        public bool CheckFileCamera { get => _checkFileCamera; set => _checkFileCamera = value; }

        public event EventHandler<ImageCapturedEventArgs> ImageCapturedSignal;

        protected virtual void RaiseEventFrameCaptured(ImageCapturedEventArgs e)
        {
            EventHandler<ImageCapturedEventArgs> handler = ImageCapturedSignal;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        public VisionCameraGroupModel(int numCamera)
        {
            TotalCameraNumber = numCamera;
            IVisionCameraGroup = new List<IVisionCamera>();
            CameraCheckDictionary = new Dictionary<ICameraInfo, int>();
            CapturedImageList = new List<int>();
            CamExtimeInfoList = new List<string>(5) { "0", "0", "0", "0" , "0"};
            CameraInfos = new Dictionary<ICameraInfo, string>();
            CheckFileCamera = false;
            try
            {

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " VisionCameraGroupModel");
            }
            for (int i = 0; i < TotalCameraNumber; i++)
            {
                //VisionCamera = new FileCamera(@"D:\TB\Image\Top", i);
                VisionCamera = new UnoCamera(i);
                
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
            //Console.WriteLine(number);

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

                lock (lockObject)
                {

                    CapturedImageList[e.cameraNumber] = 1;

                    //Console.WriteLine("Thead ID OnImageReady : _________ " + Thread.CurrentThread.ManagedThreadId);
                    for (int i = 0; i < TotalCameraNumber; i++)
                    {
                        if (CapturedImageList[i] == 0)
                        {
                            return;
                        }
                    }

                    for (int i = 0; i < TotalCameraNumber; i++)
                    {
                        CapturedImageList[i] = 0;
                    }
                }

                if (camID >= 0)
                {
                    e.cameraNumber = camID;
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

        public void OpenCamera(string OptionId)
        {
            ///////////////////// DB 에서 카메라 값 가져오기 ////////////////////////////
            List<string> caminfo = DBAcess.GiveCamSettings(OptionId);

            ///////////////////// DB 에서 노출 값 가져오기 ////////////////////////////
            List<string> camExTimeinfo = DBAcess.GiveExTimeSetting(OptionId);

            UpdateCamera(OptionId, caminfo, camExTimeinfo);
        }

        public void UpdateDeviceList()
        {
            Logger.Debug("Update Device List");
            try
            {
                List<ICameraInfo> currentCameraInfos = CameraFinder.Enumerate();

                if (currentCameraInfos.Count > 0)
                {
                    CameraInfos = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in currentCameraInfos)
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


        ///////////////////// 카메라 업데이트 및 노출 값 설정 ////////////////////////////
        public void UpdateCamera(string OptionId, List<string> camIDList, List<string> camExTime = null)
        {
            bool testDNN = true;

            //if (OptionId == null || CameraInfos == null)
            //{
            //    Logger.Debug("CameraInfos");
            //    System.Windows.Forms.MessageBox.Show("Please Select The Setting");
            //    return;
            //}

            if (OptionId == null)
            {
                Logger.Debug("CameraInfos");
                System.Windows.Forms.MessageBox.Show("Please Select The Setting");
                return;
            }

            Logger.Debug("Update and Open Cameras");

            try
            {
                if(CheckFileCamera == true)
                {
                    for (int camnumber = 0; camnumber < TotalCameraNumber; camnumber++) {

                        if (IVisionCameraGroup[camnumber].IsOpened())
                        {
                            IVisionCameraGroup[camnumber].DestroyCamera();
                        }
                        IVisionCameraGroup[camnumber].CreateByCameraInfo(null);
                        IVisionCameraGroup[camnumber].OpenCamera();
                    }
                }
                else
                {
                    int cntCamera = 0;
                    if (camID >= 0)
                    {
                        cntCamera = camID;
                    }
                    for(int i = 0; i < TotalCameraNumber; i++)
                    {
                        foreach (ICameraInfo cameraInfo in CameraInfos.Keys)
                        {
                            if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[cntCamera])
                            {
                                int camNum = cntCamera;
                                if (camID >= 0) cntCamera = 0;
                                if (IVisionCameraGroup[cntCamera].IsOpened())
                                {
                                    IVisionCameraGroup[cntCamera].DestroyCamera();
                                }
                                IVisionCameraGroup[cntCamera].CreateByCameraInfo(cameraInfo);
                                IVisionCameraGroup[cntCamera].OpenCamera();
                                if (camExTime != null && camExTime[camNum] != null)
                                {
                                    double exposureT = (double.Parse(camExTime[camNum]));
                                    IVisionCameraGroup[cntCamera].SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                                }
                                break;
                            }
                        }
                        if (camID < 0) cntCamera++;
                    }
                    if (camID < 0)
                    {
                        //////////////////  DNN 설정 정보를 Screen Window 로 전달 및 DNN 설정 세팅 ////////////////////
                        for (int i = 0; i < TotalCameraNumber; i++)
                        {
                            IVisionCameraGroup[i].SetCamParameter(PLCamera.TriggerMode, PLCamera.TriggerSource, "On", "Line1");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " UpdateCamera");
                //ShowException(exception);
            }
        }

        public void SetCamID(int CamID)
        {
            camID = CamID;
        }


        public void StopGrabbingAll()
        {
            try
            {
                for (int i = 0; i < TotalCameraNumber; i++)
                {
                    if (IVisionCameraGroup[i] != null && IVisionCameraGroup[i].IsOpened())
                    {
                        IVisionCameraGroup[i].StopGrabbing();
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                //ShowException(exception);
                Console.WriteLine(ex.Message + " StopGrabbingAll");
            }
        }
        public void ResetCameraAll()
        {
            try
            {
                for(int i = 0; i < TotalCameraNumber; i++)
                {
                    if (IVisionCameraGroup[i] != null && IVisionCameraGroup[i].IsOpened())
                    {
                        IVisionCameraGroup[i].CloseCamera();
                        IVisionCameraGroup[i].DestroyCamera();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine(ex.Message + " ResetCameraAll");
                //ShowException(exception);
            }
        }




        public void StartGrabbingContinousCameraAll()
        {
            try
            {
                for (int i = 0; i < TotalCameraNumber; i++)
                {
                    if (IVisionCameraGroup[i] != null && IVisionCameraGroup[i].IsOpened())
                    {
                        IVisionCameraGroup[i].SetMainMode();
                        if (camID > 0)
                        {
                            IVisionCameraGroup[i].SetSettingMode();
                        }

                        IVisionCameraGroup[i].StartContinuousShotGrabbing();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " StartGrabbingContinousCameraAll");
            }
        }

        
        private void SetCameraConfig(int camNum, ICameraInfo selectedCameraInfo)
        {
            try
            {
                if (selectedCameraInfo != null)
                {
                    if (CameraCheckDictionary.ContainsKey(selectedCameraInfo).Equals(true) && CameraCheckDictionary[selectedCameraInfo] > 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Camera is already opened... Please select a different camera");
                        return;
                    }

                    CameraCheckDictionary.Add(selectedCameraInfo, 1);
                    
                    if (CamExtimeInfoList[camNum] != null && CamExtimeInfoList[camNum] != " " && CamExtimeInfoList[camNum] != "0")
                    {
                        System.Windows.Forms.MessageBox.Show(CamExtimeInfoList[camNum]);
                        double exposureT = (double.Parse(CamExtimeInfoList[camNum]));
                        IVisionCameraGroup[camNum].SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                        IVisionCameraGroup[camNum].SetGainParameter(PLCamera.Gain, 1.0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " SetCameraConfig");
                Console.WriteLine(ex.Message + " SetCameraConfig");
            }
        }









        


    }
}
