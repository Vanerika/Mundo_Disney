using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Entidades
{
    public class CharactersFilms
    {
        public int CharacterId { get; set; }
        public int FilmId { get; set; }

        //propiedades de navegación
        public Character Character { get; set; }
        public Film Film { get; set; }
    }
}
