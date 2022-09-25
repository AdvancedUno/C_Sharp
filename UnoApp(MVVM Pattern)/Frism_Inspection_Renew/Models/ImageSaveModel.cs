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

        public void SaveImageThread()
        {
            try
            {
                while (ContinueSaveImage)
                {

                    SaveImage();

                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " _IOBlow");
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
            if (IOSaveModel.CheckImageBuffer())
            {
                Task.Factory.StartNew((Action)(() => {

                ImageGroupModel temp;
                temp = IOSaveModel.GetImageFromBuffer();

                string folderPath = temp.SaveFolderPath;

                for(int i = 0; i < temp.GetImageInfoModels().Count(); i++)
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
                    ImageResult.Save(folderPath + "\\" + fileName + "_Result.bmp", ImageFormat.Bmp);
                }
                }
                ));
                //Thread.Sleep(5);

            }
        }

    }
}
