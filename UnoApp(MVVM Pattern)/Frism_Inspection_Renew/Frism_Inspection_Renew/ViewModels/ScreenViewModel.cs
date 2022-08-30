using Frism_Inspection_Renew.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.ViewModels
{
    public class ScreenViewModel : ViewModelBase
    {
        private ScreenModel _screenModel;



        public ScreenViewModel()
        {

            InitVariables();
            InitCommands();
        }


        public void InitVariables()
        {
            try
            {

                _screenModel = new ScreenModel();

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public void InitCommands()
        {
            try
            {

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
