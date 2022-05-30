using database;
using model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace persistence
{
    public class BookRepository : IRepository<Book, int>
    {
        private IBookshelfContext _context;
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BookRepository(IBookshelfContext context)
        {
            _context = context;
        }
        public bool Create(Book entity)
        {
            _logger.Info($"Adding book {entity} to database ...");

            try
            {
                _context.Books.Add(entity);
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected == 1)
                {
                    _logger.Info("One row affected.");
                    _logger.Info("Exitting method.");
                    return true;
                }
                else
                {
                    _logger.Info("Book could not be added to the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Book could not be added to the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }

        public bool Delete(int id)
        {
            _logger.Info($"Deleting book with id {id} from the database ...");

            try
            {
                var book = Get(id);
                if (book == null)
                    _logger.Info("Book could not be deleted from the database because the ID cannot be found.");
                else
                {
                    _context.Books.Remove(book);
                    int rowsAffected = _context.SaveChanges();
                    if (rowsAffected == 1)
                    {
                        _logger.Info("One row affected.");
                        _logger.Info("Exitting method.");
                        return true;
                    }
                    else
                    {
                        _logger.Info("Book could not be deleted from the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Book could not be deleted from the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }

        public Book Get(int id)
        {
            _logger.Info($"Searching book by id {id} ...");
            try
            {
                var result = _context.Books.FirstOrDefault(e => e.ID == id);
                if (result != null)
                {
                    _logger.Info($"Returning book {result}.");
                    _logger.Info("Exitting method.");
                    return result;
                }
                _logger.Info("Book was not found.");
            }
            catch(Exception ex)
            {
                _logger.Info("Book could not be retrieved from the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return null;
        }

        public List<Book> GetAll()
        {
            _logger.Info("Retrieving all books ...");
            try
            {
                var result = _context.Books.ToList();
                _logger.Info($"Returning list of all books {result}.");
                _logger.Info("Exitting method.");
                return result;
            }
            catch(Exception ex)
            {
                _logger.Info("Could not retrieve the list of books. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return null;
        }

        public bool Update(Book entity)
        {
            _logger.Info($"Updating book {entity} ...");
            try
            {
                _context.Books.Update(entity);
                int rowsAffected = _context.SaveChanges();
                if (rowsAffected == 1)
                {
                    _logger.Info("One row affected.");
                    _logger.Info("Exitting method.");
                    return true;
                }
                else
                {
                    _logger.Info("Book could not be updated in the database.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Book could not be updated in the database. Error: " + ex.Message);
            }

            _logger.Info("Exitting method.");
            return false;
        }
    }
}
