using SistemaHamburgueria.Models;
using System;
using System.Data.Entity;
using System.Linq;
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
                var clientes = db.Clientes.Include(c => c.Enderecos).ToList();
                return View(clientes);
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente, Endereco endereco)
        {
            try
            {
                // Remove validações do Endereco que não queremos obrigar
                ModelState.Remove("endereco.ClienteId");
                ModelState.Remove("endereco.Cliente");

                if (ModelState.IsValid)
                {
                    db.Clientes.Add(cliente);
                    db.SaveChanges();

                    // Só salva o endereço se pelo menos o CEP ou a Rua foi informado
                    bool temEndereco = !string.IsNullOrWhiteSpace(endereco.CEP)
                                   || !string.IsNullOrWhiteSpace(endereco.Rua);

                    if (temEndereco)
                    {
                        endereco.ClienteId = cliente.Id;
                        db.Enderecos.Add(endereco);
                        db.SaveChanges();
                    }

                    TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao salvar: " + ex.Message);
                return View(cliente);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var cliente = db.Clientes.Include(c => c.Enderecos).FirstOrDefault(c => c.Id == id);
            if (cliente == null) return HttpNotFound();

            // Passa o primeiro endereço para a ViewBag para preencher os campos na View
            ViewBag.Endereco = cliente.Enderecos?.FirstOrDefault() ?? new Endereco();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente, Endereco endereco)
        {
            try
            {
                ModelState.Remove("endereco.ClienteId");
                ModelState.Remove("endereco.Cliente");

                if (ModelState.IsValid)
                {
                    db.Entry(cliente).State = EntityState.Modified;

                    // Verifica se já existe endereço para o cliente
                    var enderecoExistente = db.Enderecos.FirstOrDefault(e => e.ClienteId == cliente.Id);

                    bool temEndereco = !string.IsNullOrWhiteSpace(endereco.CEP)
                                   || !string.IsNullOrWhiteSpace(endereco.Rua);

                    if (temEndereco)
                    {
                        if (enderecoExistente != null)
                        {
                            // Atualiza o endereço existente
                            enderecoExistente.CEP = endereco.CEP;
                            enderecoExistente.Rua = endereco.Rua;
                            enderecoExistente.Numero = endereco.Numero;
                            enderecoExistente.Bairro = endereco.Bairro;
                            enderecoExistente.Cidade = endereco.Cidade;
                            db.Entry(enderecoExistente).State = EntityState.Modified;
                        }
                        else
                        {
                            // Cria novo endereço caso não existia
                            endereco.ClienteId = cliente.Id;
                            db.Enderecos.Add(endereco);
                        }
                    }

                    db.SaveChanges();
                    TempData["Sucesso"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Erro ao atualizar: " + ex.Message);
                return View(cliente);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var cliente = db.Clientes.Include(c => c.Enderecos).FirstOrDefault(c => c.Id == id);
            if (cliente == null) return HttpNotFound();

            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var cliente = db.Clientes.Include(c => c.Enderecos).FirstOrDefault(c => c.Id == id);

                // Remove endereços vinculados antes de remover o cliente
                if (cliente.Enderecos != null && cliente.Enderecos.Any())
                    db.Enderecos.RemoveRange(cliente.Enderecos);

                db.Clientes.Remove(cliente);
                db.SaveChanges();

                TempData["Sucesso"] = "Cliente removido com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao excluir: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}