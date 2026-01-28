using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    internal class Program
    {
        private static void PrintBoard(string board)
        {
            Console.WriteLine();
            for (int i = 0; i < board.Length / 9; i++)
            {
                if (i % 3 == 0 && i != 0)
                    Console.WriteLine("----------------------------");
                for (int j = 0; j < board.Length / 9; j++)
                {
                    Console.Write($" {board[9 * i + j]} ");
                    if ((j + 1) % 3 == 0 && j != 8)
                        Console.Write("|");
                }
                Console.WriteLine();
            }
        }
        private static string Solve(string board)
        {
            return board;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a 81 characters long string that represents a sudoku board\n" +
                "The numbers will vary from 0-9 where 1-9 means a filled square and a 0 an empty square");
            string board = Console.ReadLine();
            PrintBoard(Solve(board));
        }
    }
}
