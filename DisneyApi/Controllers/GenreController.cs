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

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "Genres";

        public GenreController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
        {
            var entidades = await context.Genres.ToListAsync();
            var dtos = mapper.Map<List<GenreDto>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GenreDto>> Get(int id)
        {
            var entidad = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            
            if (entidad == null)
            {
                return NotFound();
            }
            
            var dto = mapper.Map<GenreDto>(entidad);
            return dto;
        }



        [HttpPost]
        [Consumes("multipart/form-data")]
        //programacion asincrona podemos trabajar de manera más eficiente las conexiones a la
        //base de datos. 
        //devolvemos task porque es requisito de la programacion asincrona.
        //FromBody => queremos sacar desde el cuerpo de la peticion la siguiente información
        public async Task<ActionResult> Post([FromForm] GenreCreationDto genre)
        {
            var entidad = mapper.Map<Genre>(genre);

            if (genre.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await genre.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(genre.Imagen.FileName);

                    entidad.Imagen = await almacenadorArchivos.SaveFile(contenido, extension, contenedor, genre.Imagen.ContentType);
                }
            }



            context.Add(entidad);
            await context.SaveChangesAsync();
            var genreDto = mapper.Map<GenreDto>(entidad);

            return new CreatedAtRouteResult("obtenerGenero", new { id = genreDto.Id }, genreDto);
        }

        [HttpPut("{id:int}")] //api/autores/1
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Put(int id, [FromForm] GenreCreationDto genre)
        {
            var genreDB = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);

            if (genreDB == null) { return NotFound(); }

            //Esto hace que los campos recibidos en character se peguen en genreDB actualizando solo los campos
            //ingresados por el usuario.
            genreDB = mapper.Map(genre, genreDB);


            if (genre.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await genre.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(genre.Imagen.FileName);

                    genreDB.Imagen = await almacenadorArchivos.EditFile(contenido, extension, contenedor, genreDB.Imagen, genre.Imagen.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]//api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Genres.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound(); ;
            }

            context.Remove(new Genre() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
