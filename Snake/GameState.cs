using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snake
{
    internal unsafe struct BodyPart
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Direction direction;

        public BodyPart* Next;

        public BodyPart(int X, int Y, Direction direction, BodyPart* Next)
        {
            this.X = X;
            this.Y = Y;
            this.Next = Next;
            this.direction = direction;
        }
    }

    public static class GameState
    {
        private static Random rand = new();

        private static ConsoleKeyInfo currentKey;

        private static int nRefreshes = 0;

        private const int RefreshInterval = 500;

        private const int Width = 30;
        private const int Height = 10;

        private const char Food = 'X';
        private const char Player = 'O';
        private const char Wall = '█';

        private const char Empty = ' ';

        private static char[,] Map = new char[Width, Height];

        private static List<BodyPart> Body = new();

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

        public unsafe static void StartGame()
        {
            int startX = rand.Next(2, Width - 4);
            int startY = rand.Next(1, Height - 2);

            // Only the head can point Next to null
            BodyPart head = new(startX, startY, Direction.Left, null);
            Body.Add(head);

            Map[startX, startY] = Player;

            FillAndShowMap();

            new Thread(() =>
            {
                while (true)
                {
                    if (Console.KeyAvailable) currentKey = Console.ReadKey(false);
                    Thread.Sleep(20);
                }
            }).Start();

            while (true)
            {
                if (nRefreshes % 6 == 0) SpawnRandomFood();

                Direction currentDirection = ParseDirection(currentKey);

                SetDirection(ref head, currentDirection);

                MovePart(ref head);
                Thread.Sleep(RefreshInterval);
                Refresh();
            }
        }

        private static void MovePart(ref BodyPart part)
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

            Map[part.X, part.Y] = Player;
        }

        private static void SpawnRandomFood()
        {
            int x = rand.Next(2, Width - 1);
            int y = rand.Next(2, Height - 1);

            if (Map[x, y] != Empty) SpawnRandomFood();

            else { Map[x, y] = Food; }
        }

        private static void SetDirection(ref BodyPart part, Direction direction)
        {
            part.direction = direction;
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
