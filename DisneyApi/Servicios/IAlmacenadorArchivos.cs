using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> SaveFile(byte[] contenido, string extension, string contenedor, string conntType);
        Task<string> EditFile(byte[] contenido, string extension, string contenedor, string ruta, string contentType);
        Task DeleteFile(string ruta, string contenedor);
    }
}
