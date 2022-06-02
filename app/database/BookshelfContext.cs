using Microsoft.EntityFrameworkCore;
using model;
using model.many_to_many;

namespace database
{
    public class BookshelfContext : DbContext, IBookshelfContext
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
            //optionsBuilder.UseSqlServer("Data Source=bookshelf-sv.database.windows.net;Initial Catalog=bookshelf_db;User Id=adminlogin;Password=kartoffelpuree27AZURE;");
            optionsBuilder.UseSqlServer("Data Source=bookshelf-sv.database.windows.net;Initial Catalog=biggerbookshelf_db;User Id=adminlogin;Password=kartoffelpuree27AZURE;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(book =>
            {
                book.HasKey(b => b.ID);
                book.Property(b => b.ID).ValueGeneratedOnAdd();
                book.HasMany(b => b.Readers);
                book.HasMany(b => b.Enjoyers);
                book.Property(b => b.Title).IsRequired();
                book.Property(b => b.Author).IsRequired();
                book.Property(b => b.Description).IsRequired();
                book.Property(b => b.CommunalCoef).IsRequired();
                book.Property(b => b.AesthetiCoef).IsRequired();
                book.Property(b => b.DarkCoef).IsRequired();
                book.Property(b => b.CerebralCoef).IsRequired();
            });

            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.ID);
                user.Property(u => u.ID).ValueGeneratedOnAdd();
                user.HasMany(u => u.BooksOnShelf);
                user.HasMany(u => u.Favourites);
                user.Property(u => u.Age).IsRequired();
                user.Property(u => u.Username).IsRequired();
                user.Property(u => u.Password).IsRequired();
                user.Property(u => u.Openness).IsRequired();
                user.Property(u => u.Conscientiousness).IsRequired();
                user.Property(u => u.Extraversion).IsRequired();
                user.Property(u => u.Agreeableness).IsRequired();
                user.Property(u => u.Neuroticism).IsRequired();
                user.HasIndex(u => u.Username).IsUnique();

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

            modelBuilder.Entity<Rated>(rated =>
            {
                rated.HasKey(r => new { r.UserID, r.BookID });

                rated.HasOne<User>(r => r.User)
                .WithMany(u => u.RatedBooks)
                .HasForeignKey(r => r.UserID);

                rated.HasOne<Book>(r => r.Book)
                .WithMany(b => b.RatedBy)
                .HasForeignKey(r => r.BookID);
            });
        }

    }
}
