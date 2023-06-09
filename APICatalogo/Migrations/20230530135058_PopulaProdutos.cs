using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    public partial class PopulaProdutos : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into Produtos (Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaID)" +
            " Values ('Coca-Cola','Refrigerante de Cola 350ml',5.45,'cocoacola.jpg',50,now(),1)");

            mb.Sql("Insert into Produtos (Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaID)" +
            " Values ('Sanduíche','Sanduíche de presunto e queijo',5.00,'sanduiche.jpg',10,now(),2)");

            mb.Sql("Insert into Produtos (Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaID)" +
            " Values ('Pudim','Pudim de Leite Condensado 100mg',7.00,'pudim.jpg',5,now(),3)");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
