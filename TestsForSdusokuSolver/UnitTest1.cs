using static Sudoku.Utilities;
using static Sudoku.SolvingAlgorithm;
using static Sudoku.OptionMat;
using static Sudoku.Validations;

namespace TestsForSudokuSolver
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
            using (StreamReader reader = new StreamReader(@"\Sudoku\TestsForSdusokuSolver\Boards.txt"))
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
                if (e.Message != "Invalid character inputed")
                    throw new Exception("did not catch invalid char");
            }
            try
            {
                IsInputValid("00000000000000000000000000000000000000000000000000000000000000000000000000000000");
            }
            catch (Exception e)
            {
                if (e.Message != "Not enough charcters (board too small)")
                    throw new Exception("did not catch invalid board size");
            }
            try
            {
                IsInputValid("0000000000000000000000000000000000000000000000000000000000000000000000000000000000");
            }
            catch (Exception e)
            {
                if (e.Message != "To many characters (board too large)")
                    throw new Exception("did not catch invalid board size");
            }
            try // two 99's in the same row
            {
                int[,] board = ToMat("9000900000000000000000000000000000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 row: 0")
                    throw new Exception("did not catch same number in row" + e.Message);
            }

            try // two 99's in the same col
            {
                int[,] board = ToMat("900000000000000000000000000900000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 column: 0")
                    throw new Exception("did not catch same number in col " + e.Message);
            }

            try // two 99's in the same square
            {
                int[,] board = ToMat("9000000000000000000000000090000000000000000000000000000000000000000000000000000000");
                Solve(board, Scan(board));
            }
            catch (Exception e)
            {
                if (e.Message != "two or more of the same number in 1 square: 0")
                    throw new Exception("did not catch illegal board (square)");
            }
        }
    }
}
