using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Basler.Pylon;
namespace Frism
{
    public partial class SelectCameras : INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        System.Windows.Threading.DispatcherTimer timer;

        //string caminfo;
        public LEDWindow selectLED;


        public string CamID;

        public string temp;
        public string caminfo1 = "no";
        public string caminfo2 = "no";
        public string caminfo3 = "no";
        public string caminfo4 = "no";
        public List<string> caminfo;
        public List<string> camExtimeInfo;
        public List<string> dnnFileInfo;
        

        public List<string> camid;
        public List<string> camExtime;

        List<string> LEDVal;

        private MainWindow mainWindow = null;


        public static Dictionary<ICameraInfo, String> foundCameraInfos;
        private Dictionary<ICameraInfo, String> itemInfos;

        private List<string> savedInfos = null;

        public ICameraInfo selectedCamera1 = null;
        public ICameraInfo selectedCamera2 = null;
        public ICameraInfo selectedCamera3 = null;
        public ICameraInfo selectedCamera4 = null;
        public static List<ICameraInfo> selectedCameras = null;
        private List<ICameraInfo> currentCameras;
        private ICameraInfo selectedCameraInfo = null;
        private List<ICameraInfo> savedCameraInfo;
        public List<string> DnnFilePath;


        public Dictionary<ICameraInfo, String> Items
        {
            get { return itemInfos; }
            set
            {
                itemInfos = value;
                OnPropertyChanged();

            }
        }


        public void InitVariables()
        {
            try
            {
                currentCameras = new List<ICameraInfo>(4);
                selectLED = new LEDWindow();
                caminfo = new List<string>(4);
                camExtimeInfo = new List<string>(4) { "0", "0", "0", "0" };
                dnnFileInfo = new List<string>(4) { "0", "0", "0", "0" };
                //dnnFileInfo = new List<string>(4) { "0", "0", "0", "0" };
                LEDVal = new List<string>(4) { "0", "0", "0", "0" };
                savedCameraInfo = new List<ICameraInfo>();
                DnnFilePath = new List<string>(4);


        }
            catch (Exception e)
            {
                Logger.Error(e.Message + " InitVariables");
                throw e;
            }
        }



        public SelectCameras(MainWindow mainWindow)
        {
            InitializeComponent();
            
            InitVariables();
            

            DataContext = this;

            DBAcess.CreateDB();


            this.mainWindow = mainWindow;
            //camid = DBAcess.DataShow();
            //camExtime = DBAcess.DataShowExTimeInfo();

            UpdateDeviceList();
            
            UpdateCameraList();
            

            UpdateCamera(camid);
            

            currentCameras.Add(selectedCamera1);
            currentCameras.Add(selectedCamera2);
            currentCameras.Add(selectedCamera3);
            currentCameras.Add(selectedCamera4);
            

            caminfo.Add(caminfo1);
            caminfo.Add(caminfo2);
            caminfo.Add(caminfo3);
            caminfo.Add(caminfo4);



            FirstBlock.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            FirstBlock.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            FirstBlock.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            SecondBlock.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            SecondBlock.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            SecondBlock.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            ThirdBlock.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            ThirdBlock.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            ThirdBlock.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;

            FourthBlock.GetUnoCamera().GuiCameraConnectionToCameraLost += OnDeviceRemoval;
            FourthBlock.GetUnoCamera().GuiCameraOpenedCamera += OnCameraOpened;
            FourthBlock.GetUnoCamera().GuiCameraClosedCamera += OnCameraClosed;



            timer = new DispatcherTimer();

            timer.Tick += FirstBlock.GetUnoCamera().GiveImageFile;
            timer.Tick += SecondBlock.GetUnoCamera().GiveImageFile;
            timer.Tick += ThirdBlock.GetUnoCamera().GiveImageFile;
            timer.Tick += FourthBlock.GetUnoCamera().GiveImageFile;



            timer.Interval = TimeSpan.FromMilliseconds(400);

        }


        public void SetTimer(System.Windows.Threading.DispatcherTimer T)
        {
            //this.timer = T;
        }


