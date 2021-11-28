using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class FilmDetailDto:FilmDto
    {
        public List<GenreDto> Genres { get; set; }
        public List<CharacterFilmDetailDto> Characters { get; set; }
    }
}
