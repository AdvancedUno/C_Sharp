using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Basler.Pylon.PLCamera;


namespace Frism_Inspection_Renew.Models
{
    public interface IVisionCamera
    {

        event EventHandler<EventArgs> GuiCameraOpenedCamera;
        event EventHandler<EventArgs> GuiCameraClosedCamera;
        event EventHandler<EventArgs> GuiCameraGrabStarted;
        event EventHandler<EventArgs> GuiCameraGrabStopped;
        event EventHandler<EventArgs> GuiCameraConnectionToCameraLost;
        event EventHandler<EventArgs> GuiCameraFrameReadyForDisplay;

        bool IsOpened();

        void SetCamParameter(TriggerModeEnum triggerMode, TriggerSourceEnum triggerSource, string mode, string source);

        Bitmap GetLatestFrame();

        void DestroyCamera();

        //void CreateCamera();
        IParameterCollection GetParameters();

        void GiveImageFile(object sender, EventArgs e);

        void OpenCamera();

        void CloseCamera();

        void FullScreenMode();
        //bool IsCreated();

        bool CheckIsGrabbing();

        bool GetMainMode();

        void StopGrabbing();

        //void ClearLatestFrame();

        void SoftwareTrigger();

        bool CheckIsCreated();

        void SetMainMode();

        void SetSettingMode();

        void StartContinuousShotGrabbing();

        void ChangeFilePath(string fileCameraPath);

        void ExecuteSoftwareTrigger(CommandName commandName);

        void SetExTimeParameter(FloatName name, double value);

        void SetGainParameter(FloatName name, double value);

        void CreateByCameraInfo(ICameraInfo info);
    }
}
