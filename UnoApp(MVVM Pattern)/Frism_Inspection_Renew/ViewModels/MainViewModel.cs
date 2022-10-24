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
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight.Command;

namespace Frism_Inspection_Renew.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private MainModel mainModel;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        

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
        private ICommand _windowCloseCommand;
        public ICommand WindowCloseCommand {
            get
            {
               
                return _windowCloseCommand;
            }
            set
            {
                
                _windowCloseCommand = value;
                OnPropertyChanged("WindowCloseCommand");
            }
        }

        public String outputTxt { get; set; }


        /// <summary>
        /// Selection Window Commands
        /// </summary>
        public ICommand StartSingleCameraSelectionWidonBtn { get; set; }

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

        //private BlockingCollection<bool> _bufferForIOBlow;
        //public BlockingCollection<bool> BufferForIOBlow { get => _bufferForIOBlow; set => _bufferForIOBlow = value; }

        //private IOSaveModel _saveIOInfo;
        //public  IOSaveModel SaveIOInfo { get => _saveIOInfo; set => _saveIOInfo = value; }

        private VisionCameraGroupModel _visionCameraGroup;
        public VisionCameraGroupModel VisionCameraGroup { get => _visionCameraGroup; set => _visionCameraGroup = value; }

        private VisionCameraGroupModel _visionCameraGroupSecond;
        public VisionCameraGroupModel VisionCameraGroupSecond { get => _visionCameraGroupSecond; set => _visionCameraGroupSecond = value; }


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


        private string _recipeID;
        public string RecipeID
        {
            get => _recipeID;
            set
            {

                _recipeID = value;
                OnPropertyChanged("RecipeID");
            }
        }

        private string _selectedCamereIDComboBox;
        public string SelectedCamereIDComboBox { get => _selectedCamereIDComboBox; set => _selectedCamereIDComboBox = value; }

        private bool _stopInspecBtnEnable = false;
        public bool StopInspecBtnEnable
        {
            get => _stopInspecBtnEnable;
            set
            {

                _stopInspecBtnEnable = value;
                OnPropertyChanged("StopInspecBtnEnable");
            }
        }


        private bool _startInspecBtnEnable = false;
        public bool StartInspecBtnEnable
        {
            get => _startInspecBtnEnable;
            set
            {

                _startInspecBtnEnable = value;
                OnPropertyChanged("StartInspecBtnEnable");
            }
        }



        public System.Windows.Threading.DispatcherTimer timer;

        public ICommand NavigateSetCameraCommand { get; }
        public ICommand NavigateDNNSettingCommand { get; }

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

        private List<BitmapImage> _cameraImageSourceGroup;
        public List<BitmapImage> CameraImageSourceGroup
        {
            get => _cameraImageSourceGroup;
            set
            {
                _cameraImageSourceGroup = value;
            }
        }


        private BlockingCollection<Bitmap> _cameraImageBitmapQueue;
        public BlockingCollection<Bitmap> CameraImageBitmapQueue { get => _cameraImageBitmapQueue; set => _cameraImageBitmapQueue = value; }


        private List<BitmapImage> _cameraImageSourceGroupSecond;
        public List<BitmapImage> CameraImageSourceGroupSecond
        {
            get => _cameraImageSourceGroupSecond;
            set
            {
                _cameraImageSourceGroupSecond = value;
                OnPropertyChanged("CameraImageSourceGroupSecond");
            }
        }


        private List<BitmapImage> _viewImageSourceGroup;
        public List<BitmapImage> ViewImageSourceGroup
        {
            get => _viewImageSourceGroup;
            set
            {
                _viewImageSourceGroup = value;
                OnPropertyChanged("ViewImageSourceGroup");
            }
        }

        private List<BitmapImage> _viewImageSourceGroupSecond;
        public List<BitmapImage> ViewImageSourceGroupSecond
        {
            get => _viewImageSourceGroupSecond;
            set
            {
                _viewImageSourceGroupSecond = value;
                OnPropertyChanged("ViewImageSourceGroupSecond");
            }
        }

        public SideBarViewModel SideBarViewModel { get; }

        private void NavigationChaged(object obj)
        {
            Console.WriteLine("NavigationChaged");
            VisionCameraGroup.ResetCameraAll();
            VisionCameraGroupSecond.ResetCameraAll();



        }


        public ICommand WindowClosedCommand { get; private set; }



        private void WindowClosed(EventArgs e)
        {
            Console.WriteLine("aaaaaaaaaaa");
        }

        public MainViewModel(SideBarViewModel sideBarViewModel)
        {
            WindowClosedCommand = new RelayCommand<EventArgs>(WindowClosed, (e) => true);



            SideBarViewModel = sideBarViewModel;
            SideBarViewModel.NavigateHomeChanged = SetCameraBtnRun;
            SideBarViewModel.NavigateSetCameraChanged = SetCameraBtnRun;
            SideBarViewModel.NavigateDNNSettingChanged = SetCameraBtnRun;


            NumCamera = 5;
            
            InitVariables(NumCamera);
            InitCommands();

        }

        public void InitVariables(int numCamera)
        {
            try
            {
                mainModel = new MainModel();
                VisionCameraGroup = new VisionCameraGroupModel(3);
                VisionCameraGroup.CheckFileCamera = false;

                VisionCameraGroupSecond = new VisionCameraGroupModel(2);
                VisionCameraGroupSecond.CheckFileCamera = false;

                CameraImageSourceGroup = new List<BitmapImage>();
                CameraImageSourceGroupSecond = new List<BitmapImage>();

                CameraImageBitmapQueue = new BlockingCollection<Bitmap>();

                for (int i = 0; i < numCamera; i++)
                {
                    CameraImageSourceGroup.Add(new BitmapImage());
                    CameraImageSourceGroupSecond.Add(new BitmapImage());    
                }


                MainImageGroupModel = new ImageGroupModel(numCamera);
                checkCamIDList = new BlockingCollection<int>();
                IOControlModel = new IOControl();
                ImageSave = new ImageSaveModel();
                

                VisionCameraGroup.ImageCapturedSignal += OnImageReady;
                VisionCameraGroupSecond.ImageCapturedSignal += OnImageReadySecond;

                MainInferenceModel = new InferenceModel();
                MainInferenceModel.InitThread(numCamera);



                //timer = new DispatcherTimer();
                //for (int i = 0; i < VisionCameraGroup.GetIVisionCameraList().Count(); i++)
                //{
                //    //timer.Tick += VisionCameraGroup.GetIVisionCameraList()[i].GiveImageFile;
                //    checkCamIDList.Add(0);
                //}
                //timer.Interval = TimeSpan.FromMilliseconds(1000);

                IOControlModel.setBlowDevice();



                DBAcess.CreateDB();

                SavedInfosList = DBAcess.CamInfos();

                MainImageGroupModel.CreateImageInfoModel();
                MainImageGroupModel.SetDnnInfo();

                UpdatedDnnFileInfo = new List<string>(new string[numCamera]);
                CameraIDInfo = new List<string>(new string[numCamera]);
                CamExtimeInfo = new List<string>(new string[numCamera]);



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
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
                WindowCloseCommand = new Command(CloseWindow, CanExecute_func);

                // new Command(WindowCloseCommandRun, CanExecute_func);
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


        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetCameraBtnRun(object obj)
        {
            // call  mainModel.SetCamera to open
            //InitRecipe();

            VisionCameraGroup.ResetCameraAll();
            VisionCameraGroupSecond.ResetCameraAll();
            StopSavingImage();
            StopIOSig();


            MainInferenceModel = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //NavigateSetCameraCommand.Execute(obj);  
            Console.WriteLine("SetCameraBtnRun");
        }        
        private void CloseWindow(object obj)
        {
            // call  mainModel.SetCamera to open
            //InitRecipe();
            Console.WriteLine("WindowClosing");
        }

        private void InitializeBtnRun(object obj)
        {
            try
            {
                StartInspecBtnEnable = true;
                //InitRecipe(RecipeID);
                StopSavingImage();
                StopIOSig();
                StartSavingImage();
                StartIOSig();

                //OpenCamera();
                //Console.WriteLine(MainImageGroupModel.GetImageInfoModels().Count);

                //Console.WriteLine(RecipeID);
                MainImageGroupModel.UpdateDnnFiles(DBAcess.GiveDNNFileSetting(RecipeID));
                //MainImageGroupModel.UpdateCropInfoSetting(RecipeID);
                MainInferenceModel.SettingDnnFirst(MainImageGroupModel);

                IOControlModel.blow(100);
                Thread.Sleep(100);
                IOControlModel.blow(100);
                Thread.Sleep(100);
                IOControlModel.blow(100);

                //InitRecipe();

                //Console.WriteLine("InitializeBtnRun");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception + " InitializeBtnRun");
            }
        }

        private void StopInspectBtnRun(object obj)
        {
            //timer.Stop();
            StopInspecBtnEnable = false;
            StartInspecBtnEnable = true;
            VisionCameraGroup.StopGrabbingAll();
            VisionCameraGroupSecond.StopGrabbingAll();



            Console.WriteLine("StopInspectBtnRun");
        }

        private void StartInspectBtnRun(object obj)
        {
            StopInspecBtnEnable = true;
            StartInspecBtnEnable = false;
            VisionCameraGroup.StartGrabbingContinousCameraAll();
            VisionCameraGroupSecond.StartGrabbingContinousCameraAll();

            //timer.Start();
            Console.WriteLine("Insp Start");
        }

        private void ShowHistoryBtnRun(object obj)
        {
            Console.WriteLine("ShowHistoryBtnRun");
        }
        private void InspectionSettingBtnRun(object obj)
        {
            try
            {
                Console.WriteLine("InspectionSettingBtnRun");
                VisionCameraGroup.ResetCameraAll();
                VisionCameraGroupSecond.ResetCameraAll();
                MainInferenceModel = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                StopIOSig();
                StopSavingImage();
                //NavigateDNNSettingCommand.Execute(obj);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " InspectionSettingBtnRun");
            }
            
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
                VisionCameraGroupSecond.ResetCameraAll();

                ResetCameraInfo();
                CameraIDInfo = DBAcess.GiveCamSettings(RecipeID);
                CamExtimeInfo = DBAcess.GiveExTimeSetting(RecipeID);
                //LEDVal = DBAcess.GiveLEDValSetting(CamID);
                DnnFileInfo = DBAcess.GiveDNNFileSetting(RecipeID);
                //UpdateDnnFiles(DnnFilePath);
                VisionCameraGroup.UpdateDeviceList();
                VisionCameraGroupSecond.UpdateDeviceList();
                
                VisionCameraGroup.UpdateCamera(RecipeID, CameraIDInfo.GetRange(0, 3), CamExtimeInfo.GetRange(0, 3));
                VisionCameraGroupSecond.UpdateCamera(RecipeID, CameraIDInfo.GetRange(3, 2), CamExtimeInfo.GetRange(3, 2));

                //UpdateLED(LEDVal);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " SetBtn_Click");
            }
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
            try
            {
                foreach (IVisionCamera visionCamera in VisionCameraGroup.GetIVisionCameraList())
                {
                    CameraImageBitmapQueue.Add(new Bitmap(visionCamera.GetLatestFrame().CapturedBitmapImage));
                     
                    CameraImageSourceGroup[count] = ToBitmapImage(visionCamera.GetLatestFrame().CapturedBitmapImage);

                    count++;
                }

                OnPropertyChanged("CameraImageSourceGroup");
                
                //UpdateCameraImageSourceGroup(CameraImageSourceGroup);

            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message + " OnImageReady MainviewModel");
            }
        }
        private object lockInferObject = new object();
        private ImageGroupModel tempImageGroupModel;
        private void OnImageReadySecond(Object sender, EventArgs e)
        {
            int count = 0;
            int i = 3;
            
            try
            {
                
                lock (lockInferObject)
                {
                    tempImageGroupModel = new ImageGroupModel(5);

                    if (CameraImageBitmapQueue.Count < 3)
                    {
                        Console.WriteLine("No images in the queue");
                        return;
                    }
                    
                    tempImageGroupModel.ImageInfoModelList = MainImageGroupModel.ImageInfoModelList;

                    foreach (IVisionCamera visionCamera in VisionCameraGroupSecond.GetIVisionCameraList())
                    {
                        tempImageGroupModel.GetImageInfoModels()[count + i].BitmapRawImage = new Bitmap(visionCamera.GetLatestFrame().CapturedBitmapImage);
                        CameraImageSourceGroupSecond[count] = ToBitmapImage(visionCamera.GetLatestFrame().CapturedBitmapImage);
                        count++;
                    }
                    Console.WriteLine("CameraImageBitmapQueue  -- " + CameraImageBitmapQueue.Count);
                    for (i = 0; i < 3; i++)
                    {
                        tempImageGroupModel.GetImageInfoModels()[i].BitmapRawImage = CameraImageBitmapQueue.Take();

                    }

                    //myList1 = myList1.Concat(myList2).ToList();

                    OnPropertyChanged("CameraImageSourceGroupSecond");


                    //UpdateCameraImageSourceGroupSecond(CameraImageSourceGroupSecond);

                    //Console.WriteLine("ImageReady");
                }


                Task.Factory.StartNew((Action)(() =>
                {
                    Thread.Sleep(100);
                    tempImageGroupModel = MainInferenceModel.InferDLL(tempImageGroupModel);
                    IOSaveModel.AddSignalValue(tempImageGroupModel.InferResultNG);
                    IOSaveModel.AddImageToBuffer(tempImageGroupModel);
                }
                ));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " OnImageReady MainviewModel");
            }
        }



        private void UpdateCameraImageSourceGroup(List<BitmapImage> bitmapImageList)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => ViewImageSourceGroup = bitmapImageList);
            //ViewImageSourceGroup = bitmapImageList;
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

        public void StartIOSig()
        {
            try
            {
                IOControlModel.ContinueBlowSignal = true;
                Thread tSignal = new Thread(new ThreadStart(IOControlModel.IOBlowSig));
                tSignal.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " StartIOSig");
            }

        }

        public void StopIOSig()
        {
            IOControlModel.ContinueBlowSignal = false;
        }

        public void StartSavingImage()
        {
            try
            {
                ImageSave.ContinueSaveImage = true;
                Thread tSaveImage = new Thread(new ThreadStart(ImageSave.SaveImageThread));
                tSaveImage.Start();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " StartSavingImage");
            }
            
        }

        public void StopSavingImage()
        {
            ImageSave.ContinueSaveImage = false;
        }

        private void OpenCamera()
        {
            VisionCameraGroup.OpenCamera(RecipeID);
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