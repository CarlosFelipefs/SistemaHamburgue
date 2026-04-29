using SistemaHamburgueria.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Display(Name = "Data do Pedido")]
        public DateTime DataPedido { get; set; }

        [Display(Name = "Valor Total")]
        [DataType(DataType.Currency)]
        public decimal ValorTotal { get; set; }

        [Display(Name = "Forma de Pagamento")]
        public FormaPagamento FormaPagamento { get; set; }

        [Display(Name = "Status")]
        public StatusPedido StatusPedido { get; set; }

        public int? ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public int? FuncionarioId { get; set; }
        public virtual Funcionario Funcionario { get; set; }

        [Display(Name = "Mesa")]
        public int? MesaId { get; set; }
        public virtual Mesa Mesa { get; set; }

        public int? EnderecoId { get; set; }
        public virtual Endereco Endereco { get; set; }

        public virtual ICollection<ItemPedido> ItensPedido { get; set; }
    }
}
