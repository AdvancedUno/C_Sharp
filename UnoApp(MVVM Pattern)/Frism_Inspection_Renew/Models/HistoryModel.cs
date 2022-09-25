using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Frism_Inspection_Renew.Models
{
    public partial class HistoryModel
    {
        private DirectoryInfo folder = null;

        private ImgPath image1 = new ImgPath();
        private ImgPath image2 = new ImgPath();
        private ImgPath image3 = new ImgPath();
        private ImgPath image4 = new ImgPath();

        private bool pick1 = false;
        private bool pick2 = false;
        private bool pick3 = false;
        private bool pick4 = false;

        private bool showMask = true;
        DirectoryInfo temp = null;

        private string savePath;

        private List<ImgPath> _outputImages;
        private List<ImgPath> OutputImages { get => _outputImages; set => _outputImages = value; }
        

        private BitmapSource _bitmapSourceImageFirst;
        public BitmapSource BitmapSourceImageFirst { 
            get => _bitmapSourceImageFirst; 
            set {
                _bitmapSourceImageFirst = value;
            } 
        }

        private BitmapSource _bitmapSourceImageSecond;
        public BitmapSource BitmapSourceImageSecond { 
            get => _bitmapSourceImageSecond;
            set
            {
                _bitmapSourceImageSecond = value;
            }
        }


        private BitmapSource _bitmapSourceImageThird;
        public BitmapSource BitmapSourceImageThird { 
            get => _bitmapSourceImageThird;
            set
            {
                _bitmapSourceImageThird = value;
            }
        }


        private BitmapSource _bitmapSourceImageFourth;
        public BitmapSource BitmapSourceImageFourth { 
            get => _bitmapSourceImageFourth;
            set
            {
                _bitmapSourceImageFourth = value;
            }
        }

        private class ImgPath
        {
            public Bitmap image;
            public string imageS;
        }

        private List<DirectoryInfo> _directoryPath = null;
        public List<DirectoryInfo> DirectoryPath { 
            get => _directoryPath;
            set
            {
                _directoryPath = value;
            }
        }



        public class Temp
        {
            public string path { get; set; }

        }
        public HistoryModel()
        {
            OutputImages = new List<ImgPath>(new ImgPath[4]);



        }


        private void GetFolders(DirectoryInfo folder)
        {


            List<FileInfo> files = new List<FileInfo>();
            if (folder == null)
            {
                return;
            }
            foreach (FileInfo file in folder.GetFiles())
            {
                files.Add(file);

            }

            if (files.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("Please Select a Different Folder.");
                return;
            }

            try
            {
                for (int i = 0; i < files.Count; i++)
                {
                    Bitmap bitmapImg;
                    Bitmap bitmapImgResult;
                    if (i % 2 > 0)
                    {
                        continue;
                    }
                    else
                    {
                        using (Stream s = File.OpenRead(files[i].FullName))
                        {
                            bitmapImg = System.Drawing.Bitmap.FromStream(s) as System.Drawing.Bitmap;


                            if (i == 0)
                            {

                                using (Stream sR = File.OpenRead(files[i + 1].FullName))
                                {
                                    bitmapImgResult = System.Drawing.Bitmap.FromStream(sR) as System.Drawing.Bitmap;
                                    Bitmap temp = bitmapImg;
                                    if (showMask)
                                    {
                                        temp = MergedBitmaps(bitmapImgResult, bitmapImg);
                                    }

                                    image1.image = new Bitmap(temp);
                                    image1.imageS = "First";

                                    BitmapSourceImageFirst = LoadBitmap(temp);

                                    bitmapImg.Dispose();
                                    temp.Dispose();
                                    bitmapImg = null;
                                }
                            }
                            else if (i == 2)
                            {

                                using (Stream sR = File.OpenRead(files[i + 1].FullName))
                                {
                                    bitmapImgResult = System.Drawing.Bitmap.FromStream(sR) as System.Drawing.Bitmap;
                                    Bitmap temp = bitmapImg;
                                    if (showMask)
                                    {
                                        temp = MergedBitmaps(bitmapImgResult, bitmapImg);
                                    }
                                    image2.image = new Bitmap(temp);
                                    image2.imageS = "Second";

                                    BitmapSourceImageSecond = LoadBitmap(temp);

                                    bitmapImg.Dispose();
                                    temp.Dispose();
                                    bitmapImg = null;
                                }


                            }
                            else if (i == 4)
                            {

                                using (Stream sR = File.OpenRead(files[i + 1].FullName))
                                {
                                    bitmapImgResult = System.Drawing.Bitmap.FromStream(sR) as System.Drawing.Bitmap;
                                    Bitmap temp = bitmapImg;
                                    if (showMask)
                                    {
                                        temp = MergedBitmaps(bitmapImgResult, bitmapImg);
                                    }
                                    image3.image = new Bitmap(temp);
                                    image3.imageS = "240";
                                    BitmapSourceImageThird = LoadBitmap(temp);

                                    bitmapImg.Dispose();
                                    temp.Dispose();
                                    bitmapImg = null;
                                }

                            }
                            else if (i == 6)
                            {
                                using (Stream sR = File.OpenRead(files[i + 1].FullName))
                                {
                                    bitmapImgResult = System.Drawing.Bitmap.FromStream(sR) as System.Drawing.Bitmap;
                                    Bitmap temp = bitmapImg;
                                    if (showMask)
                                    {
                                        temp = MergedBitmaps(bitmapImgResult, bitmapImg);
                                    }
                                    image4.image = new Bitmap(temp);
                                    image4.imageS = "Top";

                                    BitmapSourceImageFourth = LoadBitmap(temp);

                                    bitmapImg.Dispose();
                                    temp.Dispose();
                                    bitmapImg = null;
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }



            //OutputTextBlock.Text = outputText.ToString();
            //ListView1.ItemsSource = file;
        }
        private void ShowException(Exception exception)
        {
            System.Windows.Forms.MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            bmp1.MakeTransparent(System.Drawing.Color.White);
            Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                       Math.Max(bmp1.Height, bmp2.Height));
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp2, System.Drawing.Point.Empty);
                g.DrawImage(bmp1, System.Drawing.Point.Empty);
            }
            return result;
        }


        public BitmapSource LoadBitmap(Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {

            }
            return bs;
        }

        private void ResetImages()
        {
            BitmapSourceImageFirst = null;
            BitmapSourceImageSecond = null;
            BitmapSourceImageThird = null;
            BitmapSourceImageFourth = null;

        }


        private void ResetList()
        {
            //directoryPath.Clear();
            //ListView1.ItemsSource = null;
        }


        private void NewFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetList();
            DirectoryPath = new List<DirectoryInfo>();
            FolderBrowserDialog folderPath = new FolderBrowserDialog();
            folderPath.ShowDialog();
            string path = folderPath.SelectedPath;
            //savePath = folderPath.SelectedPath;
            if (path == "")
            {
                System.Windows.Forms.MessageBox.Show("Please Select a Folder.");
                return;
            }

            //TextBlock0.Text = path;
            folder = new DirectoryInfo(path);
            if (folder.Exists)
            {

                foreach (DirectoryInfo folderInfo in folder.GetDirectories())
                {
                    DirectoryPath.Add(folderInfo);

                    //TextBlock0.Text += folderInfo.ToString();
                    //temps.Add(new Temp() { path = folderInfo.ToString() });
                }

                if (DirectoryPath.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please Select a Different Folder.");
                    return;
                }
            }
        }
        private void ListView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListView listView = sender as System.Windows.Controls.ListView;

            temp = listView.SelectedItem as DirectoryInfo;

            ResetImages();



            GetFolders(temp);


        }

        private void SaveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (ImgPath img in OutputImages)
                {
                    if (img != null)
                    {
                        img.image.Save(savePath + "\\" + img.imageS + ".bmp", ImageFormat.Bmp);
                    }

                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

        }

        private void SaveFolderBtn_Click(object sender, RoutedEventArgs e)
        {


            DirectoryPath = new List<DirectoryInfo>();
            FolderBrowserDialog folderPath = new FolderBrowserDialog();
            folderPath.ShowDialog();
            savePath = folderPath.SelectedPath;
            if (savePath == "")
            {
                System.Windows.Forms.MessageBox.Show("Please Select a Folder.");
                return;
            }
        }


        //private void OutputImageTop_MouseUp(object sender, MouseButtonEventArgs e)
        //{


        //    if (pick1)
        //    {

        //        images[0] = null;
        //        pick1 = false;
        //        pannel1.Background = System.Windows.Media.Brushes.White;
        //    }
        //    else
        //    {

        //        images[0] = image4;

        //        pick1 = true;
        //        pannel1.Background = System.Windows.Media.Brushes.Red;
        //    }


        //}

        //private void OutputImage120_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //    if (pick2)
        //    {


        //        images[1] = null;
        //        pick2 = false;
        //        pannel2.Background = System.Windows.Media.Brushes.White;
        //    }
        //    else
        //    {

        //        images[1] = image2;

        //        pick2 = true;
        //        pannel2.Background = System.Windows.Media.Brushes.Red;
        //    }


        //}

        //private void OutputImage0_MouseUp(object sender, MouseButtonEventArgs e)
        //{

        //    if (pick3)
        //    {


        //        images[2] = null;
        //        pick3 = false;
        //        pannel3.Background = System.Windows.Media.Brushes.White;
        //    }
        //    else
        //    {

        //        pick3 = true;

        //        images[2] = image1;
        //        pannel3.Background = System.Windows.Media.Brushes.Red;
        //    }

        //}

        //private void OutputImage240_MouseUp(object sender, MouseButtonEventArgs e)
        //{



        //    if (pick4)
        //    {

        //        images[3] = null;
        //        pick4 = false;
        //        pannel4.Background = System.Windows.Media.Brushes.White;
        //    }
        //    else
        //    {

        //        images[3] = image3;
        //        pick4 = true;
        //        pannel4.Background = System.Windows.Media.Brushes.Red;


        //    }

        //}

        private void ShowMaskCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                showMask = true;
                GetFolders(temp);
            }

        }

        private void ShowMaskCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (temp != null)
            {
                showMask = false;
                GetFolders(temp);
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            if (folder == null) return;
            if (folder.Exists)
            {
                DirectoryPath.Clear();
                foreach (DirectoryInfo folderInfo in folder.GetDirectories())
                {
                    DirectoryPath.Add(folderInfo);

                    //TextBlock0.Text += folderInfo.ToString();
                    //temps.Add(new Temp() { path = folderInfo.ToString() });
                }

                if (DirectoryPath.Count == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Please Select a Different Folder.");
                    return;
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                if (showMask == false)
                {
                    ShowMaskCheckbox_Checked(null, null);
                    //ShowMaskCheckbox.IsChecked = true;
                }
                else
                {
                    ShowMaskCheckbox_Unchecked(null, null);
                    //ShowMaskCheckbox.IsChecked = false;
                }

            }

        }



    }
}
