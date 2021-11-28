using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Entidades
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Creation_Date { get; set; }
        public int Score { get; set; }
        public string Imagen { get; set; }

        public List<CharactersFilms> CharactersFilms { get; set; }
        public List<FilmsGenres> FilmsGenres { get; set; }
    }
}
