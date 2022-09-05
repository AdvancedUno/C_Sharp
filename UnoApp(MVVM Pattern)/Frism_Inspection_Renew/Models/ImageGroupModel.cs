using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ImageGroupModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private List<ImageInfoModel> _imageInfoModelList;
        public List<ImageInfoModel> ImageInfoModelList { get => _imageInfoModelList; set => _imageInfoModelList = value; }
        
        private int _groupId;
        public int GroupId { get => _groupId; set => _groupId = value; }

        public Dictionary<int, bool> CheckImageInfoDict = new Dictionary<int, bool>();


        public ImageGroupModel(int cameraNum)
        {
            ImageInfoModelList = new List<ImageInfoModel>();


            try
            {
                for (int i = 0; i < cameraNum; i++)
                {
                    CheckImageInfoDict.Add(i, false);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }
        }



        public int AddImageInfoModel(ImageInfoModel imageInfoModel)
        {
            try
            {
                //if (imageInfoModel.BitmapRawImage == null) return 1;
                if (CheckImageInfoDict[imageInfoModel.CameraId] != false) return 1;
                CheckImageInfoDict[imageInfoModel.CameraId] = true;
                ImageInfoModelList.Add(imageInfoModel);
            }
            catch(Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }
            return 0;
        }

        public int RemoveAllImageInfoModel(int cameraNum)
        {
            try
            {
                for(int i = 0; i < cameraNum; i++)
                {
                    CheckImageInfoDict[i] = false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }

            return 0;
        }

        public List<ImageInfoModel> GetImageInfoModels()
        {
            return ImageInfoModelList;
        }



    }
}
