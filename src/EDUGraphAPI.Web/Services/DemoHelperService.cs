/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Web.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Web;

namespace EDUGraphAPI.Web.Services
{
    public class DemoHelperService
    {
        private static readonly string dataFilePath = "~/App_Data/demo-pages.json";

        public DemoPage GetDemoPage(string controller, string action)
        {
            var path = HttpContext.Current.Server.MapPath(dataFilePath);
            var json = File.ReadAllText(path);
            var pages = JsonConvert.DeserializeObject<DemoPage[]>(json);

            var page = pages
                .Where(i => i.Controller.IgnoreCaseEquals(controller))
                .Where(i => i.Action.IgnoreCaseEquals(action))
                .FirstOrDefault();
            if (page != null)
            {
                var sourceCodeRepositoryUrl = Constants.SourceCodeRepositoryUrl.TrimEnd('/');
                foreach (var link in page.Links)
                    link.Url = sourceCodeRepositoryUrl + link.Url;
            }
            return page;
        }
    }
}