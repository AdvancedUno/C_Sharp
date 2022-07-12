using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Frism
{
    public partial class SelectCameraWindows : System.Windows.Controls.UserControl
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        //private IVisionCamera camera = new FileCamera(@"D:\tempbbbb");
        private IVisionCamera camera = new UnoCamera();

        private Dictionary<ICameraInfo, String> itemInfos;

        public ICameraInfo selectedCameraInfo = null;
        
        public string filename = "0";

        public Dictionary<ICameraInfo, String> Items
        {
            get { return itemInfos; }
            set
            {
                itemInfos = value;
                OnPropertyChanged();
            }
        }


        #region PropertyChangedEventHandler
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public SelectCameraWindows()
        {
            InitializeComponent();
            DataContext = this;
            PreviewImage.SetCamera(camera);

            camera.SetSettingMode();

            camera.GuiCameraConnectionToCameraLost += OnDeviceRemoved;
            camera.GuiCameraOpenedCamera += OnCameraOpened;
            camera.GuiCameraClosedCamera += OnCameraClosed;
            camera.GuiCameraGrabStarted += OnGrabStarted;
            camera.GuiCameraGrabStopped += OnGrabStopped;

            GainSlider1.SetLabel(GainText1);
            ExposureTSlider1.SetLabel(ExposureText1);
        }

        public IVisionCamera GetUnoCamera()
        {
            return camera;
        }


        public void DestroyCamera()
        {
            try
            {
                if (camera != null && camera.IsOpened())
                {
                    camera.CloseCamera();
                    camera.DestroyCamera();
                    camera = null;
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                //ShowException(exception);
            }
        }

        public void ResetCamera()
        {
            try
            {
                if (camera != null && camera.IsOpened())
                {
                    camera.CloseCamera();
                    camera.DestroyCamera();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                //ShowException(exception);
            }
        }


        public void OnDeviceRemoved(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnDeviceRemoved), sender, e);
                return;
            }
            try
            {
                camera.DestroyCamera();
            }
            catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }
            
        }

        public void OnCameraOpened(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }
            try
            {
                BindParametersToControls();
            }catch(Exception ex)
            {
                Logger.Error(ex.Message + " OnCameraOpen_SelectCamWindow");
            }
            
        }

        public void OnCameraClosed(Object sender, EventArgs e)
        {
            
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

            try
            {
                PreviewImage.Clear();
                UnbindParametersFromControls();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " OnCameraClose_SelectCamWindow");
            }
           
        }


        public void OnGrabStarted(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }
            
        }

        public void OnGrabStopped(Object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new EventHandler<EventArgs>(OnGrabStopped), sender, e);
                return;
            }
            
        }


        private void BindParametersToControls()
        {
            
            IParameterCollection parameters = camera.GetParameters();
            if(parameters != null)
            {

                try
                {
                    if (parameters.Contains(PLCamera.Gain))
                    {
                        GainSlider1.Parameter = parameters[PLCamera.Gain];
                    }
                    else
                    {

                        GainSlider1.Parameter = parameters[PLCamera.GainRaw];
                    }
                    if (parameters.Contains(PLCamera.ExposureTimeAbs))
                    {
                        ExposureTSlider1.Logarithmic = true;
                        ExposureTSlider1.Parameter = parameters[PLCamera.ExposureTimeAbs];

                    }
                    else
                    {
                        ExposureTSlider1.Logarithmic = true;
                        ExposureTSlider1.Parameter = parameters[PLCamera.ExposureTime];
                    }
                
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message + " BindParametersToControls_SelectCamWindow");
                    //Helper.ShowException(e);
                }
            }
        }

        public void UnbindParametersFromControls()
        {

            GainSlider1.Parameter = null;
            ExposureTSlider1.Parameter = null;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (camera != null && camera.IsOpened())
                {
                    camera.StartContinuousShotGrabbing();
                    StartButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " StartButton_Click_SelectCamWindow");
            }

        }

        public void StartCam()
        {
            if (camera != null && camera.IsOpened())
            {
                camera.StartContinuousShotGrabbing();
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
        }

        public void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (camera != null && camera.CheckIsGrabbing())
            {
                camera.StopGrabbing();
                StopButton.IsEnabled = false;
                StartButton.IsEnabled = true;
            }
        }

        public void SetSelectionInfo(ICameraInfo info)
        {
            try
            {
                if (camera == null)
                {
                    camera = new UnoCamera();
                }
                if (camera.IsOpened())
                {
                    camera.DestroyCamera();
                }
                selectedCameraInfo = info;
                camera.CreateByCameraInfo(selectedCameraInfo);
                camera.OpenCamera();
                ExposureText1.Text = ExposureTSlider1.slider.Value.ToString();
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception caught:\n" + ex.Message, "Error");
                Logger.Error(ex.Message + " SetSelectionInfo_SelectCamWindow");
            }


        }

        private void SetDNNButton_Click(object sender, RoutedEventArgs e)
        {

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
                    DNNPathTxt.Text = filename;
                }
            }
           catch(Exception ex)
            {
                Logger.Error(ex.Message);
            }





        }


    }
}
