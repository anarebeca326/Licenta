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
    class UserRepoTest
    {
        private Mock<DbSet<User>> _mockSet;
        private Mock<IBookshelfContext> _mockContext;

        [SetUp]
        public void setup()
        {
            var user1 = new User("Username1", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            user1.ID = 1;
            var user2 = new User("Username2", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            user2.ID = 2;
            var sourceList = new List<User>();
            sourceList.Add(user1);
            sourceList.Add(user2);
            var queryable = sourceList.AsQueryable();

            _mockSet = new Mock<DbSet<User>>();
            _mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            _mockSet.Setup(d => d.Add(It.IsAny<User>())).Callback<User>(sourceList.Add);

            _mockContext = new Mock<IBookshelfContext>();
            _mockContext.Setup(m => m.Users).Returns(_mockSet.Object);
        }

        [Test]
        public void get_should_return_item_if_found()
        {
            //Setup
            var repo = new UserRepository(_mockContext.Object);
            var user = repo.Get(1);

            //Verifying
            _mockContext.Verify(m => m.Users, Times.Once);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.ID, 1);
        }

        [Test]
        public void get_should_return_null_if_not_found()
        {
            //Setup
            var repo = new UserRepository(_mockContext.Object);
            var user = repo.Get(3);

            //Verifying
            _mockContext.Verify(m => m.Users, Times.Once);
            Assert.IsNull(user);
        }

        [Test]
        public void get_should_return_null_when_encounter_error()
        {
            //Setup
            _mockContext.Setup(m => m.Users).Throws(new Exception());
            var repo = new UserRepository(_mockContext.Object);
            var user = repo.Get(1);

            //Verifying
            _mockContext.Verify(m => m.Users, Times.Once);
            Assert.IsNull(user);
        }


        [Test]
        public void get_all_should_return_all_items()
        {
            //Setup
            var repo = new UserRepository(_mockContext.Object);
            var users = repo.GetAll();

            //Verifying
            _mockContext.Verify(m => m.Users, Times.Once);
            Assert.IsNotNull(users);
            Assert.AreEqual(2, users.Count);
        }

        [Test]
        public void get_all_should_return_null_when_encounter_error()
        {
            //Setup
            _mockContext.Setup(m => m.Users).Throws(new Exception());
            var repo = new UserRepository(_mockContext.Object);
            var users = repo.GetAll();

            //Verifying
            _mockContext.Verify(m => m.Users, Times.Once);
            Assert.IsNull(users);
        }

        [Test]
        public void create_should_return_true_for_valid_data()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var repo = new UserRepository(_mockContext.Object);
            var added = repo.Create(new User("Username3", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50));

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(added);
        }

        [Test]
        public void create_should_return_false_for_invalid_data()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var repo = new UserRepository(_mockContext.Object);
            var added = repo.Create(new User());

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(added);
        }

        [Test]
        public void create_should_return_false_when_encounter_error()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var repo = new UserRepository(_mockContext.Object);
            var added = repo.Create(new User());

            // Verifying
            _mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(added);
        }

        [Test]
        public void delete_should_return_true_when_id_can_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var repo = new UserRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(deleted);
        }


        [Test]
        public void delete_should_return_false_when_id_cannot_be_found()
        {
            // Setup
            var repo = new UserRepository(_mockContext.Object);

            var deleted = repo.Delete(3);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Never());
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void delete_should_return_false_when_encounter_error()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Throws(new Exception());

            var repo = new UserRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void delete_should_return_false_when_savechanges_doesnt_return_1()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var repo = new UserRepository(_mockContext.Object);
            var deleted = repo.Delete(1);

            // Verifying
            _mockSet.Verify(m => m.Remove(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(deleted);
        }

        [Test]
        public void update_should_return_true_when_parameters_are_valid()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(1);

            var updated_user = new User("Username1", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            updated_user.ID = 1;

            var repo = new UserRepository(_mockContext.Object);
            var updated = repo.Update(updated_user);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(updated);
        }

        [Test]
        public void update_should_return_false_when_id_cannot_be_found()
        {
            // Setup
            _mockContext.Setup(m => m.Users.Update(It.IsAny<User>())).Throws(new Exception());

            var updated_user = new User("Username1", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);

            var repo = new UserRepository(_mockContext.Object);
            var updated = repo.Update(updated_user);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Never());
            Assert.IsFalse(updated);
        }

        [Test]
        public void update_should_return_false_when_savechanges_doesnt_return_1()
        {
            // Setup
            _mockContext.Setup(m => m.SaveChanges()).Returns(0);

            var updated_user = new User("Username1", "Password", model.types.Gender.ANOTHER, 30, 50, 50, 50, 50, 50);
            updated_user.ID = 5;

            var repo = new UserRepository(_mockContext.Object);
            var updated = repo.Update(updated_user);

            // Verifying
            _mockSet.Verify(m => m.Update(It.IsAny<User>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsFalse(updated);
        }
    }
}
