using System;

namespace Sudoku
{// this class can be used for a board object for optimizations later on
    public static class Utilities
    {
        // helper to save starting time of the solving of a sudoku board
        public static DateTime StartTime; 
        // helper to calculate the time it took to solve the sudoku
        public static TimeSpan EndTime; 
        // the size of a row column or a square in the sudoku board
        public static int HouseSize = (int)Math.Sqrt(81); 

        /* Converts an inputed string of a sudoku board to a matrix.
         * The matrix is houseSize(9) by houseSize and for each cell there will be zero if empty and
         * number if filled
         * returns: the board as an int matrix 
         */
        public static int[,] ToMat(string board)
        {
            int[,] mat = new int[HouseSize, HouseSize];
            for (int i = 0; i < HouseSize; i++)
            {
                for (int j = 0; j < HouseSize; j++)
                {
                    mat[i, j] = board[i * HouseSize + j] - '0';
                }
            }
            return mat;
        }

        /*
        * empty cells will be printed as - "."
        * all other cells will be printed according to their value
        */
        public static void PrintBoard(int[,] board)
        {
            if (board == null)
            {
                Console.WriteLine("Board is unsolvable");
                return;
            }

            Console.WriteLine();
            for (int i = 0; i < HouseSize; i++)
            {
                if (i % 3 == 0 && i != 0)
                    Console.WriteLine("----------------------------");
                for (int j = 0; j < HouseSize; j++)
                {
                    if (board[i, j] != 0)
                        Console.Write($" {board[i, j]} ");
                    else
                        Console.Write(" . ");
                    if ((j + 1) % 3 == 0 && j != 8)
                        Console.Write("|");
                }
                Console.WriteLine();
            }
        }
    }
}
