using NetSSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetSSL.Web
{
    public class HomeController : NetSSLController
    {

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Test(string id, string name, AesKV kv)
        {
            return Json(JResult.Success(new { Id = id + "+test", Name = name, Kv = kv }));
        }
    }
}