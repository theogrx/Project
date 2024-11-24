namespace Project.Models
{
    using System.Collections.Generic;

    public class CountryResponseDTO
    {
        public NameDTO Name { get; set; }
        public List<string> Borders { get; set; }
        public List<string> Capital { get; set; }

    }

    public class CountryDTO
    {
        public string Name { get; set; }
        public List<string> Borders { get; set; }
        public List<string> Capital { get; set; }

    }

    public class NameDTO
    {
        public string Common { get; set; }
    }


}
