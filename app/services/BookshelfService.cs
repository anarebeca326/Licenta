using model;
using model.many_to_many;
using model.types;
using model.utils;
using persistence;
using services.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace services
{
    public class BookshelfService
    {
        private IRepository<Book, int> _bookRepo;
        private IRepository<User, int> _userRepo;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public BookshelfService(IRepository<Book, int> bookRepo, IRepository<User, int> userRepo)
        {
            _bookRepo = bookRepo;
            _userRepo = userRepo;
        }


        public int Login(String username, String password) 
            /// Checks the credentials and returns the id of the user when they are valid
        {
            _logger.Info($"Logging in with username {username} ...");

            try
            {
                var user = _userRepo.GetAll().FirstOrDefault(u => u.Username == username && u.Password == password);
                if(user == null)
                {
                    _logger.Info("Credentials do not match. Returning -1.");
                    _logger.Info("Exitting method.");
                    return -1;
                }

                _logger.Info($"Credentials match for user {user}. Returning id {user.ID}.");
                _logger.Info("Exitting method.");
                return user.ID;
            }
            catch(Exception)
            {
                _logger.Info("The list of users could not be retrieved. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }
        }


        public List<Book> GetLastFavouritesForUser(int id, int nr)
            /// Returns last few favourite books for the user with the given id
            /// nr : maximum number of books returned
        {
            _logger.Info($"Finding top {nr} favourite books for user with id {id} ...");

            try
            {
                var all_favourite_links = _userRepo.Get(id).Favourites;

                var all_fav_books = new List<Book>();
                foreach (Favourite favourite in all_favourite_links)
                    all_fav_books.Add(favourite.Book);

                var result = all_fav_books.OrderByDescending(b => b.ID).Take(nr).ToList();
                _logger.Info($"Returning list of favourite books {result}.");
                _logger.Info("Exitting method.");
                return result;

            }
            catch (Exception)
            {
                _logger.Info("The list of books could not be retrieved. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

        }


        public List<Book> GetShelfForUser(int id)
            /// Returns a list of all the books user with the  
            /// given id has saved on his bookshelf
        {
            _logger.Info($"Finding shelved books for user with id {id} ...");

            try
            {
                var all_shelved_links = _userRepo.Get(id).BooksOnShelf;
                var all_shelved_books = new List<Book>();

                foreach (Shelf shelved in all_shelved_links)
                    all_shelved_books.Add(shelved.Book);

                _logger.Info($"Returning list of shelved books {all_shelved_books}");
                _logger.Info("Exitting method");
                return all_shelved_books;
            }
            catch (Exception)
            {
                _logger.Info("The list of books could not be retrieved. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }
        }

        
        public List<Book> FilterBooksByKeywords(String sequence)
            /// Returns a list of filtered books that contain the given sequence in their title
        {
            _logger.Info($"Searching books by keyword {sequence} ...");

            try
            {
                var all_books = _bookRepo.GetAll();
                var result = all_books.FindAll(b => b.Title.Contains(sequence)).ToList();
                _logger.Info($"Returning filtered books {result}");
                _logger.Info("Exitting method");
                return result;
            }
            catch (Exception)
            {
                _logger.Info("The list of books could not be retrieved. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }
        }


        public void AddFavourite(int userID, int bookID)
            /// Adds book with the given id to the list of favourites of user with the given id
        {
            _logger.Info($"Adding book with id {bookID} to favourites list of user with id {userID} ...");

            var user = _userRepo.Get(userID);
            var book = _bookRepo.Get(bookID);

            if(user == null || book == null)
            {
                _logger.Info("Could not retrieve user and book from the database. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

            user.Favourites.Add(new Favourite(user, book));
            var updated = _userRepo.Update(user);

            if (!updated)
            {
                _logger.Info("Could not create the link in the database. Throwing exception.");
                throw new Exception("User-Book link could not be created.");
            }

            _logger.Info("Book was added to favourites.");
            _logger.Info("Exitting method");

        }

        public void AddToShelf(int userID, int bookID)
            /// Adds book with the given id to the bookshelf of user with the given id
        {
            _logger.Info($"Adding book with id {bookID} to shelved list of user with id {userID} ...");

            var user = _userRepo.Get(userID);
            var book = _bookRepo.Get(bookID);

            if (user == null || book == null)
            {
                _logger.Info("Could not retrieve user and book from the database. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

            user.BooksOnShelf.Add(new Shelf(user, book));
            var updated = _userRepo.Update(user);

            if (!updated)
            {
                _logger.Info("Could not create the link in the database. Throwing exception.");
                throw new Exception("User-Book link could not be created.");
            }

            _logger.Info("Book was added to bookshelf.");
            _logger.Info("Exitting method");
        }


        public void CreateUser(String username, String password, String confirm_password, Gender gender)
            /// Creates a new user account
        {
            _logger.Info($"Creating a new user account for username {username} and password {password} with confirmation {confirm_password}");

            bool usernameAvailable;
            try
            {
                usernameAvailable = CheckUsernameAvailable(username);

                if (!usernameAvailable)
                {
                    _logger.Info("Username is already taken. Throwing exception.");
                    throw new Exception("This username is already taken.");
                }

                if (!password.Equals(confirm_password))
                {
                    _logger.Info("Password and confirmed password do not match. Throwing exception.");
                    throw new Exception("Password and Confirmed Password do not match.");
                }

                // Input is valid, creating account:
                var user = new User(username, password, gender, 30, -1, -1, -1, -1, -1);
                var created = _userRepo.Create(user);

                if (!created)
                {
                    _logger.Info("User could not be added to the database. Throwing exception");
                    throw new Exception("Account could not be created at the moment. Try again later.");
                }

                _logger.Info("Account was created.");
                _logger.Info("Exitting method");
            }
            catch (Exception e)
            {
                _logger.Info("Failed username verification. Throwing exception.");
                throw new Exception(e.Message);
            }

        }


        private bool CheckUsernameAvailable(String username)
            /// Verifies that the username is not already taken
        {
            _logger.Info($"Checking if username {username} is available ...");

            var users = _userRepo.GetAll();
            if(users == null)
            {
                _logger.Info("Could not retrieve the list of users from the database. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

            var existing = users.FirstOrDefault(u => u.Username == username);
            if (existing == null)
            {
                _logger.Info("Username is available. Returning true.");
                _logger.Info("Exitting method.");
                return true;
            }
                
            _logger.Info("Username is taken. Returning false.");
            _logger.Info("Exitting method.");
            return false;
        }

        public void UpdateUserPersonality(int userID, List<int> responses)
            /// Updates user's personality coefficients using the new responses
        {
            _logger.Info($"Updating personality coefficients for user with id {userID} considering responses {responses} ...");

            try
            {
                var coefs = UserUtils.ComputePersonalityCoefficients(responses);

                var user = _userRepo.Get(userID);
                if (user == null)
                {
                    _logger.Info("Could not retrieve user from the database. Throwing exception.");
                    throw new Exception("Could not retrieve data.");
                }

                user.Openness = coefs[0];
                user.Conscientiousness = coefs[1];
                user.Extraversion = coefs[2];
                user.Agreeableness = coefs[3];
                user.Neuroticism = coefs[4];

                var updated = _userRepo.Update(user);

                if (!updated)
                {
                    _logger.Info("Could not update user in the database. Throwing exception.");
                    throw new Exception("Could not update personality coefficients.");
                }

                _logger.Info("Personality coefficients have been updated.");
                _logger.Info("Exitting method");

            }
            catch(Exception e)
            {
                _logger.Info("The list of responses is invalid. Throwing exception.");
                throw new Exception(e.Message);
            }
        }

        public void UpdateBookCoefficients(int bookID, List<int> responses)
            /// Updating book ratings after receiving new feedback from a user
        {
            _logger.Info($"Updating coefficiens for book with id {bookID} ...");

            var book = _bookRepo.Get(bookID);
            if(book == null)
            {
                _logger.Info("Could not retrieve book from the database. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

            book = BookUtils.UpdateRatings(book, responses[0], responses[1], responses[2], responses[3], responses[4]);

            var updated = _bookRepo.Update(book);

            if (!updated)
            {
                _logger.Info("Could not update book in the database. Throwing exception.");
                throw new Exception("Could not update book coefficients.");
            }

            _logger.Info("Book coefficients have been updated.");
            _logger.Info("Exitting method");

        }

        public List<PersonalBook> BooksToPersonalBooks(List<Book> books, int userID)
            /// Returns a list of PersonalBook with info regarding the relationship
            /// between the given user and the list of books
        {
            _logger.Info($"Getting personal info about books {books} for user with id {userID} ...");

            try
            {
                var user = _userRepo.Get(userID);
                var favourites = user.Favourites;
                var shelved = user.BooksOnShelf;

                var personal_books = new List<PersonalBook>();
                foreach (Book book in books)
                {
                    var personal = new PersonalBook(book);

                    var infavs = favourites.FirstOrDefault(b => b.BookID == book.ID);
                    var onshelf = shelved.FirstOrDefault(b => b.BookID == book.ID);

                    if (infavs != null)
                        personal.IsFavourite = true;
                    if (onshelf != null)
                        personal.IsOnShelf = true;

                    personal_books.Add(personal);
                }

                _logger.Info($"Acquired personal informations of the books. Returning {personal_books}.");
                _logger.Info("Exitting method.");
                return personal_books;
            }
            catch (Exception)
            {
                _logger.Info("User information could not be retrieved. Throwing exception.");
                throw new Exception("Could not retrieve data.");
            }

        }

        public List<Book> GetPredictions(int id)
        {
            return new List<Book>();
        }

    }
}
