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
        private bool _savingStop;
        public bool SavingStop { get => _savingStop; set => _savingStop = value; }



        public void SaveImageThread()
        {
            while (true)
            {
                if (SavingStop)
                {
                    break;
                }

                if (imagePath1.Count() > 0 && imageSaveQueue1.Count() > 0)
                {
                    string filePath = imagePath1.Dequeue();
                    string fileName = filePath.Substring(filePath.Length - 9, 9);
                    Bitmap ImageOriginal = imageSaveQueue1.Dequeue();
                    Bitmap ImageResult = imageResultSaveQueue1.Dequeue();
                    if (bSaveNGImgOnly && !bCheckNGToSaveImage)
                    {
                        continue;
                    }
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    ImageOriginal.Save(filePath + "\\" + fileName + "_[1].bmp", ImageFormat.Bmp);
                    ImageResult.Save(filePath + "\\" + fileName + "_[1]_Result.bmp", ImageFormat.Bmp);
                }
                Thread.Sleep(5);

            }
        }

    }
}
