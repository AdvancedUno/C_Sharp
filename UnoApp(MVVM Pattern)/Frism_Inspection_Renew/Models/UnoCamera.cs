using Basler.Pylon;
using Frism_Inspection_Renew.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;





namespace Frism_Inspection_Renew.Models
{
    public class UnoCamera : IVisionCamera
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly double RENDERFPS = 10;

        private ImageFromCameraModel _returnImageFromCamera;
        public ImageFromCameraModel ReturnImageFromCamera { get => _returnImageFromCamera; set => _returnImageFromCamera = value; }


        private Object monitor = new Object();

        /////////////// Result Image /////////////
        private Bitmap latestFrame = null;

        /////////////// Full Screen Mode /////////////
        private bool m_bFullScreen = false;


        private Camera camera = null;
        private PixelDataConverter converter = new PixelDataConverter();

        private System.Diagnostics.Stopwatch stopwatch;

        private int frameDurationTicks;

        private BitmapImage bitmapImg { get; set; }
        private Dictionary<ICameraInfo, String> itemInfos;
        public ICameraInfo selectedCamera1 = null;

        /////////////// Grab Mode //////////////
        private bool Trigger = false;
        public bool MainMode = false;

        /////////////// Event Handlers /////////////
        public event EventHandler<EventArgs> GuiCameraOpenedCamera;
        public event EventHandler<EventArgs> GuiCameraClosedCamera;
        public event EventHandler<EventArgs> GuiCameraGrabStarted;
        public event EventHandler<EventArgs> GuiCameraGrabStopped;
        public event EventHandler<EventArgs> GuiCameraConnectionToCameraLost;
        public event EventHandler<ImageCapturedEventArgs> GuiCameraFrameReadyForDisplay;

        /////////////// Update Camera Infos /////////////
        public Dictionary<ICameraInfo, String> Items
        {
            get { return itemInfos; }
            set
            {
                itemInfos = value;
                OnPropertyChanged();
            }
        }

        public UnoCamera(int cameraId)
        {
            /////////////// Set FPS /////////////
            stopwatch = new System.Diagnostics.Stopwatch();

            double frametime = 1 / RENDERFPS;
            frameDurationTicks = (int)(System.Diagnostics.Stopwatch.Frequency * frametime);
            ReturnImageFromCamera = new ImageFromCameraModel(cameraId);
        }

        public Camera GetCam()
        {
            return camera;
        }

        /////////////// Full Screen Mode /////////////
        public void FullScreenMode()
        {
            if (!m_bFullScreen)
            {
                m_bFullScreen = true;

            }
            else
            {
                m_bFullScreen = false;
            }

        }

        /////////////// Set Grab Mode /////////////
        public void SetMainMode()
        {
            MainMode = true;
        }

        public void SetSettingMode()
        {
            MainMode = false;
        }

        /////////////// Return acquired Image /////////////
        public ImageFromCameraModel GetLatestFrame()
        {

            lock (monitor)
            {
                if (latestFrame != null)
                {

                    ReturnImageFromCamera.CapturedBitmapImage = latestFrame;
                    Trigger = false;
                    return ReturnImageFromCamera;
                }
                return null;
            }
        }


        /////////////// Check Cam Activities /////////////
        public bool IsCreated
        {
            get
            {
                return camera != null;
            }
        }
        public bool IsOpen
        {
            get
            {
                return IsCreated && camera.IsOpen;
            }
        }
        public bool IsGrabbing
        {
            get
            {
                return IsOpen && camera.StreamGrabber.IsGrabbing;
            }
        }


        /////////////// Return Cam Parameters /////////////
        public IParameterCollection GetParameters()
        {

            return camera != null ? camera.Parameters : null;

        }

        public bool IsOpened()
        {
            return IsOpen;
        }

        /////////////// Property Update Fnc /////////////
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        public void SetCamParameter(PLCamera.TriggerModeEnum triggerMode, PLCamera.TriggerSourceEnum triggerSource, string mode, string source)
        {
            camera.Parameters[triggerMode].SetValue(mode);
            camera.Parameters[triggerSource].SetValue(source);
        }


