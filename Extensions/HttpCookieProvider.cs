using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;

namespace MvcIdentity.Extensions
{
    public class HttpCookieProvider : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            NameValueCollection list = new NameValueCollection();
            var cookies = controllerContext.HttpContext.Request.Cookies;
            foreach(var key in cookies.AllKeys)
            {
                list.Add(key, cookies[key].Value);
            }
            return new NameValueCollectionValueProvider(
                list, CultureInfo.CurrentCulture);
        }
    }
}