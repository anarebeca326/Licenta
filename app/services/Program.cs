using database;
using persistence;
using System;

namespace services
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new BookshelfContext();
            var repo = new BookRepository(context);

            var books = repo.GetAll();

            Console.WriteLine(books[0].Readers.Count);
        }
    }
}
