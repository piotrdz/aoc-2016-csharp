using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC10_2
{
    enum InstructionType
    {
        InitialValue,
        BotPassSequence
    }

    enum ObjectType
    {
        Bot,
        Output
    }

    class Instruction
    {
        public InstructionType Type { get; set; }

        public int BotNumber { get; set; }
        public int InitialValue { get; set; }

        public ObjectType LowPassObjectType { get; set; }
        public int LowPassObjectNumber { get; set; }

        public ObjectType HighPassObjectType { get; set; }
        public int HighPassObjectNumber { get; set; }
    }

    class Bot
    {
        public int? Number { get; set; }
        public bool Solved { get; set; }
        public HashSet<int> ChipNumbers { get; set; }

        public ObjectType? LowPassObjectType { get; set; }
        public int? LowPassObjectNumber { get; set; }

        public ObjectType? HighPassObjectType { get; set; }
        public int? HighPassObjectNumber { get; set; }

        public Bot()
        {
            ChipNumbers = new HashSet<int>();
        }
    }

    class Output
    {
        public int? Number { get; set; }
        public bool Solved { get; set; }
        public int? ChipNumber { get; set; }
    }

    class SimulationState
    {
        public List<Bot> Bots { get; set; }
        public List<Output> Outputs { get; set; }
    }
    
    class InstructionParser
    {
        private Regex initialValueRegex;
        private Regex botPassSequenceRegex;

        public InstructionParser()
        {
            initialValueRegex = new Regex(@"^value ([0-9]+) goes to bot ([0-9]+)$");
            botPassSequenceRegex = new Regex(@"^bot ([0-9]+) gives low to (bot|output) ([0-9+]+) and high to (bot|output) ([0-9]+)$");
        }

        public Instruction Parse(string line)
        {
            Instruction instruction = TryParseInitialValue(line) ?? TryParseBotPassSequence(line);
            if (instruction == null)
            {
                throw new Exception("no parse");
            }
            return instruction;
        }

        private Instruction TryParseInitialValue(string line)
        {
            Match match = initialValueRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture value = match.Groups[1];
            Capture botNumber = match.Groups[2];

            return new Instruction { Type = InstructionType.InitialValue,
                                     BotNumber = Int32.Parse(botNumber.Value),
                                     InitialValue = Int32.Parse(value.Value) };
        }

        private Instruction TryParseBotPassSequence(string line)
        {
            Match match = botPassSequenceRegex.Match(line);
            if (!match.Success)
            {
                return null;
            }

            Capture botNumber = match.Groups[1];

            Capture lowPassObjectType = match.Groups[2];
            Capture lowPassObjectNumber = match.Groups[3];

            Capture highPassObjectType = match.Groups[4];
            Capture highPassObjectNumber = match.Groups[5];

            return new Instruction { Type = InstructionType.BotPassSequence,
                                     BotNumber = Int32.Parse(botNumber.Value),
                                     LowPassObjectType = lowPassObjectType.Value == "bot" ? ObjectType.Bot : ObjectType.Output,
                                     LowPassObjectNumber = Int32.Parse(lowPassObjectNumber.Value),
                                     HighPassObjectType = highPassObjectType.Value == "bot" ? ObjectType.Bot : ObjectType.Output,
                                     HighPassObjectNumber = Int32.Parse(highPassObjectNumber.Value) };
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

    static SimulationState CreateInitialSimulationState(List<Instruction> instructions)
    {
        var simulationState = new SimulationState { Bots = new List<Bot>(), Outputs = new List<Output>() };

        foreach (var instruction in instructions) {
            if (instruction.Type == InstructionType.InitialValue)
            {
                Bot bot = FindOrCreateBot(simulationState.Bots, instruction.BotNumber);
                bot.ChipNumbers.Add(instruction.InitialValue);
            }
            else if (instruction.Type == InstructionType.BotPassSequence)
            {
                Bot bot = FindOrCreateBot(simulationState.Bots, instruction.BotNumber);

                bot.LowPassObjectType = instruction.LowPassObjectType;
                bot.LowPassObjectNumber = instruction.LowPassObjectNumber;

                bot.HighPassObjectType = instruction.HighPassObjectType;
                bot.HighPassObjectNumber = instruction.HighPassObjectNumber;

                if (bot.LowPassObjectType == ObjectType.Bot)
                {
                    FindOrCreateBot(simulationState.Bots, bot.LowPassObjectNumber.Value);
                }
                else if (bot.LowPassObjectType == ObjectType.Output)
                {
                    FindOrCreateOutput(simulationState.Outputs, bot.LowPassObjectNumber.Value);
                }

                if (bot.HighPassObjectType == ObjectType.Bot)
                {
                    FindOrCreateBot(simulationState.Bots, bot.HighPassObjectNumber.Value);
                }
                else if (bot.HighPassObjectType == ObjectType.Output)
                {
                    FindOrCreateOutput(simulationState.Outputs, bot.HighPassObjectNumber.Value);
                }
            }
        }

        return simulationState;
    }

    static void SolveSimulation(SimulationState simulationState)
    {
        Bot bot;
        while ((bot = simulationState.Bots.Find(b => !b.Solved && b.ChipNumbers.Count == 2)) != null)
        {
            int minChipNumber = bot.ChipNumbers.Min();
            if (bot.LowPassObjectType == ObjectType.Bot)
            {
                Bot passBot = FindBot(simulationState, bot.LowPassObjectNumber.Value);
                passBot.ChipNumbers.Add(minChipNumber);
            }
            else if (bot.LowPassObjectType == ObjectType.Output)
            {
                Output passOutput = FindOutput(simulationState, bot.LowPassObjectNumber.Value);
                passOutput.ChipNumber = minChipNumber;
            }

            int maxChipNumber = bot.ChipNumbers.Max();
            if (bot.HighPassObjectType == ObjectType.Bot)
            {
                Bot passBot = FindBot(simulationState, bot.HighPassObjectNumber.Value);
                passBot.ChipNumbers.Add(maxChipNumber);
            }
            else if (bot.HighPassObjectType == ObjectType.Output)
            {
                Output passOutput = FindOutput(simulationState, bot.HighPassObjectNumber.Value);
                passOutput.ChipNumber = maxChipNumber;
            }

            bot.Solved = true;
        }
    }

    static Bot FindOrCreateBot(List<Bot> bots, int botNumber)
    {
        Bot bot = bots.Find(b => b.Number == botNumber);
        if (bot == null)
        {
            bot = new Bot { Number = botNumber };
            bots.Add(bot);
        }
        return bot;
    }

    static Output FindOrCreateOutput(List<Output> outputs, int outputNumber)
    {
        Output output = outputs.Find(o => o.Number == outputNumber);
        if (output == null)
        {
            output = new Output { Number = outputNumber };
            outputs.Add(output);
        }
        return output;
    }

    static Bot FindBot(SimulationState simulationState, int botNumber)
    {
        return simulationState.Bots.Find(b => b.Number == botNumber);
    }

    static Output FindOutput(SimulationState simulationState, int outputNumber)
    {
        return simulationState.Outputs.Find(o => o.Number == outputNumber);
    }

    public static void Main()
    {
        var instructionParser = new InstructionParser();
        var instructions = ReadInstructions(instructionParser);

        var simulationState = CreateInitialSimulationState(instructions);
        SolveSimulation(simulationState);

        int[] outputNumbers = new int[] { 0, 1, 2 };
        int multipliedOutputChipNumbers = outputNumbers
            .Select(number => FindOutput(simulationState, number))
            .Aggregate(1, (multipliedNumbers, output) => multipliedNumbers * output.ChipNumber.Value);

        Console.WriteLine(multipliedOutputChipNumbers.ToString());
    }
}
