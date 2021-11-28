using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class FilterFilmDto
    {
        public string Title { get; set; }
        public int GenreId { get; set; }

        public DateTime Creation_Date { get; set; }
    }
}
