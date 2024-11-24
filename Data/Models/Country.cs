using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    public class Country 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Border> Borders { get; set; }
        public List<Capital> Capital { get; set; }
    }
}
