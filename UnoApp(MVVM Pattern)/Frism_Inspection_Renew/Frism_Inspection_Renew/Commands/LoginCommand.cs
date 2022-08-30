using Frism_Inspection_Renew.Models;
using Frism_Inspection_Renew.Services;
using Frism_Inspection_Renew.Stores;
using Frism_Inspection_Renew.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frism_Inspection_Renew.Commands
{
    public class LoginCommand : CommandBase
    {
        private readonly LoginViewModel _viewModel;
        private readonly NavigationService<MainViewModel> _navigationService;
        private readonly AccountStore _accountStore;

        public LoginCommand(LoginViewModel viewModel, AccountStore accountStore, NavigationService<MainViewModel> navigationService)
        {
            _viewModel = viewModel;
            _accountStore = accountStore;
            _navigationService = navigationService;

        }

        public override void Execute(object parameter)
        {
            MessageBox.Show($"Logging in {_viewModel.Username} ...");
            Account account = new Account()
            {
                Email = $"{_viewModel.Username}@test.com",
                Username = _viewModel.Username
            };
            _accountStore.CurrentAccount = account;
            _navigationService.Navigate();

        }

    }
}
