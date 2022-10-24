using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using Frism_Inspection_Renew.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Commands
{
    public class NavigateCommand<TViewModel> : CommandBase
        where TViewModel : ViewModelBase
    {
        //private readonly NavigationStore _navigationStore;
        //private readonly Func<TViewModel> _createViewModel;
        private readonly NavigationService<TViewModel> _navigationService;

    

        public NavigateCommand(NavigationService<TViewModel> navigationService)
        {
            _navigationService = navigationService;
            //_navigationStore = navigationStore;
            //_createViewModel = createViewModel;
        }

        public override void Execute(object parameter)
        {
            
            _navigationService.Navigate();
            //_navigationStore.CurrentViewModel = _createViewModel();
        }


    }
}
