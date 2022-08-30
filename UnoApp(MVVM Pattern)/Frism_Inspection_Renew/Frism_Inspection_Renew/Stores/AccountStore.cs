using Frism_Inspection_Renew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Stores
{
    public class AccountStore
    {
        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set
            {
                _currentAccount = value;
            }
        }
    }
}
