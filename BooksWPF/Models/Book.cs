using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksWPF.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
        public int CountryId { get; set; }
        //public Country Country { get; set; }
        public override string ToString()
        {
            return $"{Id}    {Author}    {Price:f2}    {Title}    {CountryId}";
        }
    }
}
