using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class ItemPedido
    {
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }
        public virtual Pedido Pedido { get; set; }

        [Required(ErrorMessage = "Selecione o produto.")]
        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }
        public virtual Produto Produto { get; set; }

        [Required(ErrorMessage = "Informe a quantidade.")]
        [Range(1, 999, ErrorMessage = "A quantidade deve ser entre 1 e 999.")]
        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Preço Unitário")]
        public decimal PrecoUnitario { get; set; }

        public decimal Subtotal => PrecoUnitario * Quantidade;
    }
}
