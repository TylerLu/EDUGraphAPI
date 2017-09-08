# Basic SSO - .NET version

In this sample we show you how to integrate Azure Active Directory(Azure AD) to provide secure sign in and authorization. 

The code in the following sections is part of the full featured .NET app and presented as a new project for clarity and separation of functionality.

**Table of contents**
* [Register the application in Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Build and debug locally](#build-and-debug-locally)

# Build and deploy the Starter Project

This project can be opened with the edition of Visual Studio 2015 you already have, or download and install the Community edition to run, build and/or develop this application locally.

- [Visual Studio 2015 Community](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)

The starter project is a simple application with only SQL authentication configured.  By updating this project, you can see how to integrate O365 Single Sign On to an application with existing authentication.

1. Open Visual Studio 2015 as administrator, open the project under **Starter Project** folder. The starter project you can register a new user, login and then display a basic page with login user info.

2. Deploy the application locally by pressing F5.

3. Click the Register link to register as a user.

   ![proj01](Images/proj03.png)

4. Complete the form to add a user.

   ![proj01](Images/proj01.png)

5. Once registered, you should see a blank page.

   ![proj03](Images/proj02.png)

# Add O365 Single Sign On to the starter project
Follow these instructions to add SSO functionality to the starter project application.  You will need to configure your app in Azure, create files and copy and paste code from the instructions.  

All code referenced in these instructions is also used in the associated files in the [Demo App](https://github.com/TylerLu/EDUGraphAPI/)

## Register the application in Azure Active Directory

1. Sign in to the Azure portal: [https://portal.azure.com/](https://portal.azure.com/).

2. Choose your Azure AD tenant by selecting your account in the top right corner of the page.

3. Click **Azure Active Directory** -> **App registrations** -> **+Add**.

4. Input a **Name**, and select **Web app / API** as **Application Type**.

   Input **Sign-on URL**: https://localhost:44377/

   ![](Images/aad-create-app-02.png)

   Click **Create**.

5. Once completed, the app will show in the list.

   ![](Images/aad-create-app-03.png)

6. Click it to view its details. 

   ![](Images/aad-create-app-04.png)

7. Click **All settings**, if the setting window did not show.

     ![](Images/aad-create-app-05.png)

     Copy aside **Application ID**, then Click **Save**.

   * Click **Reply URLs**, add the following URL into it.

     [https://localhost:44377/](https://localhost:44377/)

   * Click **Required permissions**. Add the following permissions:

     | API                            | Application Permissions | Delegated Permissions         |
     | ------------------------------ | ----------------------- | ----------------------------- |
     | Windows Azure Active Directory |                         | Sign in and read user profile |

     ![](Images/aad-create-app-06.png)

   * Click **Keys**, then add a new key

     ![](Images/aad-create-app-07.png)

     Click **Save**, then copy aside the **VALUE** of the key. 

   Close the Settings window.

## Add Single Sign On

1. Open the Starter Project in Visual Studio, if it isn't already open.  

2. In the EDUGraphAPI.Web project, open **Web.config** file, find **appSetings** section, replace the **appSettings** using the following code.  Two new keys **ida:ClientId** and **ida:ClientSecret** are added. These keys will be used to identity in your apps with Windows Azure Active Directory.


   ```xml
     <appSettings>
       <add key="webpages:Version" value="3.0.0.0" />
       <add key="webpages:Enabled" value="false" />
       <add key="ClientValidationEnabled" value="true" />
       <add key="UnobtrusiveJavaScriptEnabled" value="true" />
       <add key="ida:ClientId" value="INSERT YOUR CLIENT ID HERE" />
       <add key="ida:ClientSecret" value="INSERT YOUR CLIENT SECRET HERE" />
     </appSettings>
   ```

​	**ida:ClientId**: use the Client Id of the app registration you created earlier.

​	**ida:ClientSecret**: use the Key value of the app registration you created earlier.

3. Add a new file **Constants.cs** to the  **EDUGraphAPI.Common** project at the root, remove all generated code and paste the following.  

   ```c#
     using System.Configuration;

     namespace EDUGraphAPI
     {
         /// <summary>
         /// A static class contains values of app settings and other constant values
         /// </summary>
         public static class Constants
         {
             public static readonly string AADClientId = ConfigurationManager.AppSettings["ida:ClientId"];
             public static readonly string AADClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];

             public const string AADInstance = "https://login.microsoftonline.com/";
             public const string Authority = AADInstance + "common/";
             public static class Resources
             {
                 public const string AADGraph = "https://graph.windows.net";
                 public const string MSGraph = "https://graph.microsoft.com";
                 public const string MSGraphVersion = "beta";
             }

         }
     }
   ```

   ​This is a static class and contains app settings values and other constants.

   ​To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Common/Constants.cs) in the Demo app.

4. Add a new folder **Models** to the **EDUGraphAPI.Common** project. Add a new file named **AdalTokenCache.cs** in **Models** folder, remove all generated code and paste the following. 

   ```c#
   using EDUGraphAPI.Data;
   using Microsoft.IdentityModel.Clients.ActiveDirectory;
   using System;
   using System.Linq;
   using System.Web.Security;

   namespace EDUGraphAPI.Models
   {
       public class AdalTokenCache : TokenCache
       {
           private static readonly string MachinKeyProtectPurpose = "ADALCache";

           private string userId;

           public AdalTokenCache(string signedInUserId)
           {
               this.userId = signedInUserId;
               this.AfterAccess = AfterAccessNotification;
               this.BeforeAccess = BeforeAccessNotification;

               GetCahceAndDeserialize();
           }

           public override void Clear()
           {
               base.Clear();
               ClearUserTokenCache(userId);
           }

           void BeforeAccessNotification(TokenCacheNotificationArgs args)
           {
               GetCahceAndDeserialize();
           }

           void AfterAccessNotification(TokenCacheNotificationArgs args)
           {
               if (this.HasStateChanged)
               {
                   SerializeAndUpdateCache();
                   this.HasStateChanged = false;
               }
           }


           private void GetCahceAndDeserialize()
           {
               var cacheBits = GetUserTokenCache(userId);
               if (cacheBits != null)
               {
                   try
                   {
                       var data = MachineKey.Unprotect(cacheBits, MachinKeyProtectPurpose);
                       this.Deserialize(data);
                   }
                   catch { }
               }
           }

           private void SerializeAndUpdateCache()
           {
               var cacheBits = MachineKey.Protect(this.Serialize(), MachinKeyProtectPurpose);
               UpdateUserTokenCache(userId, cacheBits);
           }


           private byte[] GetUserTokenCache(string userId)
           {
               using (var db = new ApplicationDbContext())
               {
                   var cache = GetUserTokenCache(db, userId);
                   return cache != null ? cache.cacheBits : null;
               }
           }

           private void UpdateUserTokenCache(string userId, byte[] cacheBits)
           {
               using (var db = new ApplicationDbContext())
               {
                   var cache = GetUserTokenCache(db, userId);
                   if (cache == null)
                   {
                       cache = new UserTokenCache { webUserUniqueId = userId };
                       db.UserTokenCacheList.Add(cache);
                   }

                   cache.cacheBits = cacheBits;
                   cache.LastWrite = DateTime.UtcNow;

                   db.SaveChanges();
               }
           }

           private UserTokenCache GetUserTokenCache(ApplicationDbContext db, string userId)
           {
               return db.UserTokenCacheList
                      .OrderByDescending(i => i.LastWrite)
                      .FirstOrDefault(c => c.webUserUniqueId == userId);
           }

           private void ClearUserTokenCache(string userId)
           {
               using (var db = new ApplicationDbContext())
               {
                   var cacheEntries = db.UserTokenCacheList
                       .Where(c => c.webUserUniqueId == userId)
                       .ToArray();
                   db.UserTokenCacheList.RemoveRange(cacheEntries);
                   db.SaveChanges();
               }
           }

           public static void ClearUserTokenCache()
           {
               using (var db = new ApplicationDbContext())
               {
                   var cacheEntries = db.UserTokenCacheList
                       .ToArray();
                   db.UserTokenCacheList.RemoveRange(cacheEntries);
                   db.SaveChanges();
               }
           }
       }
   }
   ```

   This class is used by Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext to store access and refresh tokens.

   To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Common/Models/AdalTokenCache.cs) in the Demo app.

5. Add a new folder **Utils**  to the  **EDUGraphAPI.Common** project at the root. Add a new file named **AuthenticationHelper.cs** in **Utils** folder of the **EDUGraphAPI.Common** project, remove all generated code and paste the following.

   ```c#
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
   ```

   This is a static helper class used to get access token, authentication result, authentication context and instances of service client.

   To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Common/Utils/AuthenticationHelper.cs) in the Demo app.

6. Add a new file named **Startup.Auth.AAD.cs** in **App_Start** folder of the **EDUGraphAPI.Web** project, remove all generated code and paste the following. 

   ```c#
   using EDUGraphAPI.Data;
   using EDUGraphAPI.Utils;
   using Microsoft.IdentityModel.Clients.ActiveDirectory;
   using Microsoft.Owin.Security;
   using Microsoft.Owin.Security.Cookies;
   using Microsoft.Owin.Security.OpenIdConnect;
   using Owin;
   using System;
   using System.Threading.Tasks;
   using System.Web;

   namespace EDUGraphAPI.Web
   {
       public partial class Startup
       {
           private ApplicationDbContext db = new ApplicationDbContext();

           public void ConfigureAADAuth(IAppBuilder app)
           {
               app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

               app.UseCookieAuthentication(new CookieAuthenticationOptions { });

               app.UseOpenIdConnectAuthentication(
                   new OpenIdConnectAuthenticationOptions
                   {
                       Caption = "Microsoft Work or school account",
                       ClientId = Constants.AADClientId,
                       Authority = Constants.Authority,
                       TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                       {
                           // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                           // we inject our own multitenant validation logic
                           ValidateIssuer = false,
                       },
                       Notifications = new OpenIdConnectAuthenticationNotifications()
                       {
                           RedirectToIdentityProvider = (context) =>
                           {
                               // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                               // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                               // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                               string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                               context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                               context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                               return Task.FromResult(0);
                           },
                           AuthorizationCodeReceived = async (context) =>
                           {
                               var identity = context.AuthenticationTicket.Identity;

                               // Get token with authorization code
                               var redirectUri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                               var credential = new ClientCredential(Constants.AADClientId, Constants.AADClientSecret);
                               var authContext = AuthenticationHelper.GetAuthenticationContext(identity, Permissions.Delegated);
                               var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(context.Code, redirectUri, credential, Constants.Resources.AADGraph);


                           },
                           AuthenticationFailed = (context) =>
                           {
                               var redirectUrl = "/Error?message=" + Uri.EscapeDataString(context.Exception.Message);
                               context.OwinContext.Response.Redirect(redirectUrl);
                               context.HandleResponse(); // Suppress the exception
                               return Task.FromResult(0);
                           }
                       }
                   });
           }
       }
   }
   ```

    This is a classed used to define AAD authentication that will be added into the OWIN runtime.

    To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/App_Start/Startup.Auth.AAD.cs) in the Demo app.

