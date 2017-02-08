/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Collections.Generic;

namespace EDUGraphAPI.Web.Models
{
    public class DemoPage
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public DemoPage()
        {
            this.Links = new HashSet<Link>();
        }

        public ISet<Link> Links { get; set; }
    }

    public class Link
    {
        public string Title { get; set; }

        public string Url { get; set; }
    }
}