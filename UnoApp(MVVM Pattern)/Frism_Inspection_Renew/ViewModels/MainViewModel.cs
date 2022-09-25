using Basler.Pylon;
using Frism_Inspection_Renew.Commands;
using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using Frism_Inspection_Renew.Views;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;

namespace Frism_Inspection_Renew.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        
        private MainModel mainModel;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        
        private Dictionary<ICameraInfo, String> _cameraInfos;
        public Dictionary<ICameraInfo, string> CameraInfos { get
            {
                return _cameraInfos;
            } 
            set
            {
                _cameraInfos = value;
                OnPropertyChanged("CameraInfos");
            }
        }

        /// <summary>
        /// Main Window Commands
        /// </summary>
        public ICommand SetCameraBtn { get; set; }
        public ICommand InitializeBtn { get; set; }
        public ICommand StopInspectBtn { get; set; }
        public ICommand StartInspectBtn { get; set; }
        public ICommand ShowHistoryBtn { get; set; }
        public ICommand InspectionSettingBtn { get; set; }
        public ICommand InspectCountResetBtn { get; set; }
        
        public String outputTxt { get; set; }


        /// <summary>
        /// Selection Window Commands
        /// </summary>
        public ICommand StartSingleCameraSelectionWidonBtn { get; set; }

        private ObservableCollection<string> _savedInfosList = null;
        public ObservableCollection<string> SavedInfosList { get => _savedInfosList; set => _savedInfosList = value; }

        //private BlockingCollection<bool> _bufferForIOBlow;
        //public BlockingCollection<bool> BufferForIOBlow { get => _bufferForIOBlow; set => _bufferForIOBlow = value; }

        //private IOSaveModel _saveIOInfo;
        //public  IOSaveModel SaveIOInfo { get => _saveIOInfo; set => _saveIOInfo = value; }

        private VisionCameraGroupModel _visionCameraGroup;
        public VisionCameraGroupModel VisionCameraGroup { get => _visionCameraGroup; set => _visionCameraGroup = value; }

        private CustomSliderModel _customSlider;
        public CustomSliderModel CustomSlider { get => _customSlider; set => _customSlider = value; }
              

        private ImageGroupModel _imageGroupModel;
        public ImageGroupModel MainImageGroupModel { get => _imageGroupModel; set => _imageGroupModel = value; }

        private InferenceModel _inferenceModel;
        public InferenceModel MainInferenceModel { get => _inferenceModel; set => _inferenceModel = value; }

        private BlockingCollection<int> checkCamIDList;

        private IOControl _iOControlModel;
        public IOControl IOControlModel { get => _iOControlModel; set => _iOControlModel = value; }

        private ImageSaveModel imageSave;
        public ImageSaveModel ImageSave { get => imageSave; set => imageSave = value; }

        private string _optionId;
        public string OptionId { get => _optionId; set => _optionId = value; }

        private int _numCamera;
        public int NumCamera { get => _numCamera; set => _numCamera = value; }

        private List<string> _dnnFileInfo;
        public List<string> DnnFileInfo { get => _dnnFileInfo; set => _dnnFileInfo = value; }

        private List<string> _updatedDnnFileInfo;
        public List<string> UpdatedDnnFileInfo { get => _updatedDnnFileInfo; set => _updatedDnnFileInfo = value; }

        private List<string> _cameraIDInfo;
        public List<string> CameraIDInfo { get => _cameraIDInfo; set => _cameraIDInfo = value; }

        private List<string> _camExtimeInfo;
        public List<string> CamExtimeInfo { get => _camExtimeInfo; set => _camExtimeInfo = value; }

        private string _firstCamExtimeValue;
        public string FirstCamExtimeValue { 
            get => _firstCamExtimeValue;
            set {
                _firstCamExtimeValue = value;
                OnPropertyChanged("FirstCamExtimeValue");
            }

        }

        private string _secondCamExtimeValue;
        public string SecondCamExtimeValue { 
            get => _secondCamExtimeValue;
            set
            {
                _secondCamExtimeValue = value;
                OnPropertyChanged("SecondCamExtimeValue");
            }
        }

        private string _thirdCamExtimeValue;
        public string ThirdCamExtimeValue { 
            get => _thirdCamExtimeValue;
            set
            {
                _thirdCamExtimeValue = value;
                OnPropertyChanged("ThirdCamExtimeValue");
            }
        }

        private string _fourthCamExtimeValue;
        public string FourthCamExtimeValue { 
            get => _fourthCamExtimeValue;
            set
            {
                _fourthCamExtimeValue = value;
                OnPropertyChanged("FourthCamExtimeValue");
            }
        }

        private string _firstCameraIDValue;
        public string FirstCameraIDValue { 
            get => _firstCameraIDValue;
            set
            {
                _firstCameraIDValue = value;
                OnPropertyChanged("FirstCameraIDValue");
            }
        }

        private string _secondCameraIDValue;
        public string SecondCameraIDValue { 
            get => _secondCameraIDValue;
            set
            {
                _secondCameraIDValue = value;
                OnPropertyChanged("SecondCameraIDValue");
            }
        }

        private string _thirdCameraIDValue;
        public string ThirdCameraIDValue { 
            get => _thirdCameraIDValue;
            set
            {
                _thirdCameraIDValue = value;
                OnPropertyChanged("ThirdCameraIDValue");
            }
        }

        private string _fourthCameraIDValue;
        public string FourthCameraIDValue { 
            get => _fourthCameraIDValue;
            set
            {
                _fourthCameraIDValue = value;
                OnPropertyChanged("FourthCameraIDValue");
            }
        }

        private string _firstExposureTime;
        public string FirstExposureTime { 
            get => _firstExposureTime;
            set
            {
                _firstExposureTime = value;
                OnPropertyChanged("FirstExposureTime");
            }
        }

        private string _secondExposureTime;
        public string SecondExposureTime { 
            get => _secondExposureTime;
            set
            {
                _secondExposureTime = value;
                OnPropertyChanged("SecondExposureTime");
            }
        }

        private string _thirdExposureTime;
        public string ThirdExposureTime { 
            get => _thirdExposureTime;
            set
            {
                _thirdExposureTime = value;
                OnPropertyChanged("ThirdExposureTime");
            }
        }

        private string _fourthExposureTime;
        public string FourthExposureTime { 
            get => _fourthExposureTime;
            set
            {
                _fourthExposureTime = value;
                OnPropertyChanged("FourthExposureTime");
            }

        }

        private string _recipeID;
        public string RecipeID { 
            get => _recipeID;
            set
            {
                Console.WriteLine("pppp");
                _recipeID = value;
                OnPropertyChanged("RecipeID");
            }
        }

        private string _selectedCamereIDComboBox;
        public string SelectedCamereIDComboBox { get => _selectedCamereIDComboBox; set => _selectedCamereIDComboBox = value; }

        private bool _startedPaint;
        public bool StartedPaint { get => _startedPaint; set => _startedPaint = value; }

        private System.Windows.Point _downPoint;
        public System.Windows.Point DownPoint { get => _downPoint; set => _downPoint = value; }

        private System.Windows.Point _upPoint;
        public System.Windows.Point UpPoint { get => _upPoint; set => _upPoint = value; }

        private double _posX = 0;
        public double PosX { 
            get => _posX; 
            set => _posX = value; 
        }

        private double _posY = 0;
        public double PosY { 
            get => _posY; 
            set => _posY = value; 
        }

        private double _width = 0;
        public double Width { 
            get => _width; 
            set => _width = value; 
        }

        private double _height = 0;
        public double Height { 
            get => _height; 
            set => _height = value; 
        }


        public System.Windows.Threading.DispatcherTimer timer;

        public ICommand NavigateSetCameraCommand { get; }

        public String OutputTxt
        {
            get
            {
                return outputTxt;
            }
            set
            {
                outputTxt = value;
                OnPropertyChanged("OutputTxt");
            }
        }

        private BitmapImage _firstCameraImageSource = new BitmapImage();
        public BitmapImage FirstCameraImageSource
        {
            get { return _firstCameraImageSource; }
            set
            {
                this._firstCameraImageSource = value;
                OnPropertyChanged("FirstCameraImageSource");
            }
        }

        private BitmapImage _secondCameraImageSource = new BitmapImage();
        public BitmapImage SecondCameraImageSource
        {
            get { return _secondCameraImageSource; }
            set
            {
                this._secondCameraImageSource = value;
                OnPropertyChanged("SecondCameraImageSource");
            }
        }

        private BitmapImage _thridCameraImageSource = new BitmapImage();
        public BitmapImage ThirdCameraImageSource
        {
            get { return _thridCameraImageSource; }
            set
            {
                this._thridCameraImageSource = value;
                OnPropertyChanged("ThirdCameraImageSource");
            }
        }


        private BitmapImage _fourthCameraImageSource = new BitmapImage();
        public BitmapImage FourthCameraImageSource
        {
            get { return _fourthCameraImageSource; }
            set
            {
                this._fourthCameraImageSource = value;
                OnPropertyChanged("FourthCameraImageSource");
            }
        }



        private List<BitmapImage> _cameraImageSourceGroup = new List<BitmapImage>();
        public List<BitmapImage> CameraImageSourceGroup { get => _cameraImageSourceGroup; set => _cameraImageSourceGroup = value; }





        public MainViewModel(NavigationStore navigationStore)
        {
            NavigateSetCameraCommand = new NavigateCommand<SetCameraViewModel>(new NavigationService<SetCameraViewModel>(navigationStore, () => new SetCameraViewModel(navigationStore)));
            NumCamera = 4;
            InitVariables(NumCamera);
            InitCommands();
            
        }

        public void InitVariables(int numCamera)
        {
            try
            {
                mainModel = new MainModel();
                VisionCameraGroup = new VisionCameraGroupModel(numCamera);

                MainImageGroupModel = new ImageGroupModel(numCamera);
                checkCamIDList = new BlockingCollection<int>();
                IOControlModel = new IOControl();
                ImageSave = new ImageSaveModel();
                CustomSlider = new CustomSliderModel();

                VisionCameraGroup.ImageCapturedSignal += OnImageReady;
                MainInferenceModel = new InferenceModel();
                MainInferenceModel.InitThread(numCamera);

                CameraImageSourceGroup.Add(FirstCameraImageSource);
                CameraImageSourceGroup.Add(SecondCameraImageSource);
                CameraImageSourceGroup.Add(ThirdCameraImageSource);
                CameraImageSourceGroup.Add(FourthCameraImageSource);

                timer = new DispatcherTimer();
                for(int i = 0; i < VisionCameraGroup.GetIVisionCameraList().Count(); i++)
                {
                    timer.Tick += VisionCameraGroup.GetIVisionCameraList()[i].GiveImageFile;
                    checkCamIDList.Add(0);
                }
                timer.Interval = TimeSpan.FromMilliseconds(1000);

                IOControlModel.setBlowDevice();

                DBAcess.CreateDB();

                SavedInfosList = DBAcess.CamInfos();

                MainImageGroupModel.CreateImageInfoModel();
                MainImageGroupModel.SetDnnInfo();

                UpdatedDnnFileInfo = new List<string>(new string[numCamera]);
                CameraIDInfo = new List<string>(new string[numCamera]);
                CamExtimeInfo = new List<string>(new string[numCamera]);


                StartSavingImage();
                UpdateDeviceList();
                StartIOSig();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void InitCommands()
        {
            try
            {
                SetCameraBtn = new Command(SetCameraBtnRun, CanExecute_func);
                InitializeBtn = new Command(InitializeBtnRun, CanExecute_func);
                StopInspectBtn = new Command(StopInspectBtnRun, CanExecute_func);
                StartInspectBtn = new Command(StartInspectBtnRun, CanExecute_func);
                ShowHistoryBtn = new Command(ShowHistoryBtnRun, CanExecute_func);
                InspectionSettingBtn = new Command(InspectionSettingBtnRun, CanExecute_func);
                InspectCountResetBtn = new Command(InspectCountResetBtnRun, CanExecute_func);
            }
            catch (Exception exception)
            
            {
                ShowException(exception);
            }
        }

        private bool CanExecute_func(object obj)
        {
            return true;
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetCameraBtnRun(object obj)
        {
            // call  mainModel.SetCamera to open
            Console.WriteLine("SetCameraBtnRun");
        }

        private void InitializeBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaa");
                OpenCamera();
                Console.WriteLine(MainImageGroupModel.GetImageInfoModels().Count);

                Console.WriteLine(RecipeID);
                MainImageGroupModel.UpdateDnnFiles(DBAcess.GiveDNNFileSetting(RecipeID));
                MainImageGroupModel.UpdateCropInfoSetting(RecipeID);

                MainInferenceModel.SettingDnnFirst(MainImageGroupModel);

                Console.WriteLine("InitializeBtnRun");
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
            }

        }        

        private void StopInspectBtnRun(object obj)
        {
            timer.Stop();
            Console.WriteLine("StopInspectBtnRun");
        }

        private void StartInspectBtnRun(object obj)
        {
            timer.Start();
            Console.WriteLine("Insp Start");
        }

        private void ShowHistoryBtnRun(object obj)
        {
            Console.WriteLine("ShowHistoryBtnRun");
        } 

        private void InspectionSettingBtnRun(object obj)
        {
            Console.WriteLine("InspectionSettingBtnRun");
        }
        
        private void InspectCountResetBtnRun(object obj)
        {
            Console.WriteLine("InspectCountResetBtnRun");
        }



        private void SettingRecipe(object obj)
        {

        }

        private void SettingDNNFolderPath(object obj)
        {
            GetDNNFromFolder(1);
        }

        private void InitRecipe(string RecipeID)
        {
            try
            {
                VisionCameraGroup.ResetCameraAll();

                ResetCameraInfo();
                CameraIDInfo = DBAcess.GiveCamSettings(RecipeID);
                CamExtimeInfo = DBAcess.GiveExTimeSetting(RecipeID);
                //LEDVal = DBAcess.GiveLEDValSetting(CamID);
                DnnFileInfo = DBAcess.GiveDNNFileSetting(RecipeID);


                //UpdateDnnFiles(DnnFilePath);

                VisionCameraGroup.UpdateCamera(RecipeID, CameraIDInfo, CamExtimeInfo);

                //UpdateLED(LEDVal);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " SetBtn_Click");
            }
        }

        private void SettingRecipe(string OptionId, List<string> savedInfos, List<string> camExposuretimeInfo, List<string> LEDVal)
        {
            if (OptionId == "")
            {
                System.Windows.Forms.MessageBox.Show("Please enter a Key Value");
                return;
            }

            List<string> listCropValues = new List<string>();
            if (savedInfos.Contains(OptionId))
            {
                ///////////// DNN //////////////////////

                DnnFileInfo = DBAcess.GiveDNNFileSetting(OptionId);

                for (int i = 0; i < UpdatedDnnFileInfo.Count(); i++)
                {
                    if (UpdatedDnnFileInfo[i] != null)
                    {
                        DnnFileInfo[i] = UpdatedDnnFileInfo[i];
                    }
                }

                //////////////// Crop /////////////////////
                //listCropValues = DBAcess.GiveCropInfoSetting(OptionId);
                
                //if (FirstBlock.PreviewImage.GetCropValToSave() != "null")
                //{
                //    listCropValues[0] = FirstBlock.PreviewImage.GetCropValToSave();
                //}
            }
            else
            {
                ///////////// Crop ////////////////
                //listCropValues.Add(FirstBlock.PreviewImage.GetCropValToSave());
            }

            if (savedInfos.Contains(OptionId))
            {
                DBAcess.UpdateDataBase(OptionId, CameraIDInfo); 
                DBAcess.UpdateDataBaseExTime(OptionId, CamExtimeInfo);

                DBAcess.UpdateDataBaseLEDVal(OptionId, LEDVal);

                if (DnnFileInfo[0] != "0")
                {
                    DBAcess.UpdateDataBaseDNNFilePath(OptionId, DnnFileInfo);
                }
                DBAcess.UpdateDataBaseCropInfo(OptionId, listCropValues);
            }
            else
            {
                DBAcess.InsertCamera(OptionId, CameraIDInfo);
                DBAcess.InsertExTime(OptionId, CamExtimeInfo);
                DBAcess.InsertLEDVal(OptionId, LEDVal);
                DBAcess.InsertDNNFilePath(OptionId, DnnFileInfo);
                DBAcess.InsertCropInfo(OptionId, listCropValues);
            }

            ResetCameraInfo();

        }

        private void ResetCameraInfo()
        {




        }

        private string GetDNNFromFolder(int pos)
        {
            string filename = "0";

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "DNN Files (*.dnn)|*.dnn";

                // Display OpenFileDialog by calling ShowDialog method 
                var result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    filename = dlg.FileName;

                }

                UpdatedDnnFileInfo[pos] = filename;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return filename;

        }

        private void OnImageReady(Object sender, EventArgs e)
        {
            
            int count = 0;
            Console.WriteLine("ImageReady");
            foreach (IVisionCamera visionCamera in VisionCameraGroup.GetIVisionCameraList())
            {
                MainImageGroupModel.GetImageInfoModels()[count].BitmapRawImage = visionCamera.GetLatestFrame().CapturedBitmapImage;
                CameraImageSourceGroup[count] = ToBitmapImage(visionCamera.GetLatestFrame().CapturedBitmapImage);
                //MainImageGroupModel.GetImageInfoModels()[count].BitmapRawImage.Save("D:/TB/Image/" + count + ".bmp", ImageFormat.Bmp);
                count++;
            }
            if(count == 4)
            {
                Console.WriteLine("ImageReady");
                MainImageGroupModel = MainInferenceModel.InferDLL(MainImageGroupModel);
                IOSaveModel.AddSignalValue(MainImageGroupModel.InferResultNG);
                IOSaveModel.AddImageToBuffer(MainImageGroupModel);
            }
        }
        
        #region Bitmap 형식을 BitmapImage 로 변환 
        public BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            BitmapImage temp = new BitmapImage();
            try
            {

                using (var memory = new MemoryStream())
                {

                    bitmap.Save(memory, ImageFormat.Bmp);

                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    temp = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + "toBitmap");
            }
            return temp;
        }
        #endregion

        public void StartIOSig()
        {
            Thread tSignal = new Thread(new ThreadStart(IOControlModel.IOBlowSig));
            tSignal.Start();
        } 

        public void StartSavingImage()
        {
            Thread tSaveImage = new Thread(new ThreadStart(ImageSave.SaveImageThread));
            tSaveImage.Start();
        }

        private void OpenCamera()
        {
            VisionCameraGroup.OpenCamera(RecipeID);
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
            if (PosX + PosY + Width + Height > 0)
            {
                listCropVal = string.Format("{0},{1},{2},{3}", Math.Round(PosX, 6), Math.Round(PosY, 6), Math.Round(Width, 6), Math.Round(Height, 6));
            }

            return listCropVal;
        }

        public void SetCropVal(string sCropInfo)
        {
            string[] collectionCropInfo = sCropInfo.Split(',');
            PosX = (int)(float.Parse(collectionCropInfo[0]) * 1600);
            PosY = (int)(float.Parse(collectionCropInfo[1]) * 1200);
            Width = (int)(float.Parse(collectionCropInfo[2]) * 1600);
            Height = (int)(float.Parse(collectionCropInfo[3]) * 1200);
        }
    }
}


