using Basler.Pylon;
using Frism_Inspection_Renew.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{


    public class DnnSetClass
    {
        public delegate void MyEventHandelr(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide);
        public event MyEventHandelr setEvent;


        public void passDnnSet(int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide)
        {
            setEvent?.Invoke(ThreadCnt, Width, Height, GpuNo, MinDefectNumTop, MinPValTop, MinDefectNumSide, MinPValSide);
        }


    };


    public class CntTimeClass
    {

        public delegate void MyEventHandelr(long chechTime);
        public event MyEventHandelr cntInspTimeEvent;
        public event MyEventHandelr cntProcessTimeEvent;

        public void IncreaseInspTime(long inspTime)
        {
            cntInspTimeEvent?.Invoke(inspTime);
        }

        public void IncreaseProcessTime(long ProcessTime)
        {
            cntProcessTimeEvent?.Invoke(ProcessTime);
        }


    }

    public class CntNGClass
    {
        public delegate void MyEventHandelr();
        public event MyEventHandelr cntNGEvent;
        public event MyEventHandelr cntOKEvent;



        public void IncreaseNG()
        {
            cntNGEvent?.Invoke();
        }
        public void IncreaseOK()
        {
            cntOKEvent?.Invoke();
        }



    };


    public class ShowSignal
    {

        public delegate void MyEventHandelr();
        public event MyEventHandelr showCameraSignal;
        public event MyEventHandelr showBlowSignal;
        public event MyEventHandelr showEndCameraSignal;
        public event MyEventHandelr showEndBlowSignal;

        public void ShowCameraSignal()
        {
            showCameraSignal?.Invoke();
        }

        public void ShowBlowSignal()
        {
            showBlowSignal?.Invoke();
        }

        public void ShowEndCameraSignal()
        {
            showEndCameraSignal?.Invoke();
        }
        public void ShowEndBlowSignal()
        {
            showEndBlowSignal?.Invoke();
        }



    }



    public class MainModel
    {

        //private HistoryView _viewHistory;
        //public HistoryView ViewHistory
        //{
        //    get{return _viewHistory;}
        //    set{_viewHistory = value;}
        //}

        //private SetCameraView _selectCameras;
        //public SetCameraView SelectCameras
        //{
        //    get{return _selectCameras;}
        //    set{_selectCameras = value;}
        //}

        ////public LEDWindow ledSource; // = new LEDWindow();
        //private DNNSetting _dnnSetWindow;
        //public DNNSetting DnnSetWindow
        //{
        //    get{return _dnnSetWindow;}
        //    set{_dnnSetWindow = value;}
        //}

        private CntTimeClass _cntTimeDelegate;
        public CntTimeClass CntTimeDelegate
        {
            get{return _cntTimeDelegate;}
            set{_cntTimeDelegate = value;}
        }

        private DnnSetClass _dnnSetDelegate;
        public DnnSetClass DnnSetDelegate
        {
            get{return _dnnSetDelegate;}
            set{_dnnSetDelegate = value;}
        }

        private CntNGClass _cntNGClassDelegate;
        public CntNGClass CntNGDelegate
        {
            get{return _cntNGClassDelegate;}
            set{_cntNGClassDelegate = value;}
        }

        private ShowSignal _showSigClassDelegate;
        public ShowSignal ShowSigClassDelegate
        {
            get{return _showSigClassDelegate;}
            set{_showSigClassDelegate = value;}
        }

        private Sub_MainView _sub_MainView1;
        public Sub_MainView Sub_MainView1
        {
            get{return _sub_MainView1;}
            set{_sub_MainView1 = value;}
        }

        private Sub_MainView _sub_MainView2;
        public Sub_MainView Sub_MainView2
        {
            get{return _sub_MainView2;}
            set{_sub_MainView2 = value;}
        }

        private Sub_MainView _sub_MainView3;
        public Sub_MainView Sub_MainView3
        {
            get{return _sub_MainView3;}
            set{_sub_MainView3 = value;}
        }

        private Sub_MainView _sub_MainView4;
        public Sub_MainView Sub_MainView4
        {
            get{return _sub_MainView4;}
            set{_sub_MainView4 = value;}
        }

        private int _iMaxThreadCnt;
        public int iMaxThreadCnt
        {
            get{return _iMaxThreadCnt;}
            set{_iMaxThreadCnt = value;}
        }

        private int _iMaxTileWidth;
        public int iMaxTileWidth
        {
            get{return _iMaxTileWidth;}
            set{_iMaxTileWidth = value;}
        }

        private int _iMaxTileHeight;
        public int iMaxTileHeight
        {
            get{return _iMaxTileHeight;}
            set{_iMaxTileHeight = value;}
        }

        private int _iGpuNo;
        public int iGpuNo
        {
            get{return _iGpuNo;}
            set{_iGpuNo = value;}
        }

        private int _iMinDefectNumTop;
        public int iMinDefectNumTop
        {
            get{return _iMinDefectNumTop;}
            set{_iMinDefectNumTop = value;}
        }

        private float _fMinPValTop;
        public float fMinPValTop
        {
            get{return _fMinPValTop;}
            set{_fMinPValTop = value;}
        }

        private int _iMinDefectNumSide;
        public int iMinDefectNumSide
        {
            get{return _iMinDefectNumSide;}
            set{_iMinDefectNumSide = value;}
        }

        private float _fMinPValSide;
        public float fMinPValSide
        {
            get{return _fMinPValSide;}
            set{_fMinPValSide = value;}
        }


        private int _iNGCnt = 0;
        public int iNGCnt
        {
            get{return _iNGCnt;}
            set{_iNGCnt = value;}
        }

        private int _iOKCnt = 0;
        public int iOKCnt
        {
            get{return _iOKCnt;}
            set{iOKCnt = value;}
        }

        private int _iNGYield = 0;
        public int iNGYield
        {
            get{return _iNGYield;}
            set{iNGYield = value;}
        }

        private int _iOKYield = 0;
        public int iOKYield
        {
            get{return _iOKYield;}
            set{_iOKYield = value;}
        }


        private long _inspTime = 0;
        public long inspTime
        {
            get{return _inspTime;}
            set{_inspTime = value;}
        }

        private long _processTime = 0;
        public long processTime
        {
            get{return _processTime;}
            set{_processTime = value;}
        }

        private long _avgInspTime = 0;
        public long AvgInspTime
        {
            get{return _avgInspTime;}
            set{_avgInspTime = value;}
        }

        private long _avgProcessTime = 0;
        public long AvgProcessTime
        {
            get{return _avgProcessTime;}
            set{_avgProcessTime = value;}
        }

        private int _iAvgInspCnt = 0;
        public int iAvgInspCnt
        {
            get{ return _iAvgInspCnt;}
            set{ _iAvgInspCnt = value;}
        }

        private int _iAvgProcessCnt = 0;
        public int iAvgProcessCnt
        {
            get{return _iAvgProcessCnt;}
            set{_iAvgProcessCnt = value;}
        }


        private bool _bCheckInspRun = false;
        public bool bCheckInspRun
        {
            get{return _bCheckInspRun;}
            set{_bCheckInspRun = value;}
        }

        private bool _bClaheBtn = false;
        public bool bClaheBtn
        {
            get{return _bClaheBtn;}
            set{_bClaheBtn = value;}
        }


        private List<string> _savedInfosList = null;
        public List<string> SavedInfosList
        {
            get{return _savedInfosList;}
            set{_savedInfosList = value;}
        }

        


        private Dictionary<ICameraInfo, String> _cameraInfosDictionary;
        public Dictionary<ICameraInfo, String> CameraInfosDictionary
        {
            get{return _cameraInfosDictionary;}
            set{_cameraInfosDictionary = value;}
        }

        private int c_maxCamerasToUse = 2;
        

        private bool _bSnapshotMode = false;
        public bool bSnapshotMode
        {
            get { return _bSnapshotMode; }
            set { _bSnapshotMode = value; }
        }


        private string _sCamID;
        public string sCamID
        {
            get { return _sCamID; }
            set { _sCamID = value; }
        }

        private List<string> _caminfoList ;
        public List<string> CaminfoList
        {
            get{return _caminfoList;}
            set{_caminfoList = value;}
        }

        private List<string> _camExTimeinfoList ;
        public List<string> CamExTimeinfoList
        {
            get{return _camExTimeinfoList;}
            set{_camExTimeinfoList = value;}
        }


        private List<string> _LEDValList;
        public List<string> LEDValList
        {
            get{return _LEDValList;}
            set{_LEDValList = value;}
        }

        private List<string> _dnnFilePathList;
        public List<string> DnnFilePathList
        {
            get{return _dnnFilePathList;}
            set{_dnnFilePathList = value;}
        }

        private List<string> _cropInfoList;
        public List<string> CropInfoList
        {
            get{return _cropInfoList;}
            set{ _cropInfoList = value;}
        }

        private List<ICameraInfo> _allDeviceInfos;
        public List<ICameraInfo> AllDeviceInfos
        {
            get{return _allDeviceInfos;}
            set{ _allDeviceInfos = value;}
        }

        public Thread t5;


        public MainModel()
        {
            InitVariables();
        }

        public void InitVariables()
        {

            CntTimeDelegate = new CntTimeClass();
            DnnSetDelegate = new DnnSetClass();
            CntNGDelegate = new CntNGClass();
            ShowSigClassDelegate = new ShowSignal();
            CaminfoList = new List<string>(4);
            CamExTimeinfoList = new List<string>(4);
            LEDValList = new List<string>(4);
            DnnFilePathList = new List<string>(4);
            CropInfoList = new List<string>(4);
            
    }


    }
}
