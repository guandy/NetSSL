using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetSSL
{
    /// <summary>
    /// 3DES加解密算法帮助类
    /// </summary>
    public static class DesHelper
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>加密后并经base64编码的字符串</returns>
        /// <remarks>静态方法，采用默认UTF8编码</remarks>
        public static string Encrypt(string text, string key)
        {
            return Encrypt(text, key, Encoding.UTF8);
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>加密后并经base64编码的字符串</returns>
        /// <remarks>重载，指定编码方式</remarks>
        public static string Encrypt(string text, string key, Encoding encoding)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = encoding.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform desEncrypt = DES.CreateEncryptor();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, desEncrypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }
                    byte[] bytes = msEncrypt.ToArray();
                    return ByteArrayToHexString(bytes);
                }
            }
            //return BitConverter.ToString(result);
        }

        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="text">要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后的字符串</returns>
        /// <exception cref="">密钥错误</exception>
        /// <remarks>静态方法，采用默认ascii编码</remarks>
        public static string Decrypt(string text, string key)
        {
            return Decrypt(text, key, Encoding.UTF8);
        }

        /// <summary>
        /// 3des解密字符串
        /// </summary>
        /// <param name="text">要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>解密后的字符串</returns>
        /// <exception cref="">密钥错误</exception>
        /// <remarks>静态方法，指定编码方式</remarks>
        public static string Decrypt(string text, string key, Encoding encoding)
        {
            byte[] data = Convert.FromBase64String(text);
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = encoding.GetBytes(key);
            DES.Mode = CipherMode.ECB;
            DES.Padding = PaddingMode.PKCS7;
            ICryptoTransform desencrypt = DES.CreateDecryptor();
            byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(result);

        }

        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
    }
}
