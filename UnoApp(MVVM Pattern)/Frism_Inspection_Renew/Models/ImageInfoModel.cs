using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ImageInfoModel
    {
        private int _cameraId;
        public int CameraId { get => _cameraId; set => _cameraId = value; }
       
        private Bitmap _bitmapRawImage = null;
        public Bitmap BitmapRawImage { get => _bitmapRawImage; set => _bitmapRawImage = value; }

        private string _cameraPosition;
        public string CameraPosition { get => _cameraPosition; set => _cameraPosition = value; }

        private int _imageWidth;
        public int ImageWidth { get => _imageWidth; set => _imageWidth = value; }
        
        private int _imageHeight;
        public int ImageHeight { get => _imageHeight; set => _imageHeight = value; }
        
        private ResultImageModel _resultImage;
        public ResultImageModel ResultImage { get => _resultImage; set => _resultImage = value; }
        
        private DnnSetingInfoModel _dnnSettingInfoModel;
        public DnnSetingInfoModel DnnSettingInfoModel { get => _dnnSettingInfoModel; set => _dnnSettingInfoModel = value; }

        private int _saveId;
        public int SaveId { get => _saveId; set => _saveId = value; }
        
        private string _savePath;
        public string SavePath { get => _savePath; set => _savePath = value; }

        private bool _resultNG;
        public bool ResultNG { get => _resultNG; set => _resultNG = value; }

        private int _cropPosX;
        public int CropPosX { get => _cropPosX; set => _cropPosX = value; }

        private int _cropPosY;
        public int CropPosY { get => _cropPosY; set => _cropPosY = value; }

        private int _cropWidth;
        public int CropWidth { get => _cropWidth; set => _cropWidth = value; }

        private int _cropHeight;
        public int CropHeight { get => _cropHeight; set => _cropHeight = value; }

        public ImageInfoModel(int cameraId)
        {
            CameraId = cameraId;
            CameraPosition = null;
            ImageWidth = -1;
            ImageHeight = -1;
            ResultImage = new ResultImageModel();
            SaveId = -1;
            SavePath = null;
            //DnnSettingInfoModel = new DnnSetingInfoModel();
        }

        public Bitmap GetBitmapRawImage()
        {
            return BitmapRawImage;
        }
        public ResultImageModel GetBitmapResultImageModel()
        {
            return ResultImage;
        }

        public void SetCropVal(string sCropInfo)
        {
            string[] collectionCropInfo = sCropInfo.Split(',');
            CropPosX = (int)(float.Parse(collectionCropInfo[0]) * 1600);
            CropPosY = (int)(float.Parse(collectionCropInfo[1]) * 1200);
            CropWidth = (int)(float.Parse(collectionCropInfo[2]) * 1600);
            CropHeight = (int)(float.Parse(collectionCropInfo[3]) * 1200);
        }


    }
}
