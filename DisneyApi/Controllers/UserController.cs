using AutoMapper;
using DisneyApi.DTOs;
using DisneyApi.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetAll()
        {
            var entidades = await context.Users.ToListAsync();
            var dtos = mapper.Map<UserDto>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerUsuario")]
        public async Task<ActionResult<List<UserDto>>> Get(int id)
        {
            var entidad = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<List<UserDto>>(entidad);
            return dto;
        }



        [HttpPost]
        [Consumes("multipart/form-data")]
        //programacion asincrona podemos trabajar de manera más eficiente las conexiones a la
        //base de datos. 
        //devolvemos task porque es requisito de la programacion asincrona.
        //FromBody => queremos sacar desde el cuerpo de la peticion la siguiente información
        public async Task<ActionResult> Post([FromForm] UserCreationDto user)
        {
            var entidad = mapper.Map<User>(user);
            context.Add(entidad);
            await context.SaveChangesAsync();
            var userDto = mapper.Map<UserDto>(entidad);

            return new CreatedAtRouteResult("obtenerPelicula", new { id = userDto.Id }, userDto);
        }

        [HttpPut("{id:int}")] //api/autores/1
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Put(int id, [FromForm] UserCreationDto user)
        {
            var entidad = mapper.Map<User>(user);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]//api/autores/1
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Users.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound(); ;
            }

            context.Remove(new User() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
