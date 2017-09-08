# Basic SSO - .NET version

In this sample we show you how to integrate Azure Active Directory(Azure AD) to provide secure sign in and authorization. 

The code in the following sections is part of the full featured .NET app and presented as a new project for clarity and separation of functionality.

**Table of contents**
* [Register the application in Azure Active Directory](#register-the-application-in-azure-active-directory)
* [Build and debug locally](#build-and-debug-locally)


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


## Build and debug locally

This project can be opened with the edition of Visual Studio 2015 you already have, or download and install the Community edition to run, build and/or develop this application locally.

- [Visual Studio 2015 Community](https://go.microsoft.com/fwlink/?LinkId=691978&clcid=0x409)

Debug the **EDUGraphAPI.Web**:

1. Open Visual Studio 2015 as administrator, open the project under **Starter Project** folder. In starter project you can register a new user, login and then display a basic page with login user info.
2. Open **Web.config** file, find **appSetings** section, replace the **appSettings** using the following code.  Two new keys **ida:ClientId** and **ida:ClientSecret** are added. These keys will be used to identity in your apps with Windows Azure Active Directory.


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


3. Add a new file **Constants.cs** on **EDUGraphAPI.Common** project root folder, delete all code and copy the following code to paste.  This file can also be found on GitHub: **URL to be added**.  This is a static class contains values of app settings and other constant values.

```
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
4. Add a new folder **Models** on **EDUGraphAPI.Common** project. Add a new file named **AdalTokenCache.cs** in **Models** folder.

















**Copyright (c) 2017 Microsoft. All rights reserved.**
