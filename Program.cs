using System;
using System.Collections.Generic;
using System.IO;

namespace Sudoku
{
    public class Program
    {
        public static DateTime time; // helper to save starting time of the solving of a sudoku board
        public static TimeSpan s; // helper to calculate the time it took to solve the sudoku
        public static int houseSize = (int)Math.Sqrt(81); // the size of a row column or a square in the sudoku board
        public static bool first = true; // a boolean helper to check if you are in the first instance of the recursive solve function
        
        public static int[,] ToMat(string board) 
        /*
         * converts an inputed string of a sudoku board to a matrix
         * the matrix is the houseSize(9) by houseSize and for each cell there will be zero if empty and
         * for filled cells there will be the inputed number
         * returns: the board as an int matrix
         */
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
        public static void PrintBoard(int[,] board) 
        /*
        * prints the board from an int matrix
        * like what was described in the "ToMat" function an slot int the matrix will be represented by 
        * the number zero and will be printed as - "."
        * all other cells will be printed according to their value
        * return: void
        */
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
        static int SolveState(int[,,] optionsMat)
        /*
         * checks the state of a board, the return value work as follows:
         * -1 - the board is not solvable
         *  0 - the board is solvable from what we know now but could be found unsolvable later
         *  1 - the board is solved
         */ 
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
        /*
         * This function takes previously found numbers in each: row, column or square and puts in each cell
         * of an 3 dimentional array the numbers that are possible in their fitting place.
         * The 3 dimentional array is a two dimentional array of the board where in each cell there is an
         * array of 10 ints, the first 9 represents the options of the cell, an one in the X index of the 
         * array is equivelent to saying that one of the options of the cell is X+1.
         * The 10th slot represents the number of options of that cell.
         * returns: void
         */ 
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
                            if (rows[i, k] > 1 || cols[j, k] > 1 || squares[square, k] > 1)
                                throw new Exception("two or more of the same number in 1 house");
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
        /*
         * This functions scans the board for the numbers in each row, column and square and then calls
         * the function that turns them into a 3 dimentional array of options like what was described before
         * returns: the optionMat
         */ 
        {
            // first dimention is the index of the row, col or square respectively
            // the second dimention is the numbers that are in the respective house(rows, cols, squares)
            int[,] inRows = new int[houseSize, houseSize];
            int[,] inCols = new int[houseSize, houseSize];
            int[,] inSquares = new int[houseSize, houseSize];
            // the options mat. explained in detail in the "TurnIntoOptions" func
            int[,,] optionsMat = new int[houseSize, houseSize, houseSize + 1];
            for (int i = 0; i < houseSize; i++)
            {
                for (int j = 0; j < houseSize; j++)// adds one to the number's index for each house
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
        /*
         * If a cell was filled this function will be called.
         * The function updates the optionMat according to the limitations of the rules of sudoku
         * returns: void
         */ 
        {
            for(int k = 0; k < houseSize; k++)
            {
                    optionsMat[i, j, k] = 0; // cleans all the options of this cell
                // updates the options in the col. if it updated a cell update the amount of options in the cell
                if (optionsMat[k,j, num - 1] == 1)
                {
                    optionsMat[k, j, num - 1] = 0;
                    optionsMat[k, j, houseSize]--;
                }
                // same but for row
                if (optionsMat[i, k, num - 1] == 1)
                {
                    optionsMat[i, k, num - 1] = 0;
                    optionsMat[i, k, houseSize]--;
                }
                //same but for square
                if (optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, num - 1] == 1)
                {
                    optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, num - 1] = 0;
                    optionsMat[i / 3 * 3 + k / 3, k % 3 + j / 3 * 3, houseSize]--;
                }
                optionsMat[i, j, houseSize] = -1; // put the amount of options as -1 to symbol its filled
            }
        }
        static void FillCell(int[,] board, int[,,] optionsMat, int i, int j, int num)
        /*
         * Fills the cell in the number that was calculated to be there
         * two numbers are calculated to must be in the same cell the function throws an exception
         * that concludes the sudoku is not solvable.
         * Because we use brute force sometimes this does not ncessarily means the inputed board is unsolvable
         * returns: void
         */
        {
            if (board[i, j] != 0 && board[i, j] != num)
                throw new Exception("not solvable two right numbers in same cell");
            board[i, j] = num;
            UpdateOptions(optionsMat, i, j, num);
        }
        static bool FillHidden(int[,,] optionsMat, int[,] board)
        /*
         * Fills numbers that only have one option in a house
         * returns: if filled a cell - true, else - false
         */
        {
            bool changed = false;
            for (int i = 0;i < houseSize;i++)
            {
                // index serves as indicator if number appeared or not.
                // for a number that showed once in the house we will save a number to relocate them
                // for numbers that did not show - 0 else, "-1"
                int[] rowstemp = new int[houseSize];
                int[] colstemp = new int[houseSize];
                int[] squarestemp = new int[houseSize];
                for (int j = 0; j < houseSize; j++)// all houses
                {
                    //checks rows (only if empty cell)
                    if (optionsMat[i, j, houseSize] != -1)
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
                    int row = i / 3 * 3 + j / 3, col = j % 3 + i % 3 * 3;// rows and cols to scan a square
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
        static bool FillNaked(int[,,] optionsMat, int[,] board)
        /*
         * Checks if its the only possible number in the cell
         * returns: true if filled a cell, else - false
         */
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
        /*
         * Extracts the row and col of the cell with the highest chance to guess right.
         * returns an array 2 ints long, index zero contains the row and index one the column
         */
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
        /*
         * The main solving function. a recursive function that calls all the functions that try to solve
         * the board, when it fails to solve it tries to guess the cell with the best chances to succeed
         * and calls the function again. If guessed wrong it will try to guess the next option and if all
         * guesses failed returns an empty board that represents it failed to solve the board or in other 
         * words the board is unsolvable
         * returns: the board if it was solved, else null
         */
        {
            if (first)
            {
                optionsMat = Scan(board);
                first = false;
            }

            while (FillHidden(optionsMat, board) || FillNaked(optionsMat, board));// try to solve by logic

            int boardstate = SolveState(optionsMat);
            if (boardstate == 1)// sudoku solved end timer and return board
            {
                s = DateTime.Now.Subtract(time);
                return board;
            }
            else if (boardstate == -1)// path unsolvable try another option or declare board unsolvable
                return null;

            int [] iAndJ = ExtractBest(optionsMat);
            int i = iAndJ[0], j = iAndJ[1];
            int[,] tempBoard = new int[houseSize, houseSize];
            int[,,] tempOptions = new int[houseSize, houseSize, houseSize + 1];

            for (int k = 0; k < houseSize; k++)
            {
                if (optionsMat[i, j, k] != 0)
                {
                    // make a copy of the board and the optionMat to save the state of them incase of failure
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
                    tempBoard = new int[houseSize, houseSize]; // reset board for next guess
                    tempOptions = new int[houseSize, houseSize, houseSize + 1]; // reset Mat for next guess
                }
            }
            if (SolveState(optionsMat) == 1)
                return board;
            return null;
        }
        public static void IsInputValid(string input)
        /*         
         * If the inputted string length is not 81, throw exception for not fitting board size.
         * If the inputted string contains non digits
         */ 
        {
            if (input.Length != 81)
                throw new Exception("invalid board size");
            for(int i = 0; i<houseSize*houseSize; i++)
                if (!Char.IsDigit(input[i]))
                    throw new Exception("invalid input, invalid character inputed");
        }
        static void Main(string[] args)
        /*
         * The main function. starts a timer to check how much time it took to solve the sudoku
         * if the sudoku is not a legal board a fitting message will be printed and you will be able
         * to enter a new board. if you input the string "exit" the program will stop.
         */
        {
            Console.WriteLine("Enter a 81 characters long string that represents a sudoku board\n" +
                "The numbers will vary from 0-9 where 1-9 means a filled square and a 0 an empty square");
            Console.WriteLine("Type exit to quit the program");
            string input = Console.ReadLine();
            while (input != "exit")
            {
                try
                {
                    IsInputValid(input);
                    int[,] board = ToMat(input);
                    PrintBoard(board);
                    time = DateTime.Now;
                    board = Solve(board, null);
                    if(board != null)
                        PrintBoard(board);
                    else
                        Console.WriteLine("board is unsolvable. ill`egal board");
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