using Microsoft.EntityFrameworkCore;

namespace BookManagement.Models
{
    public class BookContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set;}
        public DbSet<Page> Pages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(x => x.Username).IsUnique();
                e.HasMany(x => x.Pages).WithMany(x => x.Users).UsingEntity("Bookmarks");
            });

            modelBuilder.Entity<Page>(e =>
            {
                e.HasOne(x => x.Book).WithMany(x => x.Pages).OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
