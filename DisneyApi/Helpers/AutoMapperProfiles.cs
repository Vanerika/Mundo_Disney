using AutoMapper;
using DisneyApi.DTOs;
using DisneyApi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<GenreCreationDto, Genre>()
                .ForMember(x => x.Imagen, options => options.Ignore());

            CreateMap<Character, CharacterDto>().ReverseMap();
            CreateMap<Character, ListCharacterDto>().ReverseMap();
            CreateMap<CharacterCreationDto, Character>()
               .ForMember(x => x.Imagen, options => options.Ignore());

            CreateMap<Character, CharacterDetailDto>()
                .ForMember(x => x.Films, options => options.MapFrom(MapCharacterFilm));

            CreateMap<Film, FilmDto>().ReverseMap();
            CreateMap<Film, ListFilmsDto>().ReverseMap();

            
            
            CreateMap<FilmCreationDto, Film>()
                .ForMember(x => x.Imagen, options => options.Ignore())
                .ForMember(x => x.FilmsGenres, options => options.MapFrom(MapFilmsGenres))
                .ForMember(x => x.CharactersFilms, options => options.MapFrom(MapCharactersFilms));


            CreateMap<Film, FilmDetailDto>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapFilmsGenre))
                .ForMember(x => x.Characters, options => options.MapFrom(MapFilmsCharacter));

            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserCreationDto, User>();
        }

        private List<GenreDto> MapFilmsGenre(Film film, FilmDetailDto filmDetailDto)
        {
            var result = new List<GenreDto>();
            if (film.FilmsGenres == null) { return result; }
            foreach (var genero in film.FilmsGenres)
            {
                result.Add(new GenreDto() { Id = genero.GenreId, Name = genero.Genre.Name });
            }
            return result;
        }

        private List<FilmCharacterDetailDto> MapCharacterFilm(Character character, CharacterDetailDto characterDetail)
        {
            var result = new List<FilmCharacterDetailDto>();
            if (character.CharactersFilms == null) { return result; }
            foreach (var film in character.CharactersFilms)
            {
                result.Add(new FilmCharacterDetailDto() { FilmId = film.FilmId, Title = film.Film.Title });
            }
            return result;
        }

        private List<CharacterFilmDetailDto> MapFilmsCharacter(Film film, FilmDetailDto filmDetailDto)
        {
            var result = new List<CharacterFilmDetailDto>();
            if (film.CharactersFilms == null) { return result; }
            foreach (var character in film.CharactersFilms)
            {
                result.Add(new CharacterFilmDetailDto() { 
                    CharacterId = character.CharacterId, 
                    Name = character.Character.Name });
            }
            return result;
        }

        private List<FilmsGenres> MapFilmsGenres(FilmCreationDto filmCreationDto, Film film)
        {
            var result = new List<FilmsGenres>();
            if (filmCreationDto.GenresId == null) { return result; }

            foreach(var id in filmCreationDto.GenresId)
            {
                result.Add(new FilmsGenres() { GenreId = id });
            }

            return result;
        }

        private List<CharactersFilms> MapCharactersFilms(FilmCreationDto filmCreationDto, Film film)
        {
            var result = new List<CharactersFilms>();
            if (filmCreationDto.CharactersId == null) { return result; }

            foreach (var id in filmCreationDto.CharactersId)
            {
                result.Add(new CharactersFilms() { CharacterId = id });
            }

            return result;
        }

    }
}