        public void OnDeviceRemoval(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnDeviceRemoval), sender, e);
                return;
            }
        }


        private void OnCameraOpened(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }
        }

        private void OnCameraClosed(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void UpdateCameraList()
        {
            try
            {
                savedInfos = DBAcess.CamInfos();
                if(savedInfos == null)
                {
                    return;
                }
                CameraInfoBox.ItemsSource = savedInfos;

            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " UpdateCameraList");
                ShowException(ex);
            }
        }

        public void UpdateDeviceList()
        {
            try
            {

                List<ICameraInfo> cameraInfos = CameraFinder.Enumerate();

                if (cameraInfos.Count > 0)
                {
                    foundCameraInfos = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in cameraInfos)
                    {
                        
                        foundCameraInfos.Add(cameraInfo, cameraInfo[CameraInfoKey.FriendlyName]);    
                    }

                    Items = foundCameraInfos;

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDeviceList");
                //ShowException(exception);
                //Helper.ShowException(exception);
            }
        }

        
        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateCamera(List<string> camIDList)
        {
            if(foundCameraInfos == null)
            {
                return;
            }

            if(camIDList == null)
            {
                return;
            }

            try
            {
                foreach (ICameraInfo cameraInfo in foundCameraInfos.Keys)
                {
                    if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[0])
                    {
                        selectedCamera1 = cameraInfo;
                        FirstBlock.SetSelectionInfo(cameraInfo);

                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[1])
                    {
                        selectedCamera2 = cameraInfo;
                        SecondBlock.SetSelectionInfo(cameraInfo);
                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[2])
                    {
                        selectedCamera3 = cameraInfo;
                        ThirdBlock.SetSelectionInfo(cameraInfo);
                    }
                    else if (cameraInfo[CameraInfoKey.FriendlyName] == camIDList[3])
                    {
                        selectedCamera4 = cameraInfo;
                        FourthBlock.SetSelectionInfo(cameraInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateCamera");
                //ShowException(exception);
            }

           
        }


        private void UpdateLED(List<string> LEDidList)
        {
            if( selectLED == null)
            {
                selectLED = new LEDWindow();
               
            }

            selectLED.MainCom_Open("COM9", 19200);

            try
            {
                int red = Int32.Parse(LEDidList[0]);
                if (red > 0)
                {
                    selectLED.Set_DimmingValue(1, red);
                    selectLED.redVal = red;
                }

                Thread.Sleep(100);

                int green = Int32.Parse(LEDidList[1]);
                if (green > 0)
                {
                    selectLED.Set_DimmingValue(2, green);
                    selectLED.greenVal = green;
                }

                Thread.Sleep(100);

                int blue = Int32.Parse(LEDidList[2]);
                if (blue > 0)
                {
                    selectLED.Set_DimmingValue(3, blue);
                    selectLED.blueVal = blue;

                }

                Thread.Sleep(100);

                int white = Int32.Parse(LEDidList[3]);
                if (white > 0)
                {
                    selectLED.Set_DimmingValue(0, white);
                    selectLED.whiteVal = white;
                }

                selectLED.setLEDValues();



            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " UpdateLED");
                //ShowException(exception);
            }
            

           

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
                FirstBlock.DestroyCamera();
                SecondBlock.DestroyCamera();
                ThirdBlock.DestroyCamera();
                FourthBlock.DestroyCamera();

            timer.Stop();

            mainWindow.SettingCameraWindowClosing();

            if (selectLED != null)
            {
                if (selectLED.serialPort1.IsOpen)
                {
                    selectLED.Led_OnOff(0x00, 0);
                   
                    selectLED.serialPort1.Close();
                }

                //selectLED.Hide();
                selectLED.Close();
                
            }
            



        }

        private void SettingCameraButton_Click(object sender, RoutedEventArgs e)
        {

            

            if (CameraInfoTxt.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter a Key Value");
                return;
            }

            

            selectedCameras = new List<ICameraInfo>();

            List<string> listCropValues = new List<string> ();


            // 카메라 
            camExtimeInfo[0] = FirstBlock.ExposureText1.Text;
            camExtimeInfo[1] = SecondBlock.ExposureText1.Text;
            camExtimeInfo[2] = ThirdBlock.ExposureText1.Text;
            camExtimeInfo[3] = FourthBlock.ExposureText1.Text;
            
            LEDVal[0] = selectLED.redVal.ToString();
            LEDVal[1] = selectLED.greenVal.ToString();
            LEDVal[2] = selectLED.blueVal.ToString();
            LEDVal[3] = selectLED.whiteVal.ToString();

            //dnnFileInfo = DBAcess.GiveDNNFileSetting(CamID);
            



            if (savedInfos.Contains(CameraInfoTxt.Text))
            {
                ///////////////// DNN //////////////////////
                dnnFileInfo = DBAcess.GiveDNNFileSetting(CamID);
                if (FirstBlock.filename != "0")
                {
                    dnnFileInfo[0] = FirstBlock.filename;
                }
                if (SecondBlock.filename != "0")
                {
                    dnnFileInfo[1] = SecondBlock.filename;
                }
                if (ThirdBlock.filename != "0")
                {
                    dnnFileInfo[2] = ThirdBlock.filename;
                }
                if (FourthBlock.filename != "0")
                {
                    dnnFileInfo[3] = FourthBlock.filename;
                }

                



                //////////////////// Crop /////////////////////
                listCropValues = DBAcess.GiveCropInfoSetting(CamID);

                if (FirstBlock.PreviewImage.GetCropValToSave() != "null")
                {
                    listCropValues[0] = FirstBlock.PreviewImage.GetCropValToSave();
                }
                if (SecondBlock.PreviewImage.GetCropValToSave() != "null")
                {
                    listCropValues[1] = SecondBlock.PreviewImage.GetCropValToSave();
                }
                if (ThirdBlock.PreviewImage.GetCropValToSave() != "null")
                {
                    listCropValues[2] = ThirdBlock.PreviewImage.GetCropValToSave();
                }
                if (FourthBlock.PreviewImage.GetCropValToSave() != "null")
                {
                    listCropValues[3] = FourthBlock.PreviewImage.GetCropValToSave();
                }
            }
            else
            {
                ///////////////// DNN /////////////////
                dnnFileInfo[0] = FirstBlock.filename;
                dnnFileInfo[1] = SecondBlock.filename;
                dnnFileInfo[2] = ThirdBlock.filename;
                dnnFileInfo[3] = FourthBlock.filename;


                ///////////////// Crop ////////////////
                listCropValues.Add(FirstBlock.PreviewImage.GetCropValToSave());
                listCropValues.Add(SecondBlock.PreviewImage.GetCropValToSave());
                listCropValues.Add(ThirdBlock.PreviewImage.GetCropValToSave());
                listCropValues.Add(FourthBlock.PreviewImage.GetCropValToSave());
            }
            

            


            if (savedInfos.Contains(CameraInfoTxt.Text))
            {
                DBAcess.UpdateDataBase(CameraInfoTxt.Text, caminfo);
                DBAcess.UpdateDataBaseExTime(CameraInfoTxt.Text, camExtimeInfo);
                if (selectLED.LEDInfoSaved)
                {
                    DBAcess.UpdateDataBaseLEDVal(CameraInfoTxt.Text, LEDVal);
                }
                if(dnnFileInfo[0] != "0")
                {
                    DBAcess.UpdateDataBaseDNNFilePath(CameraInfoBox.Text, dnnFileInfo);
                }
                DBAcess.UpdateDataBaseCropInfo(CameraInfoBox.Text, listCropValues);

            }
            else
            {
                DBAcess.InsertCamera(CameraInfoTxt.Text, caminfo);
                DBAcess.InsertExTime(CameraInfoTxt.Text, camExtimeInfo);
                DBAcess.InsertLEDVal(CameraInfoTxt.Text, LEDVal);
                DBAcess.InsertDNNFilePath(CameraInfoTxt.Text, dnnFileInfo);
                DBAcess.InsertCropInfo(CameraInfoTxt.Text, listCropValues);
            }

            selectedCameras.Add(selectedCamera1);
            selectedCameras.Add(selectedCamera2);
            selectedCameras.Add(selectedCamera3);
            selectedCameras.Add(selectedCamera4);

            ResetCameraInfo();

            this.Close();
        }


        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (GroupComboBox.Items.Count > 0)
            {
                if (GroupComboBox.SelectedItem != null)
                {
                    selectedCameraInfo = ((KeyValuePair<ICameraInfo, String>)GroupComboBox.SelectedItem).Key;
                    temp = ((KeyValuePair<ICameraInfo, String>)GroupComboBox.SelectedItem).Value;
                }
                else
                {
                    selectedCameraInfo = null;
                }
            }
        }



       
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedCameraInfo != null)
                {
                    if (selectedCameraInfo == currentCameras[2] || selectedCameraInfo == currentCameras[3] || selectedCameraInfo == currentCameras[1])
                    {
                        System.Windows.Forms.MessageBox.Show("Camera is already opened... Please select a different camera");
                        return;
                    }
                    currentCameras[0] = selectedCameraInfo;
                    selectedCamera1 = selectedCameraInfo;
                    caminfo[0] = temp;
                   
                    
                    FirstBlock.SetSelectionInfo(selectedCameraInfo);

                    if (camExtimeInfo[0] != null && camExtimeInfo[0] != " " && camExtimeInfo[0] != "0")
                    {
                        System.Windows.Forms.MessageBox.Show(camExtimeInfo[0]);
                        double exposureT = (double.Parse(camExtimeInfo[0]));

                        //FirstBlock.GetUnoCamera().Parameters[PLCamera.ExposureTimeAbs].SetValue(exposureT);
                        FirstBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                        FirstBlock.GetUnoCamera().SetGainParameter(PLCamera.Gain, 1.0);
                        //FirstBlock.GetUnoCamera().Parameters[PLCamera.Gain].SetValue(1.0);
                    }


                }
                
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " Button_Click_1");
                ShowException(ex);
            }
            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedCameraInfo != null)
                {
                    if(selectedCameraInfo == currentCameras[2] || selectedCameraInfo == currentCameras[3] || selectedCameraInfo == currentCameras[0])
                    {
                        System.Windows.Forms.MessageBox.Show("Camera is already opened... Please select a different camera");
                        return;
                    }
                    currentCameras[1] = selectedCameraInfo;
                    selectedCamera2 = selectedCameraInfo;
                    caminfo[1] = temp;
                    SecondBlock.SetSelectionInfo(selectedCameraInfo);
                    if (camExtimeInfo[1] != null && camExtimeInfo[1] != " " && camExtimeInfo[1] != "0")
                    {
                        System.Windows.Forms.MessageBox.Show(camExtimeInfo[1]);
                        double exposureT = (double.Parse(camExtimeInfo[1]));
                        
                        SecondBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                        SecondBlock.GetUnoCamera().SetGainParameter(PLCamera.Gain, 1.0);
                    }


                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " Button_Click_2");
                //SecondBlock.DestroyCamera();
            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedCameraInfo != null)
                {
                    if (selectedCameraInfo == currentCameras[1] || selectedCameraInfo == currentCameras[3] || selectedCameraInfo == currentCameras[0])
                    {
                        System.Windows.Forms.MessageBox.Show("Camera is already opened... Please select a different camera");
                        return;
                    }
                    currentCameras[2] = selectedCameraInfo;
                    selectedCamera3 = selectedCameraInfo;
                    ThirdBlock.SetSelectionInfo(selectedCameraInfo);
                    caminfo[2] = temp;
                    if (camExtimeInfo[2] != null && camExtimeInfo[2] != " " && camExtimeInfo[2] != "0")
                    {
                        System.Windows.Forms.MessageBox.Show(camExtimeInfo[2]);
                        double exposureT = (double.Parse(camExtimeInfo[2]));
                        ThirdBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                        ThirdBlock.GetUnoCamera().SetGainParameter(PLCamera.Gain, 1.0);
                    }

                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " Button_Click_3");
                //ThirdBlock.DestroyCamera();
            }
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedCameraInfo != null)
                {
                    if (selectedCameraInfo == currentCameras[2] || selectedCameraInfo == currentCameras[1] || selectedCameraInfo == currentCameras[0])
                    {
                        System.Windows.Forms.MessageBox.Show("Camera is already opened... Please select a different camera");
                        return;
                    }
                    currentCameras[3] = selectedCameraInfo;
                    selectedCamera4 = selectedCameraInfo;
                    FourthBlock.SetSelectionInfo(selectedCameraInfo);
                    caminfo[3] = temp;
                    if (camExtimeInfo[3] != null && camExtimeInfo[3] != " " && camExtimeInfo[3] != "0")
                    {
                        System.Windows.Forms.MessageBox.Show(camExtimeInfo[3]);
                        double exposureT = (double.Parse(camExtimeInfo[3]));
                        FourthBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                        FourthBlock.GetUnoCamera().SetGainParameter(PLCamera.Gain, 1.0);
                    }

                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " Button_Click_4");
                //FourthBlock.DestroyCamera();
            }
        }

        private void SecondBlock_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void fullScreenBtn_Click(object sender, RoutedEventArgs e)
        {
            if(selectedCameraInfo == selectedCamera1)
            {
                FirstBlock.PreviewImage.GiveUno().FullScreenMode();
            }
            else if (selectedCameraInfo == selectedCamera2)
            {
                SecondBlock.PreviewImage.GiveUno().FullScreenMode();
            }
            else if (selectedCameraInfo == selectedCamera3)
            {
                ThirdBlock.PreviewImage.GiveUno().FullScreenMode();
            }
            else if (selectedCameraInfo == selectedCamera4)
            {
                FourthBlock.PreviewImage.GiveUno().FullScreenMode();
            }
        }

        private void ResetCameraInfo()
        {
            currentCameras[0] = null;
            currentCameras[1] = null;
            currentCameras[2] = null;
            currentCameras[3] = null;

            caminfo[0] = "no";
            caminfo[1] = "no";
            caminfo[2] = "no";
            caminfo[3] = "no";

            camExtimeInfo[0] = " ";
            camExtimeInfo[1] = " ";
            camExtimeInfo[2] = " ";
            camExtimeInfo[3] = " ";
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            selectedCameras = null;

            ResetCameraInfo();

            FirstBlock.StartButton.IsEnabled = true;
            SecondBlock.StartButton.IsEnabled = true;
            ThirdBlock.StartButton.IsEnabled = true;
            FourthBlock.StartButton.IsEnabled = true;

            FirstBlock.ResetCamera();
            SecondBlock.ResetCamera();
            ThirdBlock.ResetCamera();
            FourthBlock.ResetCamera();
        }

        private void CameraInfoBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CameraInfoBox.Items.Count > 0)
            {
                if (CameraInfoBox.SelectedItem != null)
                {
                    CamID = (string)CameraInfoBox.SelectedItem;
                    CameraInfoTxt.Text = CamID;
                }
                else
                {
                    CamID = null;
                }
            }
        }
        private void SetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartAllBtn.IsEnabled = true;

                FirstBlock.ResetCamera();
                SecondBlock.ResetCamera();
                ThirdBlock.ResetCamera();
                FourthBlock.ResetCamera();
                ResetCameraInfo();
                caminfo = DBAcess.GiveCamSettings(CamID);
                camExtimeInfo = DBAcess.GiveExTimeSetting(CamID);
                LEDVal = DBAcess.GiveLEDValSetting(CamID);
                DnnFilePath = DBAcess.GiveDNNFileSetting(CamID);


                UpdateDnnFiles(DnnFilePath);


                UpdateCamera(caminfo);


                UpdateLED(LEDVal);



                if (camExtimeInfo[0] != " " || camExtimeInfo[0] != "0")
                {
                    double exposureT = (double.Parse(camExtimeInfo[0]));
                    if (FirstBlock.GetUnoCamera().CheckIsCreated())
                    {
                        //FirstBlock.GetUnoCamera().Parameters[PLCamera.ExposureTimeAbs].SetValue(exposureT);
                        FirstBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                    }
                   
                    //FirstBlock.GetUnoCamera().Parameters[PLCamera.Gain].SetValue(1.0);
                }

                if (camExtimeInfo[1] != " " || camExtimeInfo[1] != "0" )
                {
                    double exposureT = (double.Parse(camExtimeInfo[1]));

                    if (SecondBlock.GetUnoCamera().CheckIsCreated())
                    {
                        //SecondBlock.GetUnoCamera().Parameters[PLCamera.ExposureTimeAbs].SetValue(exposureT);
                        SecondBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT); 
                    }
                    
                    //SecondBlock.GetUnoCamera().Parameters[PLCamera.Gain].SetValue(1.0);
                }
                if (camExtimeInfo[2] != " " || camExtimeInfo[2] != "0")
                {
                    double exposureT = (double.Parse(camExtimeInfo[2]));
                    if (ThirdBlock.GetUnoCamera().CheckIsCreated())
                    {
                        //ThirdBlock.GetUnoCamera().Parameters[PLCamera.ExposureTimeAbs].SetValue(exposureT);
                        ThirdBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                    }
                    
                    //ThirdBlock.GetUnoCamera().Parameters[PLCamera.Gain].SetValue(1.0);
                }
                if (camExtimeInfo[3] != " " || camExtimeInfo[3] != "0")
                {
                    double exposureT = (double.Parse(camExtimeInfo[3]));
                    if (FourthBlock.GetUnoCamera().CheckIsCreated())
                    {
                        //FourthBlock.GetUnoCamera().Parameters[PLCamera.ExposureTimeAbs].SetValue(exposureT);
                        FourthBlock.GetUnoCamera().SetExTimeParameter(PLCamera.ExposureTimeAbs, exposureT);
                    }
                    
                    //FourthBlock.GetUnoCamera().Parameters[PLCamera.Gain].SetValue(1.0);
                }



                FirstBlock.ExposureText1.Text = camExtimeInfo[0];
                SecondBlock.ExposureText1.Text = camExtimeInfo[1];
                ThirdBlock.ExposureText1.Text = camExtimeInfo[2];
                FourthBlock.ExposureText1.Text = camExtimeInfo[3];

                FirstBlock.GainText1.Text = "1";
                SecondBlock.GainText1.Text = "1";
                ThirdBlock.GainText1.Text = "1";
                FourthBlock.GainText1.Text = "1";


            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message + " SetBtn_Click");
            }
            


            
        }

        private void StartAllBtn_Click(object sender, RoutedEventArgs e)
        {
            FirstBlock.GetUnoCamera().ChangeFilePath(@"D:\aaaaa");
            FirstBlock.StartCam();
            SecondBlock.StartCam();
            ThirdBlock.StartCam();
            FourthBlock.StartCam();
            timer.Start();
        }
        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DBAcess.DeleteData(CamID);
            DBAcess.DeleteDataExTime(CamID);
            DBAcess.DeleteDataLEDVal(CamID);
            DBAcess.DeleteDataCropInfo(CamID);
            DBAcess.DeleteDataDNNFilePath(CamID);
            UpdateCameraList();
        }

        private void SetLEDBtn_Click(object sender, RoutedEventArgs e)
        {
            

            if (selectLED.serialPort1.IsOpen)
            {
                //selectLED.Led_OnOff(0x00, 0);

                //selectLED.serialPort1.Close();
            }
            selectLED.Close();
            selectLED = new LEDWindow();



            selectLED.Show();


            
        }



        private void UpdateDnnFiles(List<string> DnnFileList)
        {
            try
            {
                FirstBlock.DNNPathTxt.Text = DnnFileList[0];
                FirstBlock.filename = dnnFileInfo[0];
                SecondBlock.DNNPathTxt.Text = DnnFileList[1];
                SecondBlock.filename = dnnFileInfo[1];
                ThirdBlock.DNNPathTxt.Text = DnnFileList[2];
                ThirdBlock.filename = dnnFileInfo[2];
                FourthBlock.DNNPathTxt.Text = DnnFileList[3];
                FourthBlock.filename = dnnFileInfo[3];

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }


    }
}
