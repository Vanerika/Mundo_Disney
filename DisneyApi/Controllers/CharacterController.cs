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
    [Route("api/characters")]
    public class CharacterController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "Characters";

        public CharacterController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ListCharacterDto>>> GetAll()
        {
            var entidades = await context.Characters.ToListAsync();
            var dtos = mapper.Map<List<ListCharacterDto>>(entidades);
            return dtos;
        }

        [HttpGet("{id}", Name = "obtenerPersonaje")]
        public async Task<ActionResult<CharacterDto>> Get(int id)
        {
            var entidad = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return mapper.Map<CharacterDto>(entidad);
        }


        [HttpGet("name")]
        public async Task<ActionResult<List<CharacterDto>>> FilterbyName([FromQuery] FilterCharacterDto filterCharacterDto)
        {
            var characterQuery = context.Films.AsQueryable();

            if (!string.IsNullOrEmpty(filterCharacterDto.Name))
            {
                characterQuery = characterQuery.Where(x => x.Title.Contains(filterCharacterDto.Name));
            }

            var dto = await characterQuery.ToListAsync();

            return mapper.Map<List<CharacterDto>>(dto);
        }

        [HttpGet("age")]
        public async Task<ActionResult<List<CharacterDto>>> FilterbyAge([FromQuery] FilterCharacterDto filterCharacterDto)
        {
            var characterQuery = context.Characters.AsQueryable();

            //navegamos a traves de las propiedades de navegacion hasta la tabla relacionada 
            characterQuery = characterQuery.Where(x => x.Age.Equals(filterCharacterDto.Age));


            var dto = await characterQuery.ToListAsync();

            return mapper.Map<List<CharacterDto>>(dto);
        }

        [HttpGet("weight")]
        public async Task<ActionResult<List<CharacterDto>>> FilterbyWeight([FromQuery] FilterCharacterDto filterCharacterDto)
        {
            var characterQuery = context.Characters.AsQueryable();

            //navegamos a traves de las propiedades de navegacion hasta la tabla relacionada 
            characterQuery = characterQuery.Where(x => x.Weight.Equals(filterCharacterDto.Weight));


            var dto = await characterQuery.ToListAsync();

            return mapper.Map<List<CharacterDto>>(dto);
        }

        [HttpGet("movie")]
        public async Task<ActionResult<List<CharacterDto>>> FilterbyMovie([FromQuery] FilterCharacterDto filterCharacterDto)
        {
            var characterQuery = context.Characters.AsQueryable();

            //navegamos a traves de las propiedades de navegacion hasta la tabla relacionada 
            characterQuery = characterQuery
                .Where(x => x.CharactersFilms.Select(z => z.FilmId)
                .Contains(filterCharacterDto.MovieId));


            var dto = await characterQuery.ToListAsync();

            return mapper.Map<List<CharacterDto>>(dto);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        //programacion asincrona podemos trabajar de manera más eficiente las conexiones a la
        //base de datos. 
        //devolvemos task porque es requisito de la programacion asincrona.
        //FromBody => queremos sacar desde el cuerpo de la peticion la siguiente información
        public async Task<ActionResult> Post([FromForm] CharacterCreationDto character)
        {
            var entidad = mapper.Map<Character>(character);

            if (character.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await character.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(character.Imagen.FileName);
                    
                    entidad.Imagen = await almacenadorArchivos.SaveFile(contenido, extension, contenedor, character.Imagen.ContentType);
                }
            }



            context.Add(entidad);
            await context.SaveChangesAsync();
            var characterDto = mapper.Map<CharacterDto>(entidad);

            return new CreatedAtRouteResult("obtenerPersonaje", new { id = characterDto.Id }, characterDto);
        }

        [HttpPut("{id}")] //api/autores/1
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Put(int id, [FromForm] CharacterCreationDto character)
        {

            var characterDB = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);

            if (characterDB == null) { return NotFound(); }

            //Esto hace que los campos recibidos en character se peguen en characterDB actualizando solo los campos
            //ingresados por el usuario.
            characterDB = mapper.Map(character, characterDB);


            if (character.Imagen != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await character.Imagen.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(character.Imagen.FileName);

                    characterDB.Imagen = await almacenadorArchivos.EditFile(contenido, extension, contenedor, characterDB.Imagen, character.Imagen.ContentType);
                }
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]//api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Characters.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound(); ;
            }

            context.Remove(new Character() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
