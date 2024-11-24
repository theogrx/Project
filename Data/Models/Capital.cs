using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Capital
    {
        public int Id { get; set; }
        public string CapitalCode { get; set; }
        public Country Country { get; set; }
    }
}
