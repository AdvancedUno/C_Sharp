using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism
{
    public class DummyCamera 
    {
        public event EventHandler<EventArgs> GuiCameraOpenedCamera;
        public event EventHandler<EventArgs> GuiCameraClosedCamera;
        public event EventHandler<EventArgs> GuiCameraGrabStarted;
        public event EventHandler<EventArgs> GuiCameraGrabStopped;
        public event EventHandler<EventArgs> GuiCameraConnectionToCameraLost;
        public event EventHandler<EventArgs> GuiCameraFrameReadyForDisplay;

        bool isOpen = false;

        public void ClearLatestFrame()
        {
            throw new NotImplementedException();
        }

        public void CloseCamera()
        {
            throw new NotImplementedException();
        }

        public void CreateByCameraInfo(ICameraInfo info)
        {
            throw new NotImplementedException();
        }

        public void CreateCamera()
        {
            throw new NotImplementedException();
        }

        public void DestroyCamera()
        {
            throw new NotImplementedException();
        }

        public bool IsCreated()
        {
            throw new NotImplementedException();
        }

        public bool IsGrabbing()
        {
            throw new NotImplementedException();
        }

        public bool IsOpen()
        {
            //throw new NotImplementedException();
            return isOpen;
        }

        public void OpenCamera()
        {
            // 파일 경로의 이미지를 로딩함
            throw new NotImplementedException();
        }

        public void StopGrabbing()
        {
            throw new NotImplementedException();
        }
    }
}
