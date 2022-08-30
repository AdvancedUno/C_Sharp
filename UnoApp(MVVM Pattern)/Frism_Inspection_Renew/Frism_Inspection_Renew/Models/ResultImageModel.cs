using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ResultImageModel
    {

        private Bitmap _bitmapResultImage;
        public Bitmap BitmapResultImage { get => _bitmapResultImage; set => _bitmapResultImage = value; }
        
        private float _resultProbability;
        public float ResultProbability { get => _resultProbability; set => _resultProbability = value; }
        
        private float _resultMaxDefectSize;
        public float ResultMaxDefectSize { get => _resultMaxDefectSize; set => _resultMaxDefectSize = value; }








    }
}
