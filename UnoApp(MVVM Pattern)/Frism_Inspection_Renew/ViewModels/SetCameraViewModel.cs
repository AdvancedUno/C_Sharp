using Frism_Inspection_Renew.Commands;
using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Frism_Inspection_Renew.ViewModels
{


    public class SetCameraViewModel : ViewModelBase
    {
        public string WelcomeMessage => "aaaa";

        public ICommand NavigateHomeViewCommand { get; }


        public SetCameraViewModel(NavigationStore navigationStore)
        {
            NavigateHomeViewCommand = new NavigateCommand<MainViewModel>(new NavigationService<MainViewModel>(navigationStore, () => new MainViewModel(navigationStore)));
        }

    }
}
