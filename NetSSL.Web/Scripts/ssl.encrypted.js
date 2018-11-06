+function ($)
{
    var ssl = window.ssl = window.ssl || {};

    var sectype = 1;//0:AES加密，1：DES加密

    
    var Encrypted = function ()
    {
    };

    /*************************
*DES随机密钥,8位
***************************/
    Encrypted.prototype.DesKV = function ()
    {
        var rdnum = parseInt(Math.random() * (99999999 - 10000000 + 1) + 10000000, 10);
        return rdnum.toString();
    }

    /*************************
*DES加密
*   str：需要加密的字符串
***************************/
    Encrypted.prototype.DesEncrypt = function (str, _KEY)
    {
        var keyHex = CryptoJS.enc.Utf8.parse(_KEY)

        var encrypted = CryptoJS.DES.encrypt(str, keyHex, {
            mode: CryptoJS.mode.ECB,
            padding: CryptoJS.pad.Pkcs7
        })
        return encrypted.toString()
    }

    /************************
    * DES解密
    *   str：需要解密的字符串
    *************************/
    Encrypted.prototype.DesDecrypt = function (str, _KEY)
    {
        var keyHex = CryptoJS.enc.Utf8.parse(_KEY)

        var encryptedHexStr = CryptoJS.enc.Hex.parse(str)
        var srcs = CryptoJS.enc.Base64.stringify(encryptedHexStr)

        var decrypted = CryptoJS.DES.decrypt(srcs, keyHex, {
            mode: CryptoJS.mode.ECB,
            padding: CryptoJS.pad.Pkcs7
        })
        return decrypted.toString(CryptoJS.enc.Utf8)
    }

    /*************************
    *AES随机密钥
    ***************************/
    Encrypted.prototype.AesKV = function ()
    {
        var rdnum = parseInt(Math.random() * (999999 - 100000 + 1) + 100000, 10);
        var iv = rdnum + "0000000000";//0000000000
        return iv;
    }

    /*************************
    *AES加密
    *   str：需要加密的字符串
    ***************************/
    Encrypted.prototype.AesEncrypt = function(str,_KEY,_IV)
    {
        var key = CryptoJS.enc.Utf8.parse(_KEY);
        var iv = CryptoJS.enc.Utf8.parse(_IV);

        var encrypted = '';

        var srcs = CryptoJS.enc.Utf8.parse(str);
        encrypted = CryptoJS.AES.encrypt(srcs, key, {
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });

        return encrypted.ciphertext.toString();
    }

    /************************
    * AES解密
    *   str：需要解密的字符串
    *************************/
    Encrypted.prototype.AesDecrypt = function(str,_KEY,_IV)
    {
        var key = CryptoJS.enc.Utf8.parse(_KEY);
        var iv = CryptoJS.enc.Utf8.parse(_IV);
        var encryptedHexStr = CryptoJS.enc.Hex.parse(str);
        var srcs = CryptoJS.enc.Base64.stringify(encryptedHexStr);
        var decrypt = CryptoJS.AES.decrypt(srcs, key, {
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
        var decryptedStr = decrypt.toString(CryptoJS.enc.Utf8);
        return decryptedStr.toString();
    }

    /************************
    * RSA公钥，加密返回的数据（可改成请求接口动态生成公钥）
    *************************/
    Encrypted.prototype.RsaPubKey = function ()
    {
        return "MEwwDQYJKoZIhvcNAQEBBQADOwAwOAIxAKQXOR4AGfNl0fUUFqPIX+SZ1Fq9FbMAY9pSFy+y2zJSGrjc4pZ4rKDVJhckTJyX+QIDAQAB";
    }
    /************************
    * RSA私钥，解密返回的数据（可改成请求接口动态生成私钥）
    *************************/
    Encrypted.prototype.RsaPriKey = function ()
    {
        return "MIIBDAIBADANBgkqhkiG9w0BAQEFAASB9zCB9AIBAAIxAM4Y46B9JPI5wl4qUZJboRXLj3QGLJCiW8ybW8iO9nkI6XXYXgZCsO+fNpStEEWxPQIDAQABAjEAhLomrg6qIozsDfS2/8ie3whvZEstnB/SUv6Cc7ElYdNU8VjAxGkueLB+0lJ0+efJAhkA46lfN4e4e4+4odtGL5DLCw2zefwmX2/XAhkA58BZmQZpJXDNtsE8+gVH76mSISxjEtULAhh3zdHFry5uF5vZ0UKGFXRERNmGACNRZdECGQCmGEueyuNs/A3Tn2cOYd6Ou9+JewB+rMUCGQCxUZOvT2blldwkUk17KUcCr1EekaXUFT8=";
    }
    /************************
   * 创建请求的数据
   * return * //secret：密钥加密数据  encryption：业务加密数据 sectype：加密类型
   *************************/
    Encrypted.prototype.createEncryptData = function (data)
    {
        debugger
        var g = this, secret, encryption,key;
        var dataJosn = JSON.stringify(data);
        if (sectype == 0)//AES加密
        {
            key = g.AesKV();
            encryption = g.AesEncrypt(dataJosn, key + key, key);
        }
        else if (sectype == 1)//DES加密
        {
            key = g.DesKV();
            encryption = g.DesEncrypt(dataJosn, key);
        }
        
        var rsa = new JSEncrypt();
        rsa.setPublicKey(g.RsaPubKey());
        secret = encodeURI(rsa.encrypt(key)).replace(/\+/g, '%2B');//RSA加密 密钥
        return { secret: secret, encryption: encryption, sectype: sectype }
    }

    /************************
   * 解密返回的数据
   *************************/
    Encrypted.prototype.getDecryptData = function (result)
    {
        if (!result.secret)
            return result;
        var data,g = this;
        var rsa = new JSEncrypt();
        rsa.setPrivateKey(g.RsaPriKey());
        var key = rsa.decrypt(result.secret);
        if (result.sectype == 0)
        {
            data = g.AesDecrypt(result.encryption, key + key, key);
        }
        else
        {
            data = g.DesDecrypt(result.encryption, key);
        }
        return data;
    }
    /************************
   * SSL加密请求
   *************************/
    Encrypted.prototype.post = function (url,data,callback)
    {
        var g = this;
        var postdata = g.createEncryptData(data);
        console.log("加密请求数据："+JSON.stringify(postdata));
        $.ajax({
            url: url,
            data: postdata,
            contentType: 'application/json',
            type: "POST",
            success: function (result)
            {
                var data = g.getDecryptData(result);
                if (callback)
                    callback(JSON.parse(data));
            }
        });
    }
    
    $.extend(ssl, {
        encrypted: new Encrypted(),
    });

}(jQuery);