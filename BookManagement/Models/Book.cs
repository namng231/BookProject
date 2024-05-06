using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public string? Field { get; set; }

        public string? Author { get; set; }

        public string? Thumbnail { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm:ss dd/MM/yyyy}")]
        public DateTime CreatedDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm:ss dd/MM/yyyy}")]
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<Page> Pages { get; set; } = new HashSet<Page>();

    }
}
