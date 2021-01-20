using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Snake
{
    class Engine
    {
        private class Cell
        {
            public string val { get; set; }
            public int x { get; set; }
            public int y { get; set; }
            public bool visited { get; set; }
            public int decay { get; set; }

            public void decaySnake() { decay -= 1; if (decay == 0) { visited = false; val = " "; } }
            public void Clear() { val = " "; }
            public void Set(string newVal) { val = newVal; }
        }
        private static readonly int gridW = 90;
        private static readonly int gridH = 25;
        private static Cell[,] grid = new Cell[gridH, gridW];
        private static Cell currentCell;
        private static int FoodCount;
        private static int direction; //0=Up 1=Right 2=Down 3=Left
        private static readonly int speed = 1;
        private static bool Populated = false;
        private static bool Lost = false;
        private static int snakeLength;

        private void printLogo()
        {
            string logoTxt = File.ReadAllText("logo.txt");
            Console.WriteLine(logoTxt);
            Console.WriteLine("Controls: W(Up), A(Left), S(Down), D(right)");
            Console.WriteLine("\nPress any key..");
            Console.ReadKey();
        }
        public void Run()
        {
            printLogo();
            if (!Populated)
            {
                FoodCount = 0;
                snakeLength = 5;
                populateGrid();
                currentCell = grid[(int)Math.Ceiling((double)gridH / 2), (int)Math.Ceiling((double)gridW / 2)];
                updatePos();
                addFood();
                Populated = true;
            }

            while (!Lost)
            {
                Restart();
            }
        }
        private void Restart()
        {
            Console.SetCursorPosition(0, 0);
            printGrid();
            Console.WriteLine("Length: {0} | Food: {1}", snakeLength, FoodCount);
            getInput();
        }
        private void updateScreen()
        {
            Console.SetCursorPosition(0, 0);
            printGrid();
            Console.WriteLine("Length: {0} | Food: {1}", snakeLength, FoodCount);
        }
        private void getInput()
        {

            //Console.Write("Where to move? [WASD] ");
            ConsoleKeyInfo input;
            while (!Console.KeyAvailable)
            {
                Move();
                updateScreen();
            }
            input = Console.ReadKey();
            doInput(input.KeyChar);
        }
        private void checkCell(Cell cell)
        {
            if (cell.val == "%")
            {
                eatFood();
                FoodCount++;
            }
            if (cell.visited)
            {
                Lose();
            }
        }

        private void Lose()
        {
            ScoreLog();
            Console.WriteLine("\n You lose!");
            Thread.Sleep(1000);
            Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Environment.Exit(-1);
        }

        private void doInput(char inp)
        {
            switch (inp)
            {
                case 'w':
                    goUp();
                    break;
                case 's':
                    goDown();
                    break;
                case 'a':
                    goRight();
                    break;
                case 'd':
                    goLeft();
                    break;
            }
        }

        private void addFood()
        {
            Random r = new Random();
            Cell cell;
            while (true)
            {
                cell = grid[r.Next(grid.GetLength(0)), r.Next(grid.GetLength(1))];
                if (cell.val == " ")
                    cell.val = "%";
                break;
            }
        }

        private void eatFood()
        {
            snakeLength += 1;
            addFood();
        }

        private void goUp()
        {
            if (direction == 2)
                return;
            direction = 0;
        }

        private void goRight()
        {
            if (direction == 3)
                return;
            direction = 1;
        }

        private void goDown()
        {
            if (direction == 0)
                return;
            direction = 2;
        }

        private void goLeft()
        {
            if (direction == 1)
                return;
            direction = 3;
        }

        private void Move()
        {
            if (direction == 0)
            {
                //up
                if (grid[currentCell.y - 1, currentCell.x].val == "*")
                {
                    Lose();
                    return;
                }
                visitCell(grid[currentCell.y - 1, currentCell.x]);
            }
            else if (direction == 1)
            {
                //right
                if (grid[currentCell.y, currentCell.x - 1].val == "*")
                {
                    Lose();
                    return;
                }
                visitCell(grid[currentCell.y, currentCell.x - 1]);
            }
            else if (direction == 2)
            {
                //down
                if (grid[currentCell.y + 1, currentCell.x].val == "*")
                {
                    Lose();
                    return;
                }
                visitCell(grid[currentCell.y + 1, currentCell.x]);
            }
            else if (direction == 3)
            {
                //left
                if (grid[currentCell.y, currentCell.x + 1].val == "*")
                {
                    Lose();
                    return;
                }
                visitCell(grid[currentCell.y, currentCell.x + 1]);
            }
            Thread.Sleep(speed * 100);
        }

        private void visitCell(Cell cell)
        {
            currentCell.val = "#";
            currentCell.visited = true;
            currentCell.decay = snakeLength;
            checkCell(cell);
            currentCell = cell;
            updatePos();

            //checkCell(currentCell);
        }

        private void updatePos()
        {

            currentCell.Set("@");
            if (direction == 0)
            {
                currentCell.val = "^";
            }
            else if (direction == 1)
            {
                currentCell.val = "<";
            }
            else if (direction == 2)
            {
                currentCell.val = "v";
            }
            else if (direction == 3)
            {
                currentCell.val = ">";
            }

            currentCell.visited = false;
            return;
        }

        private void populateGrid()
        {
            Random random = new Random();
            for (int col = 0; col < gridH; col++)
            {
                for (int row = 0; row < gridW; row++)
                {
                    Cell cell = new Cell();
                    cell.x = row;
                    cell.y = col;
                    cell.visited = false;
                    if (cell.x == 0 || cell.x > gridW - 2 || cell.y == 0 || cell.y > gridH - 2)
                        cell.Set("*");
                    else
                        cell.Clear();
                    grid[col, row] = cell;
                }
            }
        }

        private void printGrid()
        {
            string toPrint = "";
            for (int col = 0; col < gridH; col++)
            {
                for (int row = 0; row < gridW; row++)
                {
                    grid[col, row].decaySnake();
                    toPrint += grid[col, row].val;

                }
                toPrint += "\n";
            }
            Console.WriteLine(toPrint);
        }
        private void ScoreLog()
        {
            string fileName = "scorelog.txt";
            if (!File.Exists(fileName))
            {
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("Length: {0} | Food: {1}", snakeLength, FoodCount);
                }
            }
            else if (File.Exists(fileName))
                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine("Length: {0} | Food: {1}", snakeLength, FoodCount);
                }
        }
    }
}
