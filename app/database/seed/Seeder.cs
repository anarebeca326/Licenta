using Microsoft.EntityFrameworkCore;
using model;
using model.many_to_many;
using model.types;
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
            }


            context.SaveChanges();
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
            }


            context.SaveChanges();
        }

        public static void AddImagesToBooks(BookshelfContext context)
        {
            var pairs = ReadTitleImagePairs();

            foreach(Tuple<String,String> pair in pairs)
            {
                var book = context.Books.FirstOrDefault(b => b.Title == pair.Item1);
                book.CoverImage = pair.Item2;
                context.Update(book);
                Console.WriteLine("Updated " + pair.Item1);
            }
            context.SaveChanges();
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

                    var coefs = ComputePersonalityCoefficients(responses);

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

        private static List<float> ComputePersonalityCoefficients(List<int> r)
        // r: list of responses to the 60 personality questions
        // returns the list of personality scores (OPE, COS, EXT, AGR, NEU)
        {
            if (r.Count < 60)
                throw new Exception("Invalid number of responses: " + r.Count.ToString() + ". (Should be 60)");

            int agr_points = r[0] - r[5] + r[10] + r[15] + r[20] - r[25] - r[30] + r[35] + r[43] + r[48] + r[53] + r[58];  // minimum -6p, maximum 42p
            int agr_score = (agr_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            int cos_points = r[1] - r[6] + r[11] + r[16] + r[21] + r[26] - r[31] - r[36] + r[41] - r[46] + r[51] + r[56];  // minimum -12p, maximum 36p
            int cos_score = (cos_points + 12) * 100 / 48;  // +12  =>  scale 0-48

            int ext_points = r[2] + r[7] + r[12] + r[17] - r[22] + r[27] - r[32] - r[37] - r[42] - r[47] + r[52] + r[57];  // minimum -18p, maximum 30p
            int ext_score = (ext_points + 18) * 100 / 48;  // +18  =>  scale 0-48

            int neu_points = r[3] + r[8] + r[13] - r[18] + r[23] + r[28] - r[33] - r[38] + r[44] + r[49] + r[54] + r[59];  // minimum -6p, maximum 42p
            int neu_score = (neu_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            int ope_points = r[4] + r[9] + r[14] + r[19] + r[24] - r[29] - r[34] - r[39] + r[40] + r[45] + r[50] + r[55];  // minimum -6p, maximum 42p
            int ope_score = (ope_points + 6) * 100 / 48;  // +6  =>  scale 0-48

            return new List<float> { ope_score, cos_score, ext_score, agr_score, neu_score };
        }

        private static List<Tuple<String, String>> ReadTitleImagePairs()
        {
            var pairs = new List<Tuple<String, String>>();
            using (var reader = new StreamReader(@"C:\Users\MSI\Desktop\de_toate\Facultate\Sem2\ELL\TheActualApp\database\seed\resources\title_imagename.tsv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split('\t');

                    var title = values[0];
                    var image = values[1];

                    var pair = Tuple.Create(title, image);
                    pairs.Add(pair);
                }
            }
            return pairs;
        }
    }
}
