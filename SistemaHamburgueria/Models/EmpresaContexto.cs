using SistemaHamburgueria.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace SistemaHamburgueria.Models
{
    public class EmpresaContexto : DbContext
    {
        public EmpresaContexto()
            : base("name=HamburgueriaConnection")
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<ProdutoIngrediente> ProdutoIngredientes { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove a relação 1:1 e usa FK normal
            modelBuilder.Entity<Estoque>()
                .HasRequired(e => e.Produto)
                .WithMany()
                .HasForeignKey(e => e.ProdutoId);

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasRequired(m => m.Produto)
                .WithMany()
                .HasForeignKey(m => m.ProdutoId);
        }


    }
}