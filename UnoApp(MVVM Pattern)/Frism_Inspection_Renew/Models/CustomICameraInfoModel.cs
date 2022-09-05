using Basler.Pylon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frism_Inspection_Renew.Models
{
    public class CustomICameraInfoModel : ICameraInfo
    {
        public string this[string key] => throw new NotImplementedException();

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public string GetValueOrDefault(string key, string defaultValue)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }





    }
}
