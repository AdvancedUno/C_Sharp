using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Frism_Inspection_Renew.Models
{
    public class ScreenModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //private UnoCamera camera;
        private IVisionCamera _camera = null;
        public IVisionCamera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        private Bitmap _bitImage = null;
        public Bitmap BitImage
        {
            get { return _bitImage; }
            set { _bitImage = value; }
        }

        private BitmapImage _bitmapImg = null;
        public BitmapImage BitmapImage
        {
            get { return _bitmapImg; }
            set { _bitmapImg = value; }
        }

        private Stopwatch _timeInsp;
        public Stopwatch TimeInsp
        {
            get { return _timeInsp; }
            set { _timeInsp = value; }
        }


        private Stopwatch _timeProcess;
        public Stopwatch TimeProcess
        {
            get { return _timeProcess; }
            set { _timeProcess = value; }
        }

        private int _iPosX = 0;
        public int iPosX
        {
            get { return _iPosX; }
            set { _iPosX = value; }
        }


        private int _iPosY = 0;
        public int iPosY
        {
            get { return _iPosY; }
            set { _iPosY = value; }
        }


        private int _iWidth = 0;
        public int iWidth
        {
            get { return _iWidth; }
            set { _iWidth = value; }
        }


        private int _iHeight = 0;
        public int iHeight
        {
            get { return _iHeight; }
            set { _iHeight = value; }
        }

        private double _dPosX = 0;
        public double dPosXDecimal
        {
            get { return _dPosX; }
            set { _dPosX = value; }
        }


        private double _dPosY = 0;
        public double dPosYDecimal
        {
            get { return _dPosY; }
            set { _dPosY = value; }
        }

        private double _dWidth = 0;
        public double dWidthDecimal
        {
            get { return _dWidth; }
            set { _dWidth = value; }
        }


        private double dHeight = 0;

        private bool bCheckContinueInsp = true;

        public string sDnnPath = "0";

        public int iThreadID;


        private int minDefectSize;
        private float uppperPValue;


        //public ngClass ngResult;

        private bool startedPaint;

        private System.Windows.Point downPoint;
        private System.Windows.Point upPoint;


        int cameraId;
        string savePath;
        bool saveImage;

        private bool m_bStopped = false;
        private bool m_bInferFlag = false;
        public bool m_bInferReadyFlag = false;



        public ScreenModel()
        {
            InitVariables();
        }


        public void InitVariables()
        {


        }

    }
}
