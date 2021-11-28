using DisneyApi.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class FilmDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime Creation_Date { get; set; }
        public int Score { get; set; }
        public string Imagen { get; set; }

        //public List<CharactersFilms> CharactersFilms { get; set; }
        //public List<FilmsGenres> FilmsGenres { get; set; }
    }
}
