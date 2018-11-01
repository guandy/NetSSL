using NetSSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NetSSL.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());//移除默认的JsonValueProviderFactory
            ValueProviderFactories.Factories.Add(new NetSSLJsonVauleProviderFactory());//加上SSL的JsonValueProviderFactory
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex is NetSSLException)
            {
                LogError(ex as NetSSLException);
            }
            Server.ClearError();
        }

        private void LogError(NetSSLException ex)
        {
            if (Response == null)
                return;
            Server.ClearError();
            Response.StatusCode = 200;
            Response.TrySkipIisCustomErrors = true;
            string exMsg = JsonHelper.Serialize(JResult.Error(ex.Message, ex.ExceptionCode));
            Response.Write(exMsg);
        }
    }
}
