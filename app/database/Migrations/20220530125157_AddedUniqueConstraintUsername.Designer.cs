// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using database;

namespace database.Migrations
{
    [DbContext(typeof(BookshelfContext))]
    [Migration("20220530125157_AddedUniqueConstraintUsername")]
    partial class AddedUniqueConstraintUsername
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("model.Book", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<float>("AesthetiCoef")
                        .HasColumnType("real");

                    b.Property<int>("Aesthetic")
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Cerebral")
                        .HasColumnType("int");

                    b.Property<float>("CerebralCoef")
                        .HasColumnType("real");

                    b.Property<int>("Communal")
                        .HasColumnType("int");

                    b.Property<float>("CommunalCoef")
                        .HasColumnType("real");

                    b.Property<int>("Dark")
                        .HasColumnType("int");

                    b.Property<float>("DarkCoef")
                        .HasColumnType("real");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NoRatings")
                        .HasColumnType("int");

                    b.Property<int>("Thrilling")
                        .HasColumnType("int");

                    b.Property<float>("ThrillingCoef")
                        .HasColumnType("real");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("model.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<float>("Agreeableness")
                        .HasColumnType("real");

                    b.Property<float>("Conscientiousness")
                        .HasColumnType("real");

                    b.Property<float>("Extraversion")
                        .HasColumnType("real");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<float>("Neuroticism")
                        .HasColumnType("real");

                    b.Property<float>("Openness")
                        .HasColumnType("real");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("model.many_to_many.Favourite", b =>
                {
                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.HasKey("UserID", "BookID");

                    b.HasIndex("BookID");

                    b.ToTable("Favourite");
                });

            modelBuilder.Entity("model.many_to_many.Shelf", b =>
                {
                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.HasKey("UserID", "BookID");

                    b.HasIndex("BookID");

                    b.ToTable("Shelf");
                });

            modelBuilder.Entity("model.many_to_many.Favourite", b =>
                {
                    b.HasOne("model.Book", "Book")
                        .WithMany("Enjoyers")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("model.User", "User")
                        .WithMany("Favourites")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("model.many_to_many.Shelf", b =>
                {
                    b.HasOne("model.Book", "Book")
                        .WithMany("Readers")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("model.User", "User")
                        .WithMany("BooksOnShelf")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("model.Book", b =>
                {
                    b.Navigation("Enjoyers");

                    b.Navigation("Readers");
                });

            modelBuilder.Entity("model.User", b =>
                {
                    b.Navigation("BooksOnShelf");

                    b.Navigation("Favourites");
                });
#pragma warning restore 612, 618
        }
    }
}
