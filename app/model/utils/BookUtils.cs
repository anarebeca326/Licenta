using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace model.utils
{
    public class BookUtils
    {

        public static Book UpdateRatings(Book book, float communal, float aesthetic, float dark, float thrilling, float cerebral)
            // Calculates the new mean value for the coefficients after receiving a new rating
        {
            var old_nr = book.NoRatings;
            book.NoRatings++;
            book.CommunalCoef = (book.CommunalCoef * old_nr + communal) / book.NoRatings;
            book.AesthetiCoef = (book.AesthetiCoef * old_nr + aesthetic) / book.NoRatings;
            book.DarkCoef = (book.DarkCoef * old_nr + dark) / book.NoRatings;
            book.ThrillingCoef = (book.ThrillingCoef * old_nr + thrilling) / book.NoRatings;
            book.CerebralCoef = (book.CerebralCoef * old_nr + cerebral) / book.NoRatings;

            return book;
        }

    }
}
