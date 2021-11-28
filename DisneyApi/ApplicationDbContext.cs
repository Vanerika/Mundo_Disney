using DisneyApi.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        //Debemos configurar una llave primaria compuesta de
        //CharacterFilm, FilmsGenres
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //HasKey --> tiene llave e indicamos las llaves primarias
            modelBuilder.Entity<CharactersFilms>()
                .HasKey(x => new { x.CharacterId, x.FilmId });

            modelBuilder.Entity<FilmsGenres>()
                .HasKey(x => new { x.GenreId, x.FilmId});

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Genre> Genres { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<CharactersFilms> CharactersFilms { get; set; }
        public DbSet<FilmsGenres> FilmsGenres { get; set; }
    }
}
