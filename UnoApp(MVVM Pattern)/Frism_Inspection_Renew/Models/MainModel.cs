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


    public class CntTime
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

        public HistoryView viewHistory;
        public SetCameraView selectCameras;
        public LEDWindow ledSource; // = new LEDWindow();
        public DNNSetting dnnSetWindow;

        public Thread t5;

        public CntTime cntTime;         // = new CntTime();
        public DnnSetClass dnnSetClass; // = new DnnSetClass();
        public CntNGClass cntNGClass;   // = new CntNGClass();
        public ShowSignal showSigClass;

        private int iMaxThreadCnt { get; set; }
        private int iMaxTileWidth { get; set; }
        private int iMaxTileHeight { get; set; }
        private int iGpuNo { get; set; }
        private int iMinDefectNumTop { get; set; }
        private float fMinPValTop { get; set; }
        private int iMinDefectNumSide { get; set; }
        private float fMinPValSide { get; set; }


        private int iNGCnt = 0;
        private int iOKCnt = 0;

        private int iNGYield = 0;
        private int iOKYield = 0;


        private long InspTime = 0;
        private long ProcessTime = 0;

        private long avgInspTime = 0;
        private long avgProcessTime = 0;

        private int iAvgInspCnt = 0;
        private int iAvgProcessCnt = 0;


        private bool bCheckInspRun = false;


        public List<string> SavedInfosList = null;

        private bool bClaheBtn = false;


        public Dictionary<ICameraInfo, String> CameraInfosDictionary;

        const int c_maxCamerasToUse = 2;

        private bool bSnapshotMode = false;



        public string sCamID;



        public List<string> CaminfoList = new List<string>(4);
        public List<string> CamExTimeinfoList = new List<string>(4);
        public List<string> LEDValList = new List<string>(4);
        public List<string> DnnFilePathList = new List<string>(4);
        public List<string> CropInfoList = new List<string>(4);

        List<ICameraInfo> allDeviceInfos;


        Sub_MainView sub_MainView1;
        Sub_MainView sub_MainView2;
        Sub_MainView sub_MainView3;
        Sub_MainView sub_MainView4;


        public MainModel(Sub_MainView sub_MainView1, Sub_MainView sub_MainView2, Sub_MainView sub_MainView3, Sub_MainView sub_MainView4)
        {
            this.sub_MainView1 = sub_MainView1;
            this.sub_MainView2 = sub_MainView2;
            this.sub_MainView3 = sub_MainView3;
            this.sub_MainView4 = sub_MainView4;
        }


    }
}
