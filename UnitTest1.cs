using System.Collections.Generic;
using Sudoku;

namespace TestsForSudokuProj
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            using (StreamReader reader = new StreamReader(@"D:\17_clue.txt"))
            {
                string line;
                Program.total = DateTime.Now;
                while ((line = reader.ReadLine()) != null)
                {
                    int[,] board = Program.ToMat(line);
                    Program.time = DateTime.Now;
                    if(Program.Solve(board, null) == null)
                        throw new Exception("failed to solve solvable board"+ Program.MatToString(board));
                    if (Program.s.TotalSeconds > 1)
                    {
                        Console.WriteLine($"time to solve:{Program.s.TotalSeconds}s");
                        Program.PrintBoard(board);
                        Console.WriteLine(Program.MatToString(board));
                        throw new Exception("failed to run in time");
                    }
                    Program.first = true;
                }
                TimeSpan s2 = DateTime.Now.Subtract(Program.total);
                Console.WriteLine(s2);
            }
        }
    }
}
