using Frism_Inspection_Renew.Stores;
using Frism_Inspection_Renew.ViewModels;
using Frism_Inspection_Renew.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace Frism_Inspection_Renew
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        
        public static string[] Args;
        protected override void OnStartup(StartupEventArgs e)
        {
            NavigationStore navigationStore = new NavigationStore();
            AccountStore accountStore = new AccountStore();

            navigationStore.CurrentViewModel = new LoginViewModel(accountStore, navigationStore);

            MainWindow = new MainWindow()
            {
                DataContext = new WindowViewModel(navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);


            //if (e.Args.Length > 0)
            //{
            //    Args = e.Args;

            //}

            //Task.Factory.StartNew(() =>
            //{
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        var mainWindow = new MainView();
            //        this.MainWindow = mainWindow;
            //        mainWindow.Show();

            //    });
            //});

        }
    }
}
