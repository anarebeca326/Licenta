using Microsoft.EntityFrameworkCore;
using model;
using model.many_to_many;

namespace persistence.database
{
    public class BookshelfContext : DbContext
    {

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<Shelf> Shelf { get; set; }


        public BookshelfContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=bookshelf-sv.database.windows.net;Initial Catalog=bookshelf_db;User Id=adminlogin;Password=kartoffelpuree27AZURE;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(book =>
            {
                book.HasKey(b => b.ID);
                book.Property(b => b.ID).ValueGeneratedOnAdd();
                book.HasMany(b => b.Readers);
                book.HasMany(b => b.Enjoyers);

            });

            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.ID);
                user.Property(u => u.ID).ValueGeneratedOnAdd();
                user.HasMany(u => u.BooksOnShelf);
                user.HasMany(u => u.Favourites);

            });

            modelBuilder.Entity<Shelf>(shelf =>
            {
                shelf.HasKey(s => new { s.UserID, s.BookID });

                shelf.HasOne<User>(s => s.User)
                .WithMany(u => u.BooksOnShelf)
                .HasForeignKey(s => s.UserID);

                shelf.HasOne<Book>(s => s.Book)
                .WithMany(b => b.Readers)
                .HasForeignKey(s => s.BookID);
            });

            modelBuilder.Entity<Favourite>(favourite =>
            {
                favourite.HasKey(f => new { f.UserID, f.BookID });

                favourite.HasOne<User>(f => f.User)
                .WithMany(u => u.Favourites)
                .HasForeignKey(f => f.UserID);

                favourite.HasOne<Book>(f => f.Book)
                .WithMany(b => b.Enjoyers)
                .HasForeignKey(f => f.BookID);
            });
        }

    }
}
