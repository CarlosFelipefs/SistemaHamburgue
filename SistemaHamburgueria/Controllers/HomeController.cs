using SistemaHamburgueria.Models;
using SistemaHamburgueria.Enums;
using System;
using System.Collections.Generic;
using System.Linq;          
using System.Data.Entity;  
using System.Web.Mvc;


namespace SistemaHamburgueria.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private EmpresaContexto db = new EmpresaContexto();

        public ActionResult Index()
        {
            var hoje = DateTime.Today;
            var ultimos7dias = hoje.AddDays(-6);

            // ?? Métricas dos cards ????????????????????????????????
            var pedidosHoje = db.Pedidos
                .Where(p => System.Data.Entity.DbFunctions.TruncateTime(p.DataPedido) == hoje)
                .ToList();

            decimal faturamentoHoje = pedidosHoje
                .Where(p => p.StatusPedido != StatusPedido.Cancelado)
                .Sum(p => (decimal?)p.ValorTotal) ?? 0;

            int totalPedidosHoje = pedidosHoje.Count;

            decimal ticketMedio = totalPedidosHoje > 0
                ? faturamentoHoje / totalPedidosHoje
                : 0;

            int mesasAtivas = db.Mesas
                .Count(m => m.StatusMesa == StatusMesa.Ocupada);

            int totalMesas = db.Mesas.Count();

            // ?? Gráfico: pedidos por dia nos últimos 7 dias ???????
            var pedidosPorDia = db.Pedidos
                .Where(p => System.Data.Entity.DbFunctions.TruncateTime(p.DataPedido) >= ultimos7dias)
                .ToList()
                .GroupBy(p => p.DataPedido.Date)
                .OrderBy(g => g.Key)
                .ToList();

            var labels = new List<string>();
            var qtdPorDia = new List<int>();

            for (int i = 6; i >= 0; i--)
            {
                var dia = hoje.AddDays(-i);
                labels.Add(dia.ToString("ddd", new System.Globalization.CultureInfo("pt-BR")));
                var grupo = pedidosPorDia.FirstOrDefault(g => g.Key == dia);
                qtdPorDia.Add(grupo?.Count() ?? 0);
            }

            // ?? Pedidos recentes ??????????????????????????????????
            var pedidosRecentes = db.Pedidos
                .Include("Mesa")
                .Include("ItensPedido")
                .OrderByDescending(p => p.DataPedido)
                .Take(5)
                .ToList();

            // ?? Top produtos ??????????????????????????????????????
            var topRaw = db.ItensPedido
                .Include("Produto")
                .GroupBy(i => i.Produto.Nome)
                .Select(g => new { Nome = g.Key, Qtd = g.Sum(i => i.Quantidade) })
                .OrderByDescending(x => x.Qtd)
                .Take(5)
                .ToList();

            int maxQtd = topRaw.Any() ? topRaw.Max(x => x.Qtd) : 1;

            var topProdutos = topRaw.Select(x => new TopProdutoItem
            {
                Nome = x.Nome,
                Quantidade = x.Qtd,
                Percentual = (int)((x.Qtd * 100.0) / maxQtd)
            }).ToList();

            // ?? Mesas ?????????????????????????????????????????????
            var mesas = db.Mesas.OrderBy(m => m.Numero).ToList();

            // ?? Monta ViewModel ???????????????????????????????????
            var vm = new DashboardViewModel
            {
                FaturamentoHoje = faturamentoHoje,
                PedidosHoje = totalPedidosHoje,
                TicketMedio = ticketMedio,
                MesasAtivas = mesasAtivas,
                TotalMesas = totalMesas,
                Labels = labels,
                PedidosPorDia = qtdPorDia,
                PedidosRecentes = pedidosRecentes,
                TopProdutos = topProdutos,
                Mesas = mesas
            };

            return View(vm);
        }
    }
}

