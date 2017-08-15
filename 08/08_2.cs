using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC08_2
{
    enum InstructionType
    {
        Rect,
        RotateColumn,
        RotateRow
    }

    struct Instruction
    {
        public InstructionType Type;
        public int A;
        public int B;

        public Instruction(InstructionType type, int a, int b)
        {
            Type = type;
            A = a;
            B = b;
        }
    }

    class LedDisplay
    {
        private byte[,] leds;
        private const int COLUMNS = 50;
        private const int ROWS = 6;

        public LedDisplay()
        {
            leds = new byte[COLUMNS, ROWS];
        }

        public void Perform(Instruction instruction)
        {
            switch (instruction.Type)
            {
                case InstructionType.Rect:
                {
                    Rect(instruction.A, instruction.B);
                    break;
                }
                case InstructionType.RotateColumn:
                {
                    RotateColumn(instruction.A, instruction.B);
                    break;
                }
                case InstructionType.RotateRow:
                {
                    RotateRow(instruction.A, instruction.B);
                    break;
                }
            }
        }

        public List<string> RowStrings()
        {
            var rowStrings = new List<string>();

            for (int row = 0; row < ROWS; ++row)
            {
                string rowString = "";
                for (int column = 0; column < COLUMNS; ++column)
                {
                    if (leds[column, row] == 1)
                    {
                        rowString += '#';
                    }
                    else
                    {
                        rowString += '.';
                    }
                }
                rowStrings.Add(rowString);
            }

            return rowStrings;
        }

        private void Rect(int a, int b)
        {
            for (int column = 0; column < a; ++column)
            {
                for (int row = 0; row < b; ++row)
                {
                    leds[column, row] = 1;
                }
            }
        }

        private void RotateColumn(int column, int by)
        {
            var newColumn = new byte[ROWS];
            for (int row = 0; row < ROWS; ++row)
            {
                newColumn[(row + by) % ROWS] = leds[column, row];
            }
            for (int row = 0; row < ROWS; ++row)
            {
                leds[column, row] = newColumn[row];
            }
        }

        private void RotateRow(int row, int by)
        {
            var newRow = new byte[COLUMNS];
            for (int column = 0; column < COLUMNS; ++column)
            {
                newRow[(column + by) % COLUMNS] = leds[column, row];
            }
            for (int column = 0; column < COLUMNS; ++column)
            {
                leds[column, row] = newRow[column];
            }
        }
    }

    class InstructionParser
    {
        private Regex rectRegex;
        private Regex rotateRowRegex;
        private Regex rotateColumnRegex;

        public InstructionParser()
        {
            rectRegex = new Regex(@"^rect (\d+)x(\d+)$");
            rotateRowRegex = new Regex(@"^rotate row y=(\d+) by (\d+)$");
            rotateColumnRegex = new Regex(@"^rotate column x=(\d+) by (\d+)$");
        }

        public Instruction ParseInstruction(string line)
        {
            Instruction? instruction = ParseInstructionOfType(line, InstructionType.Rect, rectRegex) ??
                                       ParseInstructionOfType(line, InstructionType.RotateRow, rotateRowRegex) ??
                                       ParseInstructionOfType(line, InstructionType.RotateColumn, rotateColumnRegex);
            if (instruction == null)
            {
                throw new Exception("no parse");
            }
            return instruction.Value;
        }

        private Instruction? ParseInstructionOfType(string line, InstructionType type, Regex regex)
        {
            Match match = regex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture a = match.Groups[1];
            Capture b = match.Groups[2];

            return new Instruction(type: type, a: Int32.Parse(a.Value), b: Int32.Parse(b.Value));
        }
    }

    static List<Instruction> ParseInstructions(InstructionParser instructionParser)
    {
        var instructions = new List<Instruction>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            Instruction instruction = instructionParser.ParseInstruction(line);
            instructions.Add(instruction);            
        }

        return instructions;
    }

    static void PerformInstructions(LedDisplay ledDisplay, List<Instruction> instructions)
    {
        foreach (Instruction instruction in instructions)
        {
            ledDisplay.Perform(instruction);
        }
    }

    static void DisplayRows(LedDisplay ledDisplay)
    {
        foreach (string rowString in ledDisplay.RowStrings())
        {
            Console.WriteLine(rowString);
        }
    }

    public static void Main()
    {
        var instructionParser = new InstructionParser();
        List<Instruction> instructions = ParseInstructions(instructionParser);

        var ledDisplay = new LedDisplay();
        PerformInstructions(ledDisplay, instructions);

        DisplayRows(ledDisplay);
    }
}
