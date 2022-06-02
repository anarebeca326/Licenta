
namespace model.many_to_many
{
    public class Rated
    {
        public int UserID { get; set; }
        public User User { get; set; }
        public int BookID { get; set; }
        public Book Book { get; set; }


        public Rated() { }

        public Rated(int userID, int bookID)
        {
            UserID = userID;
            BookID = bookID;
        }

        public Rated(User user, Book book)
        {
            User = user;
            UserID = user.ID;
            Book = book;
            BookID = book.ID;
        }
    }
}
