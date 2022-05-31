using database;
using Microsoft.EntityFrameworkCore;
using model;
using model.many_to_many;
using Moq;
using NUnit.Framework;
using persistence;
using services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace test
{
    class BookshelfServiceTest
    {
        private Mock<DbSet<Book>> _mockBooks;
        private Mock<DbSet<User>> _mockUsers;
        private Mock<IBookshelfContext> _mockContext;


        [SetUp]
        public void Setup()
        {
            var books = GetBookSourceList();
            var queryable_books = books.AsQueryable();

            _mockBooks = new Mock<DbSet<Book>>();
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(queryable_books.Provider);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(queryable_books.Expression);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(queryable_books.ElementType);
            _mockBooks.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(queryable_books.GetEnumerator());
            _mockBooks.Setup(d => d.Add(It.IsAny<Book>())).Callback<Book>(books.Add);


            var users = GetUserSourceList();
            var queryable_users = users.AsQueryable();
            _mockUsers = new Mock<DbSet<User>>();
            _mockUsers.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable_users.Provider);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable_users.Expression);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable_users.ElementType);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable_users.GetEnumerator());
            _mockUsers.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(users.Add);

            _mockContext = new Mock<IBookshelfContext>();
            _mockContext.Setup(m => m.Books).Returns(_mockBooks.Object);
            _mockContext.Setup(m => m.Users).Returns(_mockUsers.Object);
        }

        private List<Book> GetBookSourceList()
        {
            var book1 = new Book("Title1", "Author", "Description", 5, 5, 5, 5, 5);
            book1.ID = 1;
            var book2 = new Book("Title2", "Author", "Description", 5, 5, 5, 5, 5);
            book2.ID = 2;
            var sourceList = new List<Book>();
            sourceList.Add(book1);
            sourceList.Add(book2);

            return sourceList;
        }

        private List<User> GetUserSourceList()
        {
            var user1 = new User("Username1", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            user1.ID = 1;
            var user2 = new User("Username2", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            user2.ID = 2;

            var book1 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book1.ID = 1;
            var book2 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book2.ID = 2;

            user1.Favourites = new List<Favourite>();
            user2.Favourites = new List<Favourite>();
            user1.BooksOnShelf = new List<Shelf>();
            user2.BooksOnShelf = new List<Shelf>();
            user1.BooksOnShelf.Add(new Shelf(user1, book1));
            user1.Favourites.Add(new Favourite(user1, book1));
            user1.Favourites.Add(new Favourite(user1, book2));

            var sourceList = new List<User>();
            sourceList.Add(user1);
            sourceList.Add(user2);

            return sourceList;
        }


        [Test]
        public void Login_should_return_id_of_succesfully_logged_user()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification

            Assert.AreEqual(service.Login("Username1", "Password"), 1);

        }

        [Test]
        public void Login_should_return_negative_one_if_login_fails()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification

            Assert.AreEqual(service.Login("Username1", "IncorrectPass"), -1);
        }


        [Test]
        public void Login_should_throw_exception_if_list_of_users_cannot_be_retrieved()
        {
            // Setup
            _mockContext.Setup(m => m.Users).Returns((DbSet<User>)null);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.Login("Username1", "Password"));
        }
        [Test]
        public void GetLastFavouritesForUser_should_return_maximum_given_nr_of_books()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.AreEqual(service.GetLastFavouritesForUser(1, 2).Count, 2);
            Assert.AreEqual(service.GetLastFavouritesForUser(1, 3).Count, 2);
            Assert.AreEqual(service.GetLastFavouritesForUser(1, 1).Count, 1);
            Assert.AreEqual(service.GetLastFavouritesForUser(1, 1)[0].ID, 2);
            Assert.IsNotNull(service.GetLastFavouritesForUser(2, 1));
            Assert.AreEqual(service.GetLastFavouritesForUser(2, 1).Count, 0);
        }

        [Test]
        public void GetLastFavouritesForUser_should_throw_exception_when_user_cannot_be_found()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            //Verification
            Assert.Throws<Exception>(() => service.GetLastFavouritesForUser(5, 1));
        }

        [Test]
        public void GetShelfForUser_should_return_all_shelved_books()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.AreEqual(service.GetShelfForUser(1).Count, 1);
            Assert.AreEqual(service.GetShelfForUser(2).Count, 0);
        }

        [Test]
        public void GetShelfForUser_should_throw_exception_when_user_cannot_be_found()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.GetShelfForUser(5));
        }

        [Test]
        public void FilterBooksByKeywords_should_return_filtered_books()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.AreEqual(1, service.FilterBooksByKeywords("Title1").Count);
        }


        [Test]
        public void FilterBooksByKeywords_should_throw_exception_when_the_list_of_books_cannot_be_retrieved()
        {
            // Setup
            _mockContext.Setup(m => m.Books).Returns((DbSet<Book>)null);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.FilterBooksByKeywords("Title"));
        }

        [Test]
        public void AddFavourite_should_throw_exception_when_book_or_user_cannot_be_found()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.AddFavourite(5, 1));
            Assert.Throws<Exception>(() => service.AddFavourite(1, 5));
        }

        [Test]
        public void AddFavourite_should_throw_exception_user_cannot_be_updated()
        {
            // Setup
            _mockContext.Setup(m => m.Users.Update(It.IsAny<User>())).Throws(new Exception());
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.AddFavourite(1, 1));
        }

        [Test]
        public void AddFavourite_should_update_user_favourites_when_successful()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            service.AddFavourite(2, 2);
            _mockContext.Verify(m => m.Users.Update(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);

        }

        [Test]
        public void AddToShelf_should_throw_exception_when_book_or_user_cannot_be_found()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.AddToShelf(5, 1));
            Assert.Throws<Exception>(() => service.AddToShelf(1, 5));
        }

        [Test]
        public void AddToShelf_should_throw_exception_user_cannot_be_updated()
        {
            // Setup
            _mockContext.Setup(m => m.Users.Update(It.IsAny<User>())).Throws(new Exception());
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.AddToShelf(1, 1));
        }

        [Test]
        public void AddToShelf_should_update_user_favourites_when_successful()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            service.AddToShelf(2, 2);
            _mockContext.Verify(m => m.Users.Update(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);

        }

        [Test]
        public void CreateUser_should_throw_exception_when_list_of_users_cannot_be_retrieved()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            _mockContext.Setup(m => m.Users).Returns((DbSet<User>)null);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.CreateUser("u", "p", "p", model.types.Gender.ANOTHER));
        }

        [Test]
        public void CreateUser_should_throw_exception_when_username_not_available()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.CreateUser("Username1", "p", "p", model.types.Gender.ANOTHER));
        }


        [Test]
        public void CreateUser_should_throw_exception_when_confirmation_is_invalid()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.CreateUser("u", "p", "invalid", model.types.Gender.ANOTHER));
        }

        [Test]
        public void CreateUser_should_throw_exception_when_user_cannot_be_added_to_the_database()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.CreateUser("u", "p", "p", model.types.Gender.ANOTHER));
        }

        [Test]
        public void CreateUser_should_add_new_user_if_input_is_valid()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            service.CreateUser("u", "p", "p", model.types.Gender.ANOTHER);
            _mockContext.Verify(m => m.Users.Add(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void UpdateUserPersonality_should_throw_exception_if_nr_of_responses_is_incorrect()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.UpdateUserPersonality(1, new List<int>()));
        }

        private List<int> getValidResponses()
        {
            var responses = new List<int>();
            for (int idx = 0; idx < 60; idx++)
                responses.Add(1);
            return responses;
        }

        [Test]
        public void UpdateUserPersonality_should_throw_exception_if_user_cannot_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.UpdateUserPersonality(5, getValidResponses()));
        }

        [Test]
        public void UpdateUserPersonality_should_throw_exception_if_changes_cannot_be_saved()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.UpdateUserPersonality(1, getValidResponses()));
        }

        [Test]
        public void UpdateUserPersonality_should_save_new_user_personality()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            service.UpdateUserPersonality(1, getValidResponses());
            _mockContext.Verify(m => m.Users.Update(It.IsAny<User>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);

        }

        [Test]
        public void UpdateBookCoefficients_should_throw_exception_if_book_cannot_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.UpdateBookCoefficients(5, new List<int> { 6, 6, 6, 6, 6}));
        }

        [Test]
        public void UpdateBookCoefficients_should_throw_exception_if_changes_cannot_be_saved()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.UpdateBookCoefficients(1, new List<int> { 6, 6, 6, 6, 6 }));
        }

        [Test]
        public void UpdateBookCoefficients_should_save_new_book_coefficients()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            service.UpdateBookCoefficients(1, new List<int> { 6, 6, 6, 6, 6 });
            _mockContext.Verify(m => m.Books.Update(It.IsAny<Book>()), Times.Once);
            _mockContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Test]
        public void BooksToPersonalBooks_should_throw_error_when_user_cannot_be_found()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            // Verification
            Assert.Throws<Exception>(() => service.BooksToPersonalBooks(new List<Book>(), 5));
        }

        [Test]
        public void BooksToPersonalBooks_should_return_list_of_PersonalBook_with_all_initial_books()
        {
            // Setup
            var userRepo = new UserRepository(_mockContext.Object);
            var bookRepo = new BookRepository(_mockContext.Object);
            var service = new BookshelfService(bookRepo, userRepo);

            var book1 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book1.ID = 1;
            var book2 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book2.ID = 2;
            var books = new List<Book>();
            books.Add(book1);
            books.Add(book2);

            // Verification
            var personal = service.BooksToPersonalBooks(books, 1);
            Assert.AreEqual(personal.Count, books.Count);
            Assert.IsTrue(personal[0].IsFavourite);
            Assert.IsTrue(personal[0].IsOnShelf);
            Assert.IsTrue(personal[1].IsFavourite);
            Assert.IsFalse(personal[1].IsOnShelf);

        }
    }    
}
