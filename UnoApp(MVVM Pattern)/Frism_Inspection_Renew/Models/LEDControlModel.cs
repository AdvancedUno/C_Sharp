using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Frism_Inspection_Renew.Models
{
    public class LEDControlModel: INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        enum Cmd
        {
            SET_DIMMING_VALUE = 0x01,
            SET_EACH_DIMMING = 0x02,
            SET_LED_ONOFF = 0x05,

            GET_DIMMING_VALUE = 0x11,
            GET_LED_ONOFF = 0x12,
            GET_FW_VERSION = 0x1A,
            GET_TEMP = 0x1B,
            GET_CH_MAX = 0x16,

            SET_UNIT_INFO = 0x21,
            SET_TRIGGER_INFO = 0x22,
            SET_TEST = 0x2A,
            SET_UNIT_CLEAR = 0x2B,

            GET_UNIT_INFO = 0x31,
            GET_TRIGGER_INFO = 0x32,

            GET_OFFSET = 0xFB,
            SET_OFFSET = 0xFC,

        }

        private readonly int mDefaultCh_Max = 4;

        private SerialPort _serialPortObj = new SerialPort();
        public SerialPort SerialPortObj { get => _serialPortObj; set => _serialPortObj = value; }

        private DispatcherTimer _timer = new DispatcherTimer();
        public DispatcherTimer Timer { get => _timer; set => _timer = value; }

        private byte[] mRxBuf = new byte[1024];


        private int _rxDataLength;
        public int DataLength { get => _rxDataLength; set => _rxDataLength = value; }

        private Boolean _mIsComSelected = false;
        public bool MIsComSelected { get => _mIsComSelected; set => _mIsComSelected = value; }


        private int[] mCh_Values = new int[102];

        private OleDbConnection _oleDbConn;
        public OleDbConnection OleDbConn { get => _oleDbConn; set => _oleDbConn = value; }

        private BackgroundWorker _bgWorker;
        public BackgroundWorker BgWorker { get => _bgWorker; set => _bgWorker = value; }

        public static RoutedCommand MyCommand_CtrlN = new RoutedCommand();


        private Button _btnGetOffset;
        public Button BtnGetOffset { get => _btnGetOffset; set => _btnGetOffset = value; }

        private Button _btnSetOffset = new Button();
        public Button BtnSetOffset { get => _btnSetOffset; set => _btnSetOffset = value; }

        private List<string> _comportDeviceList;
        public List<string> ComportDeviceList { 
            get => _comportDeviceList;
            set
            {
                _comportDeviceList = value;
                OnPropertyChanged("ComportDeviceList");
            } 
        }

        private Byte mHeader = 0x50;// P
        private Byte mCheckSum = 0x43;// C

        private int _redVal = 0;
        public int RedVal { get => _redVal; set => _redVal = value; }

        private int _greenVal = 0;
        public int GreenVal { get => _greenVal; set => _greenVal = value; }

        private int _blueVal = 0;
        public int BlueVal { get => _blueVal; set => _blueVal = value; }

        private int _whiteVal = 0;
        public int WhiteVal { get => _whiteVal; set => _whiteVal = value; }

        private double _sldRedValue = 0;
        public double SldRedValue { get => _sldRedValue; set => _sldRedValue = value; }

        private double _sldGreenValue = 0;
        public double SldGreenValue { get => _sldGreenValue; set => _sldGreenValue = value; }

        private double _sldBlueValue = 0;
        public double SldBlueValue { get => _sldBlueValue; set => _sldBlueValue = value; }

        private double _sldWhiteValue = 0;
        public double SldWhiteValue { get => _sldWhiteValue; set => _sldWhiteValue = value; }

        private bool _myLEDInfoSaved = false;
        public bool MyLEDInfoSaved { get => _myLEDInfoSaved; set => _myLEDInfoSaved = value; }
        
        private string _baudrateValue;
        public string BaudrateValue { 
            get => _baudrateValue;
            set { 
                _baudrateValue = value;
                OnPropertyChanged("BaudrateValue");
            }

        }


        public LEDControlModel()
        {

            MyCommand_CtrlN.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            //CommandBindings.Add(new CommandBinding(MyCommand_CtrlN, onAppendOffsetBtn));
            SerialPortObj = new SerialPort();
            OleDbConn = new OleDbConnection();
            BgWorker = new BackgroundWorker();
            BtnGetOffset = new Button();
            BtnSetOffset = new Button();

            





            OleDbConn.ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\Ch102_DB.mdb";

            //GetComport(ref CboComport);
            //if (CboComport.Items.Count > 0) CboComport.SelectedIndex = (CboComport.Items.Count - 1);// 0;
                                                                                                    //Connect your access database




            BaudrateValue = "19200";

            BgWorker.WorkerReportsProgress = true;
            //스레드에서 취소 지원 여부-
            BgWorker.WorkerSupportsCancellation = true;

            BtnSetOffset.Content = "Set Offset";

            BtnGetOffset.Content = "Get Offset";
            BtnGetOffset.Click += OnbtnGetOffset_Click;




        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnbtnGetOffset_Click(object sender, RoutedEventArgs e)
        {
            byte[] message = new byte[4];
            message[0] = mHeader;    //head
            message[1] = (byte)Cmd.GET_OFFSET;    //command
            message[2] = (byte)message.Length;
            message[3] = mCheckSum;// CheckSumCal(message, message.Length - 1);

            try
            {
                SerialPortObj.DiscardInBuffer();
                DataLength = 0;
                SerialPortObj.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), " Serial Port Err Message", MessageBoxButton.OK);
                Logger.Error(ex.Message + " OnbtnGetOffset_Click");
            }
            TxData_Display(message, message.Length);
        }



        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GetComport()
        {
            
            foreach (string comPort in SerialPort.GetPortNames())
            {
                if (comPort.Length > 4)
                {
                    if (!Char.IsDigit(Char.Parse(comPort.Substring(4, 1))))
                        ComportDeviceList.Add(comPort.Substring(0, 4));
                    else
                        ComportDeviceList.Add(comPort);
                }
                else
                {
                    ComportDeviceList.Add(comPort);
                }
            }
        }

        //Display records in grid

        private bool Com_Open()
        {
            bool rValue = false;

            try
            {
                if (SerialPortObj.IsOpen) SerialPortObj.Close();

                SerialPortObj.PortName = "COM10";
                SerialPortObj.BaudRate = Int32.Parse(BaudrateValue.ToString());
                SerialPortObj.DataBits = 8;
                SerialPortObj.StopBits = StopBits.One;
                SerialPortObj.Parity = Parity.None;
                SerialPortObj.Encoding = Encoding.ASCII;

                SerialPortObj.Open();

                rValue = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Err Message");
                Logger.Error(e.Message + " Com_Open");
            }
            finally
            {

            }
            return rValue;
        }

        public void MainCom_Open(string sComport, int iBaudrate)
        {
            bool rValue = false;

            try
            {
                if (SerialPortObj.IsOpen) SerialPortObj.Close();

                SerialPortObj.PortName = "COM10"; ;
                SerialPortObj.BaudRate = iBaudrate;
                SerialPortObj.DataBits = 8;
                SerialPortObj.StopBits = StopBits.One;
                SerialPortObj.Parity = Parity.None;
                SerialPortObj.Encoding = Encoding.ASCII;

                SerialPortObj.Open();

                rValue = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Err Message");
                Logger.Error(e.Message);
            }
            finally
            {

            }

        }

        private void BtnComOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Com_Open())
            {
                //BtnComOpen.IsEnabled = false;
                //BtnComClose.IsEnabled = true;
                MIsComSelected = true;
                //Button_Active(true);
            }
        }

        private void BtnComClose_Click(object sender, RoutedEventArgs e)
        {
            SerialPortObj.Close();
            //BtnComOpen.IsEnabled = true;
            //BtnComClose.IsEnabled = false;
            MIsComSelected = false;
            //Button_Active(false);
        }


        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //this.Invoke(new EventHandler(SerialReceived));
            int RxDataLength = SerialPortObj.BytesToRead; // 수신된 데이터 갯수

            //timer1.Stop();
            if (RxDataLength != 0) // 수신된 데이터의 수가 0이 아닐때만 처리하자
            {
                byte[] buff = new byte[RxDataLength];
                SerialPortObj.Read(buff, 0, RxDataLength);

                for (int iTemp = 0; iTemp < RxDataLength; iTemp++)
                {
                    mRxBuf[DataLength++] = buff[iTemp];
                }
                Timer.Interval = TimeSpan.FromMilliseconds(100);
                Timer.Start();
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Timer.Stop();
            RxData_Display(mRxBuf, DataLength);
            //RxData_Parsing(mRxBuf, mRxDataLength);
        }
        private void TxData_Display(byte[] Buf, int DataCount)
        {
            String str = "";
            //_lblRxData.Content = str;
            for (int i = 0; i < DataCount; i++) str += " " + Buf[i].ToString("X2");
            //_lblTxData.Content = str;
        }
        private void RxData_Display(byte[] Buf, int DataCount)
        {
            String str = "";
            for (int i = 0; i < DataCount; i++) str += " " + Buf[i].ToString("X2");
            //_lblRxData.Content = str;

        }
        private Byte CheckSumCal(Byte[] data, int DataCount)
        {
            Byte ChkSum = 0;

            for (int i = 0; i < DataCount; i++)
            {
                ChkSum = (Byte)(ChkSum ^ data[i]);
            }
            return ChkSum;
        }


        private void _rdoLedOff_Checked(object sender, RoutedEventArgs e)
        {
            //if (_rdoLedOff.IsChecked.Value)
            //{
            //    Led_OnOff(0x00, 0);
            //}

        }

        private void _rdoLedOn_Checked(object sender, RoutedEventArgs e)
        {

            //if (_rdoLedOn.IsChecked.Value)
            //{
            //    Led_OnOff(0x00, 1);

            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="OnOff"></param>
        public void Led_OnOff(int Channel, Byte OnOff)
        {
            byte[] message = new byte[6];

            message[0] = mHeader;    //head
            message[1] = (byte)Cmd.SET_LED_ONOFF;    //command
            message[2] = (byte)message.Length;
            message[3] = (byte)Channel;      //data 
            message[4] = OnOff;      //data 
            message[5] = mCheckSum;// CheckSumCal(message, message.Length - 1);

            try
            {
                SerialPortObj.DiscardInBuffer();
                DataLength = 0;
                SerialPortObj.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), " Serial Port Err Message", MessageBoxButton.OK);
                Logger.Error(ex.Message);
            }
            TxData_Display(message, message.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="Value"></param>
        public void Set_DimmingValue(int Channel, int Value)
        {
            byte[] message = new byte[7];

            message[0] = mHeader;    //head
            message[1] = (byte)Cmd.SET_DIMMING_VALUE;    //command
            message[2] = (byte)message.Length;
            message[3] = (byte)Channel;      //data 
            message[4] = (byte)(Value / 0x100);      //data 
            message[5] = (byte)(Value % 0x100);      //data 
            message[6] = mCheckSum;// CheckSumCal(message, message.Length - 1);

            try
            {
                SerialPortObj.DiscardInBuffer();
                DataLength = 0;
                SerialPortObj.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), " Serial Port Err Message", MessageBoxButton.OK);
                Logger.Error(ex.Message);
            }
            TxData_Display(message, message.Length);
        }


        private void Get_DimmingValue(int Channel)
        {
            byte[] message = new byte[5];

            message[0] = mHeader;    //head
            message[1] = (byte)Cmd.GET_DIMMING_VALUE;    //command
            message[2] = (byte)message.Length;
            message[3] = (byte)Channel;      //data 
            message[4] = mCheckSum;// CheckSumCal(message, message.Length - 1);

            try
            {
                SerialPortObj.DiscardInBuffer();
                DataLength = 0;
                SerialPortObj.Write(message, 0, message.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), " Serial Port Err Message", MessageBoxButton.OK);
                Logger.Error(ex.Message);
            }
            TxData_Display(message, message.Length);
        }


        private void _btnGet_All_LedValue_Click(object sender, RoutedEventArgs e)
        {

            Get_DimmingValue(0);
        }


        private void RedSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int temp = (int)SldRedValue;
            //RedTxt.Text = temp.ToString();
        }

        private void GreenSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int temp = (int)SldGreenValue;
            //GreenTxt.Text = temp.ToString();
        }

        private void BlueSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int temp = (int)SldBlueValue;
            //BlueTxt.Text = temp.ToString();
        }

        private void WhiteSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //int temp = (int)SldWhiteValue;
            //WhiteTxt.Text = temp.ToString();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

            RedVal = (int)SldRedValue; ;
            GreenVal = (int)SldGreenValue; ;
            BlueVal = (int)SldBlueValue;
            WhiteVal = (int)SldWhiteValue;

            //BlueSld.Value = BlueVal;
            //RedSld.Value = RedVal;
            //GreenSld.Value = GreenVal;
            //WhiteSld.Value = WhiteVal;


            //LEDInfoSaved = true;

        }


        public void setLEDValues()
        {

            //RedTxt.Text = RedVal.ToString();
            //GreenTxt.Text = GreenVal.ToString();
            //BlueTxt.Text = BlueVal.ToString();
            //WhiteTxt.Text = WhiteVal.ToString();

        }


        private void RedBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = (int)RedVal; //Int32.Parse(RedTxt.Text);
            Set_DimmingValue(1, temp);
        }

        private void GreenBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = (int)GreenVal; //Int32.Parse(GreenTxt.Text);
            Set_DimmingValue(2, temp);
        }

        private void BlueBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = (int)BlueVal; //Int32.Parse(BlueTxt.Text);
            Set_DimmingValue(3, temp);
        }

        private void WhiteBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = (int)WhiteVal; //Int32.Parse(WhiteTxt.Text);
            Set_DimmingValue(0, temp);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (SerialPortObj.IsOpen)
            {
                Led_OnOff(0x00, 0);
                SerialPortObj.Close();
            }
        }

    }


   
}

