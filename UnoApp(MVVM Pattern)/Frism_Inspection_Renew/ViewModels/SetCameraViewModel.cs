using Basler.Pylon;
using Frism_Inspection_Renew.Commands;
using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using NLog;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using Frism_Inspection_Renew.Views;
using Frism_Inspection_Renew.Events;
using System.Dynamic;

namespace Frism_Inspection_Renew.ViewModels
{


    public class SetCameraViewModel : ViewModelBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        #region Commands
        public ICommand NavigateHomeViewCommand { get; }
        public ICommand SetCameraBtn { get; set; }
       
        public ICommand SetAllBtn { get; set; }
        public ICommand StartAllBtn { get; set; }
        public ICommand ResetRecipeBtn { get; set; }
        public ICommand DeleteRecipeBtn { get; set; }
        public ICommand SaveRecipeBtn { get; set; }
        public ICommand FullScreenModeBtn { get; set; }
        public ICommand SetDNNBtn { get; set; }
        public ICommand SetLEDBtn { get; set; }

        #endregion

        private CustomSliderModel _customSlider;
        public CustomSliderModel CustomSlider { get => _customSlider; set => _customSlider = value; }

        private List<string> _dnnFileInfo;
        public List<string> DnnFileInfo { get => _dnnFileInfo; set => _dnnFileInfo = value; }


        private List<string> _cameraIDInfo;
        public List<string> CameraIDInfo { get => _cameraIDInfo; set => _cameraIDInfo = value; }


        private ObservableCollection<string> _savedInfosList = null;
        public ObservableCollection<string> SavedInfosList
        {
            get => _savedInfosList;
            set
            {
                _savedInfosList = value;
                OnPropertyChanged("SavedInfosList");
            }


        }

        private bool _startedPaint;
        public bool StartedPaint { get => _startedPaint; set => _startedPaint = value; }

        private System.Windows.Point _downPoint;
        public System.Windows.Point DownPoint { get => _downPoint; set => _downPoint = value; }

        private System.Windows.Point _upPoint;
        public System.Windows.Point UpPoint { get => _upPoint; set => _upPoint = value; }

        private double _posX = 0;
        public double PosX
        {
            get => _posX;
            set => _posX = value;
        }

        private double _posY = 0;
        public double PosY
        {
            get => _posY;
            set => _posY = value;
        }

        private double _width = 0;
        public double Width
        {
            get => _width;
            set => _width = value;
        }

        private double _height = 0;
        public double Height
        {
            get => _height;
            set => _height = value;
        }


        private Dictionary<ICameraInfo, string> _cameraInfos;
        public Dictionary<ICameraInfo, string> CameraInfos
        {
            get
            {
                return _cameraInfos;
            }
            set
            {
                _cameraInfos = value;
                OnPropertyChanged("CameraInfos");
            }
        }

        private string _recipeID;
        public string RecipeID
        {
            get => _recipeID;
            set
            {

                _recipeID = value;
                NewRecipeName = _recipeID;
                OnPropertyChanged("RecipeID");
            }
        }



        private LEDWindow selectLED;

        private List<VisionCameraGroupModel> _visionCameraGroups;
        public List<VisionCameraGroupModel> VisionCameraGroups { get => _visionCameraGroups; set => _visionCameraGroups = value; }


        private ImageGroupModel _imageGroupModel;
        public ImageGroupModel MainImageGroupModel { get => _imageGroupModel; set => _imageGroupModel = value; }

        public System.Windows.Threading.DispatcherTimer timer;



        public string NewRecipeName = "";



        private List<BitmapImage> _cameraImageSourceGroup;
        public List<BitmapImage> CameraImageSourceGroup
        {
            get => _cameraImageSourceGroup;
            set
            {
                _cameraImageSourceGroup = value;
                OnPropertyChanged("CameraImageSourceGroup");
            }
        }

        
        private KeyValuePair<ICameraInfo, string> _selectedCameraInfos;
        public KeyValuePair<ICameraInfo, string> SelectedCameraInfos
        {
            get
            {
                return _selectedCameraInfos;
            }
            set
            {
                _selectedCameraInfos = value;
                
                SelectedCameraID = SelectedCameraInfos.Key;
                


                OnPropertyChanged("SelectedCameraInfos");
            }
        }

        private ICameraInfo _selectedCameraID;
        public ICameraInfo SelectedCameraID { 
            get => _selectedCameraID;
            set
            {
                _selectedCameraID = value;
                OnPropertyChanged("SelectedCameraID");
            } 
        }

