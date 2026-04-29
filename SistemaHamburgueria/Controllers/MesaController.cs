
using SistemaHamburgueria.Enums;
using SistemaHamburgueria.Models;
using System.Web.Mvc;
using System.Linq;

namespace SistemaHamburgueria.Controllers
{
    public class MesaController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        public ActionResult Index()
        {
            return View(db.Mesas.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Mesa mesa)
        {
            try
            {
                mesa.StatusMesa = StatusMesa.Livre;
                db.Mesas.Add(mesa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(mesa);
            }
        }
    }
}
