using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Frism_Inspection_Renew
{
    public class DBAcess
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INTELLIZ Corp\\Frism Inspection\\CameraInfo.db";
        public static string cs = @"URI=file:" + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\INTELLIZ Corp\\Frism Inspection\\CameraInfo.db";
        SQLiteConnection con;
        SQLiteCommand cmd;
        public static SQLiteDataReader dr;



        public static void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //////////////// Return Cam Setting Info from Input ID /////////////
        #region Give Information From DB

        public static string GiveFilePath(string camId)
        {
            string output = Application.StartupPath;

            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM BasicSet";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {



                        dr = cmd.ExecuteReader();



                        while (dr.Read())
                        {

                            if (dr.GetString(0) == camId)
                            {
                                if (dr[1].GetType() != typeof(DBNull))
                                {
                                    output = dr.GetString(1);
                                }
                                else
                                {
                                    output = null;
                                }


                            }
                        }
                        dr.Close();
                    }
                }
                //Console.WriteLine(output);
                Logger.Debug(output);

            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + " GiveFilePath");

            }
            finally
            {

            }
            return output;

        }

        public static List<string> GiveBasicSettings(string camId)
        {
            List<string> output = new List<string>();
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM BasicSet";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();



                        while (dr.Read())
                        {
                            if (dr.GetString(0) == camId)
                            {
                                if (dr[2].GetType() != typeof(DBNull))
                                {
                                    output.Add(dr.GetString(2));
                                    output.Add(dr.GetString(3));
                                    output.Add(dr.GetString(4));
                                    output.Add(dr.GetString(5));
                                    output.Add(dr.GetString(6));
                                    output.Add(dr.GetString(7));
                                    output.Add(dr.GetString(8));
                                    output.Add(dr.GetString(9));
                                }
                                else
                                {
                                    output = null;
                                }


                            }
                        }
                        dr.Close();
                    }
                }

            }
            catch (Exception exception)
            {
                ShowException(exception);
                Logger.Error(exception.Message + " GiveBasicSetting");

            }
            finally
            {

            }
            return output;

        }

        public static List<string> GiveCamSettings(string camId)
        {
            List<string> output = new List<string>();
            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM CamInfo";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == camId)
                            {
                                output.Add(dr.GetString(1));
                                output.Add(dr.GetString(2));
                                output.Add(dr.GetString(3));
                                output.Add(dr.GetString(4));
                            }
                        }
                    }
                }

            }
            catch (Exception exception)
            {
                ShowException(exception);
                Logger.Error(exception.Message + " GiveCamSet");

            }
            finally
            {

            }
            return output;

        }




        public static List<string> GiveExTimeSetting(string camId)
        {
            List<string> output = new List<string>();
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM CamExTime";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == camId)
                            {
                                output.Add(dr.GetString(1));
                                output.Add(dr.GetString(2));
                                output.Add(dr.GetString(3));
                                output.Add(dr.GetString(4));
                            }
                        }
                        dr.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GiveExtime");
            }

            return output;
        }


        public static List<string> GiveLEDValSetting(string camId)
        {
            List<string> output = new List<string>();
            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM LEDVal";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == camId)
                            {
                                output.Add(dr.GetString(1));
                                output.Add(dr.GetString(2));
                                output.Add(dr.GetString(3));
                                output.Add(dr.GetString(4));
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GiveLED");
            }

            return output;

        }


        public static List<string> GiveDNNFileSetting(string camId)
        {
            List<string> output = new List<string>();
            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM DNNFilePath";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == camId)
                            {
                                output.Add(dr.GetString(1));
                                output.Add(dr.GetString(2));
                                output.Add(dr.GetString(3));
                                output.Add(dr.GetString(4));
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GiveDnnFilePath");
            }
            return output;
        }


        public static List<string> GiveCropInfoSetting(string camId)
        {
            List<string> output = new List<string>();

            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM Camera_ROI";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {



                            if (dr.GetString(0) == camId)
                            {

                                output.Add(dr.GetString(1));
                                output.Add(dr.GetString(2));
                                output.Add(dr.GetString(3));
                                output.Add(dr.GetString(4));
                                break;
                            }
                        }
                    }
                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " GiveCropInfo");
            }
            return output;
        }

        #endregion


        /////////////// Return Setting ID /////////////
        #region Return ID
        public static List<string> CamInfos()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            List<string> output = new List<string>();
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM CamInfo";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            output.Add(dr.GetString(0));

                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " CamInfos");
            }

            return output;
        }

        #endregion


        /////////////// Return Cam Infos /////////////
        #region First Pre-set Info
        public static List<string> DataShowBasicSet()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            List<string> output = new List<string>();
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM BasicSet";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        output.Add(null);
                        output.Add(null);
                        output.Add(null);
                        output.Add(null);

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "temp")
                            {
                                output[0] = dr.GetString(1);
                                output[1] = dr.GetString(2);
                                output[2] = dr.GetString(3);
                                output[3] = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DataShowBasicSet");
            }


            return output;
        }


        public static List<string> DataShow()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            List<string> output = new List<string>();
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM CamInfo";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        output.Add(null);
                        output.Add(null);
                        output.Add(null);
                        output.Add(null);

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "temp")
                            {
                                output[0] = dr.GetString(1);
                                output[1] = dr.GetString(2);
                                output[2] = dr.GetString(3);
                                output[3] = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DataShow");
            }


            return output;
        }


        public static List<string> DataShowExTimeInfo()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            List<string> output = new List<string>();

            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM CamExTime";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        output.Add(null);
                        output.Add(null);
                        output.Add(null);
                        output.Add(null);

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "temp")
                            {
                                output[0] = dr.GetString(1);
                                output[1] = dr.GetString(2);
                                output[2] = dr.GetString(3);
                                output[3] = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DataShowExTimeInfo");
            }


            return output;
        }


        public static List<string> DataShowLEDVal()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            List<string> output = new List<string>();

            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM LEDVal";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {
                        dr = cmd.ExecuteReader();

                        output.Add(null);
                        output.Add(null);
                        output.Add(null);
                        output.Add(null);

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "temp")
                            {
                                output[0] = dr.GetString(1);
                                output[1] = dr.GetString(2);
                                output[2] = dr.GetString(3);
                                output[3] = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DataShowLEDVal");
            }


            return output;
        }


        public static List<string> DataShowDNNFilePath()
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }

            List<string> output = new List<string>();

            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM DNNFilePath";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {


                        dr = cmd.ExecuteReader();

                        output.Add(null);
                        output.Add(null);
                        output.Add(null);
                        output.Add(null);

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "temp")
                            {
                                output[0] = dr.GetString(1);
                                output[1] = dr.GetString(2);
                                output[2] = dr.GetString(3);
                                output[3] = dr.GetString(4);
                            }
                        }
                        dr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " DataShowDNNFilePath");
            }

            return output;
        }

        #endregion


        /////////////// Create DB file if it is not exist /////////////
        public static void CreateDB()
        {
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    SQLiteConnection.CreateFile(path);
                    using (var sqlite = new SQLiteConnection(@"Data Source=" + path))
                    {
                        sqlite.Open();
                        string sql = "create table CamInfo(id varchar(12),cam1 varchar(25),cam2 varchar(25),cam3 varchar(25),cam4 varchar(25))";
                        string sqlExtime = "create table CamExTime(id varchar(12),Extime1 varchar(25),Extime2 varchar(25),Extime3 varchar(25),Extime4 varchar(25))";
                        string sqlLED = "create table LEDVal(id varchar(12),LEDRed varchar(25),LEDGreen varchar(25),LEDBlue varchar(25),LEDWhite varchar(25))";
                        string sqlDNN = "create table DNNFilePath(id varchar(12),TopDnn varchar(25),FirstDnn varchar(25),SecondDnn varchar(25),ThirdDnn varchar(25))";
                        string sqlBasicSet = "create table BasicSet(id varchar(12),folderPath varchar(25),maxThreadCnt varchar(25),maxTileW varchar(25),maxTileH varchar(25)," +
                            "gpuNum varchar(25),minDefectSizeT varchar(25),minPValT varchar(25),minDefectSizeS varchar(25),minPValS varchar(25))";
                        string sqlCameraROI = "create table Camera_ROI(id varchar(12),topROI varchar(25),side1ROI varchar(25),side2ROI varchar(25),side3ROI varchar(25))";


                        SQLiteCommand command = new SQLiteCommand(sql, sqlite);
                        SQLiteCommand command2 = new SQLiteCommand(sqlExtime, sqlite);
                        SQLiteCommand command3 = new SQLiteCommand(sqlLED, sqlite);
                        SQLiteCommand command4 = new SQLiteCommand(sqlDNN, sqlite);
                        SQLiteCommand command5 = new SQLiteCommand(sqlBasicSet, sqlite);
                        SQLiteCommand command6 = new SQLiteCommand(sqlCameraROI, sqlite);

                        command.ExecuteNonQuery();
                        command2.ExecuteNonQuery();
                        command3.ExecuteNonQuery();
                        command4.ExecuteNonQuery();
                        command5.ExecuteNonQuery();
                        command6.ExecuteNonQuery();

                    }
                }
                else
                {
                    Logger.Info("DB Already Exist");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " CreateDB");
            }

        }

        /////////////// Add Cameras Infos to DB /////////////


        public static int InsertFilePath(string settingId, string filePath)
        {
            try
            {



                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM BasicSet";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {
                        dr = cmd.ExecuteReader();

                        bool bCheckExist = false;

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "0")
                            {
                                bCheckExist = true;
                                break;
                            }
                        }
                        dr.Close();


                        if (bCheckExist)
                        {
                            return 0;
                        }

                        cmd.CommandText = "INSERT INTO BasicSet(id,folderPath) VALUES(@id,@folderPath)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@folderPath", filePath);

                        ;
                        cmd.ExecuteNonQuery();
                    }
                }

                return 1;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertFilePath");
            }
            return 2;
        }

        public static int InsertBasicSet(string settingId, List<string> basicInfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    string stm = "SELECT * FROM BasicSet";
                    using (var cmd = new SQLiteCommand(stm, con))
                    {



                        dr = cmd.ExecuteReader();

                        bool bCheckExist = false;

                        while (dr.Read())
                        {
                            if (dr.GetString(0) == "0")
                            {
                                bCheckExist = true;
                                break;
                            }
                        }
                        dr.Close();

                        if (bCheckExist)
                        {
                            return 0;
                        }


                        cmd.CommandText = "INSERT INTO BasicSet(id,maxThreadCnt,maxTileW,maxTileH, gpuNum, minDefectSizeT, minPValT, minDefectSizeS, minPValS)" +
                            " VALUES(@id,@maxThreadCnt,@maxTileW,@maxTileH,@gpuNum,@minDefectSizeT,@minPValT,@minDefectSizeS,@minPValS)";

                        cmd.Parameters.AddWithValue("@id", settingId);

                        cmd.Parameters.AddWithValue("@maxThreadCnt", basicInfo[0]);
                        cmd.Parameters.AddWithValue("@maxTileW", basicInfo[1]);
                        cmd.Parameters.AddWithValue("@maxTileH", basicInfo[2]);
                        cmd.Parameters.AddWithValue("@gpuNum", basicInfo[3]);
                        cmd.Parameters.AddWithValue("@minDefectSizeT", basicInfo[4]);
                        cmd.Parameters.AddWithValue("@minPValT", basicInfo[5]);
                        cmd.Parameters.AddWithValue("@minDefectSizeS", basicInfo[6]);
                        cmd.Parameters.AddWithValue("@minPValS", basicInfo[7]);
                        ;
                        cmd.ExecuteNonQuery();
                    }
                }
                return 1;

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertBasicSet");
            }
            return 2;
        }



        public static void InsertCamera(string settingId, List<string> caminfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "INSERT INTO CamInfo(id,cam1,cam2,cam3,cam4) VALUES(@id,@cam1,@cam2,@cam3,@cam4)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@cam1", caminfo[0]);
                        cmd.Parameters.AddWithValue("@cam2", caminfo[1]);
                        cmd.Parameters.AddWithValue("@cam3", caminfo[2]);
                        cmd.Parameters.AddWithValue("@cam4", caminfo[3]);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertCamera");
            }

        }
        public static void InsertExTime(string settingId, List<string> exTimeInfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "INSERT INTO CamExTime(id,Extime1,Extime2,Extime3,Extime4) VALUES(@id,@Extime1,@Extime2,@Extime3,@Extime4)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@Extime1", exTimeInfo[0]);
                        cmd.Parameters.AddWithValue("@Extime2", exTimeInfo[1]);
                        cmd.Parameters.AddWithValue("@Extime3", exTimeInfo[2]);
                        cmd.Parameters.AddWithValue("@Extime4", exTimeInfo[3]);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertExTime");
            }

        }

        public static void InsertLEDVal(string settingId, List<string> LEDinfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "INSERT INTO LEDVal(id,LEDRed,LEDGreen,LEDBlue,LEDWhite) VALUES(@id,@LEDRed,@LEDGreen,@LEDBlue,@LEDWhite)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@LEDRed", LEDinfo[0]);
                        cmd.Parameters.AddWithValue("@LEDGreen", LEDinfo[1]);
                        cmd.Parameters.AddWithValue("@LEDBlue", LEDinfo[2]);
                        cmd.Parameters.AddWithValue("@LEDWhite", LEDinfo[3]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertLEDVal");
            }

        }


        public static void InsertDNNFilePath(string settingId, List<string> DnnFilePath)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "INSERT INTO DNNFilePath(id,TopDnn,FirstDnn,SecondDnn,ThirdDnn) VALUES(@id,@TopDnn,@FirstDnn,@SecondDnn,@ThirdDnn)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@TopDnn", DnnFilePath[0]);
                        cmd.Parameters.AddWithValue("@FirstDnn", DnnFilePath[1]);
                        cmd.Parameters.AddWithValue("@SecondDnn", DnnFilePath[2]);
                        cmd.Parameters.AddWithValue("@ThirdDnn", DnnFilePath[3]);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertDNNFilePath");
            }

        }


        public static void InsertCropInfo(string settingId, List<string> imgCropInfo)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    if (imgCropInfo[i] == "null")
                    {
                        imgCropInfo[i] = string.Format("{0},{1},{2},{3}", 0, 0, 1, 1);

                    }
                }


                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "INSERT INTO Camera_ROI(id,topROI,side1ROI,side2ROI,side3ROI) VALUES(@id,@topROI,@side1ROI,@side2ROI,@side3ROI)";

                        cmd.Parameters.AddWithValue("@id", settingId);
                        cmd.Parameters.AddWithValue("@topROI", imgCropInfo[0]);
                        cmd.Parameters.AddWithValue("@side1ROI", imgCropInfo[1]);
                        cmd.Parameters.AddWithValue("@side2ROI", imgCropInfo[2]);
                        cmd.Parameters.AddWithValue("@side3ROI", imgCropInfo[3]);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " InsertCropInfo");
            }

        }




        /////////////// Update DB with same ID /////////////

        public static void UpdateDataFilePath(string settingId, string filePath)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE BasicSet Set folderPath=@folderPath where id=@id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@folderPath", filePath);
                        cmd.Parameters.AddWithValue("@id", settingId);

                        cmd.ExecuteNonQuery();

                        //DataShow();
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Error(ex.Message + " UpdateDataFilePath");
            }



        }

        public static void UpdateDataBaseBasic(string settingId, List<string> basicInfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE BasicSet Set maxThreadCnt=@maxThreadCnt," +
                        "maxTileW=@maxTileW,maxTileH=@maxTileH,gpuNum=@gpuNum,minDefectSizeT=@minDefectSizeT,minPValT=@minPValT,minDefectSizeS=@minDefectSizeS," +
                        "minPValS=@minPValS where id=@id";

                        cmd.Prepare();


                        //cmd.Parameters.AddWithValue("@folderPath", basicInfo[0]);
                        cmd.Parameters.AddWithValue("@maxThreadCnt", basicInfo[0]);
                        cmd.Parameters.AddWithValue("@maxTileW", basicInfo[1]);
                        cmd.Parameters.AddWithValue("@maxTileH", basicInfo[2]);
                        cmd.Parameters.AddWithValue("@gpuNum", basicInfo[3]);
                        cmd.Parameters.AddWithValue("@minDefectSizeT", basicInfo[4]);
                        cmd.Parameters.AddWithValue("@minPValT", basicInfo[5]);
                        cmd.Parameters.AddWithValue("@minDefectSizeS", basicInfo[6]);
                        cmd.Parameters.AddWithValue("@minPValS", basicInfo[7]);

                        cmd.Parameters.AddWithValue("@id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                    //var cmd = new SQLiteCommand(con);

                }
                //var con = new SQLiteConnection(cs);


                DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBaseBasic");
            }


        }


        public static void UpdateDataBase(string settingId, List<string> caminfo)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE CamInfo Set cam1=@cam1,cam2=@cam2,cam3=@cam3,cam4=@cam4 where id=@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@cam1", caminfo[0]);
                        cmd.Parameters.AddWithValue("@cam2", caminfo[1]);
                        cmd.Parameters.AddWithValue("@cam3", caminfo[2]);
                        cmd.Parameters.AddWithValue("@cam4", caminfo[3]);
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                }

                DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBase");
            }


        }

        public static void UpdateDataBaseExTime(string settingId, List<string> exTimeInfo)
        {
            try
            {

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE CamExTime Set Extime1=@Extime1,Extime2=@Extime2,Extime3=@Extime3,Extime4=@Extime4 where id=@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Extime1", exTimeInfo[0]);
                        cmd.Parameters.AddWithValue("@Extime2", exTimeInfo[1]);
                        cmd.Parameters.AddWithValue("@Extime3", exTimeInfo[2]);
                        cmd.Parameters.AddWithValue("@Extime4", exTimeInfo[3]);
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                }

                DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBaseExTime");
            }


        }

        public static void UpdateDataBaseLEDVal(string settingId, List<string> LEDinfo)
        {
            try
            {


                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE LEDVal Set LEDRed=@LEDRed,LEDGreen=@LEDGreen,LEDBlue=@LEDBlue,LEDWhite=@LEDWhite where id=@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@LEDRed", LEDinfo[0]);
                        cmd.Parameters.AddWithValue("@LEDGreen", LEDinfo[1]);
                        cmd.Parameters.AddWithValue("@LEDBlue", LEDinfo[2]);
                        cmd.Parameters.AddWithValue("@LEDWhite", LEDinfo[3]);
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                }

                DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBaseLEDVal");
            }


        }

        public static void UpdateDataBaseDNNFilePath(string settingId, List<string> DNNFilePath)
        {
            try
            {
                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = "UPDATE DNNFilePath Set TopDnn=@TopDnn,FirstDnn=@FirstDnn,SecondDnn=@SecondDnn,ThirdDnn=@ThirdDnn where id=@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@TopDnn", DNNFilePath[0]);
                        cmd.Parameters.AddWithValue("@FirstDnn", DNNFilePath[1]);
                        cmd.Parameters.AddWithValue("@SecondDnn", DNNFilePath[2]);
                        cmd.Parameters.AddWithValue("@ThirdDnn", DNNFilePath[3]);
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                }

                DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBaseDNNFilePath");
            }


        }

        public static void UpdateDataBaseCropInfo(string settingId, List<string> imgCropInfo)
        {
            try
            {

                if (imgCropInfo[0] == "null") return;

                using (var con = new SQLiteConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(con))
                    {

                        cmd.CommandText = "UPDATE Camera_ROI Set topROI=@topROI,side1ROI=@side1ROI,side2ROI=@side2ROI,side3ROI=@side3ROI where id=@Id";
                        cmd.Prepare();



                        cmd.Parameters.AddWithValue("@topROI", imgCropInfo[0]);
                        cmd.Parameters.AddWithValue("@side1ROI", imgCropInfo[1]);
                        cmd.Parameters.AddWithValue("@side2ROI", imgCropInfo[2]);
                        cmd.Parameters.AddWithValue("@side3ROI", imgCropInfo[3]);
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                }

                //DataShow();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message + " UpdateDataBaseCropInfo");
            }


        }

        /////////////// Delete Camera Setting in DB /////////////

        public static void DeleteDataBasicInfo(string settingId)
        {

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM BasicSet where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteDataBasicInfo");
                        //return;
                    }
                }

                //DataShow();
            }

        }
        public static void DeleteData(string settingId)
        {

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM CamInfo where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                        //DataShow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteData");
                        return;
                    }
                }
            }

        }


        public static void DeleteDataExTime(string settingId)
        {
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM CamExTime where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                        //DataShow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteDataExTime");
                        return;
                    }
                }
            }
        }


        public static void DeleteDataLEDVal(string settingId)
        {
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM LEDVal where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                        //DataShow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteDataLEDVal");
                        return;
                    }
                }
            }
        }


        public static void DeleteDataDNNFilePath(string settingId)
        {
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM DNNFilePath where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                        //DataShow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteDataDNNFilePath");
                        return;
                    }
                }
            }
        }


        public static void DeleteDataCropInfo(string settingId)
        {
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    try
                    {
                        cmd.CommandText = "DELETE FROM Camera_ROI where id =@Id";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@Id", settingId);

                        cmd.ExecuteNonQuery();
                        //DataShow();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message + " DeleteDataCropInfo");
                        return;
                    }
                }
            }
        }




    }
}