        private List<SliderModel> _exposuretimeSliders;
        public List<SliderModel> ExposuretimeSliders 
        { 
            get => _exposuretimeSliders;
            set
            {
                _exposuretimeSliders = value;
                OnPropertyChanged("ExposuretimeSliders");
            }
        }
        

        private List<SliderModel> _gainValueSliders;
        public List<SliderModel> GainValueSliders { 
            get => _gainValueSliders;
            set
            {
                _gainValueSliders = value;
                OnPropertyChanged("GainValueSliders");
            }
        }

        public SideBarViewModel SideBarViewModel { get; }


        private void NavigationChaged(object obj)
        {
            Console.WriteLine("NavigationChaged");
            ResetCameraInfo();
        }


        public SetCameraViewModel(SideBarViewModel sideBarViewModel)
        {
            SideBarViewModel = sideBarViewModel;
            SideBarViewModel.NavigateHomeChanged = NavigationChaged;
            SideBarViewModel.NavigateSetCameraChanged = NavigationChaged;
            SideBarViewModel.NavigateDNNSettingChanged = NavigationChaged;
;
            InitCommands();
            Console.WriteLine("SetCameraViewModel");
            initVariables(5);
            UpdateDeviceList();
            //NavigationStore = navigationStore;
        }
        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        public void initVariables(int numCamera)
        {
            try
            {
                VisionCameraGroups = new List<VisionCameraGroupModel>();

                ExposuretimeSliders = new List<SliderModel>();
                GainValueSliders = new List<SliderModel>();
                DnnFileInfo = new List<string>();
                selectLED = new LEDWindow();
                CameraIDInfo = new List<string>();

                CameraImageSourceGroup = new List<BitmapImage>();

                for (int i = 0; i < numCamera; i++)
                {
                    VisionCameraGroups.Add(new VisionCameraGroupModel(1));
                    VisionCameraGroups[i].CheckFileCamera = false;
                    VisionCameraGroups[i].ImageCapturedSignal += OnImageReady;
                    VisionCameraGroups[i].SetCamID(i);

                    CameraIDInfo.Add("no");

                    CameraImageSourceGroup.Add(new BitmapImage());
                    ExposuretimeSliders.Add(new SliderModel(true));
                    GainValueSliders.Add(new SliderModel(false));
                    DnnFileInfo.Add("0");
                }


                ExposuretimeSliders = ExposuretimeSliders;
                GainValueSliders = GainValueSliders;

                DBAcess.CreateDB();
                //timer = new DispatcherTimer();

                //for (int i = 0; i < VisionCameraGroup.GetIVisionCameraList().Count(); i++)
                //{
                //    //timer.Tick += VisionCameraGroup.GetIVisionCameraList()[i].GiveImageFile;
                //}
                //timer.Interval = TimeSpan.FromMilliseconds(1000);

                UpdateSavedInfosLis();
                MainImageGroupModel = new ImageGroupModel(numCamera);
                MainImageGroupModel.CreateImageInfoModel();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " initVariables");
            }
            
        }


        public void InitCommands()
        {
            try
            {
                SetCameraBtn = new Command(SetCameraBtnRun, CanExecute_func);
                
                SetAllBtn = new Command(SetAllBtnRun, CanExecute_func);
                StartAllBtn = new Command(StartAllBtnRun, CanExecute_func);
                ResetRecipeBtn = new Command(ResetRecipeBtnRun, CanExecute_func);
                DeleteRecipeBtn = new Command(DeleteRecipeBtnRun, CanExecute_func);
                SaveRecipeBtn = new Command(SaveRecipeBtnRun, CanExecute_func);
                FullScreenModeBtn = new Command(FullScreenModeBtnRun, CanExecute_func);
                SetDNNBtn = new Command(SetDNNButtonRun, CanExecute_func);
                SetLEDBtn = new Command(SetLEDBtnRun, CanExecute_func);
            }
            catch (Exception exception)

            {
                ShowException(exception);
            }
        }


        public void UpdateSavedInfosLis()
        {
            try
            {
                SavedInfosList = DBAcess.CamInfos();

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateCameraList");
                ShowException(ex);
            }
        }

        private bool CanExecute_func(object obj)
        {
            return true;
        }


        #region Btn Command Func

        private void SetCameraBtnRun(object sender)
        {
            try
            {
                //Console.WriteLine("SetCameraBtnRun");
                string buttonId = sender as string;
                int btnNumber = Int32.Parse(buttonId);
                SettingSelectedCameraToDevice(btnNumber);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + "SetCameraBtnRun");
            }
        }
       
        private void SetAllBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("SetAllBtnRun");
                OpenCameraAll();


            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + "SetAllBtnRun");
            }
        }

        private void StartAllBtnRun(object obj)
        {
            try
            {
                //Console.WriteLine("StartAllBtnRun");
                for (int i = 0; i < VisionCameraGroups.Count; i++)
                {
                    VisionCameraGroups[i].StartGrabbingContinousCameraAll();
                }
                //timer.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + "StartAllBtnRun");
            }
        }
        
        private void ResetRecipeBtnRun(object obj)
        {
            try
            {
                RecipeID = "";
                ResetCameraInfo();
                Console.WriteLine("ResetRecipeBtnRun");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + "ResetRecipeBtnRun");
            }
        }
        
        private void DeleteRecipeBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("DeleteRecipeBtnRun");
                DBAcess.DeleteData(NewRecipeName);
                DBAcess.DeleteDataExTime(NewRecipeName);
                DBAcess.DeleteDataLEDVal(NewRecipeName);
                DBAcess.DeleteDataCropInfo(NewRecipeName);
                DBAcess.DeleteDataDNNFilePath(NewRecipeName);
                UpdateDeviceList();
                UpdateSavedInfosLis();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception+ "DeleteRecipeBtnRun");
            }
        }        
        
        private void SaveRecipeBtnRun(object obj)
        {
            try
            {
                
                List<string> listCropValues = new List<string>();
                List<string> dnnFileInfo = new List<string>();
                List<string> camExtimeInfo = new List<string>();
                List<string> LEDVal = new List<string>();


                if (NewRecipeName == "")
                {
                    System.Windows.Forms.MessageBox.Show("Please enter a Key Value");
                    return;
                }

                // 카메라 
                for (int i = 0; i < ExposuretimeSliders.Count; i++)
                {
                    camExtimeInfo.Add(ExposuretimeSliders[i].SliderValue.ToString());
                }

                LEDVal.Add(selectLED.redVal.ToString());
                LEDVal.Add(selectLED.greenVal.ToString());
                LEDVal.Add(selectLED.blueVal.ToString());
                LEDVal.Add(selectLED.whiteVal.ToString());

                //dnnFileInfo = DBAcess.GiveDNNFileSetting(NewRecipeName);


                if (SavedInfosList.Contains(NewRecipeName))
                {
                    ///////////////// DNN //////////////////////
                    dnnFileInfo = DBAcess.GiveDNNFileSetting(NewRecipeName);
                    CameraIDInfo = DBAcess.GiveCamSettings(NewRecipeName);
                    for (int i = 0; i < DnnFileInfo.Count; i++)
                    {
                        if (DnnFileInfo[i] != "0")
                        {
                            dnnFileInfo[i] = DnnFileInfo[i];
                        }
                    }

                    //////////////////// Crop /////////////////////
                    listCropValues = DBAcess.GiveCropInfoSetting(NewRecipeName);

                    //if (FirstBlock.PreviewImage.GetCropValToSave() != "null")
                    //{
                    //    listCropValues[0] = FirstBlock.PreviewImage.GetCropValToSave();
                    //}
                    //if (SecondBlock.PreviewImage.GetCropValToSave() != "null")
                    //{
                    //    listCropValues[1] = SecondBlock.PreviewImage.GetCropValToSave();
                    //}
                    //if (ThirdBlock.PreviewImage.GetCropValToSave() != "null")
                    //{
                    //    listCropValues[2] = ThirdBlock.PreviewImage.GetCropValToSave();
                    //}
                    //if (FourthBlock.PreviewImage.GetCropValToSave() != "null")
                    //{
                    //    listCropValues[3] = FourthBlock.PreviewImage.GetCropValToSave();
                    //}
                }
                else
                {
                    ///////////////// DNN /////////////////
                    
                    for(int i = 0; i < DnnFileInfo.Count; i++)
                    {
                        dnnFileInfo.Add(DnnFileInfo[i]);
                    }

                    ///////////////// Crop ////////////////
                    //listCropValues.Add(FirstBlock.PreviewImage.GetCropValToSave());
                    //listCropValues.Add(SecondBlock.PreviewImage.GetCropValToSave());
                    //listCropValues.Add(ThirdBlock.PreviewImage.GetCropValToSave());
                    //listCropValues.Add(FourthBlock.PreviewImage.GetCropValToSave());
                }





                if (SavedInfosList.Contains(NewRecipeName))
                {
                    DBAcess.UpdateDataBase(NewRecipeName, CameraIDInfo);
                    DBAcess.UpdateDataBaseExTime(NewRecipeName, camExtimeInfo);
                    if (selectLED.LEDInfoSaved)
                    {
                        DBAcess.UpdateDataBaseLEDVal(NewRecipeName, LEDVal);
                    }
                    if (DnnFileInfo[0] != "0")
                    {
                        DBAcess.UpdateDataBaseDNNFilePath(NewRecipeName, DnnFileInfo);
                    }
                    //DBAcess.UpdateDataBaseCropInfo(NewRecipeName, listCropValues);

                }
                else
                {
                    DBAcess.InsertCamera(NewRecipeName, CameraIDInfo);
                    DBAcess.InsertExTime(NewRecipeName, camExtimeInfo);
                    DBAcess.InsertLEDVal(NewRecipeName, LEDVal);
                    DBAcess.InsertDNNFilePath(NewRecipeName, dnnFileInfo);
                    //DBAcess.InsertCropInfo(NewRecipeName, listCropValues);
                }


                ResetCameraInfo();

                //NavigateHomeViewCommand.Execute(obj);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + " SaveRecipeBtnRun");
            }
        }


        private void ResetCameraInfo()
        {
            try
            {
                for (int i = 0; i < CameraIDInfo.Count; i++)
                {
                    CameraIDInfo[i] = "no";
                }
                for (int i = 0; i < VisionCameraGroups.Count; i++)
                {
                    VisionCameraGroups[i].IVisionCameraGroup[0].StopGrabbing();
                    VisionCameraGroups[i].IVisionCameraGroup[0].CloseCamera();
                    VisionCameraGroups[i].IVisionCameraGroup[0].DestroyCamera();
                }
                for (int i = 0; i < ExposuretimeSliders.Count; i++)
                {

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " ResetCameraInfo");
            }
        }

        private void FullScreenModeBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("FullScreenModeBtnRun");
                for (int i = 0; i < VisionCameraGroups.Count; i++)
                {
                    VisionCameraGroups[i].IVisionCameraGroup[0].StopGrabbing();
                    VisionCameraGroups[i].IVisionCameraGroup[0].CloseCamera();
                    VisionCameraGroups[i].IVisionCameraGroup[0].DestroyCamera();
                }
                //NavigateHomeViewCommand.Execute(obj);
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception + "FullScreenModeBtnRun");
            }
        }
        #endregion

        private void OnImageReady(Object sender, ImageCapturedEventArgs e)
        {
            try
            {
                UpdateCameraImageSource(e.cameraNumber);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " OnImageReady");
            }
        }

        private void UpdateCameraImageSource(int viewNum)
        {
            try
            {
                foreach (IVisionCamera visionCamera in VisionCameraGroups[viewNum].GetIVisionCameraList())
                {
                    var temp = visionCamera.GetLatestFrame().CapturedBitmapImage;
                    if (temp == null)
                    {
                        continue;
                    }
                    CameraImageSourceGroup[viewNum] = ToBitmapImage(temp);
                }
                UpdateViewGroupSource(CameraImageSourceGroup);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " OnImageReady Viewmodel");
            }
        }

        private void UpdateViewGroupSource(List<BitmapImage> tempCameraImageSourceGroup)
        {
            CameraImageSourceGroup = tempCameraImageSourceGroup;
        }

        #region Bitmap 형식을 BitmapImage 로 변환 
        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            try
            {
                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;
                    //memory.Seek(0, SeekOrigin.Begin);
                    //var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    //temp = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "toBitmap");
            }
            return bitmapImage;
        }
        #endregion


        private void SetDNNButtonRun(object sender)
        {

            try
            {

                string buttonId = sender as string;

                //Console.WriteLine(buttonId);
                int btnNumber = Int32.Parse(buttonId);

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "DNN Files (*.dnn)|*.dnn";


                // Display OpenFileDialog by calling ShowDialog method 
                var result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    DnnFileInfo[btnNumber] = dlg.FileName;
                    //DNNPathTxt.Text = filename;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

        }
        private void SetLEDBtnRun(object sender)
        {

            try
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
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " SetLEDBtnRun");
            }
            

        }

        public void UpdateDeviceList()
        {
            Logger.Debug("Update Device List");

            try
            {
                List<ICameraInfo> cameraInfos = CameraFinder.Enumerate();

                if (cameraInfos.Count > 0)
                {
                    CameraInfos = new Dictionary<ICameraInfo, String>();
                    foreach (ICameraInfo cameraInfo in cameraInfos)
                    {
                        CameraInfos.Add(cameraInfo, cameraInfo[CameraInfoKey.FriendlyName]);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + " UpdateDeviceList MainW");

            }
        }

        private void OpenCameraAll()
        {
            //UpdateDeviceList();

            try
            {
                List<string> extimeSetting = DBAcess.GiveExTimeSetting(RecipeID);

                for (int i = 0; i < VisionCameraGroups.Count; i++)
                {
                    VisionCameraGroups[i].UpdateDeviceList();
                    VisionCameraGroups[i].OpenCamera(RecipeID);
                    ExposuretimeSliders[i].BindParametersToControls(VisionCameraGroups[i].IVisionCameraGroup[0]);
                    ExposuretimeSliders[i].SliderValue = Convert.ToDouble(extimeSetting[i]);
                    GainValueSliders[i].BindParametersToControls(VisionCameraGroups[i].IVisionCameraGroup[0]);
                    VisionCameraGroups[i].IVisionCameraGroup[0].SetSettingMode();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " OpenCameraAll");
            }
        }

        private void SettingSelectedCameraToDevice(int DeviceNum)
        {
            try
            {
                if (VisionCameraGroups[DeviceNum].IVisionCameraGroup[0] != null && !VisionCameraGroups[DeviceNum].IVisionCameraGroup[0].IsOpened())
                {
                    CameraIDInfo[DeviceNum] = SelectedCameraInfos.Value;
                    VisionCameraGroups[DeviceNum].IVisionCameraGroup[0].CreateByCameraInfo(SelectedCameraID);
                    VisionCameraGroups[DeviceNum].IVisionCameraGroup[0].OpenCamera();
                    ExposuretimeSliders[DeviceNum].BindParametersToControls(VisionCameraGroups[DeviceNum].IVisionCameraGroup[0]);
                    GainValueSliders[DeviceNum].BindParametersToControls(VisionCameraGroups[DeviceNum].IVisionCameraGroup[0]);
                    VisionCameraGroups[DeviceNum].IVisionCameraGroup[0].SetSettingMode();
                    VisionCameraGroups[DeviceNum].IVisionCameraGroup[0].StartContinuousShotGrabbing();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " StartButton_Click_SelectCamWindow");
                Console.WriteLine(ex.Message + " StartButton_Click_SelectCamWindow");
            }
        }



        private void MainWindow_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (camera.GetMainMode()) return;

            StartedPaint = true;

            //DownPoint = e.GetPosition(ImageGrid);
        }

        private void MainWindow_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (camera.GetMainMode()) return;

            //UpPoint = e.GetPosition(ImageGrid);
            StartedPaint = false;
        }

        private void MainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //if (camera.GetMainMode()) return;
            //if (StartedPaint)
            //{

            //    var point = e.GetPosition(ImageGrid);

            //    if (point.X < DownPoint.X || point.Y < DownPoint.Y)
            //    {
            //        return;
            //    }

            //    var rect = new System.Windows.Rect(DownPoint, point);

            //    //CropRectangle.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
            //    //CropRectangle.Width = rect.Width;
            //    //CropRectangle.Height = rect.Height;
            //    //CropRectangle.Visibility = Visibility.Visible;
            //    if (rect.Width < 100 || rect.Height < 100)
            //    {
            //       // CropRectangle.Visibility = Visibility.Hidden;
            //        return;
            //    }

            //    double temp = 390.0 / 1200.0 * 1600.0;
            //    PosX = DownPoint.X / Math.Round(temp, 6);
            //    //PosY = DownPoint.Y / Math.Round(PreviewImage.Height, 6);
            //    Width = Math.Round(rect.Width, 3) / Math.Round(temp, 6);
            //    //Height = Math.Round(rect.Height, 3) / Math.Round(PreviewImage.Height, 6);
            //}
        }

        public string GetCropValToSave()
        {
            string listCropVal = "null";
            try
            {
                if (PosX + PosY + Width + Height > 0)
                {
                    listCropVal = string.Format("{0},{1},{2},{3}", Math.Round(PosX, 6), Math.Round(PosY, 6), Math.Round(Width, 6), Math.Round(Height, 6));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " GetCropValToSave");
            }
            
            return listCropVal;
        }

        public void SetCropVal(string sCropInfo)
        {
            try
            {
                string[] collectionCropInfo = sCropInfo.Split(',');
                PosX = (int)(float.Parse(collectionCropInfo[0]) * 1600);
                PosY = (int)(float.Parse(collectionCropInfo[1]) * 1200);
                Width = (int)(float.Parse(collectionCropInfo[2]) * 1600);
                Height = (int)(float.Parse(collectionCropInfo[3]) * 1200);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " SetCropVal");
            }

        }
    }






}

