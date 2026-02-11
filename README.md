# Sudoku
Welcome to my sudoku solver.

_**General Limitaions:**_

Firstly, the program has benn tested for c# .net framework from version 3.0 to 4.8 and is solving every board in less then 1 second.
The program was ran on local machine and has'nt been checked in different envirments.
All of the above to say that it may take longer to run on your local machine then excpected outcome.

_**Rules And How To Use:**_

It is reccommended to use visual studio 2026 to open and run the project for best result.
The solver has no GUI and only TUI that is ran using the CLI.
A board of sudoku will be inputted like a string and will be made as follows:
the number 0 will be interpreted as an empty cell,
any number from 1-9 will be interpreted as the number that is in the cell,
the string will be 81 characters long and will **not** contain any characters that are not a number
if it will the program will print a fitting message
for any other illegal input the program will react the same.

Example of legal board:

000000012980000000000600000100700080402000000000300600070000300050040000000010000

how the board will look:

<img width="238" height="189" alt="image" src="https://github.com/user-attachments/assets/405e168a-8b53-465b-961d-7aac81bcae5b" />

Example of invalid boards and the reason:

invalid character(the a in the start):

a0000000000000000000000000000000000000000000000000000000000000000000000000000000

board size too small:

0

board size too large(90 characters):
000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000


The sudoku solver id solving normal sudokus (9x9) and can solve any solvable sudoku
also any unsolvable sudoku or illegal board will be caught and will return a fitting error message

Example of illegal boards:

in this sudoku there are two 9's in the same row:

9000900000000000000000000000000000000000000000000000000000000000000000000000000000

for the same reason the following board will be illegal:

999999999999999999999999999999999999999999999999999999999999999999999999999999999


After learning what input is valid I can also add that every input will be handled correctly and return a fitting message or a solved board!
The program is run in a loop until the user types exit (EXIT or any other case will be counted as exit)


_**CLI Looks:**_

This is how the CLI looks for a solved board:

<img width="712" height="533" alt="image" src="https://github.com/user-attachments/assets/4c036510-cc98-40fa-8a99-88ca7597493a" />

you can see the asks for the user, then the string he entered, under it the board unsolved and under that the solved board.
as you can see the solver also adds a line that states the time it took to solve in seconds.


_**Logic And Code:**_
The algorithm uses 3 solving methods:

number 1: Naked single - this are cells where there is only one option possible in them and can be filled without any further calculation

number 2: Hidden singles - this is a case where there is only one slot where a number could be in a row/column/square

number 3: Brute force - when we can not fill any numbers using the logic ways described before we can just guess a number.
We will always find the cell where there are the list options and then guess the options one by one, each time we will try to solve 
the board like we did before (including the 3rd step) until we run into an unsolvable board or a solved board, when that happens we return 
to our last guess and try another guess if the last guesse led us to an unsolvable board and if the board was solved we return until we returned 
to the first call. if we ran out of options that means that the route we are going in is impossible to solve and if all routes were found
impossible to solve we will declare the board unsolvable

Code:

The function that solves the sudoku is the Solve function and it calls all the functions needed for the operation, you can read more on the
functions, their usage and how they do what they do by the documantation that I wrote in the code files

The code is organized to static classes and the exceptions sit in an Exception folder

Tests:

The project comes with completed tests that include:
almost 50,000 solvable hard sudokus that are stored in a text file and are read and solved, the test checks if they are solved in less then 1 second

Also there are tests to check if all the unvalid boards are being handled
