/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/Index
        public ActionResult Index(string message)
        {
            return View((object)message);
        }
    }
}