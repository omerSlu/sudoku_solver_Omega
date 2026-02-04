using System;
using static Sudoku.Utilities;

namespace Sudoku
{
    public static class OptionMat
    {
        /* This function takes previously found numbers in each: row, column or square and puts in each cell
         * of an 3 dimentional array the numbers that are possible in their fitting place.
         * The 3 dimentional array is a two dimentional array of the board where in each cell there is an
         * array of 10 ints, the first 9 represents the options of the cell. An one in the X index of the 
         * array is equivelent to saying X+1 is an option in this cell.
         * The 10th slot represents the number of options of that cell.
         */
        static void TurnIntoOptions(int[,] rows, int[,] cols, int[,] squares, int[,,] options, int[,] board)
        
        {
            int amount = 0;
            int square = 0;
            for (int i = 0; i < HouseSize; i++)
            {
                for (int j = 0; j < HouseSize; j++)
                {
                    if (board[i, j] == 0)
                    {
                        square = i / 3 * 3 + j / 3;
                        for (int k = 0; k < HouseSize; k++)
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
                            options[i, j, HouseSize] = amount;
                        else
                            throw new Exception("cell has no potential options");
                        amount = 0;
                    }
                    else
                        options[i, j, HouseSize] = -1;
                }
            }
        }
        /* This functions scans the board for the numbers in each row, column and square and then calls
         * the function that turns them into a 3 dimentional array of options like what was described before
         * returns: the optionMat
         */
        public static int[,,] Scan(int[,] board)
        {
            // first dimention is the index of the row, col or square respectively
            // the second dimention is the numbers that are in the respective house(rows, cols, squares)
            int[,] inRows = new int[HouseSize, HouseSize];
            int[,] inCols = new int[HouseSize, HouseSize];
            int[,] inSquares = new int[HouseSize, HouseSize];
            // the options mat. explained in detail in the "TurnIntoOptions" func
            int[,,] optionsMat = new int[HouseSize, HouseSize, HouseSize + 1];
            for (int i = 0; i < HouseSize; i++)
            {
                for (int j = 0; j < HouseSize; j++)// adds one to the number's index for each house
                {
                    if (board[i, j] != 0)
                    {
                        inRows[i, board[i, j] - 1]++;
                        inSquares[i / 3 * 3 + j / 3, board[i, j] - 1]++;
                    }
                    if (board[j, i] != 0)
                        inCols[i, board[j, i] - 1]++;
                }
            }
            TurnIntoOptions(inRows, inCols, inSquares, optionsMat, board);
            return optionsMat;
        }
    }
}