7. Open **Startup.cs** on the root of **EDUGraphAPI.Web** project. Edit the method named **Configuration** and paste the following. 

   ```
           public void Configuration(IAppBuilder app)
           {
               ConfigureIdentityAuth(app);
               ConfigureAADAuth(app);
               ConfigureIoC(app);
           }
   ```

      This will add AAD authentication provider into the OWIN runtime.

      To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Startup.cs) in the Demo app.


8. Add a new file **_ExternalLoginsListPartial.cshtml** in the folder **Views/Account** of **EDUGraphAPI.Web** project.  Remove all generated code and paste the following.  

   ```html

   @model EDUGraphAPI.Web.Models.ExternalLoginListViewModel
   @using Microsoft.Owin.Security

   <h4 class="margin-btm-20">Use your school account</h4>
   @{
       var loginProviders = Context.GetOwinContext().Authentication.GetExternalAuthenticationTypes();
       if (loginProviders.Count() == 0) {
           <div>
               <p>
                   There are no external authentication services configured. See <a href="http://go.microsoft.com/fwlink/?LinkId=403804">this article</a>
                   for details on setting up this ASP.NET application to support logging in via external services.
               </p>
           </div>
       }
       else {
           using (Html.BeginForm("ExternalLogin", "Account", new { ReturnUrl = Model.ReturnUrl })) {
               @Html.AntiForgeryToken()
               <div id="socialLoginList">
                   <p>
                       @foreach (AuthenticationDescription p in loginProviders) {
                           <button type="submit" class="btn btn-default btn-ms-login" id="@p.AuthenticationType" name="provider" value="@p.AuthenticationType" title="Log in using your @p.Caption"></button>
                       }
                   </p>
               </div>
           }
       }
   }
   ```

   This defines O365 login button that will be shown on the login page.

   To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Views/Account/_ExternalLoginsListPartial.cshtml) in the Demo app.

