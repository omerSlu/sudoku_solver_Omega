using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Sudoku
{
    public class Program
    {
        static int houseSize;
        static bool first = true;
        
        static int[,] ToMat(string board)
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
        static void PrintBoard(int[,] board)
        {
            Console.WriteLine();
            for (int i = 0; i < houseSize; i++)
            {
                if (i % 3 == 0 && i != 0)
                    Console.WriteLine("----------------------------");
                for (int j = 0; j < houseSize; j++)
                {
                    Console.Write($" {board[i,j]} ");
                    if ((j + 1) % 3 == 0 && j != 8)
                        Console.Write("|");
                }
                Console.WriteLine();
            }
        }
        static bool IsSolved(int[,] board)
        {
            for (int i = 0; i < houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)
                {
                    if (board[i, j] == 0)
                        return false;
                }
            }
            return true;
        }
        static void TurnInToOptions(int[,] rows, int[,] cols, int[,] squares, int[,,] options, int[,] board)
        {
            int isHidden = 0;
            int temp = 0;
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
                                isHidden++;
                                temp = k + 1;
                                options[i, j, k]++;
                            }

                        }
                        if (isHidden == 1)
                        {
                            options[i, j, houseSize] = temp;
                        }
                            isHidden = 0;
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
            TurnInToOptions(inRows, inCols, inSquares, optionsMat, board);
            //PrintOptionsMat(optionsMat);
            return optionsMat;
        }
        static void UpdateOptions()
        {

        }
        static bool FillCell(int[,] board, int[,,] optionsMat, int i, int j, int num)
        {
            return true;
        }
        static bool FillNaked(int[,,] optionsMat, int[,] board)//fills numbers that appear only once in the house
        {
            int[] rowstemp = new int[houseSize];
            int[] colstemp = new int[houseSize];
            int[] squarestemp = new int[houseSize];
            bool changed = true;
            for (int i = 0;i < houseSize;i++)
            {
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
                    if (optionsMat[(i + 1) / 3 + (j + 1) / 3, j % 3, houseSize] != -1)
                    {
                        for (int k = 0; k < houseSize; k++)
                        {
                            if (optionsMat[(i + 1) / 3 + (j + 1) / 3, j % 3, k] != 0)
                                if (squarestemp[k] > 0)
                                    squarestemp[k] = -1;
                                else if (squarestemp[k] == 0)
                                    squarestemp[k] = j;
                        }
                    }
                }
                for (int k = 0; k < houseSize; k++)
                {
                    if (rowstemp[k] > 0)
                        changed = changed || FillCell(board, optionsMat, i, rowstemp[k] - 1, k + 1);
                    else if (colstemp[k] > 0)
                        changed = changed || FillCell(board, optionsMat, rowstemp[k], i - 1, k + 1);
                    else if (squarestemp[k] > 0)
                        changed = changed || FillCell(board, optionsMat, (i+1)/3 + (squarestemp[k]+1)/3, squarestemp[k] % 3, k + 1);
                }
            }
        }
        static int[,] Solve(int[,] board)
        {
            if(first)
            {
                Scan(board);
                first = false;
            }
            if(IsSolved(board))
                return board;
            FillNaked(optionsMat,board);
            return board;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a 81 characters long string that represents a sudoku board\n" +
                "The numbers will vary from 0-9 where 1-9 means a filled square and a 0 an empty square");
            string input = Console.ReadLine();
            houseSize = (int)Math.Sqrt(81);
            int[,] board = ToMat(input);
            PrintBoard(Solve(board));
        }
    }
}
//static void PrintOptionsMat(int[,,] options)
        //{
        //    for (int i = 0; i < houseSize; i++)
        //    {
        //        if (i % 3 == 0 && i != 0)
        //            Console.WriteLine("----------------------------");
        //        for (int j = 0; j < houseSize; j++)
        //        {
        //            if (options[i, j, houseSize] > 0)
        //                Console.Write(options[i, j, houseSize]);
        //            else
        //                Console.Write(0);
        //            if ((j + 1) % 3 == 0 && j != 8)
        //                Console.Write("|");
        //            else
        //                Console.Write(",");
        //        }
        //        Console.WriteLine();
        //    }
        //}