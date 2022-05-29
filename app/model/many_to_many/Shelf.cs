
namespace model.many_to_many
{
    public class Shelf
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }

        public Shelf() { }

        public Shelf(int userID, int bookID)
        {
            UserID = userID;
            BookID = bookID;
        }

        public Shelf(User user, Book book)
        {
            User = user;
            UserID = user.ID;
            Book = book;
            BookID = book.ID;
        }

    }
}
