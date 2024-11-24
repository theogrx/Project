using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Border
    {
        public int Id { get; set; }
        public string CountryBorder { get; set; }

        public Country Country {  get; set; }
    }
}
