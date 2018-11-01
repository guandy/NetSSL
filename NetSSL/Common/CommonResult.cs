using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    public class CommonResult
    {
        /// <summary>
        /// 1-success other-falid
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// true-成功 false-失败
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
    }
}
