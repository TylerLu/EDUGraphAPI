/*   
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.  
 *   * See LICENSE in the project root for license information.  
 */
using EDUGraphAPI.Data;
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [Authorize]
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