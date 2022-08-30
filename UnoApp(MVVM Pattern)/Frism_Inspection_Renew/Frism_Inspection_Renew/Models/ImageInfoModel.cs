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
       
        private Bitmap _bitmapRawImage;
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



    }
}
