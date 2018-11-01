using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    public class NetSSLException : Exception
    {
        /// <summary>
        /// 异常编码
        /// </summary>
        public string ExceptionCode { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public NetSSLException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        public NetSSLException(string message, string code)
            : base(message)
        {
            this.ExceptionCode = code;
        }
    }
}
