using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using model;
using model.many_to_many;

namespace database
{
    public interface IBookshelfContext
    {   public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<Shelf> Shelf { get; set; }
        int SaveChanges();
    }
}
