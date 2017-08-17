using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC12_2
{
    enum InstructionType
    {
        CopyValue,
        CopyRegister,
        IncrementRegister,
        DecrementRegister,
        JumpValue,
        JumpRegister
    }

    class Instruction
    {
        public InstructionType Type { get; set; }
        public int? Value { get; set; }
        public char? SourceRegister { get; set; }
        public char? TargetRegister { get; set; }
        public int? JumpOffset { get; set; }
    }

    class InstructionParser
    {
        private Regex copyValueRegex;
        private Regex copyRegisterRegex;
        private Regex incrementRegisterRegex;
        private Regex decrementRegisterRegex;
        private Regex jumpValueRegex;
        private Regex jumpRegisterRegex;

        public InstructionParser()
        {
            copyValueRegex = new Regex(@"^cpy (-?\d+) ([abcd])$");
            copyRegisterRegex = new Regex(@"^cpy ([abcd]) ([abcd])$");
            incrementRegisterRegex = new Regex(@"^inc ([abcd])$");
            decrementRegisterRegex = new Regex(@"^dec ([abcd])$");
            jumpValueRegex = new Regex(@"^jnz (-?\d+) (-?\d+)$");
            jumpRegisterRegex = new Regex(@"^jnz ([abcd]) (-?\d+)$");
        }

        public Instruction Parse(string line)
        {
            Instruction instruction = ParseCopyValue(line) ??
                                      ParseCopyRegister(line) ??
                                      ParseIncrementRegister(line) ??
                                      ParseDecrementRegister(line) ??
                                      ParseJumpValue(line) ??
                                      ParseJumpRegister(line);
            if (instruction == null)
            {
                throw new Exception("no parse");
            }
            return instruction;
        }

        private Instruction ParseCopyValue(string line)
        {
            Match match = copyValueRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture value = match.Groups[1];
            Capture targetRegister = match.Groups[2];

            return new Instruction { Type = InstructionType.CopyValue,
                                     Value = Int32.Parse(value.Value),
                                     TargetRegister = targetRegister.Value[0] };
        }

        private Instruction ParseCopyRegister(string line)
        {
            Match match = copyRegisterRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture sourceRegister = match.Groups[1];
            Capture targetRegister = match.Groups[2];

            return new Instruction { Type = InstructionType.CopyRegister,
                                     SourceRegister = sourceRegister.Value[0],
                                     TargetRegister = targetRegister.Value[0] };
        }

        private Instruction ParseIncrementRegister(string line)
        {
            Match match = incrementRegisterRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture targetRegister = match.Groups[1];

            return new Instruction { Type = InstructionType.IncrementRegister,
                                     TargetRegister = targetRegister.Value[0] };
        }

        private Instruction ParseDecrementRegister(string line)
        {
            Match match = decrementRegisterRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture targetRegister = match.Groups[1];

            return new Instruction { Type = InstructionType.DecrementRegister,
                                     TargetRegister = targetRegister.Value[0] };
        }

        private Instruction ParseJumpValue(string line)
        {
            Match match = jumpValueRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture value = match.Groups[1];
            Capture jumpOffset = match.Groups[2];

            return new Instruction { Type = InstructionType.JumpValue,
                                     Value = Int32.Parse(value.Value),
                                     JumpOffset = Int32.Parse(jumpOffset.Value) };
        }

        private Instruction ParseJumpRegister(string line)
        {
            Match match = jumpRegisterRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture sourceRegister = match.Groups[1];
            Capture jumpOffset = match.Groups[2];

            return new Instruction { Type = InstructionType.JumpRegister,
                                     SourceRegister = sourceRegister.Value[0],
                                     JumpOffset = Int32.Parse(jumpOffset.Value) };
        }
    }

    class Computer
    {
        private Dictionary<char, int> registers;
        private List<Instruction> instructions;
        private int currentInstructionIndex;

        public Computer(List<Instruction> instructions)
        {
            this.registers = new Dictionary<char, int>();
            this.registers.Add('a', 0);
            this.registers.Add('b', 0);
            this.registers.Add('c', 0);
            this.registers.Add('d', 0);

            this.instructions = instructions;

            this.currentInstructionIndex = 0;
        }

        public int GetRegisterValue(char register)
        {
            return registers[register];
        }

        public void SetRegisterValue(char register, int value)
        {
            registers[register] = value;
        }

        public void Run()
        {
            while (currentInstructionIndex >= 0 && currentInstructionIndex < instructions.Count)
            {
                RunCurrentInstruction();
            }
        }

        private void RunCurrentInstruction()
        {
            Instruction currentInstruction = instructions[currentInstructionIndex];

            int nextInstructionIndex = currentInstructionIndex + 1;

            switch (currentInstruction.Type)
            {
                case InstructionType.CopyValue:
                {
                    registers[currentInstruction.TargetRegister.Value] = currentInstruction.Value.Value;
                    break;
                }
                case InstructionType.CopyRegister:
                {
                    registers[currentInstruction.TargetRegister.Value] = registers[currentInstruction.SourceRegister.Value];
                    break;
                }
                case InstructionType.IncrementRegister:
                {
                    registers[currentInstruction.TargetRegister.Value]++;
                    break;
                }
                case InstructionType.DecrementRegister:
                {
                    registers[currentInstruction.TargetRegister.Value]--;
                    break;
                }
                case InstructionType.JumpValue:
                {
                    if (currentInstruction.Value.Value != 0)
                    {
                        nextInstructionIndex = currentInstructionIndex + currentInstruction.JumpOffset.Value;
                    }
                    break;
                }
                case InstructionType.JumpRegister:
                {
                    if (registers[currentInstruction.SourceRegister.Value] != 0)
                    {
                        nextInstructionIndex = currentInstructionIndex + currentInstruction.JumpOffset.Value;
                    }
                    break;
                }
            }

            currentInstructionIndex = nextInstructionIndex;
        }
    }

    static List<Instruction> ReadInstructions(InstructionParser instructionParser)
    {
        var instructions = new List<Instruction>();
    
        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            Instruction instruction = instructionParser.Parse(line);
            instructions.Add(instruction);
        }

        return instructions;
    }

    public static void Main()
    {
        var instructionParser = new InstructionParser();
        List<Instruction> instructions = ReadInstructions(instructionParser);
        
        var computer = new Computer(instructions);
        computer.SetRegisterValue('c', 1);
        computer.Run();

        Console.WriteLine(computer.GetRegisterValue('a').ToString());
    }
}
