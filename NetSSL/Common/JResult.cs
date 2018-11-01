/***************************************************************************
*projectname:NetSSL.Common
*classname:JResult
*des:JResult
*author:guandy
*createtime:2018-11-01 10:58:28
*updatetime:2018-11-01 10:58:28
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    public class JResult
    {
        public static CommonResult Success(string msg)
        {
            return Success(msg, "0", null);
        }

        public static CommonResult Success(object data)
        {
            return Success("success", "0", data);
        }

        public static CommonResult Success(string msg, string code = "0", object data = null)
        {
            return new CommonResult()
            {
                Result = true,
                Message = msg,
                Code = code,
                Data = data
            };
        }

        public static CommonResult Error(string msg, string code = "-1",object data = null)
        {
            return new CommonResult()
            {
                Result = false,
                Message = msg,
                Code = code,
                Data = data
            };
        }
    }
}
