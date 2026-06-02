using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class ProdutoIngrediente
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public int IngredienteId { get; set; }
        public decimal Quantidade { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Ingrediente Ingrediente { get; set; }
    }
}