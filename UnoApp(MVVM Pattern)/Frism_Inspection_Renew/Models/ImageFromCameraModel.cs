using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ImageFromCameraModel
    {
        private Bitmap _capturedBitmapImage = null;
        public Bitmap CapturedBitmapImage { get => _capturedBitmapImage; set => _capturedBitmapImage = value; }
        
        private int _cameraId;
        public int CameraId { get => _cameraId; set => _cameraId = value; }

        private int _imageCapturedTime;
        public int ImageCapturedTime { get => _imageCapturedTime; set => _imageCapturedTime = value; }

        public ImageFromCameraModel(int cameraId)
        {
            CameraId = cameraId;
        }
    }
}
