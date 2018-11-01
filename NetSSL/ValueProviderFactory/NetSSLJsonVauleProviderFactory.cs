
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace NetSSL
{
    /// <summary>
    /// 重写JsonVauleProviderFactory，把加密数据解密成JSON数据传给Action
    /// </summary>
    public class NetSSLJsonVauleProviderFactory : ValueProviderFactory
    {
        private static void AddToBackingStore(EntryLimitedDictionary backingStore, string prefix, object value)
        {
            IDictionary<string, object> d = value as IDictionary<string, object>;
            if (d != null)
            {
                foreach (KeyValuePair<string, object> entry in d)
                {
                    AddToBackingStore(backingStore, MakePropertyKey(prefix, entry.Key), entry.Value);
                }
                return;
            }

            IList l = value as IList;
            if (l != null)
            {
                for (int i = 0; i < l.Count; i++)
                {
                    AddToBackingStore(backingStore, MakeArrayKey(prefix, i), l[i]);
                }
                return;
            }

            // primitive
            backingStore.Add(prefix, value);
        }

        private static object GetDeserializedObject(ControllerContext controllerContext)
        {
            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                // not JSON request
                return null;
            }

            StreamReader reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            string bodyText = reader.ReadToEnd();
            if (String.IsNullOrEmpty(bodyText))
            {
                // no JSON data
                return null;
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            object jsonData = serializer.DeserializeObject(bodyText);
            return jsonData;
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");
            if (!controllerContext.HttpContext.Request.ContentType.
            StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null;

            var jsonString = string.Empty;
            var requestJson = string.Empty;
            using (var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream))
            {
                requestJson = reader.ReadToEnd();
            }

            if (string.IsNullOrEmpty(requestJson))
                return null;
            int count = 0;
            if (requestJson.Contains("secret") && requestJson.Contains("encryption"))//加密请求
            {
                jsonString = GetDecryptPost(requestJson, ref count);
            }
            else
            {
                jsonString = requestJson;//非加密请求
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            try
            {
                object jsonData = serializer.DeserializeObject(jsonString);
                Dictionary<string, object> backingStore = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                EntryLimitedDictionary backingStoreWrapper = new EntryLimitedDictionary(backingStore);
                AddToBackingStore(backingStoreWrapper, String.Empty, jsonData);
                return new DictionaryValueProvider<object>(backingStore, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(Utils.RsaError);
            }
        }

        private static string GetDecryptPost(string jsonString, ref int count)
        {
            var decrypt = Utils.GetDecrypt(jsonString);
            string valueString = decrypt.encryption;
            string keyString = decrypt.secret;
            bool isDefault = decrypt.isdefault;
            if (count > 3)
                throw new NetSSLException("非json格式请求");
            try
            {
                RsaHelper rsa = new RsaHelper(Utils.RsaPriKey(decrypt.isdefault));
                keyString = rsa.Decrypt(keyString);
                var aesKV = keyString;
                if (decrypt.sectype == SecType.Aes.GetHashCode())
                {
                    AESHelper aes = new AESHelper($"{keyString}{keyString}", keyString);
                    valueString = aes.Decrypt(valueString);//修改数据
                }
                else if ((decrypt.sectype == SecType.Des.GetHashCode()))
                {
                    valueString = DesHelper.Decrypt(valueString, aesKV);//修改数据
                }

                try
                {
                    var serializer = new JavaScriptSerializer();
                    object obj = serializer.Deserialize(valueString, typeof(object));
                }
                catch (Exception ex)
                {
                    count++;
                    return GetDecryptPost(jsonString, ref count);
                }
            }
            catch (Exception ex)
            {
                throw new NetSSLException(Utils.RsaError,"40001");
            }
            return valueString;
        }

        private static string MakeArrayKey(string prefix, int index)
        {
            return prefix + "[" + index.ToString(CultureInfo.InvariantCulture) + "]";
        }

        private static string MakePropertyKey(string prefix, string propertyName)
        {
            return (String.IsNullOrEmpty(prefix)) ? propertyName : prefix + "." + propertyName;
        }

        private class EntryLimitedDictionary
        {
            private static int _maximumDepth = GetMaximumDepth();
            private readonly IDictionary<string, object> _innerDictionary;
            private int _itemCount = 0;

            public EntryLimitedDictionary(IDictionary<string, object> innerDictionary)
            {
                _innerDictionary = innerDictionary;
            }

            public void Add(string key, object value)
            {
                if (++_itemCount > _maximumDepth)
                {
                    throw new InvalidOperationException("JsonValueProviderFactory_RequestTooLarge");
                }

                _innerDictionary.Add(key, value);
            }

            private static int GetMaximumDepth()
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                {
                    string[] valueArray = appSettings.GetValues("aspnet:MaxJsonDeserializerMembers");
                    if (valueArray != null && valueArray.Length > 0)
                    {
                        int result;
                        if (Int32.TryParse(valueArray[0], out result))
                        {
                            return result;
                        }
                    }
                }

                return 1000; // Fallback default
            }
        }
    }
}
