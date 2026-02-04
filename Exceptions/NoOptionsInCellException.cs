using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Exceptions
{
    internal class NoOptionsInCellException : Exception
    {
        public NoOptionsInCellException(string message) : base(message) { }
    }
}
