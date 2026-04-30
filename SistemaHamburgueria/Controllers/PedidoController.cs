using SistemaHamburgueria.Enums;
using SistemaHamburgueria.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();
    // ─── LISTAR PEDIDOS ───────────────────────────────────────────
    public ActionResult Index()
    {
        try
        {
            var pedidos = db.Pedidos
                .Include(p => p.Mesa)
                .Include(p => p.Funcionario)
                .Include(p => p.ItensPedido.Select(i => i.Produto))
                .OrderByDescending(p => p.DataPedido)
                .ToList();

            return View(pedidos);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.Index] Erro: {ex.Message}");
            return View("Error");
        }
    }

    // ─── ABRIR NOVO PEDIDO ────────────────────────────────────────
    public ActionResult Create()
    {
        try
        {
            ViewBag.MesaId = new SelectList(
                db.Mesas.Where(m => m.StatusMesa == StatusMesa.Livre),
                "Id", "Numero"
            );
            return View();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.Create GET] Erro: {ex.Message}");
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(int MesaId)
    {
        try
        {
            var mesa = db.Mesas.Find(MesaId);
            if (mesa == null || mesa.StatusMesa != StatusMesa.Livre)
            {
                TempData["Erro"] = "Mesa indisponível. Selecione outra.";
                return RedirectToAction("Create");
            }

            // Recupera funcionário logado da sessão
            int? funcId = Session["FuncionarioId"] as int?;

            var pedido = new Pedido
            {
                MesaId       = MesaId,
                DataPedido   = DateTime.Now,
                ValorTotal   = 0,
                StatusPedido = StatusPedido.EmPreparo,
                FuncionarioId = funcId
            };

            db.Pedidos.Add(pedido);

            mesa.StatusMesa = StatusMesa.Ocupada;

            db.SaveChanges();

            return RedirectToAction("AddItem", new { id = pedido.Id });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.Create POST] Erro: {ex.Message}");
            TempData["Erro"] = "Erro ao abrir pedido. Tente novamente.";
            return RedirectToAction("Create");
        }
    }

        // ─── ADICIONAR ITEM ───────────────────────────────────────────
        public ActionResult AddItem(int id)
        {
            try
            {
                var pedido = db.Pedidos
                    .Include(p => p.Mesa)
                    .Include(p => p.ItensPedido.Select(i => i.Produto))
                    .FirstOrDefault(p => p.Id == id);

                if (pedido == null) return HttpNotFound();

                ViewBag.ProdutoId = new SelectList(db.Produtos.ToList(), "Id", "Nome");
                ViewBag.PedidoId = id;
                ViewBag.Pedido = pedido;
                return View(pedido);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Pedido.AddItem GET] Erro: {ex.Message}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddItem(int PedidoId, int ProdutoId, int Quantidade)
        {
            try
            {
                if (Quantidade <= 0)
                {
                    TempData["Erro"] = "Quantidade deve ser maior que zero.";
                    return RedirectToAction("AddItem", new { id = PedidoId });
                }

                var produto = db.Produtos.Find(ProdutoId);
                if (produto == null)
                {
                    TempData["Erro"] = "Produto não encontrado.";
                    return RedirectToAction("AddItem", new { id = PedidoId });
                }

                var estoque = db.Estoques.FirstOrDefault(e => e.ProdutoId == ProdutoId);
                if (estoque == null || estoque.QuantidadeDisponivel < Quantidade)
                {
                    int disponivel = estoque?.QuantidadeDisponivel ?? 0;
                    TempData["Erro"] = $"Estoque insuficiente! Disponível: {disponivel}.";
                    return RedirectToAction("AddItem", new { id = PedidoId });
                }

                var pedido = db.Pedidos.Find(PedidoId);
                if (pedido == null) return HttpNotFound();

                var itemExistente = db.ItensPedido
                    .FirstOrDefault(i => i.PedidoId == PedidoId && i.ProdutoId == ProdutoId);

                if (itemExistente != null)
                {
                    itemExistente.Quantidade += Quantidade;
                }
                else
                {
                    db.ItensPedido.Add(new ItemPedido
                    {
                        PedidoId = PedidoId,
                        ProdutoId = ProdutoId,
                        Quantidade = Quantidade,
                        PrecoUnitario = produto.Preco
                    });
                }

                estoque.QuantidadeDisponivel -= Quantidade;
                estoque.DataAtualizacao = DateTime.Now;

                db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    ProdutoId = ProdutoId,
                    TipoMovimentacao = TipoMovimentacao.Saida,
                    Quantidade = Quantidade,
                    DataMovimentacao = DateTime.Now
                });

                pedido.ValorTotal += produto.Preco * Quantidade;
                db.SaveChanges();

                TempData["Sucesso"] = $"{produto.Nome} adicionado!";
                return RedirectToAction("Details", new { id = PedidoId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Pedido.AddItem POST] Erro: {ex.Message}");
                TempData["Erro"] = "Erro ao adicionar item.";
                return RedirectToAction("AddItem", new { id = PedidoId });
            }
        }

        // ─── DETALHES DO PEDIDO ───────────────────────────────────────
        public ActionResult Details(int id)
    {
        try
        {
            var pedido = db.Pedidos
                .Include(p => p.Mesa)
                .Include(p => p.Funcionario)
                .Include(p => p.ItensPedido.Select(i => i.Produto))
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null) return HttpNotFound();
            return View(pedido);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.Details] Erro: {ex.Message}");
            return View("Error");
        }
    }

    // ─── ADICIONAR ITEM VIA PDV (AJAX) ────────────────────────────
    [HttpPost]
    public JsonResult AdicionarItemPDV(int pedidoId, int produtoId)
    {
        try
        {
            var produto = db.Produtos.Find(produtoId);
            if (produto == null)
                return Json(new { sucesso = false, mensagem = "Produto não encontrado." });

            var estoque = db.Estoques.FirstOrDefault(e => e.ProdutoId == produtoId);
            if (estoque == null || estoque.QuantidadeDisponivel <= 0)
                return Json(new { sucesso = false, mensagem = "Estoque insuficiente." });

            var pedido = db.Pedidos.Find(pedidoId);
            if (pedido == null)
                return Json(new { sucesso = false, mensagem = "Pedido não encontrado." });

            var itemExistente = db.ItensPedido
                .FirstOrDefault(i => i.PedidoId == pedidoId && i.ProdutoId == produtoId);

            if (itemExistente != null)
            {
                itemExistente.Quantidade++;
            }
            else
            {
                db.ItensPedido.Add(new ItemPedido
                {
                    PedidoId      = pedidoId,
                    ProdutoId     = produtoId,
                    Quantidade    = 1,
                    PrecoUnitario = produto.Preco
                });
            }

            estoque.QuantidadeDisponivel--;
            estoque.DataAtualizacao = DateTime.Now;

            db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
            {
                ProdutoId        = produtoId,
                TipoMovimentacao = TipoMovimentacao.Saida,
                Quantidade       = 1,
                DataMovimentacao = DateTime.Now
            });

            pedido.ValorTotal += produto.Preco;
            db.SaveChanges();

            return Json(new
            {
                sucesso = true,
                nome    = produto.Nome,
                preco   = produto.Preco,
                total   = pedido.ValorTotal
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.AdicionarItemPDV] Erro: {ex.Message}");
            return Json(new { sucesso = false, mensagem = "Erro interno. Tente novamente." });
        }
    }

        // ─── FINALIZAR PEDIDO ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Finalizar(int id, int formaPagamento)
        {
            try
            {
                var pedido = db.Pedidos
                    .Include(p => p.ItensPedido)
                    .FirstOrDefault(p => p.Id == id);

                if (pedido == null) return HttpNotFound();

                pedido.StatusPedido = StatusPedido.Entregue;
                pedido.FormaPagamento = (FormaPagamento)formaPagamento;

                if (pedido.MesaId.HasValue)
                {
                    var mesa = db.Mesas.Find(pedido.MesaId.Value);
                    if (mesa != null)
                        mesa.StatusMesa = StatusMesa.Livre;
                }

                db.SaveChanges();
                TempData["Sucesso"] = "Pedido finalizado!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Pedido.Finalizar] Erro: {ex.Message}");
                TempData["Erro"] = "Erro ao finalizar pedido.";
                return RedirectToAction("Details", new { id });
            }
        }


        // ─── CANCELAR PEDIDO ──────────────────────────────────────────
        [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Cancelar(int id)
    {
        try
        {
            var pedido = db.Pedidos
                .Include(p => p.ItensPedido)
                .FirstOrDefault(p => p.Id == id);

            if (pedido == null) return HttpNotFound();

            // Devolve estoque de todos os itens
            foreach (var item in pedido.ItensPedido)
            {
                var estoque = db.Estoques.FirstOrDefault(e => e.ProdutoId == item.ProdutoId);
                if (estoque != null)
                {
                    estoque.QuantidadeDisponivel += item.Quantidade;
                    estoque.DataAtualizacao       = DateTime.Now;

                    db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                    {
                        ProdutoId        = item.ProdutoId,
                        TipoMovimentacao = TipoMovimentacao.Entrada,
                        Quantidade       = item.Quantidade,
                        DataMovimentacao = DateTime.Now
                    });
                }
            }

            pedido.StatusPedido = StatusPedido.Cancelado;

            if (pedido.MesaId.HasValue)
            {
                var mesa = db.Mesas.Find(pedido.MesaId.Value);
                if (mesa != null)
                    mesa.StatusMesa = StatusMesa.Livre;
            }

            db.SaveChanges();
            TempData["Sucesso"] = "Pedido cancelado. Estoque devolvido.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Pedido.Cancelar] Erro: {ex.Message}");
            TempData["Erro"] = "Erro ao cancelar pedido.";
            return RedirectToAction("Index");
            }
        }

        // ─── REMOVER ITEM (com devolução de estoque) ──────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverItem(int id)
        {
            int pedidoId = 0;
            try
            {
                var item = db.ItensPedido.Find(id);
                if (item == null) return HttpNotFound();

                pedidoId = item.PedidoId;

                var estoque = db.Estoques.FirstOrDefault(e => e.ProdutoId == item.ProdutoId);
                if (estoque != null)
                {
                    estoque.QuantidadeDisponivel += item.Quantidade;
                    estoque.DataAtualizacao = DateTime.Now;

                    db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                    {
                        ProdutoId = item.ProdutoId,
                        TipoMovimentacao = TipoMovimentacao.Entrada,
                        Quantidade = item.Quantidade,
                        DataMovimentacao = DateTime.Now
                    });
                }

                var pedido = db.Pedidos.Find(item.PedidoId);
                pedido.ValorTotal -= item.PrecoUnitario * item.Quantidade;

                db.ItensPedido.Remove(item);
                db.SaveChanges();

                TempData["Sucesso"] = "Item removido e estoque devolvido.";
                return RedirectToAction("Details", new { id = pedidoId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Pedido.RemoverItem] Erro: {ex.Message}");
                TempData["Erro"] = "Erro ao remover item.";
                return RedirectToAction("Details", new { id = pedidoId });
            }
        }   
    }
    
    }
