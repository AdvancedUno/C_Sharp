using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Frism
{



   

    public partial class ScreenWindow : INotifyPropertyChanged
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //private UnoCamera camera;
        private IVisionCamera camera = null;
        private Bitmap image = null;
        private BitmapImage bitmapImg = null;
        public static Bitmap outImage = null;

        private Stopwatch timeInsp;
        private Stopwatch timeProcess;

        private int iPosX = 0;
        private int iPosY = 0;
        private int iWidth = 0;
        private int iHeight = 0;

        private double dPosX = 0;
        private double dPosY = 0;
        private double dWidth = 0;
        private double dHeight = 0;

        private bool bCheckContinueInsp = true;

        public string sDnnPath = "0";

        public int iThreadID;


        private int minDefectSize;
        private float uppperPValue;


        public ngClass ngResult;

        private bool startedPaint;

        private System.Windows.Point downPoint;
        private System.Windows.Point upPoint;


        int cameraId;
        string savePath;
        bool saveImage;

        private bool m_bStopped = false;
        private bool m_bInferFlag = false;

        

        


        public ScreenWindow()
        {
            InitializeComponent();
            InitVariables();

            MouseDown += MainWindow_MouseDown;
            MouseMove += MainWindow_MouseMove;
            MouseUp += MainWindow_MouseUp;

            DataContext = this;
            //BitmapImage img = new BitmapImage(new Uri("FrismMain\\frism_V splash.png"));
            //setImage(img);
            

                 
        }


        #region INTELLIZ DLL 마샬링
        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InitInspectNet", ExactSpelling = false)]
        extern public static int InitInspectNet(int maxThreadCount);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "SetInitImageSize", ExactSpelling = true)]
        extern public static void SetInitImageSize(int iNo, int iMaxWidth, int iMaxHeight);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InitInferMultiMaxTile", ExactSpelling = true)]
        extern public static void InitInferMultiMaxTile(int iNo, int iMaxWidth, int iMaxHeight);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InitInferenceMulti", ExactSpelling = true)]
        extern public static void InitInferenceMulti(int iNo, string path);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InitInferenceMultiGPU", ExactSpelling = true)]
        extern public static int InitInferenceMultiGPU(int iNo, int gpuNo);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "SetMultiClassProbability", ExactSpelling = true)]
        extern public static void SetMultiClassProbability(int iNo, int iClass, double fProb);
        
        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "SetMultiProbabilityAll", ExactSpelling = true)]
        extern public static void SetMultiProbabilityAll(double fProb);


        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InspectMultiGetP_Ptr", ExactSpelling = true)]
        extern public static int InspectMultiGetP_Ptr(int iNo, IntPtr srcImg, IntPtr outImg, ref float pValue);
        
        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InspectMultiGetP_Select", ExactSpelling = true)]
        extern public static int InspectMultiGetP_Select(int iNo, IntPtr srcImg, int selectNum, IntPtr outImg, ref float pValue);

        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "SetInferFileLogOn", ExactSpelling = true)]
        extern public static void SetInferFileLogOn(string path);

        [DllImport("INTELLIZ_D_OCR.dll", CharSet = CharSet.None, EntryPoint = "AnalyzeDefectInfo", ExactSpelling = true)]
        extern public static int AnalyzeDefectInfo(IntPtr resultImage, int index, double fFilterSize);

        //[DllImport("INTELLIZ_D_OCR.dll", CharSet = CharSet.None, EntryPoint = "SetParserFileLogOn", ExactSpelling = true)]
        //extern public static int SetParserFileLogOn(string path);



        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.Unicode, EntryPoint = "getClassJsonMulti", ExactSpelling = true)]
        extern public static IntPtr getClassJsonMulti(int id);

        [DllImport("INTELLIZ_D_OCR.dll", CharSet = CharSet.Unicode, EntryPoint = "Read_OCR_Info", ExactSpelling = true)]
        extern public static void Read_OCR_Info(string unicode);

        //[DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "DestoryAllNet", ExactSpelling = true)]
        //extern public static void DestoryAllNet();
        #endregion



        public void InitVariables()
        {
            try
            {
                timeInsp = new Stopwatch();
                timeProcess = new Stopwatch();
                minDefectSize = 5;
                uppperPValue = 0.80f;
                timeInsp = new Stopwatch();
                timeProcess = new Stopwatch();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " InitVariables");
                throw e;
            }
        }

        public BitmapImage ImageSource
        {
            get { return bitmapImg; }
            set
            {
                this.bitmapImg = value;
                OnPropertyChanged("ImageSource");
            }
        }


        #region 카메라 및 저장 경로 세팅

        public void SetCamera(IVisionCamera cam)
        {

            camera = cam;
            camera.GuiCameraFrameReadyForDisplay += OnImageReady;


        }

        public void SetSavePath(string path, int id)
        {

            this.cameraId = id;
            this.savePath = path;
            this.saveImage = true;
        }

        #endregion

        public void setImage(BitmapImage img)
        {
            ImageSource = img;
        }

        public void GetNG(ngClass ngClassInfo)
        {
            ngResult = ngClassInfo;
        }


        public IVisionCamera GiveUno()
        {

            return camera;
        }

        public static Bitmap GetBitmapImg()
        {

            return outImage;
        }




        #region PropertyChangedEventHandler 연동
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
         }
        #endregion



        #region DNN Init
        public void InitThread()
        {


            Logger.Debug("InitThread");
            if (Program.bInspectBtn != true) return;
            try
            {
                int retCode = InitInspectNet(4);

                if (retCode > 0)
                {
                    Logger.Info("code : " + retCode);
                    return;
                }

                //SetParserFileLogOn("intelliz_d_test1_logs.txt");
                

            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + "InitThread");
            }
            
        }
        #endregion

        
        #region DLL Inference



        public void InferDLL(int iMaxThreadCount, int iMaxTileWidth, int iMaxTileHeight, int iGpuNo, int iMinDefectSizeTop, float fUppperPValueTop, int iMinDefectSizeSide, float fUppperPValueSide)
        {
            Logger.Debug("InferDLL");
            int maxThreadCount;
            int maxTileWidth;
            int maxTileHeight;
            int gpuNo;

            bCheckContinueInsp = true;

            if (Program.bInspectBtn != true) return;
            
            ///////////////// DNN 설정을 위한 파라미터들이 저장 되어있는지 확인 & 없으면 Base 값으로 세팅 //////////////
            if (iMaxThreadCount > 0 && iMaxTileHeight > 0 && iMaxTileWidth > 0 && iGpuNo > -1 && iMinDefectSizeTop > 0 && fUppperPValueTop > 0)
            {
                maxThreadCount = iMaxThreadCount;
                maxTileWidth = iMaxTileWidth;
                maxTileHeight = iMaxTileHeight;
                gpuNo = iGpuNo;
                if(cameraId != 100)
                {
                    minDefectSize = iMinDefectSizeSide;
                    uppperPValue = fUppperPValueSide;
                    Logger.Info(minDefectSize + " + " + uppperPValue);
                }
                else
                {
                    minDefectSize = iMinDefectSizeTop;
                    uppperPValue = fUppperPValueTop;
                    Logger.Info(minDefectSize + " + " + uppperPValue);
                }
                


            }
            else
            {
                maxThreadCount = 1;
                maxTileWidth = 800;
                maxTileHeight = 600;
                gpuNo = 0;
            }
       

           
            if (sDnnPath == "0" || sDnnPath == null)
            {
                Logger.Info("Cannot Find Dnn");
                return;
            }
            else
            {
               
                sDnnPath = sDnnPath.Replace('\\', '/');
                Logger.Info(sDnnPath);
            }


            try
            {
                Console.WriteLine("ThreadID : __" + iThreadID);

                InitInferMultiMaxTile(iThreadID, maxTileWidth, maxTileHeight);
                SetInitImageSize(iThreadID, maxTileWidth, maxTileHeight);
                InitInferenceMulti(iThreadID, sDnnPath);

               
                int retCode = InitInferenceMultiGPU(iThreadID, gpuNo);

               

                IntPtr jsonInt = getClassJsonMulti(iThreadID);
                

                string json = Marshal.PtrToStringUni(jsonInt);

                Logger.Info(json);

                Read_OCR_Info(json);

               

                double lowerPValue = uppperPValue;
                SetMultiClassProbability(iThreadID, 2, lowerPValue);
                
                float pValue = new float();


                Mat image = Cv2.ImRead("../../../../test.bmp", ImreadModes.Grayscale);


                //Mat src1 = new Mat(new OpenCvSharp.Size(1600, 1200), MatType.CV_8UC1, 255);
                int errorCode = 0;
                for (int i = 0; i < 3; i++)
                {
                    
                    Mat src1 = new Mat(new OpenCvSharp.Size(1600, 1200), MatType.CV_8UC1, 255);
                    //errorCode = InspectMultiGetP_Ptr(iThreadID, src1.CvPtr, src1.CvPtr, ref pValue);
                    errorCode = InspectMultiGetP_Select(iThreadID, image.CvPtr, 2, src1.CvPtr, ref pValue);
                    //Console.WriteLine("Error Code :  " + errorCode);
                    src1.Dispose();
                }
                Console.WriteLine("Thead ID : _________ " + Thread.CurrentThread.ManagedThreadId);




                image.Dispose();

                
                if (errorCode > 0)
                {
                    Program.ErrorBlow();
                    Logger.Warn("error Code : " + errorCode);
                }

                if (retCode > 0)
                {
                    Program.ErrorBlow();

                    Logger.Error("Error : " + retCode + "Thread ID: " + iThreadID);


                    return;
                }
                bCheckContinueInsp = false;

                m_bStopped = false;
                while (true)
                {

                    Thread.Sleep(1);

                    if (m_bStopped)
                    {
                        //Console.WriteLine("bbbbbb");
                        break;
                    }

                    if (m_bInferFlag)
                    {
                        //Console.WriteLine("aaaaaaaa");
                        InferImage();
                    }


                }


            }
            catch(Exception exception)
            {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                Logger.Error(exception.Message + " InferDLL");
            }
        }

        #endregion


       

        #region  "OnImageReady" - 이미지 받아오기 및 후 처리
        private void OnImageReady(Object sender, EventArgs e)
        {

            
            

            try
                {
                //Thread cur_thread = Thread.CurrentThread;
                //Console.WriteLine(cur_thread.ManagedThreadId.ToString());
                //Console.WriteLine("current thread = {1}", cur_thread.ManagedThreadId);

                if (camera != null)
                {
                    Console.WriteLine("Thead ID : _________ " + Thread.CurrentThread.ManagedThreadId);



                    m_bInferFlag = true;

                    //}));
                }
                
            }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message + "ScreenWindow");
                }
            
        }


        public void InferImage()
        {
            timeProcess.Reset();
            timeInsp.Reset();


            timeProcess.Start(); ///// process time 시작
            Console.WriteLine("Thead ID : _________ " + Thread.CurrentThread.ManagedThreadId);
            m_bInferFlag = false;
            int label = 1; // fixed & unsed
            int iNG = 0;
            Mat mat;
            Mat outputMat;// = OpenCvSharp.Extensions.BitmapConverter.ToMat(newImage); ;

            System.Drawing.Rectangle rRoiArea = new System.Drawing.Rectangle(iPosX, iPosY, iWidth, iHeight);
            Bitmap newImage = camera.GetLatestFrame();
            Bitmap bRoiImage = camera.GetLatestFrame();
            Bitmap resultImage = camera.GetLatestFrame();



            if (newImage != null && camera.GetMainMode())
            {
                Program.SendShowCameraSignal();

                if (iWidth > 0 && iHeight > 0)
                {

                    bRoiImage = newImage.Clone(rRoiArea, newImage.PixelFormat);
                }

                if (!bCheckContinueInsp)
                {

                    bCheckContinueInsp = true;


                    mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bRoiImage);
                    if (mat.Channels() > 1)
                    {
                        Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
                    }

                    outputMat = new Mat();

                    try
                    {
                        if (Program.bInspectBtn != false)
                        {


                            timeInsp.Start();

                            float pValue = new float();

                            int errorCode = 0;

                            Console.WriteLine("ITHreadID : __ " + iThreadID);

                            //errorCode = InspectMultiGetP_Ptr(iThreadID, mat.CvPtr, outputMat.CvPtr, ref pValue);
                            errorCode = InspectMultiGetP_Select(iThreadID, mat.CvPtr, 2, outputMat.CvPtr, ref pValue);

                            timeInsp.Stop();

                            //Cv2.ImWrite("image.bmp", outputMat);

                            if (errorCode > 0)
                            {
                                Logger.Warn("error Code : " + errorCode);
                            }

                            //if (pValue >= uppperPValue)
                            //{
                            Console.WriteLine("pvalue : " + pValue);
                            Console.WriteLine("minDefectSize : " + minDefectSize);
                            //int defectCount = 0;
                            int defectCount = AnalyzeDefectInfo(outputMat.CvPtr, label, minDefectSize);

                            Console.WriteLine("defectCount : " + defectCount);
                            if (defectCount > 0)
                            {
                                iNG = defectCount;
                            }
                            //}



                            bool checkNG = true;

                            if (iNG > 0)
                            {
                                ngResult.imgNG();
                                checkNG = true;
                                Console.WriteLine("NG");
                            }
                            else
                            {
                                ngResult.imgOK();
                                checkNG = false;
                                Console.WriteLine("OK");
                            }

                            if (cameraId == 0)
                            {
                                Console.WriteLine(checkNG);
                                Program.GetNG1(checkNG);
                            }
                            else if (cameraId == 120)
                            {
                                Console.WriteLine(checkNG);
                                Program.GetNG2(checkNG);
                            }
                            else if (cameraId == 100)
                            {
                                Console.WriteLine(checkNG);
                                Program.GetNGTop(checkNG);
                            }
                            else if (cameraId == 240)
                            {
                                Console.WriteLine(checkNG);
                                Program.GetNG3(checkNG);
                            }
                            resultImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outputMat.Clone());
                            outputMat.Release();
                            mat.Release();

                            bCheckContinueInsp = false;

                        }



                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message + "OnImageReady");
                    }
                }

            }

            if (newImage != null)
            {
                if (image != null)
                {
                    image.Dispose();
                }
                if (outImage != null)
                {
                    outImage.Dispose();
                }
                image = bRoiImage;

                ImageSource = ToBitmapImage(bRoiImage);

                timeProcess.Stop();
                //Task.Factory.StartNew((Action)(() =>
                //{




                if (camera.GetMainMode())
                {
                    if (cameraId == 0)
                    {
                        Program.GetTime1(timeInsp.ElapsedMilliseconds, timeProcess.ElapsedMilliseconds);
                        Program.SaveImg2(image, resultImage);
                    }
                    else if (cameraId == 120)
                    {
                        Program.GetTime2(timeInsp.ElapsedMilliseconds, timeProcess.ElapsedMilliseconds);
                        Program.SaveImg3(image, resultImage);
                    }
                    else if (cameraId == 100)
                    {
                        Program.GetTimeTop(timeInsp.ElapsedMilliseconds, timeProcess.ElapsedMilliseconds);
                        Program.SaveImg1(image, resultImage);
                    }
                    else if (cameraId == 240)
                    {
                        Program.GetTime3(timeInsp.ElapsedMilliseconds, timeProcess.ElapsedMilliseconds);
                        Program.SaveImg4(image, resultImage);
                    }
                }

                if (camera.GetMainMode())
                {
                    Program.SendEndShowCameraSignal();
                }



            }
        }


        #endregion


        #region Bitmap 형식을 BitmapImage 로 변환 
        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage temp = new BitmapImage();
            try
            {

                using (var memory = new MemoryStream())
                {

                    bitmap.Save(memory, ImageFormat.Bmp);

                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    temp = bitmapImage;
                }
            }catch(Exception ex)
            {
                Logger.Error(ex.Message + "toBitmap");
            }
            return temp;
        }

        #endregion

        public void Clear()
        {
            if (this.image != null)
            {
                image.Dispose();
            }
            this.image = null;
        }


        public void Destory()
        {
            Logger.Info("Net Destroy");
            m_bStopped = true;
            //DestoryAllNet();
            m_bStopped = true;
        }





        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (camera.GetMainMode()) return;

            startedPaint = true;

            downPoint =  e.GetPosition(ImageGrid);
        }



        private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (camera.GetMainMode()) return;

            upPoint = e.GetPosition(ImageGrid);
            startedPaint = false;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (camera.GetMainMode()) return;
            if (startedPaint)
            {

                var point = e.GetPosition(ImageGrid);

                if(point.X < downPoint.X || point.Y < downPoint.Y)
                {
                    return;
                }

                var rect = new System.Windows.Rect(downPoint, point);

                
                
                
                
                CropRectangle.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
                CropRectangle.Width = rect.Width;
                CropRectangle.Height = rect.Height;
                CropRectangle.Visibility = Visibility.Visible;
                if (rect.Width < 100 || rect.Height < 100)
                {
                    CropRectangle.Visibility = Visibility.Hidden;
                    return;
                }

                double temp = 390.0 / 1200.0 * 1600.0;
                dPosX = downPoint.X / Math.Round(temp,6);
                dPosY = downPoint.Y / Math.Round(PreviewImage.Height,6);
                dWidth = Math.Round(rect.Width,3)/ Math.Round(temp, 6);
                dHeight = Math.Round(rect.Height,3)/ Math.Round(PreviewImage.Height,6);
            }
        }

        public string GetCropValToSave()
        {
            string listCropVal = "null";
            if (dPosX + dPosY + dWidth + dHeight > 0)
            {
               listCropVal = string.Format("{0},{1},{2},{3}", Math.Round(dPosX, 6), Math.Round(dPosY, 6), Math.Round(dWidth, 6), Math.Round(dHeight, 6));
            }
            
            return listCropVal;
        }

        public void SetCropVal(string sCropInfo)
        {
            string[] collectionCropInfo = sCropInfo.Split(',');
            iPosX = (int)(float.Parse(collectionCropInfo[0])*1600);
            iPosY = (int)(float.Parse(collectionCropInfo[1])*1200);
            iWidth = (int)(float.Parse(collectionCropInfo[2])*1600);
            iHeight = (int)(float.Parse(collectionCropInfo[3])*1200);
        }

    }

}