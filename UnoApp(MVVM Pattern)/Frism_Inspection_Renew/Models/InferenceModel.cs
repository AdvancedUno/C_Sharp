using NLog;
using OpenCvSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Basler.Pylon;

namespace Frism_Inspection_Renew.Models
{
    public class InferenceModel
    {

        private bool _stopped = false;
        public bool Stopped { get => _stopped; set => _stopped = value; }
       

        private bool _inferFlag = false;
        public bool InferFlag { get => _inferFlag; set => _inferFlag = value; }

        private bool _inferReadyFlag = false;
        public bool InferReadyFlag { get => _inferReadyFlag; set => _inferReadyFlag = value; }

        private bool _checkContinueInsp = true;
        public bool CheckContinueInsp { get => _checkContinueInsp; set => _checkContinueInsp = value; }

        private Stopwatch _timeInsp;
        public Stopwatch TimeInsp { get => _timeInsp; set => _timeInsp = value; }

        private Stopwatch _totalTimeProcess;
        public Stopwatch TotalTimeProcess { get => _totalTimeProcess; set => _totalTimeProcess = value; }

        private BlockingCollection<ImageGroupModel> _imageGroupModelBuffer;
        public BlockingCollection<ImageGroupModel> ImageGroupModelBuffer { get => _imageGroupModelBuffer; set => _imageGroupModelBuffer = value; }

        

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


        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public InferenceModel()
        {
            TotalTimeProcess = new Stopwatch();
            TimeInsp = new Stopwatch();
            ImageGroupModelBuffer = new BlockingCollection<ImageGroupModel>();
        }



        #region DNN Init
        public void InitThread(int numThread)
        {
            Logger.Debug("InitThread");
            try
            {
                int retCode = InitInspectNet(numThread);
                if (retCode > 0)
                {
                    Logger.Info("code : " + retCode);
                    return;
                }
                //SetParserFileLogOn("intelliz_d_test1_logs.txt");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "InitThread");
            }
        }
        #endregion

        #region DLL Inference


        public ImageGroupModel InferDLL(ImageGroupModel imageGroupModel)
        {
            Logger.Debug("InferDLL");
            
            try {


                CheckContinueInsp = false;
                Stopped = false;
                InferReadyFlag = true;

                Logger.Info("Ready to process");



                
                for (int i = 0; i < imageGroupModel.ImageInfoModelList.Count; i++)
                {
                    ImageInfoModel imageInfoModel = imageGroupModel.ImageInfoModelList[i];
                    imageGroupModel.ImageInfoModelList[i] = InferImage(ref imageInfoModel);
                }
                
                InferReadyFlag = false;
            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                Logger.Error(exception.Message + " InferDLL");
            }

            return imageGroupModel;
        }

        public void SettingDnnFirst(ImageGroupModel imageGroupModel)
        {
            
            foreach (ImageInfoModel imageInfoModel in imageGroupModel.GetImageInfoModels())
            {
                SetDnn(imageInfoModel);
            }
        }

