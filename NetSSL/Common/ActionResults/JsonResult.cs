using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mvc = System.Web.Mvc;

namespace NetSSL.ActionResults
{
    public class JsonResult : Mvc.JsonResult
    {
        /// <summary>
        /// 重写Controler中的Json方法
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var requestData = (context.Controller as NetSSLController).RequestData;
            if (JsonRequestBehavior == Mvc.JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("指定的操作不允许Get的Ajax请求方式访问");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                if (requestData.Contains("secret") && requestData.Contains("encryption"))
                {
                    var decrypt = Utils.GetDecrypt(requestData);
                    RsaHelper rsa = new RsaHelper(null, Utils.RsaPubKey(decrypt.isdefault));
                   
                    string secret = string.Empty;
                    string encryption = string.Empty;
                    if (decrypt.sectype == SecType.Des.GetHashCode())
                    {
                        var desKv = Utils.DesKV();

                        secret = rsa.Encrypt(desKv);
                        encryption = DesHelper.Encrypt(JsonHelper.Serialize(Data), desKv);
                    }
                    else
                    {
                        var aesKv = Utils.AesKV();
                        AESHelper aes = new AESHelper($"{aesKv}{aesKv}", aesKv);

                        secret = rsa.Encrypt(aesKv);
                        encryption = aes.Encrypt(JsonHelper.Serialize(Data));
                    }

                    try
                    {
                        response.Write(JsonHelper.Serialize(new Decrypt
                        {
                            secret = secret,
                            encryption = encryption,
                            isdefault = decrypt.isdefault,
                            sectype = decrypt.sectype
                        }));
                    }
                    catch (Exception ex)
                    {
                        response.Write(JsonHelper.Serialize(JResult.Error($"加密失败,异常:{ex.Message}")));
                    }
                }
                else
                {
                    response.Write(JsonHelper.Serialize(Data));
                }
            }
        }

        private Decrypt GetDecryptResult(string requestData,object Data) {
            var decrypt = Utils.GetDecrypt(requestData);
            RsaHelper rsa = new RsaHelper(null, Utils.RsaPubKey(decrypt.isdefault));
            var aesKv = Utils.AesKV();

            AESHelper aes = new AESHelper($"{aesKv}{aesKv}", aesKv);

            return new Decrypt
            {
                secret = rsa.Encrypt(aesKv),
                encryption = aes.Encrypt(JsonHelper.Serialize(Data)),
                isdefault = decrypt.isdefault
            };
        }
    }
}
