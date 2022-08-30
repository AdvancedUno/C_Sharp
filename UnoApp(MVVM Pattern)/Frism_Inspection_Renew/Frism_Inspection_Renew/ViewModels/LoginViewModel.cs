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
    public class LoginViewModel : ViewModelBase
    {

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }

        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(AccountStore accountStore, NavigationStore navigationStore)
        {
           NavigationService<MainViewModel> navigationService = new NavigationService<MainViewModel>(
           navigationStore, () => new MainViewModel(navigationStore));

            LoginCommand = new LoginCommand(this, accountStore, navigationService);


        }

    }
}
