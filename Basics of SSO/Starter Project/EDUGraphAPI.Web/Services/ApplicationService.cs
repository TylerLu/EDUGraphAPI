/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Data;
using EDUGraphAPI.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace EDUGraphAPI.Web.Services
{
    /// <summary>
    /// An instance of the class handles getting/updating user/organization
    /// </summary>
    public class ApplicationService
    {
        static readonly string UserContextKey = typeof(UserContext).Name + "Context";
        static readonly string AdminContextKey = typeof(UserContext).Name + "AdminContextKey";

        private HttpContextBase httpContext;
        private ApplicationDbContext dbContext;
        private ApplicationUserManager userManager;

        public ApplicationService(ApplicationDbContext dbContext, ApplicationUserManager userManager, HttpContextBase httpContext)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.httpContext = httpContext;
        }

        public ApplicationService(ApplicationDbContext dbContext, ApplicationUserManager userManager) :
            this(dbContext, userManager, new HttpContextWrapper(HttpContext.Current))
        { }

        /// <summary>
        /// Get current user
        /// </summary>
        public ApplicationUser GetCurrentUser()
        {
            var userId = GetUserId();
            if (userId.IsNullOrEmpty()) return null;

            return dbContext.Users
                .Include(i => i.Organization)
                .Where(i => i.Id == userId || i.O365UserId == userId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get current user
        /// </summary>
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = GetUserId();
            if (userId.IsNullOrEmpty()) return null;

            return await dbContext.Users
                .Include(i => i.Organization)
                .Where(i => i.Id == userId || i.O365UserId == userId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        public async Task<ApplicationUser> GetUserAsync(string id)
        {
            return await dbContext.Users
                .Include(i => i.Organization)
                .Where(i => i.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get user by email.
        /// </summary>
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users               
                .Where(i => i.Email == email)
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get current user's context
        /// </summary>
        public UserContext GetUserContext()
        {
            var currentUser = GetCurrentUser();
            return new UserContext(HttpContext.Current, currentUser);
        }

        private string GetUserId()
        {
            var userId = httpContext.User.Identity.GetUserId();

            var aadUserId = httpContext.User.GetObjectIdentifier();
            if (aadUserId.IsNotNullAndEmpty()) userId = aadUserId;
            return userId;
        }

        /// <summary>
        /// Get current user's context
        /// </summary>
        public async Task<UserContext> GetUserContextAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            return new UserContext(HttpContext.Current, currentUser);
        }





    }
}