using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class AOC02_1
{
    struct Position
    {
        public int Row;
        public int Col;
    
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    class Keypad
    {
        private char[,] keys;

        public Keypad()
        {
            keys = new char[3,3]
            {
                { '1', '2', '3' },
                { '4', '5', '6' },
                { '7', '8', '9' }
            };
        }

        public char? KeyForPosition(Position pos)
        {
            if (pos.Row < 1 || pos.Row > keys.GetLength(0))
                return null;
            if (pos.Col < 1 || pos.Col > keys.GetLength(1))
                return null;

            return keys[pos.Row-1, pos.Col-1];
        }

        public Position? PositionForKey(char key)
        {
            for (int row = 1; row <= keys.GetLength(0); row++)
            {
                for (int col = 1; col <= keys.GetLength(1); col++)
                {
                    if (keys[row-1, col-1] == key)
                    {
                        return new Position(row: row, col: col);
                    }
                }
            }
            return null;
        }
    }

    static Keypad KEYPAD = new Keypad();

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    struct DigitInstructions
    {
        public List<Direction> Directions;

        public DigitInstructions(List<Direction> directions)
        {
            Directions = directions;
        }
    }

    static List<DigitInstructions> ParseInstructions()
    {
        var instructions = new List<DigitInstructions>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine())) {
            var digitInstructions = ParseDigitInstructions(line);
            instructions.Add(digitInstructions);
        }

        return instructions;
    }

    static DigitInstructions ParseDigitInstructions(string line)
    {
        var digitInstructions = new DigitInstructions(directions: new List<Direction>());

        foreach (char ch in line)
        {
            if (ch == 'L')
            {
                digitInstructions.Directions.Add(Direction.Left);
            }
            else if (ch == 'R')
            {
                digitInstructions.Directions.Add(Direction.Right);
            }
            else if (ch == 'U')
            {
                digitInstructions.Directions.Add(Direction.Up);
            }
            else if (ch == 'D')
            {
                digitInstructions.Directions.Add(Direction.Down);
            }
            else
            {
                throw new Exception("no parse");
            }
        }

        return digitInstructions;
    }

    static Position DecodeDigit(Position startPosition, DigitInstructions digitInstructions)
    {
        Position currentPosition = startPosition;
        foreach (var direction in digitInstructions.Directions)
        {
            Position nextPosition = currentPosition;

            if (direction == Direction.Up)
            {
                nextPosition.Row--;  
            }
            else if (direction == Direction.Down)
            {
                nextPosition.Row++;
            }
            else if (direction == Direction.Left)
            {
                nextPosition.Col--;
            }
            else // if (direction == Direction.Right)
            {
                nextPosition.Col++;
            }

            if (KEYPAD.KeyForPosition(nextPosition) != null)
            {
                currentPosition = nextPosition;
            }
        }

        return currentPosition;
    }

    static string DecodeDigits(List<DigitInstructions> instructions)
    {
        string digits = "";

        var position = KEYPAD.PositionForKey('5').Value;

        foreach (var digitInstructions in instructions)
        {
            var digitPosition = DecodeDigit(startPosition: position, digitInstructions: digitInstructions);
            digits += KEYPAD.KeyForPosition(digitPosition).Value;
            position = digitPosition;
        }

        return digits;
    }

    public static void Main()
    {
        List<DigitInstructions> instructions = ParseInstructions();

        string digits = DecodeDigits(instructions);
        Console.WriteLine(digits);
    }
}
