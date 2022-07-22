using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Automation.BDaq;

namespace Frism
{
    public class BlowDelegateClass
    {
        public delegate void MyEventHandelr();
        public event MyEventHandelr blowEvent;
        public void StartBlowing()
        {
            blowEvent?.Invoke();
        }
    };

    public class Images
    {
        public Bitmap ImageTop;
        public Bitmap ImageSide1;
        public Bitmap ImageSide2;
        public Bitmap ImageSide3;

        public Bitmap ResultImageTop;
        public Bitmap ResultImageSide1;
        public Bitmap ResultImageSide2;
        public Bitmap ResultImageSide3;

        public string sSaveFolderPath;

    }



    public class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static bool NGTop;
        public static bool NG1;
        public static bool NG2;
        public static bool NG3;
        public static long elpsTimeTop = 0;
        public static long elpsTime1 = 0;
        public static long elpsTime2 = 0;
        public static long elpsTime3 = 0;
        public static long processTimeTop = 0;
        public static long processTime1 = 0;
        public static long processTime2 = 0;
        public static long processTime3 = 0;
        public static long maxInspTime = 0;
        public static long maxProcessTime = 0;
        public static bool checkFolderPath = false;
        public static object lockObject = new object();
        public static Queue<string> imagePathTop = new Queue<string>();
        public static Queue<string> imagePath1 = new Queue<string>();
        public static Queue<string> imagePath2 = new Queue<string>();
        public static Queue<string> imagePath3 = new Queue<string>();


        public static Queue<bool> makeFolderQueue = new Queue<bool>();

        public static Queue<Bitmap> imageSaveQueueTop = new Queue<Bitmap>();
        public static Queue<Bitmap> imageSaveQueue1 = new Queue<Bitmap>();
        public static Queue<Bitmap> imageSaveQueue2 = new Queue<Bitmap>();
        public static Queue<Bitmap> imageSaveQueue3 = new Queue<Bitmap>();

        public static Queue<Bitmap> imageResultSaveQueueTop = new Queue<Bitmap>();
        public static Queue<Bitmap> imageResultSaveQueue1 = new Queue<Bitmap>();
        public static Queue<Bitmap> imageResultSaveQueue2 = new Queue<Bitmap>();
        public static Queue<Bitmap> imageResultSaveQueue3 = new Queue<Bitmap>();


        




        public static object lockThreadCntObject = new object();
        public static object lockShowCameraSignalObject = new object();
        public static object lockShowEndCameraSignalObject = new object();
        public static object lockBlowObject = new object();
        public static object lockGetTimeObject = new object();

        public static int checkThreadCnt = 0;
        public static int NGCallCnt = 0;
        public static int TimeCallCnt = 0;
        public static int NGCnt = 0;
        public static int OKCnt = 0;

        public static string desiredName = null;
        public static string saveFolderPath = null;
        public static string saveImagePath;

        public static string saveImagePathTrainingTop;
        public static string saveImagePathTraining1;
        public static string saveImagePathTraining2;
        public static string saveImagePathTraining3;

        public static bool checkSet = false;

        public static bool m_bCam1Insp = false;
        public static bool m_bCam2Insp = false;
        public static bool m_bCam3Insp = false;
        public static bool m_bCam4Insp = false;
        public static Bitmap img1= null;
        public static Bitmap img2 = null;
        public static Bitmap img3 = null;
        public static Bitmap img4 = null;

        public static Bitmap clImage1 = null;
        public static Bitmap clImage2 = null;
        public static Bitmap clImage3 = null;
        public static Bitmap clImage4 = null;

        public static Bitmap imgResult1 = null;
        public static Bitmap imgResult2 = null;
        public static Bitmap imgResult3 = null;
        public static Bitmap imgResult4 = null;

        public static CntNGClass useNGClass = null;
        public static CntTime useCntTimeClass = null;
        public static ShowSignal useShowSigClass = null;

        public static int checkCameraSignal = 0;
        public static int checkEndCameraSignal = 0;

        public static int checkCnt = 0;

        public static bool bInspectBtn = true;

        public static BlockingCollection<bool> qSaveBlowSignal;
        public static bool bContinueBlowSignal = false;

        public static Stopwatch watch = new Stopwatch();


        public static string deviceDescription = "PCI-1730,BID#0";
        public static string profilePath = "D:\\profile_i\\dev2.xml";

        public static int startPort = 0;
        public static int portCount = 1;

        public static bool bSaveImage = false;

