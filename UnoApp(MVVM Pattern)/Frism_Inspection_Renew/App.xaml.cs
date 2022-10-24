using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Services;
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

        private readonly NavigationStore _navigationStore;
        private readonly SideBarViewModel _sideBarViewModel;

        public App()
        {
            _navigationStore = new NavigationStore();
            _sideBarViewModel = new SideBarViewModel(
                CreateHomeNaviationService(),
                CreateSetCameraNavigationService(),
                CreateDNNSettingNavigationService()
                ) ;
        }

        

        protected override void OnStartup(StartupEventArgs e)
        {

            NavigationService<MainViewModel> maineNavigationService = CreateHomeNaviationService();
            maineNavigationService.Navigate();


            MainWindow = new MainWindow()
            {
                DataContext = new WindowViewModel(_navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);


          

        }

        private NavigationService<MainViewModel> CreateHomeNaviationService()
        {
            return new NavigationService<MainViewModel>(_navigationStore,
                () => new MainViewModel(_sideBarViewModel));
        }
        private NavigationService<SetCameraViewModel> CreateSetCameraNavigationService()
        {
            return new NavigationService<SetCameraViewModel>(_navigationStore,
                () => new SetCameraViewModel(_sideBarViewModel));
        }

        private NavigationService<DNNSettingViewModel> CreateDNNSettingNavigationService()
        {
            return new NavigationService<DNNSettingViewModel>(_navigationStore,
                () => new DNNSettingViewModel(_sideBarViewModel));
        }
    }
}
