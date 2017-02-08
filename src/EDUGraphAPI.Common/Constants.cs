/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System;
using System.Collections.Generic;
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

        public static readonly string AADInstance = "https://login.microsoftonline.com/";
        public static readonly string Authority = AADInstance + "common/";

        public const string GraphResourceRootUrl = "https://graph.windows.net";

        public static readonly string BingMapKey = ConfigurationManager.AppSettings["BingMapKey"];

        public static readonly string SourceCodeRepositoryUrl = ConfigurationManager.AppSettings["SourceCodeRepositoryUrl"];
        
        public static readonly string O365GroupConversationsUrl = "https://outlook.office.com/owa/?path=/group/{0}/mail&exsvurl=1&ispopout=0";

        public static readonly string AADCompanyAdminRoleName = "Company Administrator";

        public static readonly List<ColorEntity> FavoriteColors = new List<ColorEntity>() { new ColorEntity() { DisplayName= "Blue",Value= "#2F19FF" }, new ColorEntity() { DisplayName = "Green", Value = "#127605" }, new ColorEntity() { DisplayName = "Grey", Value = "#535353" } };
        public static readonly string UsernameCookie = "O365CookieUsername";
        public static readonly string EmailCookie = "O365CookieEmail";

        public static class Resources
        {
            public static readonly string AADGraph = "https://graph.windows.net";
            public static readonly string MSGraph = "https://graph.microsoft.com";
        }

        public static class Roles
        {
            public static readonly string Admin = "Admin";
            public static readonly string Faculty = "Faculty";
            public static readonly string Student = "Student";
        }

        public static class O365ProductLicenses
        {
            /// <summary>
            /// Microsoft Classroom Preview
            /// </summary>
            public static readonly Guid Classroom = new Guid("80f12768-d8d9-4e93-99a8-fa2464374d34");
            /// <summary>
            /// Office 365 Education for faculty
            /// </summary>
            public static readonly Guid Faculty = new Guid("94763226-9b3c-4e75-a931-5c89701abe66");
            /// <summary>
            /// Office 365 Education for students
            /// </summary>
            public static readonly Guid Student = new Guid("314c4481-f395-4525-be8b-2ec4bb1e9d91");
            /// <summary>
            /// Office 365 Education for faculty
            /// </summary>
            public static readonly Guid FacultyPro = new Guid("78e66a63-337a-4a9a-8959-41c6654dfb56");
            /// <summary>
            /// Office 365 Education for students
            /// </summary>
            public static readonly Guid StudentPro = new Guid("e82ae690-a2d5-4d76-8d30-7c6e01e6022e");
        }

    }
    public class ColorEntity
    {
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }
}