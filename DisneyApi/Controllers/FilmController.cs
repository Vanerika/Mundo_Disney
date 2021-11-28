using AutoMapper;
using DisneyApi.DTOs;
using DisneyApi.Entidades;
using DisneyApi.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class FilmController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "Films";

        public FilmController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ListFilmsDto>>> GetAll()
        {
            var entidades = await context.Films.ToListAsync();
            var dtos = mapper.Map<List<ListFilmsDto>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<FilmDetailDto>> Get(int id)
        {
            var entidad = await context.Films
                .Include(x=>x.CharactersFilms).ThenInclude(x=>x.Character)
                .Include(x=>x.FilmsGenres).ThenInclude(x=>x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<FilmDetailDto>(entidad);
            return dto;
        }


        [HttpGet("name")]
        public async Task<ActionResult<List<FilmDto>>> FilterbyName([FromQuery] FilterFilmDto filterFilmDto)
        {
            var movieQuery = context.Films.AsQueryable();

            if (!string.IsNullOrEmpty(filterFilmDto.Title))
            {
                movieQuery = movieQuery.Where(x => x.Title.Contains(filterFilmDto.Title));
            }

            var dto = await movieQuery.ToListAsync();

            return mapper.Map<List<FilmDto>>(dto);
        }

        [HttpGet("genre")]
        public async Task<ActionResult<List<FilmDto>>> FilterbyGenre([FromQuery] FilterFilmDto filterFilmDto)
        {
            var movieQuery = context.Films.AsQueryable();

            //navegamos a traves de las propiedades de navegacion hasta la tabla relacionada 
            movieQuery = movieQuery
                .Where(x => x.FilmsGenres.Select(z => z.GenreId)
                .Contains(filterFilmDto.GenreId));


            var dto = await movieQuery.ToListAsync();                
                
            return mapper.Map<List<FilmDto>>(dto);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        //programacion asincrona podemos trabajar de manera más eficiente las conexiones a la
        //base de datos. 
        //devolvemos task porque es requisito de la programacion asincrona.
        //FromBody => queremos sacar desde el cuerpo de la peticion la siguiente información
        public async Task<ActionResult> Post([FromForm] FilmCreationDto film)
        {
            var entidad = mapper.Map<Film>(film);

            if (film.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await film.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(film.Imagen.FileName);

                    entidad.Imagen = await almacenadorArchivos.SaveFile(contenido, extension, contenedor, film.Imagen.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();
            var filmDto = mapper.Map<FilmDto>(entidad);

            return new CreatedAtRouteResult("obtenerPelicula", new { id = filmDto.Id }, filmDto);
        }

        [HttpPut("{id:int}")] //api/autores/1
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Put(int id, [FromForm] FilmCreationDto film)
        {
            var filmDB = await context.Films
                .Include(x=>x.CharactersFilms)
                .Include(x=>x.FilmsGenres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (filmDB == null) { return NotFound(); }

            //Esto hace que los campos recibidos en character se peguen en filmDB.
            filmDB = mapper.Map(film, filmDB);


            if (film.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await film.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(film.Imagen.FileName);

                    filmDB.Imagen = await almacenadorArchivos.EditFile(contenido, extension, contenedor, filmDB.Imagen, film.Imagen.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]//api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Films.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound(); ;
            }

            context.Remove(new Film() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
