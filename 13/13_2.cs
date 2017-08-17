using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC13_2
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

    static HashSet<Point> SolveMaze(Point start, int targetLength, int magicNumber)
    {
        var visitedPoints = new HashSet<Point>();
        var frontier = new Queue<Tuple<Point, int>>();

        frontier.Enqueue(Tuple.Create(start, 0));

        while (frontier.Count > 0)
        {
            var frontierElement = frontier.Dequeue();
            Point point = frontierElement.Item1;
            int length = frontierElement.Item2;

            if (visitedPoints.Contains(point) || AreaTypeAtPoint(point, magicNumber) == AreaType.Wall)
            {
                continue;
            }

            visitedPoints.Add(point);
            if (length < targetLength)
            {
                foreach (Point p in NeighbouringPoints(point))
                {
                    frontier.Enqueue(Tuple.Create(p, length+1));
                }
            }
        }

        return visitedPoints;
    }

    public static void Main()
    {
        int magicNumber = ReadMagicNumber();
        Point start = new Point { X = 1, Y = 1 };
        int targetLength = 50;
        var pointsAtMaxTargetLength = SolveMaze(start: start, targetLength: targetLength, magicNumber: magicNumber);
        Console.WriteLine(pointsAtMaxTargetLength.Count.ToString());
    }
}