9. Edit **Login.cshtml** in the folder **Views/Account** of **EDUGraphAPI.Web** project. On the bottom of the page search for the tag with id **socialLoginForm**, update the tag and paste the following.  

   ```html
               <section id="socialLoginForm">
                   @Html.Partial("_ExternalLoginsListPartial", new ExternalLoginListViewModel { ReturnUrl = ViewBag.ReturnUrl })
               </section>
   ```

   This adds an O365 login button on the right of login page.

   To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Views/Account/Login.cshtml) in the Demo app.

10. Add a new folder **Infrastructure** to the **EDUGraphAPI.Web** project. Add a new file named **EduAuthorizeAttribute.cs** in **Infrastructure** folder, remove all generated code and paste the following. 

    ```c#
    using System.Web.Mvc;

    namespace EDUGraphAPI.Web.Infrastructure
    {
        /// <summary>
        /// Allow the web app to redirect the current user to the proper login page in our multi-authentication-method scenario
        /// </summary>
        public class EduAuthorizeAttribute : AuthorizeAttribute
        {
            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                    filterContext.Result = new JsonResult { Data = new { error = "Unauthorized" } };
                else
                {
                    filterContext.Result = new RedirectResult("/Account/Login");

                }
            }
        }
    }
    ```

    This allows the web app to redirect the current user to the login page.

    To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Infrastructure/EduAuthorizeAttribute.cs) in the Demo app.

