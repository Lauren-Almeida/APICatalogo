using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters);
        Task<IEnumerable<Produto>> GetProdutosPorPreco();
    }
}
