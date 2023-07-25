using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            var categorias = _uof.CategoriaRepository.GetCategoriasProdutos().ToList();
            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>>
             Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            var categorias = _uof.CategoriaRepository.
                            GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);
            return categoriasDto;
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com id= {id} não encontrada...");
            }

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            return categoriaDto;
        }

        [HttpPost]
        public ActionResult Post([FromBody] CategoriaDTO categoriaDto)
        {
            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Add(categoria);
            _uof.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoria.CategoriaId }, categoriaDTO);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, [FromBody] CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

            if (categoria == null)
            {
                return NotFound($"Categoria com id={id} não encontrada...");
            }
            _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return categoriaDto;
        }
    }
}
