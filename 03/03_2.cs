using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC03_2
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

        while (true) {
            string line1 = Console.ReadLine();
            string line2 = Console.ReadLine();
            string line3 = Console.ReadLine();

            if (String.IsNullOrEmpty(line1) ||
                String.IsNullOrEmpty(line2) ||
                String.IsNullOrEmpty(line3)) {
                break;
            }

            var triangle1 = ParseTriangle(line1);
            var triangle2 = ParseTriangle(line2);
            var triangle3 = ParseTriangle(line3);

            triangles.Add(new Triangle(triangle1.SideA, triangle2.SideA, triangle3.SideA));
            triangles.Add(new Triangle(triangle1.SideB, triangle2.SideB, triangle3.SideB));
            triangles.Add(new Triangle(triangle1.SideC, triangle2.SideC, triangle3.SideC));
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
