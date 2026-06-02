using SistemaHamburgueria.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers
{
    public class ProdutoController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        // GET: Produto
        public ActionResult Index()
        {
            var produtos = db.Produtos.Include("Categoria").ToList();
            return View(produtos);
        }

        // GET: Produto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var produto = db.Produtos
                .Include("Categoria")
                .Include("ProdutoIngredientes.Ingrediente")
                .FirstOrDefault(p => p.Id == id);

            if (produto == null)
                return HttpNotFound();

            return View(produto);
        }

        // GET: Produto/Create
        public ActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome");
            ViewBag.Ingredientes = db.Ingredientes.ToList();
            return View();
        }

        // POST: Produto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Produto produto, int[] ingredienteIds, decimal[] quantidades)
        {
            if (ModelState.IsValid)
            {
                db.Produtos.Add(produto);
                db.SaveChanges();

                if (ingredienteIds != null)
                {
                    for (int i = 0; i < ingredienteIds.Length; i++)
                    {
                        db.ProdutoIngredientes.Add(new ProdutoIngrediente
                        {
                            ProdutoId = produto.Id,
                            IngredienteId = ingredienteIds[i],
                            Quantidade = quantidades != null && quantidades.Length > i ? quantidades[i] : 0
                        });
                    }
                    db.SaveChanges();
                }

                TempData["Sucesso"] = "Produto criado com sucesso.";
                return RedirectToAction("Index");
            }

            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", produto.CategoriaId);
            ViewBag.Ingredientes = db.Ingredientes.ToList();
            return View(produto);
        }

        // GET: Produto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var produto = db.Produtos
                .Include("ProdutoIngredientes")
                .FirstOrDefault(p => p.Id == id);

            if (produto == null)
                return HttpNotFound();

            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", produto.CategoriaId);
            ViewBag.Ingredientes = db.Ingredientes.ToList();
            return View(produto);
        }

        // POST: Produto/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Produto produto, int[] ingredienteIds, decimal[] quantidades)
        {
            if (ModelState.IsValid)
            {
                db.Entry(produto).State = EntityState.Modified;

                // Remove ingredientes antigos e recria
                var antigos = db.ProdutoIngredientes
                    .Where(pi => pi.ProdutoId == produto.Id).ToList();
                db.ProdutoIngredientes.RemoveRange(antigos);

                if (ingredienteIds != null)
                {
                    for (int i = 0; i < ingredienteIds.Length; i++)
                    {
                        db.ProdutoIngredientes.Add(new ProdutoIngrediente
                        {
                            ProdutoId = produto.Id,
                            IngredienteId = ingredienteIds[i],
                            Quantidade = quantidades != null && quantidades.Length > i ? quantidades[i] : 0
                        });
                    }
                }

                db.SaveChanges();
                TempData["Sucesso"] = "Produto atualizado com sucesso.";
                return RedirectToAction("Index");
            }

            ViewBag.CategoriaId = new SelectList(db.Categorias, "Id", "Nome", produto.CategoriaId);
            ViewBag.Ingredientes = db.Ingredientes.ToList();
            return View(produto);
        }

        // GET: Produto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var produto = db.Produtos
                .Include("Categoria")
                .Include("ProdutoIngredientes.Ingrediente")  // adicione isso
                .FirstOrDefault(p => p.Id == id);

            if (produto == null)
                return HttpNotFound();

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var produto = db.Produtos.Find(id);

            // Remove ingredientes vinculados antes de deletar
            var ingredientes = db.ProdutoIngredientes
                .Where(pi => pi.ProdutoId == id).ToList();
            db.ProdutoIngredientes.RemoveRange(ingredientes);

            db.Produtos.Remove(produto);
            db.SaveChanges();

            TempData["Sucesso"] = "Produto removido com sucesso.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}