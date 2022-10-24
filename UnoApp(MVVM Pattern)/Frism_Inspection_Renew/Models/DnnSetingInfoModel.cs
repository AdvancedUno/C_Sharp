using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class DnnSetingInfoModel
    {
        private int _threadId;
        public int ThreadId { get => _threadId; set => _threadId = value; }

        private int _maxThreadCount;
        public int MaxThreadCount { get => _maxThreadCount; set => _maxThreadCount = value; }
        
        private int _maxTileWidth;
        public int MaxTileWidth { get => _maxTileWidth; set => _maxTileWidth = value; }
        
        private int _maxTileHeight;
        public int MaxTileHeight { get => _maxTileHeight; set => _maxTileHeight = value; }

        private int _gpuNum;
        public int GpuNum { get => _gpuNum; set => _gpuNum = value; }

        private int _minDefectSize;
        public int MinDefectSize { get => _minDefectSize; set => _minDefectSize  = value; }
        
        private float _uppperPValue;
        public float UppperPValue { get => _uppperPValue; set => _uppperPValue = value; }
        
        private string _dnnPath = "0";
        public string DnnPath { get => _dnnPath; set => _dnnPath = value; }

        public DnnSetingInfoModel()
        {
            ThreadId = -1;
            MaxThreadCount = -1;
            MaxTileWidth = -1;
            MaxTileHeight = -1;
            GpuNum = -1;
            MinDefectSize = 1;
            UppperPValue = -1;
            DnnPath = null;
        }
    }
}
