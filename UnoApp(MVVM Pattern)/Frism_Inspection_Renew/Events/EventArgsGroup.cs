using Frism_Inspection_Renew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Events
{
    public class ImageCapturedEventArgs : EventArgs
    {
        public int cameraNumber;
        public ImageFromCameraModel imageFromCameraModel;


    }
}
