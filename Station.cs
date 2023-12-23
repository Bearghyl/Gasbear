using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gasbear
{
    public class Station
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Street { get; set; }
        public string? Place { get; set; }
        public float? Lat { get; set; }
        public float? Lng { get; set; }
        public float? Dist { get; set; }
        public float? Diesel { get; set; }
        public float? E5 { get; set; }
        public float? E10 { get; set; }
        public bool IsOpen { get; set; }
        public string? HouseNumber { get; set; }
        public int PostCode { get; set; }
    }
}
