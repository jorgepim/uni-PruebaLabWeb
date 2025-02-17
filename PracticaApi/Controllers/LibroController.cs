using Microsoft.AspNetCore.Mvc;
using PracticaApi.Models;

namespace PracticaApi.Controllers
{
    [Route("Biblioteca/[controller]")]
    [ApiController]
    public class LibroController : Controller
    {
        private readonly bibliotecaContext _context;
        public LibroController(bibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<Libro> libros = (from b in _context.Libro
                                  select b).ToList();
            if (libros.Count == 0)
            {
                return NotFound();
            }
            return Ok(libros);
        }

        [HttpGet]
        [Route("Get/{id}")]
        public IActionResult Get(int id)
        {
            var libro = (from b in _context.Libro
                         join a in _context.Autor on b.autor_id equals a.id
                         where b.id == id
                         select new
                         {
                             b.id,
                             b.titulo,
                             b.anio_publicacion,
                             autor = a.nombre,
                             b.categoria_id,
                             b.resumen
                         }).ToList();
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpGet]
        [Route("GetByDate")]
        public IActionResult GetByDate()
        {
            var libro = (from b in _context.Libro
                         join a in _context.Autor on b.autor_id equals a.id
                         where b.anio_publicacion >= 2000
                         select new
                         {
                             b.id,
                             b.titulo,
                             b.anio_publicacion,
                             autor = a.nombre,
                             b.categoria_id,
                             b.resumen
                         }).ToList();
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }
        [HttpGet]
        [Route("GetByTitle")]
        public IActionResult GetByTitle(string title)
        {
            var libro = (from b in _context.Libro
                         join a in _context.Autor on b.autor_id equals a.id
                         where b.titulo.Contains(title)
                         select new
                         {
                             b.id,
                             b.titulo,
                             b.anio_publicacion,
                             autor = a.nombre,
                             b.categoria_id,
                             b.resumen
                         }).ToList();
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpGet]
        [Route("GetSkip")]
        public IActionResult GetSkip(int skip)
        {
            var libro = (from b in _context.Libro
                         join a in _context.Autor on b.autor_id equals a.id
                         select new
                         {
                             b.id,
                             b.titulo,
                             b.anio_publicacion,
                             autor = a.nombre,
                             b.categoria_id,
                             b.resumen
                         }).Skip((skip - 1) * 2).Take(2).ToList();
            if (libro == null)
            {
                return NotFound();
            }
            return Ok(libro);
        }

        [HttpGet]
        [Route("GetLatestBooks/{cantidad}")]
        public IActionResult GetLatestBooks(int cantidad)
        {
            var libros = (from b in _context.Libro
                          orderby b.anio_publicacion descending
                          select new
                          {
                              b.id,
                              b.titulo,
                              b.anio_publicacion,
                              b.autor_id,
                              b.categoria_id,
                              b.resumen
                          }).Take(cantidad).ToList();

            if (!libros.Any())
            {
                return NotFound();
            }

            return Ok(libros);
        }

        [HttpGet]
        [Route("GetTotalBooksByYear")]
        public IActionResult GetTotalBooksByYear()
        {
            var librosPorAnio = (from b in _context.Libro
                                 group b by b.anio_publicacion into g
                                 orderby g.Key descending
                                 select new
                                 {
                                     anio = g.Key,
                                     cantidad = g.Count()
                                 }).ToList();

            if (!librosPorAnio.Any())
            {
                return NotFound();
            }

            return Ok(librosPorAnio);
        }

        [HttpGet]
        [Route("GetFirstBookByAuthor/{autorId}")]
        public IActionResult GetFirstBookByAuthor(int autorId)
        {
            var primerLibro = (from b in _context.Libro
                               where b.autor_id == autorId
                               orderby b.anio_publicacion ascending
                               select new
                               {
                                   b.id,
                                   b.titulo,
                                   b.anio_publicacion,
                                   b.categoria_id,
                                   b.resumen
                               }).FirstOrDefault();

            if (primerLibro == null)
            {
                return NotFound();
            }

            return Ok(primerLibro);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult CreateLibro([FromBody] Libro libro)
        {
            try
            {
                _context.Libro.Add(libro);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route("Update/{id}")]
        public IActionResult UpdateLibro(int id, [FromBody] Libro libro)
        {
            try
            {
                Libro libroOriginal = (from e in _context.Libro
                                       where e.id == id
                                       select e).FirstOrDefault();
                if (libroOriginal == null)
                {
                    return NotFound();
                }
                libroOriginal.titulo = libro.titulo;
                libroOriginal.anio_publicacion = libro.anio_publicacion;
                libroOriginal.autor_id = libro.autor_id;
                libroOriginal.categoria_id = libro.categoria_id;
                libroOriginal.resumen = libro.resumen;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult DeleteLibro(int id)
        {
            try
            {
                Libro libro = (from e in _context.Libro
                               where e.id == id
                               select e).FirstOrDefault();
                if (libro == null)
                {
                    return NotFound();
                }
                _context.Libro.Remove(libro);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
