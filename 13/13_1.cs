using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC13_1
{
    struct Point 
    {
        public int X { get;set; }
        public int Y { get;set; }
    }

    static int ReadMagicNumber()
    {
        string line = Console.ReadLine();
        return Int32.Parse(line);
    }

    enum AreaType
    {
        OpenSpace,
        Wall
    }

    static AreaType AreaTypeAtPoint(Point point, int magicNumber)
    {
        int number = point.X*point.X + 3*point.X + 2*point.X*point.Y + point.Y + point.Y*point.Y + magicNumber;
        string binaryNumber = Convert.ToString(number, 2);
        bool binaryNumberHasEvenNumberOfOnes = binaryNumber.Count(ch => ch == '1') % 2 == 0;
        return binaryNumberHasEvenNumberOfOnes ? AreaType.OpenSpace : AreaType.Wall;
    }

    static List<Point> NeighbouringPoints(Point point)
    {
        var offsets = new Point[] {
            new Point { X =  1, Y =  0 },
            new Point { X =  0, Y =  1 },
            new Point { X = -1, Y =  0 },
            new Point { X =  0, Y = -1 },
        };

        var neighbouringPoints = new List<Point>();

        foreach (Point offset in offsets)
        {
            Point neighbouringPoint = new Point { X = point.X + offset.X, Y = point.Y + offset.Y };
            if (neighbouringPoint.X >= 0 && neighbouringPoint.Y >= 0)
            {
                neighbouringPoints.Add(neighbouringPoint);
            }
        }

        return neighbouringPoints;
    }

    static int SolveMaze(Point start, Point destination, int magicNumber)
    {
        var mazeSolutions = new Dictionary<Point, int>();

        mazeSolutions.Add(start, 0);

        var frontier = new Queue<Point>();
        foreach (Point p in NeighbouringPoints(start))
        {
            frontier.Enqueue(p);
        }

        while (frontier.Count > 0 && !mazeSolutions.ContainsKey(destination))
        {
            var point = frontier.Dequeue();

            if (mazeSolutions.ContainsKey(point) || AreaTypeAtPoint(point, magicNumber) == AreaType.Wall)
            {
                continue;
            }

            int minPathLengthFromNeighbours = NeighbouringPoints(point)
                .Select(p => mazeSolutions.ContainsKey(p) ? (mazeSolutions[p] + 1) : Int32.MaxValue)
                .Min();
            if (minPathLengthFromNeighbours != Int32.MaxValue)
            {
                mazeSolutions[point] = minPathLengthFromNeighbours;
                foreach (Point p in NeighbouringPoints(point))
                {
                    frontier.Enqueue(p);
                }
            }
        }

        return mazeSolutions[destination];
    }

    public static void Main()
    {
        int magicNumber = ReadMagicNumber();
        Point start = new Point { X = 1, Y = 1 };
        Point destination = new Point { X = 31, Y = 39 };
        int shortestPathLength = SolveMaze(start, destination, magicNumber);
        Console.WriteLine(shortestPathLength.ToString());
    }
}
