using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EDUGraphAPI.Web.Services
{
    public class CookieService
    {
        static readonly string UsernameCookie = Constants.UsernameCookie;
        static readonly string EmailCookie = Constants.EmailCookie;
        public string GetCookiesOfUsername()
        {
            var user = HttpContext.Current.Request.Cookies.Get(UsernameCookie);
            if (user == null)
                return string.Empty;
            return user.Value;
        }
        public string GetCookiesOfEmail()
        {
            var email = HttpContext.Current.Request.Cookies.Get(EmailCookie);
            if (email == null)
                return string.Empty;
            return email.Value;
        }

    }
}