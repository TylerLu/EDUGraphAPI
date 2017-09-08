using EDUGraphAPI.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Security.Claims;

namespace EDUGraphAPI.Utils
{
    public enum Permissions
    {
        /// <summary>
        /// The client accesses the web API as the signed-in user.
        /// </summary>
        Delegated,
        /// <summary>
        /// The client accesses the web API directly as itself (no user context).
        /// </summary>
        /// <remarks>
        /// This type of permission requires administrator consent.
        /// </remarks>
        Application
    }

    /// <summary>
    /// A static helper class used to get access token, authentication result, authentication context and instances of service client.
    /// </summary>
    public static class AuthenticationHelper
    {

        /// <summary>
        /// Get an instance of AuthenticationContext
        /// </summary>
        public static AuthenticationContext GetAuthenticationContext(ClaimsIdentity claimsIdentity, Permissions permissions)
        {
            var tenantID = claimsIdentity.GetTenantId();
            var userId = claimsIdentity.GetObjectIdentifier();
            var signedInUserID = permissions == Permissions.Delegated ? userId : tenantID;

            var authority = string.Format("{0}{1}", Constants.AADInstance, tenantID);
            var tokenCache = new AdalTokenCache(signedInUserID);
            return new AuthenticationContext(authority, tokenCache);
        }
    }
}