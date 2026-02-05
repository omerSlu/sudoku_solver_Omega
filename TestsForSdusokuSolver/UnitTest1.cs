using System.Collections.Generic;
using System.Linq.Expressions;
using static Sudoku.Utilities;
using static Sudoku.Program;
using static Sudoku.OptionMat;

namespace TestsForSudokuProj
{

    public class UnitTest1
    {
        public string MatToString(int[,] board)
        {
            string sBoard = "";
            for (int i = 0; i < HouseSize; i++)
                for (int j = 0; j < HouseSize; j++)
                    sBoard += board[i, j];
            return sBoard;
        }
        [Fact]
        public void Test1()
        {
            using (StreamReader reader = new StreamReader(@"\TestsForSudokuProj\Boards.txt"))
            {
                string line;
                DateTime total = DateTime.Now;
                while ((line = reader.ReadLine()) != null)
                {
                    int[,] board = ToMat(line);
                    StartTime = DateTime.Now;
                    int[,,] optionsMat = Scan(board);
                    if (Solve(board, optionsMat) == null)
                        throw new Exception("failed to solve solvable board" + MatToString(board));
                    if (EndTime.TotalSeconds > 1)
                    {
                        Console.WriteLine($"time to solve:{EndTime.TotalSeconds}s");
                        PrintBoard(board);
                        Console.WriteLine(MatToString(board));
                        throw new Exception("failed to run in time");
                    }
                }
                TimeSpan s2 = DateTime.Now.Subtract(total);
                Console.WriteLine(s2);
            }
        }
        [Fact]
        public void Test2()
        {
            try
            {
                IsInputValid("00000000000000000000000000000000000000000000000000000000000000000000000000000000a");
            }
            catch (Exception e)
            {
                if (e.Message != "invalid input, invalid character inputed")
                    throw new Exception("did not catch invalid char");
            }
            try
            {
                IsInputValid("00000000000000000000000000000000000000000000000000000000000000000000000000000000");
            }
            catch (Exception e)
            {
                if (e.Message != "invalid board size")
                    throw new Exception("did not catch invalid board size");
            }
            try
            {
                IsInputValid("0000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            }
            catch (Exception e)//two or more of the same number in 1 house
            {
                if (e.Message != "invalid board size")
                    throw new Exception("did not catch invalid board size");
            }
            try // two 99's in the same row
            {
                int[,] board = ToMat("9000900000000000000000000000000000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 house")
                    throw new Exception("did not catch illegal board (row)" + e.Message);
            }
            try // two 99's in the same col
            {
                int[,] board = ToMat("9000000000900000000000000000000000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 house")
                    throw new Exception("did not catch illegal board  (col)");
            }
            try // two 99's in the same square
            {
                int[,] board = ToMat("9000000000090000000000000000000000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 house")
                    throw new Exception("did not catch illegal board  (square)");
            }
        }
    }
}
