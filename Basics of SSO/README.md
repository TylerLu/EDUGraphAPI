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

5. Once registered, you should see a black page.

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
- **ida:ClientId**: use the Client Id of the app registration you created earlier.

- **ida:ClientSecret**: use the Key value of the app registration you created earlier.


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
​	This is a static class and contains app settings values and other constants.

​	To see how this file works in the Demo app, refer to the file located [here](../src/EDUGraphAPI.Common/Constants.cs) in the Demo app.


4. Add a new folder **Models** to the **EDUGraphAPI.Common** project. Add a new file named **AdalTokenCache.cs** in **Models** folder, remove all generated code and paste the following. 


   ```c#
   /*   
    *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
    *   * See LICENSE in the project root for license information.  
    */
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

   To see how this file works in the Demo app, refer to the file located [here](src/EDUGraphAPI.Common/Models/AdalTokenCache.cs) in the Demo app.














**Copyright (c) 2017 Microsoft. All rights reserved.**
