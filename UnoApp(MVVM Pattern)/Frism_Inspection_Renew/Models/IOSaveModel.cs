using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public static class IOSaveModel
    {
        private static BlockingCollection<bool> _saveBlowSignal = new BlockingCollection<bool>();
        public static BlockingCollection<bool> SaveBlowSignal { get => _saveBlowSignal; set => _saveBlowSignal = value; }

        private static BlockingCollection<ImageGroupModel> _saveImageBuffer = new BlockingCollection<ImageGroupModel>();
        public static BlockingCollection<ImageGroupModel> SaveImageBuffer { get => _saveImageBuffer; set => _saveImageBuffer = value; }

        public static bool CheckBlowSignalBuffer()
        {
            if (SaveBlowSignal.Count() < 1) return false;
            else return true;
        }

        public static bool GetBlowSignalValue()
        {
            return SaveBlowSignal.Take();
        }

        public static void AddSignalValue(bool signal)
        {
            SaveBlowSignal.Add(signal);
        }

        public static bool CheckImageBuffer()
        {
            if (SaveImageBuffer.Count() < 1) return false;
            else return true;
        }

        public static ImageGroupModel GetImageFromBuffer()
        {
            return SaveImageBuffer.Take();
        }

        public static void AddImageToBuffer(ImageGroupModel imageGroupModel)
        {
            SaveImageBuffer.Add(imageGroupModel);
        }





    }
}
