using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    public class Decrypt
    {
        public string secret { get; set; }

        public int sectype { get; set; }

        public string encryption { get; set; }

        public bool isdefault { get; set; }
    }

    public class AesKV {
        public string key { get; set; }

        public string iv { get; set; }
    }
}
