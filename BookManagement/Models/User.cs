namespace BookManagement.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Username { get; set; }

        public required string Password { get; set; }

        // Bookmarks
        public virtual ICollection<Page> Pages { get; set; } = new HashSet<Page>();

    }
}
