/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System;
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
        public void ClearCookies()
        {
            HttpContext.Current.Response.Cookies.Get(UsernameCookie).Expires = new DateTime(1970, 1, 1);
            HttpContext.Current.Response.Cookies.Get(EmailCookie).Expires = new DateTime(1970, 1, 1);
        }
    }
}