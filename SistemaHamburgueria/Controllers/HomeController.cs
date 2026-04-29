using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
