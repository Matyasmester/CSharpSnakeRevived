using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snake
{
    internal class BodyPart
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int prevX { get; set; }
        public int prevY { get; set; }

        public Direction direction;

        public BodyPart parent;

        public BodyPart(int X, int Y, Direction direction, BodyPart parent)
        {
            this.X = X;
            this.Y = Y;
            this.parent = parent;
            this.direction = direction;
        }

        public void SetDirection(Direction direction)
        {
            this.direction = direction;
        }
    }

    public static class GameState
    {
        private static Random rand = new();

        private static ConsoleKeyInfo currentKey;

        private static int nRefreshes = 0;
        private static int score = 0;

        private const int RefreshInterval = 400;

        private const int Width = 30;
        private const int Height = 15;

        private const char Food = 'X';
        private const char Player = 'O';
        private const char Wall = '█';

        private const char Empty = ' ';

        private static char[,] Map = new char[Width, Height];

        private static List<BodyPart> Body = new();
        private static BodyPart? NONE = null;

        private static bool hasPickedUpFood = false;
        private static BodyPart? currentParent = NONE;

        private static void FillAndShowMap()
        {
            string MapString = "";
            for(int y = 0; y < Height; y++)
            {
                
                for(int x = 0; x < Width; x++)
                {
                    char current = Empty;
                    if(x == 0 || y == 0 || x == Width - 1 || y == Height - 1) current = Wall;
                    
                    Map[x, y] = current;
                    MapString += current;

                    if (x == Width - 1) MapString += Environment.NewLine;
                }
            }

            Console.Write(MapString);
        }

        private static void ShowMap()
        {
            string MapString = "";
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    MapString += Map[x, y];
                    if (x == Width - 1) MapString += Environment.NewLine;
                }
            }

            Console.Write(MapString);
        }

        public static void StartGame()
        {
            int startX = rand.Next(2, Width - 4);
            int startY = rand.Next(1, Height - 2);

            // Only the head can point Next to null
            BodyPart head = new(startX, startY, Direction.Left, NONE);
            Body.Add(head);

            FillAndShowMap();

            Map[startX, startY] = Player;

            new Thread(() =>
            {
                while (true)
                {
                    currentKey = Console.ReadKey(false);
                }
            }).Start();

            while (true)
            {
                if (hasPickedUpFood)
                {
                    AddPartToBody(Body.Last());
                    hasPickedUpFood = false;
                    score++;
                }
                if (nRefreshes % 12 == 0) SpawnRandomFood();

                Direction currentDirection = ParseDirection(currentKey);

                head.SetDirection(currentDirection);

                MoveBody();
                Refresh();
            }
        }

        private static void MoveBody()
        {
            BodyPart head = Body.First();
            MovePart(head);
            
            for(int i = 1; i < Body.Count; i++)
            {
                BodyPart current = Body[i];

                int x = current.X;
                int y = current.Y;

                Map[x, y] = Empty;

                current.prevX = x;
                current.prevY = y;

                current.X = current.parent.prevX;
                current.Y = current.parent.prevY;

                Map[current.X, current.Y] = Player; 
            }
        }

        private static void MovePart(BodyPart part)
        {
            int x = part.X;
            int y = part.Y;
            Map[x, y] = Empty;

            part.prevX = x;
            part.prevY = y;

            switch (part.direction)
            {
                case Direction.Left:
                    part.X -= 1;
                    break;
                case Direction.Right:
                    part.X += 1;
                    break;
                case Direction.Up:
                    part.Y -= 1;
                    break;
                case Direction.Down:
                    part.Y += 1;
                    break;
                default:
                    break;
            }

            int newX = part.X;
            int newY = part.Y;

            char destination = Map[newX, newY];

            if (destination == Food)
            {
                hasPickedUpFood = true;
                currentParent = part;
            }
            
            if (destination == Wall || destination == Player)
            {
                EndCurrentGame("you died xd, score: " + score, score);
            }

            Map[newX, newY] = Player;
        }
        private static void EndCurrentGame(string msg, int exitCode)
        {
            Console.WriteLine(msg);
            Environment.Exit(exitCode);
        }

        private static void AddPartToBody(BodyPart parent)
        {
            Direction parentDirection = parent.direction;
            Direction opposite = GetOppositeDirection(parentDirection);

            int x = parent.X;
            int y = parent.Y;

            switch (opposite)
            {
                case Direction.Up:
                    y--;
                    break;
                case Direction.Down: 
                    y++;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Right:
                    x++;
                    break;
                default:
                    break;
            }

            BodyPart newPart = new BodyPart(x, y, parentDirection, parent);
            Body.Add(newPart);
            Map[x, y] = Player;
        }

        private static Direction GetOppositeDirection(Direction direction)
        {
            int directionID = (int)direction;
            Direction opposite = (Direction)(directionID*(-1));
            return opposite;
        }

        private static void SpawnRandomFood()
        {
            int x = rand.Next(2, Width - 1);
            int y = rand.Next(2, Height - 1);

            if (Map[x, y] != Empty) SpawnRandomFood();

            else { Map[x, y] = Food; }
        }

        private static Direction ParseDirection(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow: return Direction.Up;
                case ConsoleKey.DownArrow: return Direction.Down;
                case ConsoleKey.LeftArrow: return Direction.Left;
                case ConsoleKey.RightArrow: return Direction.Right;
                default: return Direction.Left;
            }
        }

        private static void Refresh()
        {
            Thread.Sleep(RefreshInterval);
            Console.Clear();
            ShowMap();
            nRefreshes++;
        }
    }
}
