using Sudoku.Exceptions;
using System;
using static Sudoku.OptionMat;
using static Sudoku.Utilities;
using static Sudoku.Validations;

namespace Sudoku
{
    public class Program
    {
        /* checks the state of a board, the return value work as follows:
         * -1 - the board is not solvable
         *  0 - the board seems solvable but now solved
         *  1 - the board is solved
         */
        static int SolveState(int[,,] optionsMat)
        {
            for (int i = 0; i < HouseSize; i++)
            {
                for (int j = 0; j < HouseSize; j++)
                {
                    if (optionsMat[i, j, HouseSize] == 0) // no options in unfilled cell
                        return -1;
                    if (optionsMat[i, j, HouseSize] != -1) // not a filled cell
                        return 0;
                }
            }
            return 1;
        }
        
        /* If a cell was filled this function will be called.
         * The function updates the optionMat according to the limitations of the rules of sudoku
         */ 
        static void UpdateOptions(int[,,] optionsMat, int i, int j, int num)
        {
            for(int k = 0; k < HouseSize; k++)
            {
                optionsMat[i, j, k] = 0; // cleans all the options of this cell
                // updates the options in the col. if it updated a cell update the amount of options in the cell
                if (optionsMat[k,j, num - 1] == 1)
                {
                    optionsMat[k, j, num - 1] = 0;
                    optionsMat[k, j, HouseSize]--;
                }
                // same but for row
                if (optionsMat[i, k, num - 1] == 1)
                {
                    optionsMat[i, k, num - 1] = 0;
                    optionsMat[i, k, HouseSize]--;
                }
                //same but for square
                int row = i / 3 * 3 + k / 3, col = k % 3 + j / 3 * 3;
                if (optionsMat[row, col, num - 1] == 1)
                {
                    optionsMat[row, col, num - 1] = 0;
                    optionsMat[row, col, HouseSize]--;
                }
                optionsMat[i, j, HouseSize] = -1; // put the amount of options as -1 to symbol its filled
            }
        }
        
         /* Fills the cell in the number that was calculated to be there
         * two numbers are calculated to must be in the same cell the function throws an exception
         * that concludes the sudoku is not solvable.
         * Because brute force is used, this does not ncessarily means the inputed board is unsolvable
         */
        static void FillCell(int[,] board, int[,,] optionsMat, int i, int j, int num)
        {
            if (board[i, j] != 0 && board[i, j] != num)
                throw new MultipleAnswersToSingleCellException("Two or more answers to a single cell in " +
                    $"row: {i}, col:{j}");
            board[i, j] = num;
            UpdateOptions(optionsMat, i, j, num);
        }

