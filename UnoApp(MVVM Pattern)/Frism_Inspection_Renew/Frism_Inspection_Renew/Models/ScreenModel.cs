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

        private int _iPosX;
        public int iPosX
        {
            get { return _iPosX; }
            set { _iPosX = value; }
        }


        private int _iPosY;
        public int iPosY
        {
            get { return _iPosY; }
            set { _iPosY = value; }
        }


        private int _iWidth;
        public int iWidth
        {
            get { return _iWidth; }
            set { _iWidth = value; }
        }


        private int _iHeight;
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

        private double _dHeight = 0;
        public double dHeight
        {
            get { return _dHeight; }
            set { _dHeight = value; }
        }

        private bool _bCheckContinueInsp = true;
        public bool bCheckContinueInsp
        {
            get { return _bCheckContinueInsp; }
            set { _bCheckContinueInsp = value; }
        }

        private string _sDnnPath;
        public string sDnnPath
        {
            get { return _sDnnPath; }
            set { _sDnnPath = value; }
        }

        public int _iThreadID;
        public int iThreadID
        {
            get { return _iThreadID; }
            set { _iThreadID = value; }
        }


        private int _minDefectSize;
        public int minDefectSize
        {
            get { return _minDefectSize; }
            set { _minDefectSize = value; }
        }

        private float _uppperPValue;
        public float uppperPValue
        {
            get { return _uppperPValue; }
            set { _uppperPValue = value; }
        }

        //public ngClass ngResult;

        private bool _startedPaint;
        public bool startedPaint
        {
            get { return _startedPaint; }
            set { _startedPaint  = value; }
        }

        private System.Windows.Point _downPoint;
        public System.Windows.Point downPoint
        {
            get { return _downPoint; }
            set { _downPoint = value; }
        }

        private System.Windows.Point _upPoint;
        public System.Windows.Point upPoint
        {
            get { return _upPoint; }
            set { _upPoint = value; }
        }


        private int _cameraId;
        public int cameraId
        {
            get { return _cameraId; }
            set { _cameraId = value; }
        }

        private string _savePath;
        public string savePath
        {
            get { return _savePath; }
            set { _savePath = value; }
        }

        private bool _checkToSaveImage;
        public bool checkToSaveImage
        {
            get { return _checkToSaveImage; }
            set { _checkToSaveImage = value; }
        }

        private bool _bStopped = false;
        public bool bStopped
        {
            get { return _bStopped; }
            set { _bStopped = value; }
        }

        private bool _bInferFlag = false; 
        public bool bInferFlag
        {
            get { return _bInferFlag; }
            set { _bInferFlag = value; }
        }

        public bool _bInferReadyFlag = false;
        public bool bInferReadyFlag
        {
            get { return _bInferReadyFlag; }
            set { _bInferReadyFlag = value; }
        }



        public ScreenModel()
        {
            InitVariables();
        }


        public void InitVariables()
        {
            sDnnPath = "0";
            iPosX = 0;
            iPosY = 0;
            iPosY = 0;
            iWidth = 0;
            iHeight = 0;
        }

    }


}
