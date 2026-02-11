using System;
using static Sudoku.OptionMat;
using static Sudoku.Utilities;
using static Sudoku.Validations;
using static Sudoku.SolvingAlgorithm;
namespace Sudoku
{
    public class Program
    {
        /* The main function. starts a timer to check how much time it took to solve the sudoku
        * if the sudoku is not a legal board a fitting message will be printed and you will be able
        * to enter a new board. if you input the string "exit" the program will stop.
        */
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs e) =>
            {
                e.Cancel = true;
            };// makes it so you can take Ctrl+C as input and not terminate the run
            Console.WriteLine("Enter a 81 characters long string that represents a sudoku board\n" +
                "The numbers will vary from 0-9 where 1-9 means a filled square and a 0 an empty square");
            Console.WriteLine("Type exit to quit the program");
            string input = Console.ReadLine();
            int[,] board;
            int[,,] optionsMat;
            while (input.ToLower() != "exit")
            {
                try
                {
                    IsInputValid(input);
                    board = ToMat(input);
                    PrintBoard(board);

                    StartTime = DateTime.Now;

                    optionsMat = Scan(board);
                    PrintBoard(Solve(board, optionsMat));

                    EndTime = DateTime.Now.Subtract(StartTime);
                    Console.WriteLine($"\ntime to solve:{DateTime.Now.Subtract(StartTime).TotalSeconds}s");
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message); 
                }
                Console.WriteLine("\nEnter a new board: \n Type exit to quit the program");
                input = Console.ReadLine();
            }
        }
    }
}