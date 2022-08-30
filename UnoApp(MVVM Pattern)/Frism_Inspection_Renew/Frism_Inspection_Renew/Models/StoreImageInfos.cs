using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class StoreImageInfos
    {
        private BlockingCollection<ImageInfoModel> _imageInfoQueue;
        public BlockingCollection<ImageInfoModel> ImageInfoQueue { get => _imageInfoQueue; set => _imageInfoQueue = value; }




        public StoreImageInfos()
        {
            ImageInfoQueue = new BlockingCollection<ImageInfoModel>();
        }

        public ImageInfoModel GetImageInfoModel()
        {

            if (ImageInfoQueue.Count() < 1) return null;

            return ImageInfoQueue.Take();
        }







    }
}
