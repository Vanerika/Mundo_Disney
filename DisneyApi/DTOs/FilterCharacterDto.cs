using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisneyApi.DTOs
{
    public class FilterCharacterDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }

        public int MovieId { get; set; }
    }
}
