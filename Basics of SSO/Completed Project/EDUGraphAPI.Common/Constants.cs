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