        /////////////// Update Cam List //////////////
        public void UpdateDeviceList()
        {
            try
            {

                List<ICameraInfo> cameraInfos = CameraFinder.Enumerate();


                if (cameraInfos.Count > 0)
                {

                    Dictionary<ICameraInfo, String> foundCameraInfos = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in cameraInfos)
                    {
                        foundCameraInfos.Add(cameraInfo, cameraInfo[CameraInfoKey.FriendlyName]);
                    }
                    /////////////// Assign Cam infos to Items (comboBox) on XAML /////////////
                    Items = foundCameraInfos;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }


        /////////////// Stop & End Grabbing /////////////
        public void StopGrabbing()
        {

            camera.StreamGrabber.Stop();
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }
        public void CloseCamera()
        {
            if (IsOpen)
            {

                ClearLatestFrame();
                camera.Close();
            }
        }
        protected void ClearLatestFrame()
        {
            lock (monitor)
            {
                if (latestFrame != null)
                {
                    latestFrame.Dispose();
                    latestFrame = null;
                }
            }
        }
        public void DestroyCamera()
        {
            try
            {
                if (IsGrabbing)
                {
                    StopGrabbing();
                }
                if (IsOpen)
                {
                    CloseCamera();
                }
                if (IsCreated)
                {
                    DisconnectFromCameraEvents();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        public bool CheckIsGrabbing()
        {
            return IsGrabbing;

        }

        public bool CheckIsCreated()
        {
            return IsCreated;
        }

        /////////////// Events /////////////
        public void OnConnectionLost(Object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = GuiCameraConnectionToCameraLost;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DestroyCamera();
                if (latestFrame != null)
                {
                    latestFrame.Dispose();
                }
                converter.Dispose();
            }
        }
        protected virtual void OnCameraOpened(Object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = GuiCameraOpenedCamera;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        protected virtual void OnCameraClosed(Object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = GuiCameraClosedCamera;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        protected virtual void OnGrabStarted(Object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = GuiCameraGrabStarted;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        protected virtual void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            EventHandler<EventArgs> handler = GuiCameraGrabStopped;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        protected virtual void RaiseEventFrameCaptured(ImageCapturedEventArgs e)
        {
            EventHandler<ImageCapturedEventArgs> handler = GuiCameraFrameReadyForDisplay;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }
        public void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                if (grabResult.GrabSucceeded)
                {

                    if (!stopwatch.IsRunning || stopwatch.ElapsedTicks >= 33)
                    {
                        stopwatch.Restart();
                        Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        converter.OutputPixelFormat = PixelType.BGRA8packed;
                        IntPtr ptrBmp = bmpData.Scan0;
                        converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                        bitmap.UnlockBits(bmpData);
                        lock (monitor)
                        {
                            if (latestFrame != null)
                            {
                                latestFrame.Dispose();
                            }
                            latestFrame = bitmap;
                            ReturnImageFromCamera.CapturedBitmapImage = latestFrame;
                        }
                        if (m_bFullScreen)
                        {
                            ImageWindow.DisplayImage(0, grabResult);
                        }

                        ImageCapturedEventArgs capturedEvent = new ImageCapturedEventArgs();
                        capturedEvent.imageFromCameraModel = ReturnImageFromCamera;

                        RaiseEventFrameCaptured(capturedEvent);


                    }
                }
                else
                {
                    System.Diagnostics.Debug.Print(grabResult.ErrorDescription);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);

            }
        }

        /////////////// Assign Fnc to Cam /////////////
        protected void ConnectToCameraEvents()
        {
            if (IsCreated)
            {
                camera.ConnectionLost += OnConnectionLost;
                camera.CameraOpened += OnCameraOpened;
                camera.CameraClosed += OnCameraClosed;
                camera.StreamGrabber.GrabStarted += OnGrabStarted;
                camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                camera.StreamGrabber.GrabStopped += OnGrabStopped;
            }
        }

        protected void DisconnectFromCameraEvents()
        {
            if (IsCreated)
            {
                camera.ConnectionLost -= OnConnectionLost;
                camera.CameraOpened -= OnCameraOpened;
                camera.CameraClosed -= OnCameraClosed;
                camera.StreamGrabber.GrabStarted -= OnGrabStarted;
                camera.StreamGrabber.ImageGrabbed -= OnImageGrabbed;
                camera.StreamGrabber.GrabStopped -= OnGrabStopped;
            }
        }


        public bool GetMainMode()
        {
            return MainMode;
        }
        public void ChangeFilePath(string fileCameraPath)
        {

        }
        public void SetExTimeParameter(FloatName name, double value)
        {
            camera.Parameters[name].SetValue(value);
            //camera.Parameters[PLCamera.Width].GetValue();
        }

        public void SetGainParameter(FloatName name, double value)
        {
            camera.Parameters[name].SetValue(value);
            //camera.Parameters[PLCamera.Width].GetValue();
        }


        /////////////// Create Cam by using ICameraInfo /////////////
        public void CreateByCameraInfo(ICameraInfo info)
        {
            if (IsCreated)
            {
                DestroyCamera();
            }
            camera = new Camera(info);



            ICameraInfo info2 = camera.CameraInfo;



            ConnectToCameraEvents();
        }

        /////////////// Continuous Grab /////////////
        public void StartContinuousShotGrabbing()
        {
            if (!MainMode)
            {
                Configuration.AcquireContinuous(camera, null);
            }

            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);


        }

        /////////////// Single Grab /////////////
        public void StartSingleShotGrabbing()
        {

            Configuration.AcquireSingleFrame(camera, null);
            camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByUser);
            camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);
            camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Line1);
        }

        /////////////// Software Trigger /////////////
        public void SoftwareTrigger()
        {
            camera.WaitForFrameTriggerReady(100, TimeoutHandling.ThrowException);
            camera.ExecuteSoftwareTrigger();
            Trigger = true;
        }

        public void ExecuteSoftwareTrigger(CommandName commandName)
        {
            camera.Parameters[commandName].Execute();
        }

        /////////////// Open Camera /////////////
        public void OpenCamera()
        {
            if (!IsCreated)
            {
                throw new InvalidOperationException("Cannot open camera. No camera has been created.");
            }
            if (!IsOpen)
            {
                camera.Open();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public void GiveImageFile(object sender, EventArgs e)
        {
            return;
        }

    }
}