//< Window
//    x: Class = "WpfCombo.MainWindow"
//    xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
//    xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
//    xmlns: b = "http://schemas.microsoft.com/xaml/behaviors"
//    xmlns: d = "http://schemas.microsoft.com/expression/blend/2008"
//    xmlns: local = "clr-namespace:WpfCombo"
//    xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
//    Title = "MainWindow"
//    Width = "800"
//    Height = "450"
//    mc: Ignorable = "d" >
 
//     < Window.DataContext >
 
//         < local:MainWindowViewModel />
  
//      </ Window.DataContext >
  
//      < Grid >
  
//          < StackPanel HorizontalAlignment = "Center" VerticalAlignment = "Center" >
     
//                 < TextBlock FontSize = "30" Text = "ComboBox Sample" />
        
//                    < ComboBox ItemsSource = "{Binding EnumStrings}" >
         
//                         < b:Interaction.Triggers >
          
//                              < b:EventTrigger EventName = "SelectionChanged" >
           
//                                   < b:InvokeCommandAction Command = "{Binding SelectionChangedCommand}" PassEventArgsToCommand="True" />
//                    </b:EventTrigger >
//                </ b:Interaction.Triggers >
 
//             </ ComboBox >
 
//         </ StackPanel >
 
//     </ Grid >
// </ Window >