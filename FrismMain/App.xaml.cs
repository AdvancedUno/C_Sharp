
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;

namespace Frism
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static string[] Args;


        [DllImport("INTELLIZ_D_INSPECT.dll", CharSet = CharSet.None, EntryPoint = "InitInspectNet", ExactSpelling = true)]
        extern public static int InitInspectNet(int maxThreadCount);


        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                int error = InitInspectNet(0);

            }catch(Exception exception)
            {
                Logger.Debug(exception);
            }
            finally
            {
            }
            

   
            Logger.Debug("Start App");

            if (e.Args.Length > 0)
            {
                Args = e.Args;
                Logger.Debug("Cmd Args = {0}", Args);
            }

            var splashScreen = new SplashScreenWindow();
            this.MainWindow = splashScreen;

            splashScreen.Show();
            Thread.Sleep(1000);


            Task.Factory.StartNew(() =>
            {
                ////simulate some work being done
                //for (int i = 1; i <= 100; i++)
                //{
                //    //simulate a part of work being done
                //    System.Threading.Thread.Sleep(300);

                //    //because we're not on the UI thread, we need to use the Dispatcher
                //    //associated with the splash screen to update the progress bar
                //    splashScreen.Dispatcher.Invoke(() => splashScreen.Progress = i);
                //}

                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow;
                    mainWindow.Show();

                    splashScreen.Close();

                });
            });

            //var mainWindow = new MainWindow();
            //mainWindow.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.Debug("Exit App");
            //MessageBox.Show("App exit");

            var splashScreen = new SplashScreenWindow();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(5000);

                this.Dispatcher.Invoke(() =>
                {
                    splashScreen.Close();
                });
            });

        }
    
    }
}