11. Edit **SchoolsController.cs** in the **Controllers** folder of **EDUGraphAPI.Web** project. Remove all code and paste the following. 

    ```c#
    using EDUGraphAPI.Web.Infrastructure;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    namespace EDUGraphAPI.Web.Controllers
    {
        [EduAuthorize]
        public class SchoolsController : Controller
        {
            public SchoolsController()
            {
            }

            public async Task<ActionResult> Index()
            {
                return View();
            }

            
        }
    }
    ```

    The attribute of **SchoolsController** is updated from **[Authorize]** to **[EduAuthorize]**. This will force redirect the user to login page if the user is not authorized. 

    To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Controllers/SchoolsController.cs) in the Demo app.

12. Edit **AccountController.cs** in folder **Controllers** of  **EDUGraphAPI.Web** project. Remove all code and paste the following. 

    ```c#
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Web.Models;
    using EDUGraphAPI.Web.Services;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web;


    namespace EDUGraphAPI.Web.Controllers
    {

        public class AccountController : Controller
        {
            private ApplicationSignInManager signInManager;
            private ApplicationUserManager userManager;

            private ApplicationService applicationService;


            public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,
                ApplicationService applicationService)
            {
                this.userManager = userManager;
                this.signInManager = signInManager;
                this.applicationService = applicationService;

            }

            //
            // GET: /Account/Login
            [AllowAnonymous]
            public ActionResult Login(string returnUrl)
            {
                ViewBag.ReturnUrl = returnUrl;

                return View();
            }

            //
            // POST: /Account/Login
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        ApplicationUser user = await applicationService.GetUserByEmailAsync(model.Email);
                        return RedirectToAction("Index", "Home");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }



            //
            // GET: /Account/Register
            [AllowAnonymous]
            public ActionResult Register()
            {
                EducationRegisterViewModel model = new EducationRegisterViewModel();
                return View(model);
            }

            //
            // POST: /Account/Register
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Register(EducationRegisterViewModel model)
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(result);
                }

                // If we got this far, something failed, redisplay form
                return View(model);
            }


            //
            // POST: /Account/LogOff
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult LogOff()
            {
                AuthenticationManager.SignOut(
                    DefaultAuthenticationTypes.ApplicationCookie,
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
                return RedirectToAction("Index", "Home");
            }
            //
            // POST: /Account/ExternalLogin
            [HttpPost]
            [AllowAnonymous]
            [ValidateAntiForgeryToken]
            public ActionResult ExternalLogin(string provider, string returnUrl)
            {
                // The line of code below is used to fix a bug that O365 accounts could not login in some cases.
                Session["AnyName"] = "AnyValue";

                // Request a redirect to the external login provider
                return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account",
                    new { ReturnUrl = returnUrl }));
            }

            //
            // GET: /Account/ExternalLoginCallback
            [AllowAnonymous]
            public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
            {
                var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    // AAD 
                    var authResult = await AuthenticationManager.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationType);
                    loginInfo = GetExternalLoginInfo(authResult);
                }

                return RedirectToLocal(returnUrl);


            }
            private ExternalLoginInfo GetExternalLoginInfo(AuthenticateResult result)
            {
                if ((result == null) || (result.Identity == null)) return null;

                var claim = result.Identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                if (claim == null) return null;

                var name = result.Identity.Name;
                if (name != null) name = name.Replace(" ", "");
                var str2 = result.Identity.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
                return new ExternalLoginInfo
                {
                    ExternalIdentity = result.Identity,
                    Login = new UserLoginInfo(claim.Issuer, claim.Value),
                    DefaultUserName = name,
                    Email = str2
                };
            }


            #region Helpers
            // Used for XSRF protection when adding external logins
            private const string XsrfKey = "XsrfId";

            private IAuthenticationManager AuthenticationManager
            {
                get
                {
                    return HttpContext.GetOwinContext().Authentication;
                }
            }

            private void AddErrors(IdentityResult result)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            private ActionResult RedirectToLocal(string returnUrl)
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            internal class ChallengeResult : HttpUnauthorizedResult
            {
                public ChallengeResult(string provider, string redirectUri)
                    : this(provider, redirectUri, null)
                {
                }

                public ChallengeResult(string provider, string redirectUri, string userId)
                {
                    LoginProvider = provider;
                    RedirectUri = redirectUri;
                    UserId = userId;
                }

                public string LoginProvider { get; set; }
                public string RedirectUri { get; set; }
                public string UserId { get; set; }

                public override void ExecuteResult(ControllerContext context)
                {
                    var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                    if (UserId != null)
                    {
                        properties.Dictionary[XsrfKey] = UserId;
                    }
                    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
                }
            }
            #endregion

        }
    }
    ```

    **ActionResult** named **ExternalLogin** is added to handle O365 user login and then redirect to a basic page.

    To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Web/Controllers/AccountController.cs) in the Demo app.


13. Deploy the application locally by pressing F5. 

14. Click the **Sign in with Office 365** button and then login to O365.

    ![proj04](Images/proj04.png)

15. After login with O365 user it will redirect to a basic page.

    ![proj05](Images/proj05.png)




**Copyright (c) 2017 Microsoft. All rights reserved.**