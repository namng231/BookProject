namespace BookManagement.Models
{
    public class Page
    {
        public Guid Id { get; set; }

        public int PageNumber { get; set; }

        public string? FilePath { get; set; }

        public Guid? BookId { get; set; }

        public virtual Book? Book { get; set; }

        public virtual ICollection<User> Users { get; set; } = new HashSet<User>();
    }
}
