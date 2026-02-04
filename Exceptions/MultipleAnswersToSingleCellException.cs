using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public class MultipleAnswersToSingleCellException : Exception
    {
        public MultipleAnswersToSingleCellException(string message) : base(message) {}
    }
}
