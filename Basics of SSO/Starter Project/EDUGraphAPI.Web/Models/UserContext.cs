/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Data;
using Microsoft.AspNet.Identity;
using System.Web;

namespace EDUGraphAPI.Web.Models
{
    /// <summary>
    /// Context for the logged-in user
    /// </summary>
    public class UserContext
    {
        public UserContext(HttpContext httpContext, ApplicationUser user)
        {
            this.HttpContext = httpContext;
            this.User = user;
        }

        /// <summary>
        /// The HttpContext
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// The underlying user
        /// </summary>
        /// For unlinked Office 365 user, the property is null.
        /// <remarks>
        public ApplicationUser User { get; private set; }

        /// <summary>
        /// Display name of the logged-in user
        /// </summary>
        public string UserDisplayName => HttpContext.User.Identity.GetUserName();

      
    }
}