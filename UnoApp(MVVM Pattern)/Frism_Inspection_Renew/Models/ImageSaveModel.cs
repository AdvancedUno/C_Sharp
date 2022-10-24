using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ImageSaveModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private bool _savingStop;
        public bool SavingStop { get => _savingStop; set => _savingStop = value; }

        private bool _continueSaveImage = true;
        public bool ContinueSaveImage { get => _continueSaveImage; set => _continueSaveImage = value; }

        string saveFolderPath = null;

        public void SaveImageThread()
        {
            try
            {
                saveFolderPath = (DBAcess.GiveFilePath("0"));
                while (ContinueSaveImage)
                {

                    SaveImage();

                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " _IOBlow");
                Console.WriteLine(e.Message + " SaveImageThread");
            }

        }
        //                Task.Factory.StartNew((Action)(() =>
        //                {
        //                    saveImg1.Save(saveImagePath + "\\" + desiredName + "_[Top].bmp", ImageFormat.Bmp);
        //                    saveImg1 = null;
        //                    saveResult1.Save(saveImagePath + "\\" + desiredName + "_[Top]_Output.bmp", ImageFormat.Bmp);
        //                    saveResult1 = null;
        //                    Console.WriteLine(1);
        //                }
        //                ));

        public void SaveImage()
        {
            try
            {
                if (IOSaveModel.CheckImageBuffer())
                {
                    Task.Factory.StartNew((Action)(() =>
                    {
                        try
                        {


                            ImageGroupModel temp;

                            temp = IOSaveModel.GetImageFromBuffer();


                            //string folderPath = temp.SaveFolderPath;
                            string pathName = String.Format(DateTime.Now.ToString("HHmmssfff"));

                            if(saveFolderPath == null)
                            {
                                saveFolderPath = temp.SaveFolderPath;

                            }

                            string folderPath = System.IO.Path.Combine(saveFolderPath, pathName);

                            

                            for (int i = 0; i < temp.GetImageInfoModels().Count(); i++)
                            {
                                ImageInfoModel tempImageInfoModel = temp.GetImageInfoModels()[i];
                                string fileName = tempImageInfoModel.CameraPosition + "_" + tempImageInfoModel.CameraId;
                                Bitmap ImageOriginal = tempImageInfoModel.GetBitmapRawImage();
                                Bitmap ImageResult = tempImageInfoModel.GetBitmapResultImageModel().GetBitmapResultImage();

                                if (!Directory.Exists(folderPath))
                                {
                                    Directory.CreateDirectory(folderPath);
                                }

                                ImageOriginal.Save(folderPath + "\\" + fileName + ".bmp", ImageFormat.Bmp);
                                if (ImageResult != null)
                                {
                                    ImageResult.Save(folderPath + "\\" + fileName + "_Result.bmp", ImageFormat.Bmp);

                                }
                                else
                                {
                                    Console.WriteLine("------------- No Images ------------");
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message + " SaveImage");
                        }
                    }
                    ));

                    //Thread.Sleep(5);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " SaveImage");
            }
            
            
        }

    }
}
