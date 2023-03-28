using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksWPF.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        //public ICollection<Book> Books { get; set; }

        public override string ToString()
        {
            return $"{Id}    {CountryName}";
        }
    }
}
