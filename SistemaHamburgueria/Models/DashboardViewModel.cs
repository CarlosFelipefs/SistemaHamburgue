using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class DashboardViewModel
    {
        // Cards de métricas
        public decimal FaturamentoHoje { get; set; }
        public int PedidosHoje { get; set; }
        public decimal TicketMedio { get; set; }
        public int MesasAtivas { get; set; }
        public int TotalMesas { get; set; }

        // Gráfico de pedidos por dia (últimos 7 dias)
        public List<string> Labels { get; set; }        // ["Seg", "Ter"...]
        public List<int> PedidosPorDia { get; set; }   // [32, 48...]

        // Pedidos recentes
        public List<Pedido> PedidosRecentes { get; set; }

        // Top produtos
        public List<TopProdutoItem> TopProdutos { get; set; }

        // Status das mesas
        public List<Mesa> Mesas { get; set; }
    }

    public class TopProdutoItem
    {
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public int Percentual { get; set; } // relativo ao maior
    }
}
