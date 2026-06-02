using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Range(0.01, 9999.99, ErrorMessage = "O preço deve ser entre R$ 0,01 e R$ 9.999,99.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Selecione uma categoria.")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }

        public virtual ICollection<ItemPedido> ItensPedido { get; set; }
        public virtual ICollection<MovimentacaoEstoque> Movimentacoes { get; set; }
        public virtual ICollection<ProdutoIngrediente> ProdutoIngredientes { get; set; } = new List<ProdutoIngrediente>();
    }
}
