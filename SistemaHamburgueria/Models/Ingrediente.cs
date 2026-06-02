using SistemaHamburgueria.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Ingrediente
    {
        public int id { get; set; }
        public string nome { get; set; } = string.Empty;
        public int QuantidadeEstoque { get; set; }
        public UnidadesMedidas unidade { get; set; }
        public Decimal custo { get; set; }
        public virtual ICollection<ProdutoIngrediente> ProdutoIngredientes { get; set; } = new List<ProdutoIngrediente>();
    }
}