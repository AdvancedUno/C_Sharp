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
    public class AccountViewModel : ViewModelBase
    {
        private readonly AccountStore _accountStore;
        public string Username => _accountStore.CurrentAccount?.Username;
        public string Email => _accountStore.CurrentAccount?.Email;



        public ICommand NavigateHomeCommand { get; }
        public AccountViewModel(AccountStore accountStore, NavigationStore navigationStore)
        {
            _accountStore = accountStore;

            NavigateHomeCommand = new NavigateCommand<MainViewModel>(
                new NavigationService<MainViewModel>(
                    navigationStore, () => new MainViewModel(navigationStore)));

        }
    }
}
