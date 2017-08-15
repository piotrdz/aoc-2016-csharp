using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC03_1
{
    struct Triangle
    {
        public int SideA;
        public int SideB;
        public int SideC;
    
        public Triangle(int sideA, int sideB, int sideC)
        {
            SideA = sideA;
            SideB = sideB;
            SideC = sideC;
        }

        public bool IsValid()
        {
            return (SideA < SideB + SideC) &&
                   (SideB < SideA + SideC) &&
                   (SideC < SideA + SideB);
        }
    }

    static List<Triangle> ParseTriangles()
    {
        var triangles = new List<Triangle>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine())) {
            var triangle = ParseTriangle(line);
            triangles.Add(triangle);
        }

        return triangles;
    }

    static Triangle ParseTriangle(string line)
    {
        Regex triangleRegex = new Regex(@"^\s*(\d+)\s*(\d+)\s*(\d+)\s*$");

        Match match = triangleRegex.Match(line);
        if (!match.Success)
        {
            throw new Exception("no parse");
        }

        Capture sideA = match.Groups[1];
        Capture sideB = match.Groups[2];
        Capture sideC = match.Groups[3];

        return new Triangle(Int32.Parse(sideA.Value), Int32.Parse(sideB.Value), Int32.Parse(sideC.Value));
    }

    public static void Main()
    {
        List<Triangle> triangles = ParseTriangles();

        int validTriangleCount = triangles.Where(triangle => triangle.IsValid()).Count();
        Console.WriteLine(validTriangleCount.ToString());
    }
}
