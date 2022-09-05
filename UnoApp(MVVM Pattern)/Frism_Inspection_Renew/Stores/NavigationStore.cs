using Frism_Inspection_Renew.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Stores
{
    public class NavigationStore
    {
        public event Action CurrentViewModelChanged;

        private ViewModelBase _currentViewModel;


        public ViewModelBase CurrentViewModel
        {

            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnCurrentViewModelChaged();

            }

        }

        private void OnCurrentViewModelChaged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
