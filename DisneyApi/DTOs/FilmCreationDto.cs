using DisneyApi.Helpers;
using DisneyApi.Validaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class FilmCreationDto
    {
        
        public string Title { get; set; }
        public DateTime Creation_Date { get; set; }
        public int Score { get; set; }

        [PesoArchivoValidacion(PesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Imagen { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder))]
        public List<int> GenresId { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder))]
        public List<int> CharactersId { get; set; }
    }
}
