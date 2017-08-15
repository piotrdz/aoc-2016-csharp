using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class AOC01_1
{
    struct Position
    {
        public int PosX;
        public int PosY;

        public Position(int PosX, int PosY)
        {
            this.PosX = PosX;
            this.PosY = PosY;
        }
    }

    enum Heading
    {
        North,
        South,
        East,
        West
    }

    struct State
    {
        public Position Position;
        public Heading Heading;

        public State(Position position, Heading heading)
        {
            this.Position = position;
            this.Heading = heading;
        }
    }

    enum Direction
    {
        Left,
        Right
    }

    struct Instruction
    {
        public Direction TurnDirection;
        public int TravelDistance;
        
        public Instruction(Direction TurnDirection, int TravelDistance)
        {
            this.TurnDirection = TurnDirection;
            this.TravelDistance = TravelDistance;
        }
    }

    static List<Instruction> ParseInstructions()
    {
        List<Instruction> instructions = new List<Instruction>();

        string line = Console.ReadLine();
        if (String.IsNullOrEmpty(line)) {
            throw new Exception("i/o error");
        }

        string[] instructionStrs = line.Split(new string[] { ", " }, StringSplitOptions.None);

        foreach (string instructionStr in instructionStrs)
        {
            instructions.Add(ParseInstruction(instructionStr));
        }

        return instructions;
    }

    static Instruction ParseInstruction(string line)
    {
        Regex lineRegex = new Regex(@"^([RL])(\d+)$");

        Match match = lineRegex.Match(line);
        if (!match.Success)
        {
            throw new Exception("no parse");
        }

        Capture direction = match.Groups[1];
        Capture distance = match.Groups[2];

        return new Instruction(direction.Value == "L" ? Direction.Left : Direction.Right,
                               Int32.Parse(distance.Value));
    }

    static State FollowInstructions(State state, List<Instruction> instructions)
    {
        foreach (Instruction instruction in instructions)
        {
            state = FollowInstruction(state, instruction);
        }

        return state;
    }

    static State FollowInstruction(State state, Instruction instruction)
    {
        Heading newHeading = FollowTurn(state.Heading, instruction.TurnDirection);
        Position newPosition = new Position(state.Position.PosX, state.Position.PosY);
        switch (newHeading)
        {
            case Heading.North:
            {
                newPosition.PosY += instruction.TravelDistance;
                break;
            }
            case Heading.South:
            {
                newPosition.PosY -= instruction.TravelDistance;
                break;
            }
            case Heading.East:
            {
                newPosition.PosX += instruction.TravelDistance;
                break;
            }
            case Heading.West:
            {
                newPosition.PosX -= instruction.TravelDistance;
                break;
            }
        }
        return new State(position: newPosition, heading: newHeading);
    }

    static Heading FollowTurn(Heading heading, Direction turnDirection)
    {
        if (turnDirection == Direction.Left)
        {
            if (heading == Heading.North)
            {
                return Heading.West;
            }
            else if (heading == Heading.South)
            {
                return Heading.East;
            }
            else if (heading == Heading.East)
            {
                return Heading.North;
            }
            else // if (heading == Heading.West)
            {
                return Heading.South;
            }
        }
        else
        {
            if (heading == Heading.North)
            {
                return Heading.East;
            }
            else if (heading == Heading.South)
            {
                return Heading.West;
            }
            else if (heading == Heading.East)
            {
                return Heading.South;
            }
            else // if (heading == Heading.West)
            {
                return Heading.North;
            }
        }
    }

    static int ManhattanDistance(Position position)
    {
        return Math.Abs(position.PosX) + Math.Abs(position.PosY);
    }

    public static void Main()
    {
        List<Instruction> instructions = ParseInstructions();

        State initialState = new State(position: new Position(PosX: 0, PosY: 0), heading: Heading.North);

        State finalState = FollowInstructions(initialState, instructions);

        int DistanceFromOrigin = ManhattanDistance(finalState.Position);
        Console.WriteLine(DistanceFromOrigin.ToString());
    }
}
