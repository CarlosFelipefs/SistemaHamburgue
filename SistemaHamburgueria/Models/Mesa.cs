using SistemaHamburgueria.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaHamburgueria.Models
{
    public class Mesa
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public StatusMesa StatusMesa { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}