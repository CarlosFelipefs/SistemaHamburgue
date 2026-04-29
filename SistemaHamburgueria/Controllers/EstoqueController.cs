using SistemaHamburgueria.Enums;
using SistemaHamburgueria.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class EstoqueController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        public ActionResult Index()
        {
            try
            {
                var estoques = db.Estoques
                    .Include(e => e.Produto)
                    .OrderBy(e => e.Produto.Nome)
                    .ToList();
                return View(estoques);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Estoque.Index] Erro: {ex.Message}");
                return View("Error");
            }
        }

        public ActionResult Create()
        {
            try
            {
                // Apenas produtos sem estoque cadastrado
                var produtosSemEstoque = db.Produtos
                    .Where(p => !db.Estoques.Any(e => e.ProdutoId == p.Id))
                    .ToList();

                ViewBag.ProdutoId = new SelectList(produtosSemEstoque, "Id", "Nome");
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Estoque.Create GET] Erro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Estoque estoque)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", estoque.ProdutoId);
                    return View(estoque);
                }

                // Verifica se já existe estoque para o produto
                bool jaExiste = db.Estoques.Any(e => e.ProdutoId == estoque.ProdutoId);
                if (jaExiste)
                {
                    ModelState.AddModelError("ProdutoId", "Já existe um registro de estoque para este produto.");
                    ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", estoque.ProdutoId);
                    return View(estoque);
                }

                estoque.DataAtualizacao = DateTime.Now;
                db.Estoques.Add(estoque);

                // Registra movimentação de entrada
                db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    ProdutoId = estoque.ProdutoId,
                    TipoMovimentacao = TipoMovimentacao.Entrada,
                    Quantidade = estoque.QuantidadeDisponivel,
                    DataMovimentacao = DateTime.Now
                });

                db.SaveChanges();
                TempData["Sucesso"] = "Estoque cadastrado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Estoque.Create POST] Erro: {ex.Message}");
                ModelState.AddModelError("", "Erro ao salvar o estoque. Tente novamente.");
                ViewBag.ProdutoId = new SelectList(db.Produtos, "Id", "Nome", estoque.ProdutoId);
                return View(estoque);
            }
        }

        // Ajuste de quantidade (entrada/saída manual)
        public ActionResult Ajustar(int id)
        {
            try
            {
                var estoque = db.Estoques.Include(e => e.Produto).FirstOrDefault(e => e.Id == id);
                if (estoque == null) return HttpNotFound();
                return View(estoque);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Estoque.Ajustar GET] Erro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ajustar(int id, int quantidade, TipoMovimentacao tipo)
        {
            try
            {
                if (quantidade <= 0)
                {
                    TempData["Erro"] = "A quantidade deve ser maior que zero.";
                    return RedirectToAction("Ajustar", new { id });
                }

                var estoque = db.Estoques.Find(id);
                if (estoque == null) return HttpNotFound();

                if (tipo == TipoMovimentacao.Saida && estoque.QuantidadeDisponivel < quantidade)
                {
                    TempData["Erro"] = $"Quantidade insuficiente. Disponível: {estoque.QuantidadeDisponivel}.";
                    return RedirectToAction("Ajustar", new { id });
                }

                estoque.QuantidadeDisponivel += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;
                estoque.DataAtualizacao = DateTime.Now;

                db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    ProdutoId = estoque.ProdutoId,
                    TipoMovimentacao = tipo,
                    Quantidade = quantidade,
                    DataMovimentacao = DateTime.Now
                });

                db.SaveChanges();
                TempData["Sucesso"] = "Estoque ajustado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Estoque.Ajustar POST] Erro: {ex.Message}");
                TempData["Erro"] = "Erro ao ajustar estoque.";
                return RedirectToAction("Ajustar", new { id });
            }
        }
    }
    }