        public void SetDnn(ImageInfoModel imageInfoModel)
        {
            
               
           
            Console.WriteLine("SetDnn");
            Logger.Debug("InferDLL");
            int maxThreadCount;
            int maxTileWidth;
            int maxTileHeight;
            int gpuNo;
            string dnnPath;
            int threadId;

            //bCheckContinueInsp = true;
            ///////////////// DNN 설정을 위한 파라미터들이 저장 되어있는지 확인 & 없으면 Base 값으로 세팅 //////////////
            if (imageInfoModel.DnnSettingInfoModel.MaxThreadCount > 0 && imageInfoModel.DnnSettingInfoModel.MaxTileHeight > 0 &&
                imageInfoModel.DnnSettingInfoModel.MaxTileWidth > 0 && imageInfoModel.DnnSettingInfoModel.GpuNum > -1 &&
                imageInfoModel.DnnSettingInfoModel.MinDefectSize > 0 && imageInfoModel.DnnSettingInfoModel.UppperPValue > 0)
            {
                maxThreadCount = imageInfoModel.DnnSettingInfoModel.MaxThreadCount;
                maxTileWidth = imageInfoModel.DnnSettingInfoModel.MaxTileWidth;
                maxTileHeight = imageInfoModel.DnnSettingInfoModel.MaxTileHeight;
                gpuNo = imageInfoModel.DnnSettingInfoModel.GpuNum;
                Logger.Info(imageInfoModel.DnnSettingInfoModel.MinDefectSize + " + " + imageInfoModel.DnnSettingInfoModel.UppperPValue);
            }
            else
            {
                maxThreadCount = 1;
                maxTileWidth = 800;
                maxTileHeight = 600;
                gpuNo = 0;
            }


            if (imageInfoModel.DnnSettingInfoModel.DnnPath == "0" || imageInfoModel.DnnSettingInfoModel.DnnPath == null)
            {
                Console.WriteLine("Cannot Find Dnn");
                Logger.Info("Cannot Find Dnn");
                return;
            }
            else
            {
                dnnPath = imageInfoModel.DnnSettingInfoModel.DnnPath.Replace('\\', '/');
                Console.WriteLine(dnnPath);
                Logger.Info(dnnPath);
            }

            try
            {

                Console.WriteLine("ThreadID  InferDLL : __" + imageInfoModel.DnnSettingInfoModel.ThreadId);
                threadId = imageInfoModel.DnnSettingInfoModel.ThreadId;
                threadId = 0;

                //InitInferMultiMaxTile(threadId, maxTileWidth, maxTileHeight);
                InitInferMultiMaxTile(threadId, maxTileWidth, maxTileHeight);
                SetInitImageSize(threadId, maxTileWidth, maxTileHeight);
                InitInferenceMulti(threadId, dnnPath);
                
                int retCode = InitInferenceMultiGPU(threadId, gpuNo);

                if (retCode > 0)
                {
                    Logger.Error("InitInferenceMultiGPU Error" + retCode);
                    Console.WriteLine("InitInferenceMultiGPU Error" + retCode);
                }

                IntPtr jsonInt = getClassJsonMulti(threadId);

                string json = Marshal.PtrToStringUni(jsonInt);

                Logger.Info(json);

                if (json.Length < 1)
                {
                    MessageBox.Show("클라스 정보를 확인 할 수 없습니다");
                }

                Console.WriteLine(json);

                Read_OCR_Info(json);



                double lowerPValue = imageInfoModel.DnnSettingInfoModel.UppperPValue;
                SetMultiClassProbability(threadId, 2, lowerPValue);

                float pValue = new float();


                Mat image = Cv2.ImRead("../../../../test.bmp", ImreadModes.Grayscale);

                //Mat src1 = new Mat(new OpenCvSharp.Size(1600, 1200), MatType.CV_8UC1, 255);
                int errorCode = 0;
                for (int i = 0; i < 5; i++)
                {

                    Mat src1 = new Mat(new OpenCvSharp.Size(1600, 1200), MatType.CV_8UC1, 255);
                    //errorCode = InspectMultiGetP_Ptr(iThreadID, src1.CvPtr, src1.CvPtr, ref pValue);
                    errorCode = InspectMultiGetP_Select(threadId, image.CvPtr, 2, src1.CvPtr, ref pValue);
                    if (errorCode > 0)
                    {
                        Logger.Error("error Code : " + errorCode);
                        break;
                    }



                    //Console.WriteLine("Error Code :  " + errorCode);
                    src1.Dispose();
                }

                image.Dispose();

                if (errorCode > 0)
                {
                    //Program.ErrorBlow();
                    Logger.Warn("error Code : " + errorCode);
                }

                if (retCode > 0)
                {
                    //Program.ErrorBlow();

                    Logger.Error("Error : " + retCode + "Thread ID: " + imageInfoModel.DnnSettingInfoModel.ThreadId);


                    return;
                }

            }
            catch (Exception exception)
            {
                System.Windows.Forms.MessageBox.Show(exception.Message);
                Logger.Error(exception.Message + " SetDnn");
            }
            

        }

        #endregion

        #region DLL Inference
        public ImageInfoModel InferImage(ref ImageInfoModel imageInfoModel)
        {

            return imageInfoModel;
            TotalTimeProcess.Reset();
            TimeInsp.Reset();
            TotalTimeProcess.Start(); ///// process time 시작
            Console.WriteLine("Thead ID InferImage() : _________ " + Thread.CurrentThread.ManagedThreadId);
            //m_bInferFlag = false;
            int label = 1; // fixed & unsed
            int iNG = 0;
            Mat mat;
            Mat outputMat;// = OpenCvSharp.Extensions.BitmapConverter.ToMat(newImage);

            //System.Drawing.Rectangle rRoiArea = new System.Drawing.Rectangle(iPosX, iPosY, iWidth, iHeight);
            Bitmap newImage = imageInfoModel.BitmapRawImage;
            Bitmap bRoiImage = imageInfoModel.BitmapRawImage;
            Bitmap resultImage;

            if (newImage != null)
            {
                //Program.SendShowCameraSignal();
                //if (iWidth > 0 && iHeight > 0)
                //{
                //    bRoiImage = newImage.Clone(rRoiArea, newImage.PixelFormat);
                //}
                mat = OpenCvSharp.Extensions.BitmapConverter.ToMat(bRoiImage);
                if (mat.Channels() > 1)
                {
                    Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2GRAY);
                }
                outputMat = new Mat();
                try
                {

                    TimeInsp.Start();
                    float pValue = new float();
                    int errorCode = 0;

                    errorCode = InspectMultiGetP_Select(imageInfoModel.DnnSettingInfoModel.ThreadId, mat.CvPtr, 2, outputMat.CvPtr, ref pValue);
                    if (errorCode > 0)
                    {
                        Logger.Warn("error Code : " + errorCode);
                    }
                    int defectCount = AnalyzeDefectInfo(outputMat.CvPtr, label, imageInfoModel.DnnSettingInfoModel.MinDefectSize);
                    TimeInsp.Stop();

                    if (defectCount > 0)
                    {
                        iNG = defectCount;
                    }

                    bool checkNG = true;

                    if (iNG > 0)
                    {
                        // ngResult.imgNG();
                        checkNG = true;
                        Console.WriteLine("NG");
                    }
                    else
                    {
                        //ngResult.imgOK();
                        checkNG = false;
                    }

                    imageInfoModel.ResultImage.BitmapResultImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(outputMat.Clone());

                    outputMat.Release();
                    mat.Release();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception.Message + " ");
                }
            }
            else
            {
                Console.WriteLine("NoImageggggggggggggggggggggggggggggg");
            }
            return imageInfoModel;


        }


    #endregion




    }
}
