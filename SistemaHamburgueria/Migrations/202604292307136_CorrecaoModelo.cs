namespace SistemaHamburgueria.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrecaoModelo : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pedidoes", "ClienteId", "dbo.Clientes");
            DropForeignKey("dbo.Pedidoes", "FuncionarioId", "dbo.Funcionarios");
            DropIndex("dbo.Pedidoes", new[] { "ClienteId" });
            DropIndex("dbo.Pedidoes", new[] { "FuncionarioId" });
            AlterColumn("dbo.Produtoes", "Nome", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Produtoes", "Descricao", c => c.String(maxLength: 500));
            AlterColumn("dbo.Pedidoes", "ClienteId", c => c.Int());
            AlterColumn("dbo.Pedidoes", "FuncionarioId", c => c.Int());
            AlterColumn("dbo.Funcionarios", "Nome", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Funcionarios", "Cargo", c => c.String(maxLength: 50));
            AlterColumn("dbo.Funcionarios", "login", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Funcionarios", "Senha", c => c.String(nullable: false, maxLength: 100));
            CreateIndex("dbo.Pedidoes", "ClienteId");
            CreateIndex("dbo.Pedidoes", "FuncionarioId");
            AddForeignKey("dbo.Pedidoes", "ClienteId", "dbo.Clientes", "Id");
            AddForeignKey("dbo.Pedidoes", "FuncionarioId", "dbo.Funcionarios", "Id");
            DropColumn("dbo.ItemPedidoes", "Subtotal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemPedidoes", "Subtotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.Pedidoes", "FuncionarioId", "dbo.Funcionarios");
            DropForeignKey("dbo.Pedidoes", "ClienteId", "dbo.Clientes");
            DropIndex("dbo.Pedidoes", new[] { "FuncionarioId" });
            DropIndex("dbo.Pedidoes", new[] { "ClienteId" });
            AlterColumn("dbo.Funcionarios", "Senha", c => c.String(nullable: false));
            AlterColumn("dbo.Funcionarios", "login", c => c.String(nullable: false));
            AlterColumn("dbo.Funcionarios", "Cargo", c => c.String());
            AlterColumn("dbo.Funcionarios", "Nome", c => c.String(nullable: false));
            AlterColumn("dbo.Pedidoes", "FuncionarioId", c => c.Int(nullable: false));
            AlterColumn("dbo.Pedidoes", "ClienteId", c => c.Int(nullable: false));
            AlterColumn("dbo.Produtoes", "Descricao", c => c.String());
            AlterColumn("dbo.Produtoes", "Nome", c => c.String(nullable: false));
            CreateIndex("dbo.Pedidoes", "FuncionarioId");
            CreateIndex("dbo.Pedidoes", "ClienteId");
            AddForeignKey("dbo.Pedidoes", "FuncionarioId", "dbo.Funcionarios", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Pedidoes", "ClienteId", "dbo.Clientes", "Id", cascadeDelete: true);
        }
    }
}
