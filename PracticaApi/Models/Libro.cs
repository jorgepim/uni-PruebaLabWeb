namespace PracticaApi.Models
{
    public class Libro
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public int anio_publicacion { get; set; }
        public int autor_id { get; set; }
        public int categoria_id { get; set; }
        public string resumen { get; set; }
    }
}