        /* Fills numbers that only have one option in a house
         * returns: if filled a cell - true, else - false
         */
        static bool FillHidden(int[,,] optionsMat, int[,] board)
        {
            bool changed = false;
            for (int i = 0;i < HouseSize;i++)
            {
                // index serves as indicator if number appeared or not.
                // for a number that showed once in the house we will save a number to relocate them
                // for numbers that did not show - 0 else, "-1"
                int[] rowstemp = new int[HouseSize];
                int[] colstemp = new int[HouseSize];
                int[] squarestemp = new int[HouseSize];
                for (int j = 0; j < HouseSize; j++)// all houses
                {
                    //checks rows (only if empty cell)
                    if (optionsMat[i, j, HouseSize] != -1)
                    {
                        for (int k = 0; k < HouseSize; k++)
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
                    if (optionsMat[j, i, HouseSize] != -1)
                    {
                        for (int k = 0; k < HouseSize; k++)
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
                    if (optionsMat[row, col, HouseSize] != -1)
                    {
                        
                        for (int k = 0; k < HouseSize; k++)
                        {
                            if (optionsMat[row, col, k] != 0)
                                if (squarestemp[k] > 0)
                                    squarestemp[k] = -1;
                                else if (squarestemp[k] == 0)
                                    squarestemp[k] = j + 1;
                        }
                    }
                }
                for (int k = 0; k < HouseSize; k++)
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

        /* Checks if its the only possible number in the cell
         * returns: true if filled a cell, else - false
         */
        static bool FillNaked(int[,,] optionsMat, int[,] board)
        {
            bool changed = false;
            int k;
            for (int i = 0; i < HouseSize; i++)
                for (int j = 0; j < HouseSize; j++)
                    if (board[i, j] == 0)
                    {
                        if (optionsMat[i, j, HouseSize] == 1)
                        {
                            k = 0;
                            while (optionsMat[i, j, k] == 0) k++;
                            FillCell(board, optionsMat, i, j,k+1);
                            changed = true;
                        }
                    }
            return changed;
            
        }

        /* Extracts the row and col of the cell with the highest chance to guess right.
         * returns an array of 2 ints, index zero contains the row and index one the column
         */
        static int[] ExtractBest(int[,,] optionsMat)
        {
            int minOptions = HouseSize;
            int[] iAndJ = new int[2];
            for (int i = 0;i < HouseSize;i++)
                for(int j = 0;j < HouseSize;j++)
                    if (optionsMat[i, j, HouseSize] > 0 && optionsMat[i,j, HouseSize] < minOptions)
                    {
                        iAndJ[0] = i;
                        iAndJ[1] = j;
                        minOptions = optionsMat[i, j, HouseSize];
                    }
            return iAndJ;       
        }
        /* The main solving function. a recursive function that calls all the functions that try to solve
         * the board, when it fails to solve it tries to guess the cell with the best chances to succeed
         * and calls the function again. If guessed wrong it will try to guess the next option and if all
         * guesses failed returns an empty board that represents it failed to solve the board or in other 
         * words the board is unsolvable
         * returns: the board if it was solved, else null
         */
        public static int[,] Solve(int[,] board, int[,,] optionsMat)
        {
            while (FillHidden(optionsMat, board) || FillNaked(optionsMat, board));// try to solve by logic
            int boardstate = SolveState(optionsMat);
            if (boardstate == 1)// sudoku solved end timer and return board
            {
                
                return board;
            }
            else if (boardstate == -1)// path unsolvable try another option or declare board unsolvable
                return null;

            int [] iAndJ = ExtractBest(optionsMat);
            int row = iAndJ[0], column = iAndJ[1];
            int[,] tempBoard;
            int[,,] tempOptions;
            for (int k = 0; k < HouseSize; k++)
            {
                if (optionsMat[row, column, k] != 0)
                {
                    tempBoard = new int[HouseSize, HouseSize]; // reset board for guess
                    tempOptions = new int[HouseSize, HouseSize, HouseSize + 1]; // reset Mat for guess
                    // make a copy of the board and the optionMat to save the state of them incase of failure
                    Array.Copy(board, tempBoard, HouseSize * HouseSize);
                    Array.Copy(optionsMat, tempOptions, HouseSize * HouseSize * (HouseSize + 1));
                    FillCell(tempBoard, tempOptions, row, column, k + 1);
                    try
                    {
                        tempBoard = Solve(tempBoard, tempOptions);
                    }
                    catch
                    {
                        tempBoard = new int[HouseSize, HouseSize];
                        tempOptions = new int[HouseSize, HouseSize, HouseSize + 1];
                        continue;
                    }
                    if (tempBoard != null)
                        return tempBoard;
                }
            }
            if (SolveState(optionsMat) == 1)
                return board;
            return null;
        } 
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
            while (input.ToLower() != "exit")
            {
                try
                {
                    IsInputValid(input);
                    int[,] board = ToMat(input);
                    PrintBoard(board);

                    StartTime = DateTime.Now;

                    int[,,]optionsMat = Scan(board);
                    PrintBoard(Solve(board, optionsMat));

                    EndTime = DateTime.Now.Subtract(StartTime);
                    Console.WriteLine($"\ntime to solve:{EndTime.TotalSeconds}s");
                }
                catch (Exception e) 
                {
                    Console.WriteLine(e.Message); 
                }
                Console.WriteLine("\nEnter a new board:");
                Console.WriteLine("Type exit to quit the program");
                input = Console.ReadLine();
            }
        }
    }
}