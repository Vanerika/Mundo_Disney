using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class CharacterDetailDto:CharacterDto
    {
        public List<FilmCharacterDetailDto> Films { get; set; }
    }
}
