using SistemaHamburgueria.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();
        public ActionResult Index()
        {
            try
            {
                return View(db.Categorias.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Categoria.Index] Erro: {ex.Message}");
                return View("Error");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categoria categoria)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(categoria);

                db.Categorias.Add(categoria);
                db.SaveChanges();
                TempData["Sucesso"] = "Categoria criada com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Categoria.Create] Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao salvar a categoria.");
                return View(categoria);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var categoria = db.Categorias.Find(id);
            if (categoria == null) return HttpNotFound();
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categoria categoria)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(categoria);

                db.Entry(categoria).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["Sucesso"] = "Categoria atualizada!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Categoria.Edit] Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao atualizar.");
                return View(categoria);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var categoria = db.Categorias.Find(id);
            if (categoria == null) return HttpNotFound();
            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var categoria = db.Categorias.Find(id);
                db.Categorias.Remove(categoria);
                db.SaveChanges();
                TempData["Sucesso"] = "Categoria removida!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Categoria.Delete] Erro: {ex.Message}");
                TempData["Erro"] = "Não foi possível remover. Verifique se há produtos vinculados.";
                return RedirectToAction("Index");
            }
        }

    }
}
