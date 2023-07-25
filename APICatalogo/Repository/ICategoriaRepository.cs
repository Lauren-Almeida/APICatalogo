using APICatalogo.Pagination;
using APICatalogo.Models;
using System.Collections.Generic;

namespace APICatalogo.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        PagedList<Categoria>
            GetCategorias(CategoriasParameters categoriaParameters);
        IEnumerable<Categoria> GetCategoriasProdutos();
    }
}
