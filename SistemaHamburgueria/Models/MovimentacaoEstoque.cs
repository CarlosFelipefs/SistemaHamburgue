using SistemaHamburgueria.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class MovimentacaoEstoque
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        public TipoMovimentacao TipoMovimentacao { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataMovimentacao { get; set; }

        public virtual Produto Produto { get; set; }
    }
}