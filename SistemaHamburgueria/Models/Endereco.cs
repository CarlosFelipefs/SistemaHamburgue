using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Endereco
    {
        public int Id { get; set; }

        [Required]
        public string Rua { get; set; }

        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string CEP { get; set; }

        public int ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}