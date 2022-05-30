using Microsoft.EntityFrameworkCore;
using model;
using model.many_to_many;
using model.types;
using model.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace database.seed
{
    public class Seeder
    {
        public static void PopulateBooks(BookshelfContext context)
        {
            var books = ReadBooksFromCSV();

            foreach (Book book in books){
                context.Books.Add(book);
            }
            context.SaveChanges();
        }

        public static void PopulateUsers(BookshelfContext context)
        {
            var users = ReadUsersFromCSV();

            foreach (User user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();
        }

        public static void PopulateFavourite(BookshelfContext context)
        {
            var pairs = ReadRelationsFromCSV();

            foreach(Tuple<int, int> pair in pairs)
            {
                var user_fav = context.Users.Include(u => u.Favourites).FirstOrDefault(u => u.ID == pair.Item1);
                var book_fav = context.Books.Include(u => u.Enjoyers).FirstOrDefault(b => b.ID == pair.Item2);

                user_fav.Favourites.Add(new Favourite(user_fav, book_fav));
                context.Update(user_fav);
                context.SaveChanges();
            }
        }

        public static void PopulateShelf(BookshelfContext context)
        {
            var pairs = ReadRelationsFromCSV();

            foreach (Tuple<int, int> pair in pairs)
            {
                var user_shelf = context.Users.Include(u => u.BooksOnShelf).FirstOrDefault(u => u.ID == pair.Item1);
                var book_shelf = context.Books.Include(u => u.Readers).FirstOrDefault(b => b.ID == pair.Item2);

                user_shelf.BooksOnShelf.Add(new Shelf(user_shelf, book_shelf));
                context.Update(user_shelf);
                context.SaveChanges();
            }
        }

        private static List<Book> ReadBooksFromCSV()
        {

            List<Book> books = new List<Book>();

            using (var reader = new StreamReader(@"C:\Users\MSI\Desktop\de_toate\Facultate\Sem2\ELL\TheActualApp\database\seed\resources\books.tsv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    var title = values[0];
                    var author = values[1];
                    var communal = float.Parse(values[2]);
                    var aesthetic = float.Parse(values[3]);
                    var dark = float.Parse(values[4]);
                    var thrilling = float.Parse(values[5]);
                    var cerebral = float.Parse(values[6]);
                    var description = values[8];

                    books.Add(new Book(title, author, description, communal, aesthetic, dark, thrilling, cerebral));
                }
            }

            return books;
        }


        private static List<User> ReadUsersFromCSV()
        {

            List<User> users = new List<User>();

            using (var reader = new StreamReader(@"C:\Users\MSI\Desktop\de_toate\Facultate\Sem2\ELL\TheActualApp\database\seed\resources\users.tsv"))
            {
                reader.ReadLine();

                int user_id = 0;
                while (!reader.EndOfStream)
                {
                    user_id++;
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    var username = "user" + user_id.ToString();
                    var password = "password" + user_id.ToString();
                    var gender_str = values[2];
                    var age = int.Parse(values[3]);

                    var responses = new List<int>();
                    for(int idx = 4; idx < 64; idx++)
                    {
                        responses.Add(int.Parse(values[idx]));
                    }

                    var coefs = UserUtils.ComputePersonalityCoefficients(responses);

                    Gender gender;
                    switch (gender_str)
                    {
                        case "Masculin":
                            gender = Gender.MALE;
                            break;

                        case "Feminin":
                            gender = Gender.FEMALE;
                            break;

                        default:
                            gender = Gender.ANOTHER;
                            break;
                    }

                    users.Add(new User(username, password, gender, age, coefs[0], coefs[1], coefs[2], coefs[3], coefs[4]));
                }
            }

            return users;
        }

        private static List<Tuple<int, int>> ReadRelationsFromCSV()
        {
            var pairs = new List<Tuple<int, int>>();
            using (var reader = new StreamReader(@"C:\Users\MSI\Desktop\de_toate\Facultate\Sem2\ELL\TheActualApp\database\seed\resources\book-user.tsv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    var userID = int.Parse(values[0]);
                    var bookID = int.Parse(values[1]);

                    var pair = Tuple.Create(userID, bookID);
                    pairs.Add(pair);
                }
            }
            return pairs;
        }
    }
}
