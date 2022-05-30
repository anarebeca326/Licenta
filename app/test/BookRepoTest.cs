using database;
using Microsoft.EntityFrameworkCore;
using model;
using Moq;
using NUnit.Framework;
using persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace test
{
    public class BookRepoTest
    {
        private Mock<DbSet<Book>> _mockSet;
        private Mock<IBookshelfContext> _mockContext;

        [SetUp]
        public void setup()
        {
            var book1 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book1.ID = 1;
            var book2 = new Book("Title", "Author", "Description", 5, 5, 5, 5, 5);
            book2.ID = 2;
            var sourceList = new List<Book>();
            sourceList.Add(book1);
            sourceList.Add(book2);
            var queryable = sourceList.AsQueryable();

            _mockSet = new Mock<DbSet<Book>>();
            _mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            _mockSet.Setup(d => d.Add(It.IsAny<Book>())).Callback<Book>(sourceList.Add);

            _mockContext = new Mock<IBookshelfContext>();
            _mockContext.Setup(m => m.Books).Returns(_mockSet.Object);
        }

        [Test]
        public void get_should_return_item_if_found()
        {
            //Setup
            var repo = new BookRepository(_mockContext.Object);
            Book book = repo.Get(1);

            //Verifying
            _mockContext.Verify(m => m.Books, Times.Once);
            Assert.IsNotNull(book);
            Assert.AreEqual(book.ID, 1);
        }

        [Test]
        public void get_should_return_null_if_not_found()
        {
            //Setup
            var repo = new BookRepository(_mockContext.Object);
            Book book = repo.Get(3);

            //Verifying
            _mockContext.Verify(m => m.Books, Times.Once);
            Assert.IsNull(book);
        }

        [Test]
        public void get_should_return_null_when_encounter_error()
        {
            //Setup
            _mockContext.Setup(m => m.Books).Throws(new Exception());
            var repo = new BookRepository(_mockContext.Object);
            Book book = repo.Get(1);

            //Verifying
            _mockContext.Verify(m => m.Books, Times.Once);
            Assert.IsNull(book);
        }


        [Test]
        public void get_all_should_return_all_items()
        {
            //Setup
            var repo = new BookRepository(_mockContext.Object);
            var books = repo.GetAll();

            //Verifying
            _mockContext.Verify(m => m.Books, Times.Once);
            Assert.IsNotNull(books);
            Assert.AreEqual(2, books.Count);
        }

        [Test]
        public void get_all_should_return_null_when_encounter_error()
        {
            //Setup
            _mockContext.Setup(m => m.Books).Throws(new Exception());
            var repo = new BookRepository(_mockContext.Object);
            var books = repo.GetAll();

            //Verifying
            _mockContext.Verify(m => m.Books, Times.Once);
            Assert.IsNull(books);
        }

        [Test]
        public void create_should_return_true_for_valid_data()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var repo = new BookRepository(_mockContext.Object);
            var added = repo.Create(new Book("Title", "Author", "Description", 5, 5, 5, 5, 5));

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(added);
        }

        [Test]
        public void create_should_return_false_for_invalid_data()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var repo = new BookRepository(_mockContext.Object);
            var added = repo.Create(new Book());

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(added);
        }

        [Test]
        public void create_should_return_false_when_encounter_error()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var repo = new BookRepository(_mockContext.Object);
            var added = repo.Create(new Book());

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(added);
        }

        [Test]
        public void delete_should_return_true_when_id_can_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var repo = new BookRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(deleted);
        }


        [Test]
        public void delete_should_return_false_when_id_cannot_be_found()
        {
            // Setup
            var repo = new BookRepository(_mockContext.Object);

            var deleted = repo.Delete(3);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<Book>()), Times.Never());
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void delete_should_return_false_when_encounter_error()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var repo = new BookRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void delete_should_return_false_when_savechanges_doesnt_return_1()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var repo = new BookRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void update_should_return_true_when_parameters_are_valid()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var updated_book = new Book("NewTitle", "Author", "Description", 5, 5, 5, 5, 5);
            updated_book.ID = 1;

            var repo = new BookRepository(_mockContext.Object);
            var updated = repo.Update(updated_book);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(updated);
        }

        [Test]
        public void update_should_return_false_when_id_cannot_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.Books.Update(It.IsAny<Book>())).Throws(new Exception());

            var updated_book = new Book("NewTitle", "Author", "Description", 5, 5, 5, 5, 5);

            var repo = new BookRepository(_mockContext.Object);
            var updated = repo.Update(updated_book);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
            Assert.IsFalse(updated);
        }

        [Test]
        public void update_should_return_false_when_savechanges_doesnt_return_1()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var updated_book = new Book("NewTitle", "Author", "Description", 5, 5, 5, 5, 5);
            updated_book.ID = 5;

            var repo = new BookRepository(_mockContext.Object);
            var updated = repo.Update(updated_book);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<Book>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(updated);
        }
    }
}