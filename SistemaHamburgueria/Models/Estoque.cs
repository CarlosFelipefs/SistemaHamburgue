using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Estoque
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecione o produto.")]
        [Display(Name = "Produto")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
        [Display(Name = "Quantidade Disponível")]
        public int QuantidadeDisponivel { get; set; }

        [Display(Name = "Última Atualização")]
        public DateTime DataAtualizacao { get; set; }

        public virtual Produto Produto { get; set; }
    }
}
