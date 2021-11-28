using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class ListFilmsDto
    {
        public string Title { get; set; }
        public DateTime Creation_Date { get; set; }
        public string Imagen { get; set; }
    }
}
