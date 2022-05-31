using model;

namespace services.dto
{
    public class PersonalBook
    {
        public Book Book { get; set; }
        public bool IsFavourite { get; set; }
        public bool IsOnShelf { get; set; }

        public PersonalBook(Book book, bool favourite, bool shelf)
        {
            Book = book;
            IsFavourite = favourite;
            IsOnShelf = shelf;
        }

        public PersonalBook(Book book)
        {
            Book = book;
            IsFavourite = false;
            IsOnShelf = false;
        }

    }
}