        public static ErrorCode errorCode = ErrorCode.Success;

        public static InstantDoCtrl instantDoCtrl;

        public static byte[] bufferForWriting;

        int sum = 0;

        public static BlowDelegateClass blowDelegateClass;


        public static bool bSavingStop = false;



        //public static void Saving()
        //{
        //    if (Program.saveFolderPath == null)
        //    {
        //        Program.saveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INTELLIZ Corp\\Frism Inspection";
        //        System.Windows.Forms.MessageBox.Show("Temporary Path has been Assigned. Please Select a Folder First. Path: " + Program.saveFolderPath);


        //        if (!Directory.Exists(Program.saveFolderPath))
        //        {
        //            Directory.CreateDirectory(Program.saveFolderPath);
        //        }
        //    }

        //    while (checkSet)
        //    {

        //        if (CheckImage())
        //        {
        //                Bitmap saveImg1 = img1;
        //                Bitmap saveImg2 = img2;
        //                Bitmap saveImg3 = img3;
        //                Bitmap saveImg4 = img4;

        //                Bitmap saveResult1 = imgResult1;
        //                Bitmap saveResult2 = imgResult2;
        //                Bitmap saveResult3 = imgResult3;
        //                Bitmap saveResult4 = imgResult4;
        //            try
        //            {

        //                if (desiredName == null)
        //                {
        //                    desiredName = String.Format(DateTime.Now.ToString("HHmmssfff"));
        //                }

        //                string pathName = String.Format(DateTime.Now.ToString("HHmmssfff"));
        //                string path = System.IO.Path.Combine(saveFolderPath, pathName);

        //                Directory.CreateDirectory(path);
        //                saveImagePath = path;

        //                Task.Factory.StartNew((Action)(() =>
        //                {
        //                    saveImg1.Save(saveImagePath + "\\" + desiredName + "_[Top].bmp", ImageFormat.Bmp);
        //                    saveImg1 = null;
        //                    saveResult1.Save(saveImagePath + "\\" + desiredName + "_[Top]_Output.bmp", ImageFormat.Bmp);
        //                    saveResult1 = null;
        //                    Console.WriteLine(1);
        //                }
        //                ));
        //                img1 = null;

        //                Task.Factory.StartNew((Action)(() =>
        //                {
        //                    saveImg2.Save(saveImagePath + "\\" + desiredName + "_[1].bmp", ImageFormat.Bmp);
        //                    saveImg2 = null;
        //                    saveResult2.Save(saveImagePath + "\\" + desiredName + "_[1]_Output.bmp", ImageFormat.Bmp);
        //                    saveResult2 = null;
        //                    Console.WriteLine(2);
        //                }
        //                ));
        //                img2 = null;

        //                Task.Factory.StartNew((Action)(() =>
        //                {
        //                    saveImg3.Save(saveImagePath + "\\" + desiredName + "_[2].bmp", ImageFormat.Bmp);
        //                    saveImg3 = null;
        //                    saveResult3.Save(saveImagePath + "\\" + desiredName + "_[2]_Output.bmp", ImageFormat.Bmp);
        //                    saveResult3 = null;
        //                    Console.WriteLine(3);
        //                }
        //                ));
        //                img3 = null;

        //                Task.Factory.StartNew((Action)(() =>
        //                {
        //                    saveImg4.Save(saveImagePath + "\\" + desiredName + "_[3].bmp", ImageFormat.Bmp);
        //                    saveImg4 = null;
        //                    saveResult4.Save(saveImagePath + "\\" + desiredName + "_[3]_Output.bmp", ImageFormat.Bmp);
        //                    saveResult4 = null;
        //                    Console.WriteLine(4);
        //                }
        //                ));
        //                img4 = null;
        //                desiredName = null;
        //            }

        //            catch (Exception ex)
        //            {
        //                Logger.Error(ex.Message + " Saving");
        //            }
        //        }
        //    }
        //}

        public static void SendShowCameraSignal()
        {
            //lock (lockShowCameraSignalObject)
            //{
                checkCameraSignal++;
            //}
            if(checkCameraSignal > 3)
            {
                checkCameraSignal = 0;
                m_bCam1Insp = true;
                Task.Delay(100).ContinueWith(_ =>
                {
                    useShowSigClass.ShowCameraSignal();
                });
                
            }
        }

