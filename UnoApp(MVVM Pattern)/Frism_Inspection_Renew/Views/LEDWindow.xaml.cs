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
namespace Frism_Inspection_Renew.Views
{
    /// <summary>
    /// LEDWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LEDWindow : Window
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
        public SerialPort serialPort1 = new SerialPort();
        private DispatcherTimer timer1 = new DispatcherTimer();

        private byte[] mRxBuf = new byte[1024];
        private int mRxDataLength;
        private int size = 1024;


        private Boolean mIsComSelected = false;

        private int[] mCh_Values = new int[102];

        private OleDbConnection mConn;
        private DataTable mDt;
        private int mFullTrans_RowCount;
        private int mFullTrans_UnitNum;
        private int mSubBoard_Count;
        private int mChannelCount = 8;
        private int mGet_LedValue_Ch;//광량 값 요청 채널

        private BackgroundWorker bw = new BackgroundWorker(); //BackgroundWorker클래스를 선언 및 할당

        public static RoutedCommand MyCommand_CtrlN = new RoutedCommand();

        private Button mbtnGetOffset = new Button();
        private Button mbtnSetOffset = new Button();

        private Byte mHeader = 0x50;// P
        private Byte mCheckSum = 0x43;// C

        public int redVal = 0;
        public int greenVal = 0;
        public int blueVal = 0;
        public int whiteVal = 0;

        public bool LEDInfoSaved = false;

        public LEDWindow()
        {
            InitializeComponent();

            MyCommand_CtrlN.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            //CommandBindings.Add(new CommandBinding(MyCommand_CtrlN, onAppendOffsetBtn));



            GetComport(ref CboComport);
            if (CboComport.Items.Count > 0) CboComport.SelectedIndex = (CboComport.Items.Count - 1);// 0;
            //Connect your access database
            mConn = new OleDbConnection();
            mConn.ConnectionString = "Provider=Microsoft.Jet.Oledb.4.0; Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "\\Ch102_DB.mdb";



            cboBaudrate.Text = "19200";

            bw.WorkerReportsProgress = true;
            //스레드에서 취소 지원 여부
            bw.WorkerSupportsCancellation = true;

            mbtnSetOffset.Content = "Set Offset";

            mbtnGetOffset.Content = "Get Offset";
            mbtnGetOffset.Click += OnbtnGetOffset_Click;
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
                serialPort1.DiscardInBuffer();
                mRxDataLength = 0;
                serialPort1.Write(message, 0, message.Length);
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

        private void GetComport(ref ComboBox cboCom)
        {
            cboCom.Items.Clear();
            foreach (string comPort in SerialPort.GetPortNames())
            {
                if (comPort.Length > 4)
                {
                    if (!Char.IsDigit(Char.Parse(comPort.Substring(4, 1))))
                        cboCom.Items.Add(comPort.Substring(0, 4));
                    else
                        cboCom.Items.Add(comPort);
                }
                else
                {
                    cboCom.Items.Add(comPort);
                }
            }
        }

        //Display records in grid

        private bool Com_Open()
        {
            bool rValue = false;

            try
            {
                if (serialPort1.IsOpen) serialPort1.Close();

                serialPort1.PortName = "COM10";
                serialPort1.BaudRate = Int32.Parse(cboBaudrate.Text.ToString());
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.Encoding = Encoding.ASCII;

                serialPort1.Open();

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
                if (serialPort1.IsOpen) serialPort1.Close();

                serialPort1.PortName = "COM10"; ;
                serialPort1.BaudRate = iBaudrate;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.Encoding = Encoding.ASCII;

                serialPort1.Open();

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
                BtnComOpen.IsEnabled = false;
                BtnComClose.IsEnabled = true;
                mIsComSelected = true;
                //Button_Active(true);
            }
        }

        private void BtnComClose_Click(object sender, RoutedEventArgs e)
        {
            serialPort1.Close();
            BtnComOpen.IsEnabled = true;
            BtnComClose.IsEnabled = false;
            mIsComSelected = false;
            //Button_Active(false);
        }


        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //this.Invoke(new EventHandler(SerialReceived));
            int RxDataLength = serialPort1.BytesToRead; // 수신된 데이터 갯수

            //timer1.Stop();
            if (RxDataLength != 0) // 수신된 데이터의 수가 0이 아닐때만 처리하자
            {
                byte[] buff = new byte[RxDataLength];
                serialPort1.Read(buff, 0, RxDataLength);

                for (int iTemp = 0; iTemp < RxDataLength; iTemp++)
                {
                    mRxBuf[mRxDataLength++] = buff[iTemp];
                }
                timer1.Interval = TimeSpan.FromMilliseconds(100);
                timer1.Start();
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            RxData_Display(mRxBuf, mRxDataLength);
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
            if (_rdoLedOff.IsChecked.Value)
            {
                Led_OnOff(0x00, 0);

            }

        }

        private void _rdoLedOn_Checked(object sender, RoutedEventArgs e)
        {

            if (_rdoLedOn.IsChecked.Value)
            {
                Led_OnOff(0x00, 1);

            }
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
                serialPort1.DiscardInBuffer();
                mRxDataLength = 0;
                serialPort1.Write(message, 0, message.Length);
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
                serialPort1.DiscardInBuffer();
                mRxDataLength = 0;
                serialPort1.Write(message, 0, message.Length);
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
                serialPort1.DiscardInBuffer();
                mRxDataLength = 0;
                serialPort1.Write(message, 0, message.Length);
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
            int temp = (int)RedSld.Value;
            RedTxt.Text = temp.ToString();

        }

        private void GreenSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int temp = (int)GreenSld.Value;

            GreenTxt.Text = temp.ToString();
        }

        private void BlueSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int temp = (int)BlueSld.Value;
            BlueTxt.Text = temp.ToString();
        }

        private void WhiteSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int temp = (int)WhiteSld.Value;
            WhiteTxt.Text = temp.ToString();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {

            redVal = Int32.Parse(RedTxt.Text);
            greenVal = Int32.Parse(GreenTxt.Text);
            blueVal = Int32.Parse(BlueTxt.Text);
            whiteVal = Int32.Parse(WhiteTxt.Text);

            BlueSld.Value = blueVal;
            RedSld.Value = redVal;
            GreenSld.Value = greenVal;
            WhiteSld.Value = whiteVal;




            LEDInfoSaved = true;

            this.Hide();
        }


        public void setLEDValues()
        {

            RedTxt.Text = redVal.ToString();
            GreenTxt.Text = greenVal.ToString();
            BlueTxt.Text = blueVal.ToString();
            WhiteTxt.Text = whiteVal.ToString();

        }


        private void RedBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = Int32.Parse(RedTxt.Text);
            Set_DimmingValue(1, temp);
        }

        private void GreenBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = Int32.Parse(GreenTxt.Text);
            Set_DimmingValue(2, temp);
        }

        private void BlueBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = Int32.Parse(BlueTxt.Text);
            Set_DimmingValue(3, temp);
        }

        private void WhiteBtn_Click(object sender, RoutedEventArgs e)
        {
            int temp = Int32.Parse(WhiteTxt.Text);
            Set_DimmingValue(0, temp);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                Led_OnOff(0x00, 0);
                serialPort1.Close();
            }
        }
    }
}
