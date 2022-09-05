using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class IOSaveModel
    {
        private BlockingCollection<int> _saveBlowSignal;
        public BlockingCollection<int> SaveBlowSignal { get => _saveBlowSignal; set => _saveBlowSignal = value; }

        public IOSaveModel()
        {
            SaveBlowSignal = new BlockingCollection<int>();
        }

        public int GetImageInfoModel()
        {
            if (SaveBlowSignal.Count() < 1) return -1;
            return SaveBlowSignal.Take();
        }


    }
}
