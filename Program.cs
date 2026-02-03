using System;
using System.Collections.Generic;
using System.IO;

namespace Sudoku
{
    public class Program
    {
        public static DateTime time;
        public static DateTime total;
        public static TimeSpan s;
        public static int houseSize = (int)Math.Sqrt(81);
        public static bool first = true;
        
        public static int[,] ToMat(string board)
        {
            int[,] mat = new int [houseSize, houseSize]; 
            for (int i = 0; i< houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)
                {
                    mat[i, j] = board[i * houseSize + j] - '0';
                }
            }
            return mat;
        }
        public static void PrintBoard(int[,] board)// prints the board from a matrix
        {
            Console.WriteLine();
            for (int i = 0; i < houseSize; i++)
            {
                if (i % 3 == 0 && i != 0)
                    Console.WriteLine("----------------------------");
                for (int j = 0; j < houseSize; j++)
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
        static int SolveState(int[,,] optionsMat)//returns: -1 - not solvable 0 - solvable but not solved 1 - solved 
        {
            int tempIllegal = 0;
            int tempEmpty = 0;
            bool notSolved = false;
            for (int i = 0; i < houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)
                {
                    if (optionsMat[i, j, houseSize] == 0)
                        return -1;
                    if (optionsMat[i, j, houseSize] != -1)
                        return 0;
                }
            }
            return 1;
        }
        static void TurnIntoOptions(int[,] rows, int[,] cols, int[,] squares, int[,,] options, int[,] board)
        {
            int amount = 0;
            int square = 0;
            for (int i = 0; i < houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)
                {
                    if (board[i, j] == 0)
                    {
                        square = i / 3 * 3 + j / 3;
                        for (int k = 0; k < houseSize; k++)
                        {
                            if (rows[i, k] == 0 && cols[j, k] == 0 && squares[square, k] == 0)
                            {
                                amount++;
                                options[i, j, k]++;
                            }

                        }
                        if (amount > 0)
                        {
                            options[i, j, houseSize] = amount;
                        }
                        else
                            throw new Exception("cell has no potential options");
                        amount = 0;
                    }
                    else
                        options[i, j, houseSize] = -1;
                }
            }
        }
        static int[,,] Scan(int[,] board)
        {
            int[] temp = new int[houseSize];
            int[,] inRows = new int[houseSize, houseSize];
            int[,] inCols = new int[houseSize, houseSize];
            int[,] inSquares = new int[houseSize, houseSize];
            int[,,] optionsMat = new int[houseSize, houseSize, houseSize + 1];
            for (int i = 0; i < houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)
                {
                    if (board[i, j] != 0)
                    {
                        inRows[i, board[i, j] - 1]++;
                        inSquares[i / 3 * 3 + j / 3, board[i, j] - 1]++;
                    }
                    if (board[j, i] != 0)
                    {
                        inCols[i, board[j, i] - 1]++;
                    }
                }
            }
            TurnIntoOptions(inRows, inCols, inSquares, optionsMat, board);
            return optionsMat;
        }
        static void UpdateOptions(int[,,] optionsMat, int i, int j, int num)
        {
            
            for(int k = 0; k < houseSize; k++)
            {
                if (optionsMat[i, j, k] == 1)
                    optionsMat[i, j, k] = 0;
                if (optionsMat[k,j, num - 1] == 1)
                {
                    optionsMat[k, j, num - 1] = 0;
                    optionsMat[k, j, houseSize]--;
                }
                if (optionsMat[i, k, num - 1] == 1)
                {
                    optionsMat[i, k, num - 1] = 0;
                    optionsMat[i, k, houseSize]--;
                }
                if (optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, num - 1] == 1)
                {
                    optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, num - 1] = 0;
                    optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, houseSize]--;
                }
                optionsMat[i, j, houseSize] = -1;
            }
        }
        static bool FillCell(int[,] board, int[,,] optionsMat, int i, int j, int num)
        {
            if (board[i, j] != 0 && board[i, j] != num)
                throw new Exception("not solvable two nakeds in same cell");
            if (board[i, j] == num)
                return true; 
            board[i, j] = num;
            UpdateOptions(optionsMat, i, j, num);

            return true;
        }
        static bool FillHidden(int[,,] optionsMat, int[,] board)//fills numbers that appear only once in the house
        {
            
            bool changed = false;
            for (int i = 0;i < houseSize;i++)
            {
                int[] rowstemp = new int[houseSize];
                int[] colstemp = new int[houseSize];
                int[] squarestemp = new int[houseSize];
                for (int j = 0; j < houseSize; j++)// all houses
                {
                    //checks rows (only if empty cell)
                    if (optionsMat[i, j, houseSize] != -1)//using a dynamic array will let optimize the search for what numbers do not possible in this slot
                    {
                        for (int k = 0; k < houseSize; k++)
                        {
                            if (optionsMat[i, j, k] != 0)
                            {
                                if (rowstemp[k] > 0)
                                    rowstemp[k] = -1;
                                else if (rowstemp[k] == 0)
                                    rowstemp[k] = j + 1;
                            }
                        }
                    }
                    //checks cols (only if empty cell)
                    if (optionsMat[j, i, houseSize] != -1)
                    {
                        for (int k = 0; k < houseSize; k++)
                        {
                            if (optionsMat[j, i, k] != 0)
                            {
                                if (colstemp[k] > 0)
                                    colstemp[k] = -1;
                                else if (colstemp[k] == 0)
                                    colstemp[k] = j + 1;
                            }
                        }
                    }
                    int row = i / 3 * 3 + j / 3, col = j % 3 + i % 3 * 3;// rows and cols to sacn a square
                    if (optionsMat[row, col, houseSize] != -1)
                    {
                        
                        for (int k = 0; k < houseSize; k++)
                        {
                            if (optionsMat[row, col, k] != 0)
                                if (squarestemp[k] > 0)
                                    squarestemp[k] = -1;
                                else if (squarestemp[k] == 0)
                                    squarestemp[k] = j + 1;
                        }
                    }
                }
                for (int k = 0; k < houseSize; k++)
                {
                    if (rowstemp[k] > 0)
                    {
                        FillCell(board, optionsMat, i, rowstemp[k] - 1, k + 1);
                        changed = true;
                    }
                    if (colstemp[k] > 0)
                    {
                        FillCell(board, optionsMat, colstemp[k] - 1, i, k + 1);
                        changed = true;
                    }
                    if (squarestemp[k] > 0)
                    {
                        FillCell(board, optionsMat, i / 3 * 3 + (squarestemp[k] - 1) / 3, (squarestemp[k] - 1) % 3 + i % 3 * 3, k + 1);
                        changed = true;
                    }
                }
            }
            return changed;
        }
        static bool FillNaked(int[,,] optionsMat, int[,] board)//checks if its the onlt possible number in the cell
        {
            bool changed = false;
            int temp = 0;
            int k;
            for (int i = 0; i < houseSize; i++)
                for (int j = 0; j < houseSize; j++)
                    if (board[i, j] == 0)
                    {
                        if (optionsMat[i, j, houseSize] == 1)
                        {
                            k = 0;
                            while (optionsMat[i, j, k] == 0) k++;
                            FillCell(board, optionsMat, i, j,k+1);
                            changed = true;
                        }
                    }
            return changed;
            
        }
        static int[] ExtractBest(int[,,] optionsMat)
        {
            int minI = 0;
            int minJ = 0;
            int minOptions = houseSize;
            int[] iAndJ = new int[2];
            for (int i = 0;i < houseSize;i++)
                for(int j = 0;j < houseSize;j++)
                {
                    if (optionsMat[i, j, houseSize] > 0 && optionsMat[i,j, houseSize] < minOptions)
                    {
                        minI = i;
                        minJ = j;
                        minOptions = optionsMat[i, j, houseSize];
                    }
                }
            iAndJ[0] = minI;
            iAndJ[1] = minJ;
            return iAndJ;
                    
        }
        public static int[,] Solve(int[,] board, int[,,] optionsMat)
        {
            if (first)
            {
                optionsMat = Scan(board);
                first = false;
            }
            while (FillHidden(optionsMat, board) || FillNaked(optionsMat, board));
            int boardstate = SolveState(optionsMat);// -1 - not solvabel, 0 - solvable but not solved, 1 - solved 
            if (boardstate == 1)
            {
                s = DateTime.Now.Subtract(time);
                return board;
            }
            if (boardstate == -1)// path unsolvable try another option or declare board unsolvable
                return null;
            int [] iAndJ = ExtractBest(optionsMat);
            int i = iAndJ[0], j = iAndJ[1];
            int[,] tempBoard = new int[houseSize, houseSize];
            int[,,] tempOptions = new int[houseSize, houseSize, houseSize + 1];
            for (int k = 0; k < houseSize; k++)
            {
                if (optionsMat[i, j, k] != 0)
                {
                    Array.Copy(board, tempBoard, houseSize * houseSize);
                    Array.Copy(optionsMat, tempOptions, houseSize * houseSize * (houseSize + 1));
                    FillCell(tempBoard, tempOptions, i, j, k + 1);
                    try
                    {
                        tempBoard = Solve(tempBoard, tempOptions);
                    }
                    catch
                    {
                        tempBoard = new int[houseSize, houseSize];
                        tempOptions = new int[houseSize, houseSize, houseSize + 1];
                        continue;
                    }
                    if (tempBoard != null)
                    {
                        return tempBoard;
                    }
                    tempBoard = new int[houseSize, houseSize];
                    tempOptions = new int[houseSize, houseSize, houseSize + 1];
                }
            }
            if (SolveState(optionsMat) == 1)
                return board;
            return null;
        }
        public static string MatToString(int[,] board)
        {
            string sBoard = "";
            for(int i = 0; i<houseSize; i++)
                for(int j = 0; j<houseSize; j++)
                    sBoard += board[i, j];
            return sBoard;
        }
        public static void IsInputValid(string input)
        {
            if (input.Length != 81)
                throw new Exception("invalid board size");
            for(int i = 0; i<houseSize*houseSize; i++)
                if (!Char.IsDigit(input[i]))
                    throw new Exception("invalid input, invalid chaacter inputed");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a 81 characters long string that represents a sudoku board\n" +
                "The numbers will vary from 0-9 where 1-9 means a filled square and a 0 an empty square");
            
            string input = Console.ReadLine();
            while (input != "exit")
            {
                try
                {
                    IsInputValid(input);
                    Console.WriteLine("hi");
                    int[,] board = ToMat(input);
                    PrintBoard(board);
                    time = DateTime.Now;
                    board = Solve(board, null);
                    if(board != null)
                        PrintBoard(board);
                    else
                        Console.WriteLine("board is unsolvable. illagel board");
                    Console.WriteLine($"time to solve:{s.TotalSeconds}s");
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.ToString()); 
                }
                Console.WriteLine("enter a new board");
                input = Console.ReadLine();
                first = true;
            }
        }
    }
}