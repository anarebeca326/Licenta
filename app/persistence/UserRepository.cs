using database;
using Microsoft.EntityFrameworkCore;
using model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace persistence
{
    public class UserRepository : IRepository<User, int>
    {
        private IBookshelfContext _context;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public UserRepository(IBookshelfContext context)
        {
            _context = context;
        }
        public bool Create(User entity)
        {
            _logger.Info($"Adding user {entity} to database ...");

            try
            {
                _context.Users.Add(entity);
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected == 1)
                {
                    _logger.Info("One row affected.");
                    _logger.Info("Exitting method.");
                    return true;
                }
                else
                {
                    _logger.Info("User could not be added to the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("User could not be added to the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }

        public bool Delete(int id)
        {
            _logger.Info($"Deleting user with id {id} from the database ...");

            try
            {
                var user = Get(id);
                if (user == null)
                    _logger.Info("User could not be deleted from the database because the ID cannot be found.");
                else
                {
                    _context.Users.Remove(user);
                    int rowsAffected = _context.SaveChanges();
                    if (rowsAffected == 1)
                    {
                        _logger.Info("One row affected.");
                        _logger.Info("Exitting method.");
                        return true;
                    }
                    else
                    {
                        _logger.Info("User could not be deleted from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("User could not be deleted from the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }

        public User Get(int id)
        {
            _logger.Info($"Searching user by id {id} ...");
            try
            {
                var result = _context.Users.Include(u => u.BooksOnShelf).Include(u => u.Favourites).FirstOrDefault(e => e.ID == id);
                if (result != null)
                {
                    _logger.Info($"Returning user {result}.");
                    _logger.Info("Exitting method.");
                    return result;
                }
                _logger.Info("User was not found.");
            }
            catch (Exception ex)
            {
                _logger.Info("User could not be retrieved from the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return null;
        }

        public List<User> GetAll()
        {
            _logger.Info("Retrieving all users ...");
            try
            {
                var result = _context.Users.Include(u => u.BooksOnShelf).Include(u => u.Favourites).ToList();
                _logger.Info($"Returning list of all users {result}.");
                _logger.Info("Exitting method.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Info("Could not retrieve the list of users. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return null;
        }

        public bool Update(User entity)
        {
            _logger.Info($"Updating user {entity} ...");
            try
            {
                _context.Users.Update(entity);
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected == 1)
                {
                    _logger.Info("One row affected.");
                    _logger.Info("Exitting method.");
                    return true;
                }
                else
                {
                    _logger.Info("User could not be updated in the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("User could not be updated in the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }
    }
}
