using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NetSSL
{
    public class Utils
    {
        public static Decrypt GetDecrypt(string jsonString)
        {
            string valueString = "";
            string keyString = "";
            bool isDefault = false;
            int sectype = SecType.Aes.GetHashCode();

            if (jsonString.Contains("secret=") && jsonString.Contains("encryption="))
            {
                var kv = jsonString.Split('&');

                foreach (var item in kv)
                {
                    if (item.Contains("secret="))
                        keyString = item.Replace("secret=", "");
                    if (item.Contains("encryption="))
                        valueString = item.Replace("encryption=", "");
                    if (item.Contains("isdefault="))
                        isDefault = item.Replace("isdefault=", "") == "true";
                    if (item.Contains("sectype="))
                        sectype = int.Parse(item.Replace("sectype=", ""));
                }
            }
            else
            {
                try
                {
                    var result = JsonHelper.Deserialize<Decrypt>(jsonString);
                    valueString = result.encryption;
                    keyString = result.secret;
                    sectype = result.sectype;
                    isDefault = result.isdefault;
                }
                catch (Exception ex)
                {
                    throw new NetSSLException(ex.Message);
                    //WebLogger.Log(ex);
                }
            }

            keyString = HttpUtility.UrlDecode(keyString);
            keyString = keyString.Replace("%2B", "+");

            return new Decrypt() { secret = keyString, encryption = valueString, isdefault = isDefault, sectype = sectype };
        }

        public static string RequestData(int? sleepTime = null)
        {
            try
            {
                if (HttpContext.Current == null || HttpContext.Current.Request == null)
                    return string.Empty;
                if (HttpContext.Current.Request.Files != null && HttpContext.Current.Request.Files.Count > 0)
                    return string.Empty;
                var reader = new StreamReader(HttpContext.Current.Request.InputStream);
                if (reader.BaseStream.Position > 0)
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                var jsonStringData = reader.ReadToEnd();
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                return jsonStringData;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string DesKV()
        {
            var rd = new Random();
            return $"{rd.Next(10000000, 99999999)}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string AesKV()
        {
            var rd = new Random();
            return $"{rd.Next(100000, 999999)}0000000000";
        }

        /// <summary>
        /// RSA密钥
        /// </summary>
        /// <returns></returns>
        public static string RsaPriKey(bool isDefault)
        {
            return @"<RSAKeyValue><Modulus>pBc5HgAZ82XR9RQWo8hf5JnUWr0VswBj2lIXL7LbMlIauNzilnisoNUmFyRMnJf5</Modulus><Exponent>AQAB</Exponent><P>yv4AooLfXsHiYg9VqYrHA22TD7mje0sD</P><Q>zvCpmIm4B3/rzH8gRXky4rPYIIhYmcJT</Q><DP>AsgMNi3Y5bF+ap2PLO2L4I4lz7dZeB0F</DP><DQ>boG7vzZD7NVV4QU+AXuAPemWD4Ff9vP7</DQ><InverseQ>lRACnFDZJEqznqZcOdbK66wqNxO5S8f6</InverseQ><D>TEAWZKB7bqVj+VTKaHVWSi5JhZjUGogVIAKTwOP6ITt5tdNXnkEWRLf8JAoKjdGZ</D></RSAKeyValue>";
        }
        /// <summary>
        /// RSA明钥
        /// </summary>
        /// <returns></returns>
        public static string RsaPubKey(bool isDefault)
        {
            return @"<RSAKeyValue><Modulus>zhjjoH0k8jnCXipRkluhFcuPdAYskKJbzJtbyI72eQjpddheBkKw7582lK0QRbE9</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly string RsaError = "系统繁忙，请重新加载页面！";
    }
}
