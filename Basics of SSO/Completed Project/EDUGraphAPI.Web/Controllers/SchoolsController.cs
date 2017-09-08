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