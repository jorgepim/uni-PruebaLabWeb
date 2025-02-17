using Microsoft.AspNetCore.Mvc;
using PracticaApi.Models;

namespace PracticaApi.Controllers
{
    [Route("Biblioteca/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly bibliotecaContext _context;

        public AutorController(bibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Autor> autores = (from b in _context.Autor
                                   select b).ToList();
            if (autores.Count == 0)
            {
                return NotFound();
            }
            return Ok(autores);
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult GetAutor(int id)
        {
            var autor = (from b in _context.Autor
                         join a in _context.Libro on b.id equals a.autor_id
                         where b.id == id
                         select new
                         {
                             b.id,
                             b.nombre,
                             b.nacionalidad,
                             libros = a.titulo
                         }).ToList();
            if (autor == null)
            {
                return NotFound();
            }
            return Ok(autor);
        }

        [HttpGet]
        [Route("GetCountBooks")]
        public IActionResult GetCountBooks()
        {
            var autor = (from b in _context.Autor
                         join a in _context.Libro on b.id equals a.autor_id
                         group b by new { b.id, b.nombre, b.nacionalidad } into g
                         select new
                         {
                             g.Key.id,
                             g.Key.nombre,
                             g.Key.nacionalidad,
                             count = g.Count()
                         }).ToList();
            if (autor == null)
            {
                return NotFound();
            }
            return Ok(autor);
        }

        [HttpGet]
        [Route("GetTopAutores")]
        public IActionResult GetTopAutores()
        {
            var autores = (from b in _context.Autor
                           join l in _context.Libro on b.id equals l.autor_id
                           group l by new { b.id, b.nombre, b.nacionalidad } into g
                           orderby g.Count() descending
                           select new
                           {
                               g.Key.id,
                               g.Key.nombre,
                               g.Key.nacionalidad,
                               cantidad_libros = g.Count()
                           }).ToList();

            if (!autores.Any())
            {
                return NotFound();
            }

            return Ok(autores);
        }

        [HttpGet]
        [Route("HasBooks/{id}")]
        public IActionResult HasBooks(int id)
        {
            bool tieneLibros = _context.Libro.Any(l => l.autor_id == id);

            return Ok(new { autor_id = id, tiene_libros = tieneLibros });
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult CreateAutor([FromBody] Autor autor)
        {
            try
            {
                _context.Autor.Add(autor);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult UpdateAutor(int id, [FromBody] Autor autor)
        {
            try
            {
                Autor autorUpdate = (from e in _context.Autor
                                     where e.id == id
                                     select e).FirstOrDefault();
                if (autorUpdate != null)
                {
                    autorUpdate.nombre = autor.nombre;
                    autorUpdate.nacionalidad = autor.nacionalidad;
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeleteAutor(int id)
        {
            try
            {
                Autor autorDelete = (from e in _context.Autor
                                     where e.id == id
                                     select e).FirstOrDefault();
                if (autorDelete != null)
                {
                    _context.Autor.Remove(autorDelete);
                    _context.SaveChanges();
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
