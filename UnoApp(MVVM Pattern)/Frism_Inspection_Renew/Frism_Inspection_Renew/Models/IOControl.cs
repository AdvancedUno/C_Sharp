using Automation.BDaq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class IOControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private bool _continueBlowSignal = false;
        public bool ContinueBlowSignal { get => _continueBlowSignal; set => _continueBlowSignal = value; }
       

        private object _lockBlowObject = new object();
        private object _lockGetTimeObject = new object();

        private ErrorCode _errorCode = ErrorCode.Success;
        public ErrorCode ErrorCode { get => _errorCode; set => _errorCode = value; }

        
        private InstantDoCtrl _instantDoCtrl;
        public InstantDoCtrl InstantDoCtrl { get => _instantDoCtrl; set => _instantDoCtrl = value; }

        private string _deviceDescription = "PCI-1730,BID#0";
        public string DeviceDescription { get => _deviceDescription; set => _deviceDescription = value; }

        private string _profilePath = "D:\\profile_i\\dev2.xml";
        public string ProfilePath { get => _profilePath; set => _profilePath = value; }

        private byte[] _bufferForWriting;
        public byte[] BufferForWriting { get => _bufferForWriting; set => _bufferForWriting = value; }

        private int _startPort = 0;
        public int StartPort { get => _startPort; set => _startPort = value; }

        private int _portCount = 1;
        public int PortCount { get => _portCount; set => _portCount = value; }

        public void IOBlowSig()
        {

            ErrorCode errorCode = ErrorCode.Success;

            InstantDiCtrl instantDiCtrl = new InstantDiCtrl();
            try
            {
                instantDiCtrl.SelectedDevice = new DeviceInformation(DeviceDescription);
                errorCode = instantDiCtrl.LoadProfile(ProfilePath);
                if (BioFailed(errorCode))
                {
                    throw new Exception();
                }

                //Console.WriteLine("Reading ports' status is in progress..., any key to quit!\n");

                byte[] buffer = new byte[64];
                bool bSensorFlag = false;

                while (ContinueBlowSignal)
                {
                    //Console.WriteLine("ReadBuffer");
                    errorCode = instantDiCtrl.Read(StartPort, PortCount, buffer);
                    if (BioFailed(errorCode))
                    {
                        throw new Exception();
                    }
                    int intbyte = buffer[0];

                    if (buffer[0] > 0)
                    {
                        while (true)
                        {
                            //Console.WriteLine("Buffer Value : " + buffer[0]);
                            errorCode = instantDiCtrl.Read(StartPort, PortCount, buffer);
                            if (buffer[0] == 0)
                            {
                                //blowDelegateClass.StartBlowing();
                                break;
                            }
                        }
                    }
                    Thread.Sleep(5);
                }
            }
            catch (Exception e)
            {
                string errStr = BioFailed(errorCode) ? " Some error occurred. And the last error code is " + errorCode.ToString()
                                                          : e.Message;
                Console.WriteLine(errStr);
                Logger.Error(e.Message + " _IOBlow");
            }
            finally
            {
                instantDiCtrl.Dispose();
                Console.ReadKey(false);

            }
        }
        public bool BioFailed(ErrorCode err)
        {
            return err < ErrorCode.Success && err >= ErrorCode.ErrorHandleNotValid;
        }

        public void ErrorBlow()
        {
            lock (_lockBlowObject)
            {
                for (int i = 0; i < 5; i++)
                {
                    blow(10);
                }
            }
        }


        public void setBlowReady()
        {
            bool temp;
            int nSleep = 100;
            //Console.WriteLine("setBlowReady : " + qSaveBlowSignal.Count);

            if (SaveBlowSignal.Count > 0)
            {
                temp = SaveBlowSignal.Take();

                if (!temp)
                {
                    Console.WriteLine("NG Blow\n");
                    blow(nSleep);

                }
                else
                {
                    Console.WriteLine("OK NON Blow\n");
                    //Thread.Sleep(nSleep);
                }

            }



        }


        public void setBlowDevice()
        {
            try
            {
                ErrorCode = ErrorCode.Success;

                InstantDoCtrl = new InstantDoCtrl();
                InstantDoCtrl.SelectedDevice = new DeviceInformation(DeviceDescription);
                ErrorCode = InstantDoCtrl.LoadProfile(ProfilePath);
                if (BioFailed(ErrorCode))
                {
                    throw new Exception();
                }

                //blowDelegateClass = new BlowDelegateClass();
                //blowDelegateClass.blowEvent += setBlowReady;



            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " _Blow");
            }

        }

        public void blow(int nDelay)
        {


            Console.WriteLine("BLOWWWWWWWWWWWW");
            BufferForWriting = new byte[64];
            try
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < PortCount; ++i)
                    {
                        string data;
                        if (j == 0)
                            data = "0x01";
                        else
                            data = "0x00";
                        BufferForWriting[i] = byte.Parse(data.Contains("0x") ? data.Remove(0, 2) : data, System.Globalization.NumberStyles.HexNumber);
                    }
                    ErrorCode = InstantDoCtrl.Write(StartPort, PortCount, BufferForWriting);
                    if (BioFailed(ErrorCode))
                    {
                        throw new Exception();
                    }
                    if (j == 0)
                    {
                        //Console.WriteLine("sleep!");
                        Thread.Sleep(nDelay);
                    }
                }
            }
            catch (Exception e)
            {
                string errStr = BioFailed(ErrorCode) ? " Some error occurred. And the last error code is " + ErrorCode.ToString()
                                                           : e.Message;
                Console.WriteLine(errStr);
                Logger.Error(e.Message + " _Blow");
            }
        }



    }
}
