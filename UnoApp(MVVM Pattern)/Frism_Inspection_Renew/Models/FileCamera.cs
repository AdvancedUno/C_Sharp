using Basler.Pylon;
using Frism_Inspection_Renew.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Basler.Pylon.PLCamera;

namespace Frism_Inspection_Renew.Models
{
    public class MyParameterCollection : IParameterCollection
    {

        System.Windows.Threading.DispatcherTimer timer;

        public IArrayParameter this[ArrayName name] => throw new NotImplementedException();

        public IEnumParameter this[EnumName name] => throw new NotImplementedException();

        public IStringParameter this[StringName name] => throw new NotImplementedException();

        public ICommandParameter this[CommandName name] => throw new NotImplementedException();

        public IBooleanParameter this[BooleanName name] => throw new NotImplementedException();

        public IFloatParameter this[FloatName name] => throw new NotImplementedException();

        public IIntegerParameter this[IntegerName name] => throw new NotImplementedException();

        public IParameter this[string name] => throw new NotImplementedException();

        public bool Contains(ArrayName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(EnumName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(StringName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(CommandName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(BooleanName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(FloatName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IntegerName name)
        {
            throw new NotImplementedException();
        }

        public bool Contains(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IParameter> GetEnumerator()
        {
            //return null;
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetParameterRelation(string name, ParameterRelation relation, bool filterInternalParameters)
        {
            throw new NotImplementedException();
        }

        public void Load(string filename, string parameterPath)
        {
            throw new NotImplementedException();
        }

        public void Poll(long elapsedTime)
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void Save(string filename, string parameterPath)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class FileCamera : IVisionCamera
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        bool isOpen = false;
        bool isCreated = false;

        private ImageFromCameraModel _returnImageFromCamera;
        public ImageFromCameraModel ReturnImageFromCamera { get => _returnImageFromCamera; set => _returnImageFromCamera = value; }


        private Queue<Bitmap> queue = new Queue<Bitmap>();

        private readonly double RENDERFPS = 10;

        private Object monitor = new Object();

        /////////////// Result Image /////////////
        private Bitmap latestFrame = null;

        /////////////// Full Screen Mode /////////////
        private bool m_bFullScreen = false;

        //Camera garbageCam = new Camera();


        IParameterCollection parameters = new MyParameterCollection();


        //private Camera camera = null;
        private PixelDataConverter converter = new PixelDataConverter();



        private int frameDurationTicks;

        private BitmapImage bitmapImg { get; set; }
        private Dictionary<ICameraInfo, String> itemInfos;
        public ICameraInfo selectedCamera1 = null;

        /////////////// Grab Mode //////////////
        private bool Trigger = false;
        public bool MainMode = false;
        private bool bIsGrabbing = false;



        public string directoryPath = null;

        System.Windows.Threading.DispatcherTimer timer;

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


        public FileCamera(string path, int cameraId)
        {
            directoryPath = path;

            ReturnImageFromCamera = new ImageFromCameraModel(cameraId);
            /////////////// Set FPS /////////////
            //stopwatch = new System.Diagnostics.Stopwatch();




            //timer = new System.Windows.Threading.DispatcherTimer();

            //timer.Tick += timer_Tick;

            //timer.Interval = TimeSpan.FromMilliseconds(400);



            // parameters[PLCamera.Gain].SetValue(1.0);
            // parameters[PLCamera.ExposureTime].SetValue(2000.0);

            //timer.Start();
        }


        //public Camera GetCam()
        //{
        //    return camera;
        //}

        /////////////// Full Screen Mode /////////////
        //public void FullScreenMode()
        //{
        //    if (!m_bFullScreen)
        //    {
        //        m_bFullScreen = true;

        //    }
        //    else
        //    {
        //        m_bFullScreen = false;
        //    }

        //}

        /////////////// Set Grab Mode /////////////
        public void SetMainMode()
        {
            MainMode = true;
        }

        public void SetSettingMode()
        {
            MainMode = false;
        }

        public void ChangeFilePath(string fileCameraPath)
        {
            directoryPath = fileCameraPath;
        }



        /////////////// Return acquired Image /////////////
        public ImageFromCameraModel GetLatestFrame()
        {
            //if (MainMode)
            //{
            //    return null;
            //}
            if (latestFrame != null)
            {
                //Trigger = false;
                ReturnImageFromCamera.CapturedBitmapImage = latestFrame;
                Trigger = false;
                return ReturnImageFromCamera;


            }
            Console.WriteLine("Returning Null");
            return null;


        }


        /////////////// Return Cam Parameters /////////////
        public IParameterCollection Parameters
        {
            get
            {
                //return camera != null ? camera.Parameters : null;
                return null;
            }
        }





        /////////////// Property Update Fnc /////////////
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                Logger.Error(ex.Message + " UpdateDeviceList");
                //Helper.ShowException(exception);
            }
        }


        public bool GetMainMode()
        {
            return MainMode;
        }


        public IParameterCollection GetParameters()
        {


            //return null;
            return parameters;
        }



        /////////////// Stop & End Grabbing /////////////
        public void StopGrabbing()
        {

            //camera.StreamGrabber.Stop();


            timer.Stop();
        }

        public void CloseCamera()
        {
            if (isOpen)
            {

                timer.Stop();

                ClearLatestFrame();

                //camera.Close();
            }
        }
        public void ClearLatestFrame()
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
                if (isOpen)
                {
                    CloseCamera();
                }
                if (isCreated)
                {
                    //DisconnectFromCameraEvents();
                    //camera.Dispose(); 
                    //camera = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DestroyCamera");
                //ShowException(exception);
            }
        }

        public void FullScreenMode()
        {

        }


        public void SetExTimeParameter(FloatName name, double value)
        {

        }

        public void SetGainParameter(FloatName name, double value)
        {

        }

        public bool CheckIsCreated()
        {
            return isCreated;
        }


        /////////////// Events /////////////
        public void OnConnectionLost(object sender, EventArgs e)
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
        protected virtual void OnCameraOpened(object sender, EventArgs e)
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
            
            GuiCameraFrameReadyForDisplay.Invoke(this, e);

        }

        public void GiveImageFile(object sender, EventArgs e)
        {
            try
            {

                //Program.SendShowCameraSignal();

                latestFrame = queue.Dequeue();

                //queue.Enqueue((Bitmap)latestFrame.Clone());
                //}
                ReturnImageFromCamera.CapturedBitmapImage = latestFrame;
                ImageCapturedEventArgs capturedEvent = new ImageCapturedEventArgs();
                capturedEvent.imageFromCameraModel = ReturnImageFromCamera;
                capturedEvent.cameraNumber = ReturnImageFromCamera.CameraId;

                RaiseEventFrameCaptured(capturedEvent);


            }
            catch (Exception ex)
            {

                Logger.Error(ex.Message + " timer_Tick");
            }
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            try
            {


                //Program.SendShowCameraSignal();

                //lock (monitor)
                //{
                //    if (latestFrame != null)
                //    {
                //        latestFrame.Dispose();
                //    }

                latestFrame = queue.Dequeue();
                if(latestFrame != null)
                {
                    ReturnImageFromCamera.CapturedBitmapImage = latestFrame;
                }
                
                //queue.Enqueue((Bitmap)latestFrame.Clone());

                //}

                //Console.WriteLine("aaaaa");

                ImageCapturedEventArgs capturedEvent = new ImageCapturedEventArgs();
                capturedEvent.imageFromCameraModel = ReturnImageFromCamera;

                RaiseEventFrameCaptured(capturedEvent);


            }
            catch (Exception ex)
            {

                Logger.Error(ex.Message + " timer_Tick");
            }
        }



        /////////////// Create Cam by using ICameraInfo /////////////
        public void CreateByCameraInfo(ICameraInfo info)
        {
            if (isCreated)
            {
                DestroyCamera();
            }

            //camera = new Camera(info);

            //ConnectToCameraEvents();
        }

        /////////////// Continuous Grab /////////////
        public void StartContinuousShotGrabbing()
        {

            if (!MainMode)
            {
                //Configuration.AcquireContinuous(camera, null);
                OpenCamera();
            }
            OpenCamera();


            //camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);

        }

        public void ExecuteSoftwareTrigger(CommandName commandName)
        {

        }

        /////////////// Single Grab /////////////
        //public void StartSingleShotGrabbing()
        //{            
        //    Configuration.AcquireSingleFrame(camera, null);
        //    camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByUser);
        //    camera.Parameters[PLCamera.TriggerMode].SetValue(PLCamera.TriggerMode.On);
        //    camera.Parameters[PLCamera.TriggerSource].SetValue(PLCamera.TriggerSource.Line1);
        //}

        /////////////// Software Trigger /////////////
        public void SoftwareTrigger()
        {
            //camera.WaitForFrameTriggerReady(100, TimeoutHandling.ThrowException);
            //camera.ExecuteSoftwareTrigger();
            Trigger = true;
        }

        protected List<string> GetImageFiles()
        {

            List<string> imageFileList = new List<string>();


            foreach (string fileName in Directory.GetFiles(directoryPath))
            {
                if (Regex.IsMatch(fileName, @".jpg|.png|.bmp|.JPG|.PNG|.BMP|.JPEG|.jpeg$"))
                {
                    imageFileList.Add(fileName);
                }
            }


            return imageFileList;
        }

        /////////////// Open Camera /////////////
        public void OpenCamera()
        {


            if (!isCreated)
            {
                //throw new InvalidOperationException("Cannot open camera. No camera has been created.");
            }
            if (true)
            {
                try
                {
                    // 폴더의 이미지를 읽어와 큐에 넣는다.
                    List<string> imageFiles = GetImageFiles();


                    foreach (string imagePath in imageFiles)
                    {
                        Bitmap bitmap = new Bitmap(imagePath);


                        queue.Enqueue(bitmap);
                    }



                    //timer.Start();
                    GuiCameraOpenedCamera.Invoke(this, EventArgs.Empty);

                    //timer.Tick += GiveImageFile;
                    //timer.Interval = TimeSpan.FromMilliseconds(300);

                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + "FileCamerea");
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool IsOpened()
        {
            //throw new NotImplementedException();
            return true;
        }

        public void SetCamParameter(TriggerModeEnum triggerMode, TriggerSourceEnum triggerSource, string mode, string source)
        {

        }




        public void CreateCamera()
        {
            throw new NotImplementedException();
        }



        public bool CheckIsGrabbing()
        {
            return bIsGrabbing;
        }

    }
}

