using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{

    public class Sub_MainModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //public IVisionCamera camera = new FileCamera(@"D:\TB\Image\Top");
        private IVisionCamera camera = new UnoCamera();
        public static Stopwatch _timeS = new Stopwatch();
        private int _imaxThreadCnt;
        public int iMaxThreadCnt
        {
            get { return _imaxThreadCnt; }
            set { _imaxThreadCnt = value; }
        }


        private int _imaxTileWidth;
        public int  iMaxTileWidth
        {
            get { return _imaxTileWidth; }
            set { _imaxTileWidth = value; }
        }

        private int _imaxTileHeight;
        public int iMaxTileHeight
        {
            get { return _imaxTileHeight; }
            set { _imaxTileHeight = value; }
        }

        private int _iCpuNo;
        public int iCpuNo
        {
            get { return _iCpuNo; }
            set { _iCpuNo = value; }
        }

        private int _iminDefectNumTop;
        public int iMinDefectNumTop
        {
            get { return _iminDefectNumTop; }
            set { _iminDefectNumTop = value; }
        }

        private float _fminPValTop;
        public float fMinPValTop
        {
            get { return _fminPValTop; }
            set { _fminPValTop = value; }
        }

        private int _iminDefectNumSide;
        public int iMinDefectNumSide
        {
            get { return _iminDefectNumSide; }
            set { _iminDefectNumSide = value; }
        }

        private float _fminPValSide;
        public float fMinPValSide
        {
            get { return _fminPValSide; }
            set { _fminPValSide = value; }
        }




        public Sub_MainModel() {
            InitVariables();
        }


        public void InitVariables()
        {
        

        }

    }
}
