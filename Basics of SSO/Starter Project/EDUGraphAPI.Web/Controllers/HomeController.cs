/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    public class HomeController : Controller
    {
        
        public HomeController()
        {
        }

        //
        // GET: /Home/Index
        public async Task<ActionResult> Index()
        {
            return RedirectToAction("Index", "Schools");
        }


    }
}