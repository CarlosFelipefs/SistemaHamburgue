
using SistemaHamburgueria.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers   
{
    [Authorize]
    public class ClienteController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        public ActionResult Index()
        {
            try
            {
                return View(db.Clientes.ToList());
            }
            catch
            {
                return View("Error");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(cliente);
            }
            catch
            {
                ModelState.AddModelError("", "Erro ao salvar");
                return View(cliente);
            }
        }

        public ActionResult Edit(int? id)
        {
            var cliente = db.Clientes.Find(id);
            return View(cliente);
        }

        [HttpPost]
        public ActionResult Edit(Cliente cliente)
        {
            try
            {
                db.Entry(cliente).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View(cliente);
            }
        }

        public ActionResult Delete(int? id)
        {
            var cliente = db.Clientes.Find(id);
            return View(cliente);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var cliente = db.Clientes.Find(id);
                db.Clientes.Remove(cliente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
    }