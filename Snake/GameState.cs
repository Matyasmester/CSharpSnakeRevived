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

        public Direction direction;

        public BodyPart Next;

        public BodyPart(int X, int Y, Direction direction, BodyPart Next)
        {
            this.X = X;
            this.Y = Y;
            this.Next = Next;
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

        private const int RefreshInterval = 400;

        private const int Width = 30;
        private const int Height = 10;

        private const char Food = 'X';
        private const char Player = 'O';
        private const char Wall = '█';

        private const char Empty = ' ';

        private static char[,] Map = new char[Width, Height];

        private static List<BodyPart> Body = new();
        private static BodyPart? NONE = null;

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

            Map[startX, startY] = Player;

            FillAndShowMap();

            new Thread(() =>
            {
                while (true)
                {
                    currentKey = Console.ReadKey(false);
                }
            }).Start();

            while (true)
            {
                if (nRefreshes % 12 == 0) SpawnRandomFood();

                Direction currentDirection = ParseDirection(currentKey);

                head.SetDirection(currentDirection);

                MovePart(head);
                Thread.Sleep(RefreshInterval);
                Refresh();
            }
        }

        private static void MoveBody()
        {
            //TODO
        }

        private static void MovePart(BodyPart part)
        {
            Map[part.X, part.Y] = Empty;

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
                AddPartToBody(part);
            };

            Map[newX, newY] = Player;
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
            Console.Clear();
            ShowMap();
            nRefreshes++;
        }
    }
}
