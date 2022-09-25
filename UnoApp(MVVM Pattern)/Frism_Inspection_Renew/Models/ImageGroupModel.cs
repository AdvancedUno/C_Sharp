using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class ImageGroupModel
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private List<ImageInfoModel> _imageInfoModelList;
        public List<ImageInfoModel> ImageInfoModelList { get => _imageInfoModelList; set => _imageInfoModelList = value; }

        private ImageInfoModel _imageInfoModel;
        public ImageInfoModel ImageInfo { get => _imageInfoModel; set => _imageInfoModel = value; }

        private int _groupId;
        public int GroupId { get => _groupId; set => _groupId = value; }

        private string _saveFolderPath;
        public string SaveFolderPath { get => _saveFolderPath; set => _saveFolderPath = value; }


        public Dictionary<int, bool> CheckImageInfoDict = new Dictionary<int, bool>();

        private bool _inferResultNG = false;
        public bool InferResultNG { get => _inferResultNG; set => _inferResultNG = value; }

        private int _cameraNum;
        public int CameraNum { get => _cameraNum; set => _cameraNum = value; }

        public ImageGroupModel(int cameraNum)
        {
            ImageInfoModelList = new List<ImageInfoModel>();
            SaveFolderPath = "D:/TB/Image";
            CameraNum = cameraNum;
            try
            {
                for (int i = 0; i < CameraNum; i++)
                {
                    CheckImageInfoDict.Add(i, false);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }
        }

        public int AddImageInfoModel(ImageInfoModel imageInfoModel)
        {
            try
            {
                //if (imageInfoModel.BitmapRawImage == null) return 1;
                if (CheckImageInfoDict[imageInfoModel.CameraId] != false) return 1;
                CheckImageInfoDict[imageInfoModel.CameraId] = true;
                ImageInfoModelList.Add(imageInfoModel);
            }
            catch(Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }
            return 0;
        }

        public int RemoveAllImageInfoModel()
        {
            try
            {
                for(int i = 0; i < CameraNum; i++)
                {
                    CheckImageInfoDict[i] = false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " AddImageInfoModel");
            }

            return 0;
        }

        public List<ImageInfoModel> GetImageInfoModels()
        {
            return ImageInfoModelList;
        }

        public void CreateImageInfoModel()
        {
            for (int i = 0; i < CameraNum; i++)
            {
                ImageInfo = new ImageInfoModel(i);
                //ImageInfo.DnnSettingInfoModel.DnnPath = @"D:\training_tb\project\Mint_Cup\Side\dnn\MintSide.dnn";
                if (i == 0) ImageInfo.CameraPosition = "Top";
                else ImageInfo.CameraPosition = "Side";

                AddImageInfoModel(ImageInfo);
            }
        }

        public void SetDnnInfo()
        {
            List<string> basicInfoList = DBAcess.GiveBasicSettings("0");
            for (int i = 0; i < CameraNum; i++)
            {
                if (basicInfoList != null && basicInfoList.Count > 7)
                {
                    ImageInfoModelList[i].DnnSettingInfoModel = DnnSetEvent(ImageInfoModelList[i].CameraPosition, Int32.Parse(basicInfoList[0]), Int32.Parse(basicInfoList[1]), Int32.Parse(basicInfoList[2]), Int32.Parse(basicInfoList[3]),
                    Int32.Parse(basicInfoList[4]), float.Parse(basicInfoList[5]), Int32.Parse(basicInfoList[6]), float.Parse(basicInfoList[7]));
                }
                else
                {
                    ImageInfoModelList[i].DnnSettingInfoModel = DnnSetEvent(ImageInfoModelList[i].CameraPosition, 1, 1600, 800, 4, 10, 0.5f, 10, 0.5f);
                }
            }
        }

        public DnnSetingInfoModel DnnSetEvent(string CamPosition, int ThreadCnt, int Width, int Height, int GpuNo, int MinDefectNumTop, float MinPValTop, int MinDefectNumSide, float MinPValSide)
        {
            DnnSetingInfoModel DnnSettingInfoModel = new DnnSetingInfoModel();
            try
            {

                DnnSettingInfoModel.MaxThreadCount = ThreadCnt;
                DnnSettingInfoModel.MaxTileHeight = Height;
                DnnSettingInfoModel.MaxTileWidth = Width;
                DnnSettingInfoModel.GpuNum = GpuNo;
                if (CamPosition != "Side")
                {
                    DnnSettingInfoModel.MinDefectSize = MinDefectNumTop;
                    DnnSettingInfoModel.UppperPValue = MinPValTop;
                }
                else
                {
                    DnnSettingInfoModel.MinDefectSize = MinDefectNumSide;
                    DnnSettingInfoModel.UppperPValue = MinPValSide;
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " DnnSetEvent");
                //ShowException(exception);
            }
            return DnnSettingInfoModel;
        }

        public void UpdateDnnFiles(List<string> DnnFileList)
        {
            try
            {
                Logger.Debug("Update and Assign DNN Files");
                for(int i = 0; i < CameraNum; i++)
                {
                    ImageInfoModelList[i].DnnSettingInfoModel.ThreadId = i;
                    ImageInfoModelList[i].DnnSettingInfoModel.DnnPath = DnnFileList[i];
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDnnFiles MainW");
            }
        }

        public void UpdateCropInfoSetting(string OptionId)
        {
            List<string> CropInfo = DBAcess.GiveCropInfoSetting(OptionId);
            for (int i = 0; i < CameraNum; i++)
            {
                ImageInfoModelList[i].SetCropVal(CropInfo[i]);
            }
        }
    }
}
