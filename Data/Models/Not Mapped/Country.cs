namespace Project.Models
{
    using System.Collections.Generic;

    public class Country
    {
        public Name Name { get; set; }
        public List<string> Borders { get; set; }
        public List<string> Capital { get; set; }

    }

    public class Name
    {
        public string Common { get; set; }
        public string Official { get; set; }
    }


}
