namespace SistemaHamburgueria.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrigirRelacaoEstoque : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categorias",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Produtoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Descricao = c.String(maxLength: 500),
                        Preco = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoriaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categorias", t => t.CategoriaId, cascadeDelete: true)
                .Index(t => t.CategoriaId);
            
            CreateTable(
                "dbo.ItemPedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PedidoId = c.Int(nullable: false),
                        ProdutoId = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                        PrecoUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidoes", t => t.PedidoId, cascadeDelete: true)
                .ForeignKey("dbo.Produtoes", t => t.ProdutoId, cascadeDelete: true)
                .Index(t => t.PedidoId)
                .Index(t => t.ProdutoId);
            
            CreateTable(
                "dbo.Pedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataPedido = c.DateTime(nullable: false),
                        ValorTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FormaPagamento = c.Int(nullable: false),
                        StatusPedido = c.Int(nullable: false),
                        ClienteId = c.Int(),
                        FuncionarioId = c.Int(),
                        MesaId = c.Int(),
                        EnderecoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Enderecoes", t => t.EnderecoId)
                .ForeignKey("dbo.Clientes", t => t.ClienteId)
                .ForeignKey("dbo.Funcionarios", t => t.FuncionarioId)
                .ForeignKey("dbo.Mesas", t => t.MesaId)
                .Index(t => t.ClienteId)
                .Index(t => t.FuncionarioId)
                .Index(t => t.MesaId)
                .Index(t => t.EnderecoId);
            
            CreateTable(
                "dbo.Clientes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Telefone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Enderecoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rua = c.String(nullable: false),
                        Numero = c.String(),
                        Bairro = c.String(),
                        Cidade = c.String(),
                        CEP = c.String(),
                        ClienteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clientes", t => t.ClienteId, cascadeDelete: true)
                .Index(t => t.ClienteId);
            
            CreateTable(
                "dbo.Funcionarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Cargo = c.String(maxLength: 50),
                        login = c.String(nullable: false, maxLength: 150),
                        Senha = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Mesas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.Int(nullable: false),
                        StatusMesa = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MovimentacaoEstoques",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdutoId = c.Int(nullable: false),
                        TipoMovimentacao = c.Int(nullable: false),
                        Quantidade = c.Int(nullable: false),
                        DataMovimentacao = c.DateTime(nullable: false),
                        Produto_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produtoes", t => t.ProdutoId, cascadeDelete: true)
                .ForeignKey("dbo.Produtoes", t => t.Produto_Id)
                .Index(t => t.ProdutoId)
                .Index(t => t.Produto_Id);
            
            CreateTable(
                "dbo.ProdutoIngredientes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdutoId = c.Int(nullable: false),
                        IngredienteId = c.Int(nullable: false),
                        Quantidade = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ingredientes", t => t.IngredienteId, cascadeDelete: true)
                .ForeignKey("dbo.Produtoes", t => t.ProdutoId, cascadeDelete: true)
                .Index(t => t.ProdutoId)
                .Index(t => t.IngredienteId);
            
            CreateTable(
                "dbo.Ingredientes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(),
                        QuantidadeEstoque = c.Int(nullable: false),
                        unidade = c.Int(nullable: false),
                        custo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Estoques",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProdutoId = c.Int(nullable: false),
                        QuantidadeDisponivel = c.Int(nullable: false),
                        DataAtualizacao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Produtoes", t => t.ProdutoId, cascadeDelete: true)
                .Index(t => t.ProdutoId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Estoques", "ProdutoId", "dbo.Produtoes");
            DropForeignKey("dbo.ProdutoIngredientes", "ProdutoId", "dbo.Produtoes");
            DropForeignKey("dbo.ProdutoIngredientes", "IngredienteId", "dbo.Ingredientes");
            DropForeignKey("dbo.MovimentacaoEstoques", "Produto_Id", "dbo.Produtoes");
            DropForeignKey("dbo.MovimentacaoEstoques", "ProdutoId", "dbo.Produtoes");
            DropForeignKey("dbo.ItemPedidoes", "ProdutoId", "dbo.Produtoes");
            DropForeignKey("dbo.Pedidoes", "MesaId", "dbo.Mesas");
            DropForeignKey("dbo.ItemPedidoes", "PedidoId", "dbo.Pedidoes");
            DropForeignKey("dbo.Pedidoes", "FuncionarioId", "dbo.Funcionarios");
            DropForeignKey("dbo.Pedidoes", "ClienteId", "dbo.Clientes");
            DropForeignKey("dbo.Pedidoes", "EnderecoId", "dbo.Enderecoes");
            DropForeignKey("dbo.Enderecoes", "ClienteId", "dbo.Clientes");
            DropForeignKey("dbo.Produtoes", "CategoriaId", "dbo.Categorias");
            DropIndex("dbo.Estoques", new[] { "ProdutoId" });
            DropIndex("dbo.ProdutoIngredientes", new[] { "IngredienteId" });
            DropIndex("dbo.ProdutoIngredientes", new[] { "ProdutoId" });
            DropIndex("dbo.MovimentacaoEstoques", new[] { "Produto_Id" });
            DropIndex("dbo.MovimentacaoEstoques", new[] { "ProdutoId" });
            DropIndex("dbo.Enderecoes", new[] { "ClienteId" });
            DropIndex("dbo.Pedidoes", new[] { "EnderecoId" });
            DropIndex("dbo.Pedidoes", new[] { "MesaId" });
            DropIndex("dbo.Pedidoes", new[] { "FuncionarioId" });
            DropIndex("dbo.Pedidoes", new[] { "ClienteId" });
            DropIndex("dbo.ItemPedidoes", new[] { "ProdutoId" });
            DropIndex("dbo.ItemPedidoes", new[] { "PedidoId" });
            DropIndex("dbo.Produtoes", new[] { "CategoriaId" });
            DropTable("dbo.Estoques");
            DropTable("dbo.Ingredientes");
            DropTable("dbo.ProdutoIngredientes");
            DropTable("dbo.MovimentacaoEstoques");
            DropTable("dbo.Mesas");
            DropTable("dbo.Funcionarios");
            DropTable("dbo.Enderecoes");
            DropTable("dbo.Clientes");
            DropTable("dbo.Pedidoes");
            DropTable("dbo.ItemPedidoes");
            DropTable("dbo.Produtoes");
            DropTable("dbo.Categorias");
        }
    }
}
