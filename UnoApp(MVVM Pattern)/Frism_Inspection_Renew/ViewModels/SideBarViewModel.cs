using Frism_Inspection_Renew.Commands;
using Frism_Inspection_Renew.Services;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Frism_Inspection_Renew.ViewModels
{
    public class SideBarViewModel: ViewModelBase

    {

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateSetCameraCommand { get; }
        public ICommand NavigateSetDnnCommand { get; }

        public ICommand NavigateHome { get; }
        public ICommand NavigateSetCamera { get; }
        public ICommand NavigateSetDnn { get; }

        public Action<object>  NavigateHomeChanged;
        public Action<object>  NavigateSetCameraChanged;
        public Action<object>  NavigateDNNSettingChanged;


        public SideBarViewModel(NavigationService<MainViewModel> homeNavigationService, NavigationService<SetCameraViewModel> setCameraNavigationService, NavigationService<DNNSettingViewModel> dnnSettingNavigationService)
        {
            try
            {
                NavigateHomeCommand = new NavigateCommand<MainViewModel>(homeNavigationService);
                NavigateSetCameraCommand = new NavigateCommand<SetCameraViewModel>(setCameraNavigationService);
                NavigateSetDnnCommand = new NavigateCommand<DNNSettingViewModel>(dnnSettingNavigationService);
                NavigateHome = new Command(NavigateHomeBtnRun, CanExecute_func);
                NavigateSetCamera = new Command(NavigateSetCameraBtnRun, CanExecute_func);
                NavigateSetDnn = new Command(NavigateDNNSettingBtnRun, CanExecute_func);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " SideBarViewModel");
            }
        }

        private bool CanExecute_func(object obj)
        {
            return true;
        }

        private void NavigateHomeBtnRun(object obj)
        {
            try
            {
                NavigateHomeChanged.Invoke(obj);
                NavigateHomeCommand.Execute(obj);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " NavigateHomeBtnRun");
            }
        }

        private void NavigateSetCameraBtnRun(object obj)
        {
            try
            {
                NavigateSetCameraChanged.Invoke(obj);
                NavigateSetCameraCommand.Execute(obj);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message + " NavigateSetCameraBtnRun");
            }
        }

        private void NavigateDNNSettingBtnRun(object obj)
        {

            try
            {
                NavigateDNNSettingChanged.Invoke(obj);
                NavigateSetDnnCommand.Execute(obj);
            }
            catch (Exception exception) 
            {
                Console.WriteLine(exception.Message + " NavigateDNNSettingBtnRun");
            }
        }



    }
}
