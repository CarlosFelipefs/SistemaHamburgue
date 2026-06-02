using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SistemaHamburgueria.Models;
using SistemaHamburgueria.Enums;

namespace SistemaHamburgueria.Controllers
{
    public class IngredienteController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        public ActionResult Index()
        {
            return View(db.Ingredientes.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.Unidades = new SelectList(
                System.Enum.GetValues(typeof(UnidadesMedidas))
                    .Cast<UnidadesMedidas>()
                    .Select(u => new { Value = (int)u, Text = u.ToString() }),
                "Value", "Text"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                db.Ingredientes.Add(ingrediente);
                db.SaveChanges();
                TempData["Sucesso"] = "Ingrediente criado com sucesso.";
                return RedirectToAction("Index");
            }
            ViewBag.Unidades = new SelectList(
                System.Enum.GetValues(typeof(UnidadesMedidas))
                    .Cast<UnidadesMedidas>()
                    .Select(u => new { Value = (int)u, Text = u.ToString() }),
                "Value", "Text"
            );
            return View(ingrediente);
        }

        public ActionResult Edit(int id)
        {
            var ingrediente = db.Ingredientes.Find(id);
            if (ingrediente == null) return HttpNotFound();

            ViewBag.Unidades = new SelectList(
                System.Enum.GetValues(typeof(UnidadesMedidas))
                    .Cast<UnidadesMedidas>()
                    .Select(u => new { Value = (int)u, Text = u.ToString() }),
                "Value", "Text", (int)ingrediente.unidade
            );
            return View(ingrediente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Ingrediente ingrediente)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingrediente).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Sucesso"] = "Ingrediente atualizado com sucesso.";
                return RedirectToAction("Index");
            }
            ViewBag.Unidades = new SelectList(
                System.Enum.GetValues(typeof(UnidadesMedidas))
                    .Cast<UnidadesMedidas>()
                    .Select(u => new { Value = (int)u, Text = u.ToString() }),
                "Value", "Text", (int)ingrediente.unidade
            );
            return View(ingrediente);
        }

        public ActionResult Delete(int id)
        {
            var ingrediente = db.Ingredientes.Find(id);
            if (ingrediente == null) return HttpNotFound();
            return View(ingrediente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ingrediente = db.Ingredientes.Find(id);
            db.Ingredientes.Remove(ingrediente);
            db.SaveChanges();
            TempData["Sucesso"] = "Ingrediente removido com sucesso.";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var ingrediente = db.Ingredientes
                .Include(i => i.ProdutoIngredientes.Select(pi => pi.Produto))
                .FirstOrDefault(i => i.id == id);
            if (ingrediente == null) return HttpNotFound();
            return View(ingrediente);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}