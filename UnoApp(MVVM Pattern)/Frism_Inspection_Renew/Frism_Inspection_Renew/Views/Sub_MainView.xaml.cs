using Frism_Inspection_Renew.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frism_Inspection_Renew.Views
{
    /// <summary>
    /// Sub_MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Sub_MainView : UserControl
    {
        private Sub_MainViewModel _subMainViewModel = new Sub_MainViewModel();
        public Sub_MainViewModel SubMainViewModel
        {
            get { return _subMainViewModel; }
            set { _subMainViewModel = value; }
        }

        public Sub_MainView()
        {
            InitializeComponent();
            this.DataContext = SubMainViewModel;
        }
    }
}