        public static void SendEndShowCameraSignal()
        {

            lock (lockShowEndCameraSignalObject)
            {
                checkEndCameraSignal++;
            }
            if (checkEndCameraSignal > 3)
            {
                checkEndCameraSignal = 0;
                m_bCam1Insp = false;
                Task.Delay(100).ContinueWith(_ =>
                {
                    useShowSigClass.ShowEndCameraSignal();
                });
            }
        }

        public static bool CheckImage()
        {
            if (img1 != null && img2 != null && img3 != null && img4 != null )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public static void SavingForTrain()
        //{
        //    while (checkSet)
        //    {
        //        if ((img1 != null && img2 != null) && (img3 != null && img4 != null))
        //        {
        //            try
        //            {
        //                if (desiredName == null)
        //                {
        //                    desiredName = String.Format(DateTime.Now.ToString("HHmmssfff"));
        //                }
        //                img1.Save(saveImagePathTrainingTop + "\\" + desiredName + "_[Top].bmp", ImageFormat.Bmp);
        //                img2.Save(saveImagePathTraining1 + "\\" + desiredName + "_[1].bmp", ImageFormat.Bmp);
        //                img3.Save(saveImagePathTraining2 + "\\" + desiredName + "_[2].bmp", ImageFormat.Bmp);
        //                img4.Save(saveImagePathTraining3 + "\\" + desiredName + "_[3].bmp", ImageFormat.Bmp);
        //                desiredName = null;
        //                img1 = null;
        //                img2 = null;
        //                img3 = null;
        //                img4 = null;
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Error(ex.Message + " SavingForTrain");
        //            }
        //        }
        //    }
        //}

        public static void SaveImageStart()
        {

            makeFolderQueue.Enqueue(true);
            //bSaveImage = true;
            //lock (lockThreadCntObject)
            //{
            //checkThreadCnt++;

            //}
            // if (checkThreadCnt > 3)
            //{
            //checkThreadCnt = 0;
            //bSaveImage = true;
            //}
        }




        public static void SaveAllImagesThread()
        {

            while (true)
            {
                
                if (bSavingStop)
                {
                    break;
                }


                if (makeFolderQueue.Count()>0 && makeFolderQueue.Dequeue())
                {
                    //bSaveImage = false;
                    //if (!checkFolderPath)
                    //{
                    //    checkFolderPath = true;
                        if (Program.saveFolderPath == null)
                        {
                            Program.saveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INTELLIZ Corp\\Frism Inspection";
                            //System.Windows.Forms.MessageBox.Show("Temporary Path has been Assigned. Please Select a Folder First. Path: " + Program.saveFolderPath);

                            if (!Directory.Exists(Program.saveFolderPath))
                            {
                                Directory.CreateDirectory(Program.saveFolderPath);
                            }
                        }

                        string pathName = String.Format(DateTime.Now.ToString("HHmmssfff"));
                        string path = System.IO.Path.Combine(saveFolderPath, pathName);
                        Directory.CreateDirectory(path);
                        saveImagePath = path;
                        imagePathTop.Enqueue(saveImagePath);
                        imagePath1.Enqueue(saveImagePath);
                        imagePath2.Enqueue(saveImagePath);
                        imagePath3.Enqueue(saveImagePath);
                }
               
                
                Thread.Sleep(1);



                //}
            }


        }


        public static void SaveImageThreadTop()
        {
            while (true)
            {
                if (bSavingStop)
                {
                    break;
                }

                if (imagePathTop.Count() > 0 && imageSaveQueueTop.Count() > 0)
                {
                    string filePath = imagePathTop.Dequeue();
                    string fileName = filePath.Substring(filePath.Length - 9, 9);
                    imageSaveQueueTop.Dequeue().Save(filePath + "\\" + fileName + "_[Top].bmp", ImageFormat.Bmp);
                    imageResultSaveQueueTop.Dequeue().Save(filePath + "\\" + fileName + "_[Top]_Result.bmp", ImageFormat.Bmp);
                }
                Thread.Sleep(5);
            }
        }

        public static void SaveImageThread1()
        {
            while (true)
            {
                if (bSavingStop)
                {
                    break;
                }

                if (imagePath1.Count() > 0 && imageSaveQueue1.Count() > 0)
                {
                    string filePath = imagePath1.Dequeue();
                    string fileName = filePath.Substring(filePath.Length - 9, 9);
                    imageSaveQueue1.Dequeue().Save(filePath + "\\" + fileName + "_[1].bmp", ImageFormat.Bmp);
                    imageResultSaveQueue1.Dequeue().Save(filePath + "\\" + fileName + "_[1]_Result.bmp", ImageFormat.Bmp);
                }
                Thread.Sleep(5);

            }
        }


        public static void SaveImageThread2()
        {
            while (true)
            {
                if (bSavingStop)
                {
                    break;
                }

                if (imagePath2.Count() > 0 && imageSaveQueue2.Count() > 0)
                {
                    string filePath = imagePath2.Dequeue();
                    string fileName = filePath.Substring(filePath.Length - 9, 9);
                    imageSaveQueue2.Dequeue().Save(filePath + "\\" + fileName + "_[2].bmp", ImageFormat.Bmp);
                    imageResultSaveQueue2.Dequeue().Save(filePath + "\\" + fileName + "_[2]_Result.bmp", ImageFormat.Bmp);
                }
                Thread.Sleep(5);

            }
        }


        public static void SaveImageThread3()
        {
            while (true)
            {
                if (bSavingStop)
                {
                    Console.WriteLine("StopSaveing3");
                    break;
                }

                if (imagePath3.Count() > 0 && imageSaveQueue3.Count() > 0)
                {

                   
                    string filePath = imagePath3.Dequeue();
                    Console.WriteLine(filePath);
                    string fileName = filePath.Substring(filePath.Length - 9, 9);
                    imageSaveQueue3.Dequeue().Save(filePath + "\\" + fileName + "_[3].bmp", ImageFormat.Bmp);
                    imageResultSaveQueue3.Dequeue().Save(filePath + "\\" + fileName + "_[3]_Result.bmp", ImageFormat.Bmp);
                }
                Thread.Sleep(5);

            }
        }






        public static void SaveImg1(Bitmap img, Bitmap resultImg)
        {

            imageSaveQueueTop.Enqueue(img);
            imageResultSaveQueueTop.Enqueue(resultImg);

            //Task.Factory.StartNew((Action)(() =>
            //{
            //SaveAllImages();

            //if (imagePathTop.Count() > 0)
            //{
            //    string filePath = imagePathTop.Dequeue();
            //    string fileName = filePath.Substring(filePath.Length - 10, 9);
            //    img.Save(saveImagePath + "\\" + fileName + "_[Top].bmp", ImageFormat.Bmp);
            //    resultImg.Save(saveImagePath + "\\" + fileName + "_[Top]_Result.bmp", ImageFormat.Bmp);
            //}
            //lock (lockThreadCntObject)
            //{
            //    checkThreadCnt++;
            //}
            //if (checkThreadCnt > 3)
            //{
            //    checkThreadCnt = 0;
            //    checkFolderPath = false;
            //}
            //}));
        }

        public static void SaveImg2( Bitmap img, Bitmap resultImg)
        {
            imageSaveQueue1.Enqueue(img);
            imageResultSaveQueue1.Enqueue(resultImg);
            //Task.Factory.StartNew((Action)(() =>
            //{
            //    SaveAllImages();
            //if (imagePath1.Count() > 0)
            //{
            //    string filePath = imagePath1.Dequeue();
            //    string fileName = filePath.Substring(filePath.Length - 10, 9);
            //    img.Save(saveImagePath + "\\" + fileName + "_[1].bmp", ImageFormat.Bmp);
            //    resultImg.Save(saveImagePath + "\\" + fileName + "_[1]_Result.bmp", ImageFormat.Bmp);
            //}
            //lock (lockThreadCntObject)
            //{
            //    checkThreadCnt++;
            //}
            //if (checkThreadCnt > 3)
            //{
            //    checkThreadCnt = 0;
            //    checkFolderPath = false;
            //}
            //}));
        }


        public static void SaveImg3( Bitmap img, Bitmap resultImg)
        {
            imageSaveQueue2.Enqueue(img);
            imageResultSaveQueue2.Enqueue(resultImg);
            //Task.Factory.StartNew((Action)(() =>
            //{
            //    SaveAllImages();

            //if (imagePath2.Count() > 0)
            //{

            //    string filePath = imagePath2.Dequeue();
            //    string fileName = filePath.Substring(filePath.Length - 10, 9);
            //    img.Save(saveImagePath + "\\" + fileName + "_[2].bmp", ImageFormat.Bmp);
            //    resultImg.Save(saveImagePath + "\\" + fileName + "_[2]_Result.bmp", ImageFormat.Bmp);
            //    //Console.WriteLine(3);

            //}
            //lock (lockThreadCntObject)
            //{
            //    checkThreadCnt++;
            //}

            //if (checkThreadCnt > 3)
            //{
            //    checkThreadCnt = 0;
            //    checkFolderPath = false;
            //}

            //}));
        }
        public static void SaveImg4(Bitmap img, Bitmap resultImg)
        {
            imageSaveQueue3.Enqueue(img);
            imageResultSaveQueue3.Enqueue(resultImg);
            //Task.Factory.StartNew((Action)(() =>
            //{
            //    SaveAllImages();
            //if (imagePath3.Count() > 0)
            //{
            //    string filePath = imagePath3.Dequeue();
            //    string fileName = filePath.Substring(filePath.Length - 10, 9);
            //    img.Save(saveImagePath + "\\" + fileName + "_[3].bmp", ImageFormat.Bmp);
            //    resultImg.Save(saveImagePath + "\\" + fileName + "_[3]_Result.bmp", ImageFormat.Bmp);
            //}
            //lock (lockThreadCntObject)
            //{
            //    checkThreadCnt++;
            //}
            //if (checkThreadCnt > 3)
            //{
            //    checkThreadCnt = 0;
            //    checkFolderPath = false;
            //}
            //}));
        }

        public static void ChangeNGRate()
        {
            if (NGCallCnt >= 4)
            {
                if (NGTop != true && NG1 != true && NG2 != true && NG3 != true)
                {
                    SaveIOSig2Queue(true);
                    Logger.Info("OK");
                    useNGClass.IncreaseOK();
                    
                }
                else
                {
                    SaveIOSig2Queue(false);
                    Logger.Info("NG");
                    useNGClass.IncreaseNG();
                    
                }
                NGCallCnt = 0;
            }
        }

        public static void ChangeTime()
        {
            if (TimeCallCnt >= 4)
            {
                Task.Factory.StartNew((Action)(() =>
                {
                try
                {
                    if (maxInspTime < elpsTimeTop) maxInspTime = elpsTimeTop; 
                    if (maxInspTime < elpsTime1) maxInspTime = elpsTime1;
                    if (maxInspTime < elpsTime2) maxInspTime = elpsTime2;
                    if (maxInspTime < elpsTime3) maxInspTime =elpsTime3;

                    if (maxProcessTime < processTimeTop) maxProcessTime = processTimeTop;
                    if (maxProcessTime < processTime1) maxProcessTime = processTime1;
                    if (maxProcessTime < processTime2) maxProcessTime = processTime2;
                    if (maxProcessTime < processTime3) maxProcessTime = processTime3;

                    useCntTimeClass.IncreaseInspTime(maxInspTime);
                    useCntTimeClass.IncreaseProcessTime(maxProcessTime);
                    maxProcessTime = 0;
                    maxInspTime = 0;
                    TimeCallCnt = 0;
                    } catch (Exception ex)
                {
                    Logger.Error(ex.Message + " ChangeTime");
                }
                }));
            }
        }

        public static void SetClass(CntNGClass classInfo)
        {
            useNGClass = classInfo;
        }

        public static void GetNGTop(bool NGResult)
        {
            try
            {
                NGTop = NGResult;
                NGCallCnt++;
                ChangeNGRate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GetNGTop");
            }
        }

        public static void GetNG1(bool NGResult)
        {
            try
            {
                NG1 = NGResult;
                NGCallCnt++; 
                ChangeNGRate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GetNG1");
            }
        }

        public static void GetNG2(bool NGResult)
        {
            try
            {
                NG2 = NGResult;
                NGCallCnt++;
                ChangeNGRate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GetNG2");
            }
        }

        public static void GetNG3(bool NGResult)
        {
            try
            {
                NG3 = NGResult;
                NGCallCnt++;
                ChangeNGRate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GetNG3");
            }
        }

        public static void GetTime(long elpsTime, long processTime)
        {
            lock (lockGetTimeObject)
            {
                processTimeTop = processTime;
                elpsTimeTop = elpsTime;
                TimeCallCnt++;
                ChangeTime();
            }
        }


        public static void SaveIOSig2Queue(bool signal)
        {
            lock (qSaveBlowSignal)
            {
                qSaveBlowSignal.Add(signal);
                Console.WriteLine("ADD signal to Queue --------");
            }
        }

        public static void StartIOSig()
        {
            if (bContinueBlowSignal) return;
            bContinueBlowSignal = true;
            qSaveBlowSignal = new BlockingCollection<bool>();
            Thread tSignal = new Thread(new ThreadStart(IOBlowSig));
            tSignal.Start();
        }

        public static void IOBlowSig()
        {
            string deviceDescription = "PCI-1730,BID#0";
            string profilePath = "D:\\profile_i\\dev2.xml";
            int startPort = 0;
            int portCount = 1;
            ErrorCode errorCode = ErrorCode.Success;

            InstantDiCtrl instantDiCtrl = new InstantDiCtrl();
            try
            {
                instantDiCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
                errorCode = instantDiCtrl.LoadProfile(profilePath);
                if (BioFailed(errorCode))
                {
                    throw new Exception();
                }

                //Console.WriteLine("Reading ports' status is in progress..., any key to quit!\n");

                byte[] buffer = new byte[64];
                bool bSensorFlag = false;
                
                while (bContinueBlowSignal)
                {
                    //Console.WriteLine("ReadBuffer");
                    errorCode = instantDiCtrl.Read(startPort, portCount, buffer);
                    if (BioFailed(errorCode))
                    {
                        throw new Exception();
                    }
                    int intbyte = buffer[0];
                    
                    if(buffer[0] > 0)
                    {
                        
                        while (true)
                        {
                            //Console.WriteLine("Buffer Value : " + buffer[0]);
                            errorCode = instantDiCtrl.Read(startPort, portCount, buffer);
                            if (buffer[0] == 0)
                            {
                               
                                
                                blowDelegateClass.StartBlowing();
                                break;
                            }
                        }
                        
                    }


                    

                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                string errStr = BioFailed(errorCode) ? " Some error occurred. And the last error code is " + errorCode.ToString()
                                                          : e.Message;
                Console.WriteLine(errStr);
                Logger.Error(e.Message + " _IOBlow");
            }
            finally
            {
                instantDiCtrl.Dispose();
                Console.ReadKey(false);
                
            }
        }

        public static bool BioFailed(ErrorCode err)
        {
            return err < ErrorCode.Success && err >= ErrorCode.ErrorHandleNotValid;
        }

        public static void ErrorBlow()
        {
            lock (lockBlowObject)
            {
                for (int i = 0; i < 5; i++)
                {
                    blow(10);
                }
            }
        }


        public static void setBlowReady()
        {
            bool temp;
            int nSleep = 100;
            //Console.WriteLine("setBlowReady : " + qSaveBlowSignal.Count);

            if (qSaveBlowSignal.Count > 0)
            {
                temp = qSaveBlowSignal.Take();
                
                if (!temp)
                {
                    Console.WriteLine("NG Blow\n");
                    blow(nSleep);

                }
                else
                {
                    Console.WriteLine("OK NON Blow\n");
                    //Thread.Sleep(nSleep);
                }

            }



        }


        public static void setBlowDevice()
        {
            try
            {
                errorCode = ErrorCode.Success;

                instantDoCtrl = new InstantDoCtrl();
                instantDoCtrl.SelectedDevice = new DeviceInformation(deviceDescription);
                errorCode = instantDoCtrl.LoadProfile(profilePath);
                if (BioFailed(errorCode))
                {
                    throw new Exception();
                }

                blowDelegateClass = new BlowDelegateClass();


                blowDelegateClass.blowEvent += setBlowReady;



            }
            catch(Exception exception)
            {
                Logger.Error(exception.Message + " _Blow");
            }

        }

        public static void blow(int nDelay)
        {


            Console.WriteLine("BLOWWWWWWWWWWWW");
            bufferForWriting = new byte[64];
            try
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < portCount; ++i)
                    {
                        string data;
                        if (j == 0)
                            data = "0x01";
                        else
                            data = "0x00";
                        bufferForWriting[i] = byte.Parse(data.Contains("0x") ? data.Remove(0, 2) : data, System.Globalization.NumberStyles.HexNumber);
                    }
                    errorCode = instantDoCtrl.Write(startPort, portCount, bufferForWriting);
                    if (BioFailed(errorCode))
                    {
                        throw new Exception();
                    }
                    if (j == 0)
                    {
                        //Console.WriteLine("sleep!");
                        Thread.Sleep(nDelay);
                    }
                }
            }
            catch (Exception e)
            {
                string errStr = BioFailed(errorCode) ? " Some error occurred. And the last error code is " + errorCode.ToString()
                                                           : e.Message;
                Console.WriteLine(errStr);
                Logger.Error(e.Message + " _Blow");
            }
        }




    }
}
