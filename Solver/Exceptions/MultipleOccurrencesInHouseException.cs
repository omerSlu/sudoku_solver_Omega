using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Exceptions
{
    public class MultipleOccurrencesInHouseException : Exception //(of numbers)
    {
        public MultipleOccurrencesInHouseException(string message) : base(message) { }
    }
}
