using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Entidades
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public string History { get; set; }

        public string Imagen { get; set; }

        public List<CharactersFilms> CharactersFilms { get; set; }
    }
}
