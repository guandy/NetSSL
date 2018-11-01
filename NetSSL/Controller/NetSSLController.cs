using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace NetSSL
{
    public class NetSSLController : Controller
    {
        private string _RequestData;
        public string RequestData
        {
            get
            {
                if (_RequestData == null)
                    _RequestData = Utils.RequestData();
                return _RequestData;
            }
            set {
                _RequestData = value;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            base.OnActionExecuting(filterContext);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            this.RequestData = Utils.RequestData();
        }

        #region json
        protected override System.Web.Mvc.JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new ActionResults.JsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }
        #endregion
    }
}
