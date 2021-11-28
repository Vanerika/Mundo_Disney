using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Entidades
{
    public class FilmsGenres
    {
        public int GenreId { get; set; }
        public int FilmId { get; set; }

        //propiedades de navegación
        public Genre Genre { get; set; }
        public Film Film { get; set; }
    }
}
