using Sudoku.Exceptions;
using System;
using static Sudoku.Utilities;

namespace Sudoku
{
    public static class Validations
    {
        /* If the inputted string length is not 81, throw exception for not fitting board size.
         * If the inputted string contains non digits
         */
        public static void IsInputValid(string input)
        {
            if (input == null)
                throw new InvalidInputException("No input was given (empty board)");
            for (int i = 0; i < input.Length; i++)
                if (!Char.IsDigit(input[i]))
                    throw new InvalidInputException("Invalid character inputed");
            if (input.Length > 81)
                throw new InvalidInputException("To many characters (board too large)");
            if (input.Length < 81)
                throw new InvalidInputException("Not enough charcters (board too small)");
        }
        public static void CheckIfBoardLegal(int[,] rows, int[,] cols, int[,] squares)
        {
            for (int i = 0; i < HouseSize; i++)
                for (int k = 0; k < HouseSize; k++)
                {
                    if (rows[i, k] > 1)
                        throw new MultipleOccurrencesInHouseException("two or more of the same number in " +
                            $"1 row: {i}");
                    else if(cols[i, k] > 1)
                        throw new MultipleOccurrencesInHouseException("two or more of the same number in " +
                            $"1 column: {i}");
                    else if(squares[i, k] > 1)
                        throw new MultipleOccurrencesInHouseException("two or more of the same number in " +
                            $"1 square: {i}");

                }
        }
    }
}
