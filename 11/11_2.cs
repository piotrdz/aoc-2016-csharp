using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC11_2
{
    enum ObjectType
    {
        Generator,
        Chip
    }

    class ObjectDescription
    {
        public ObjectType Type { get; set; }
        public string Affinity { get; set; }
    }

    class FloorDescription
    {
        public int Number { get; set; }
        public List<ObjectDescription> Objects { get; set; }
    }

    class SimulationObject
    {
        public ObjectType Type { get; set; }
        public string Affinity { get; set; }
        public int FloorNumber { get; set; }

        public SimulationObject Clone()
        {
            return new SimulationObject { Type = Type,
                                          Affinity = Affinity,
                                          FloorNumber = FloorNumber };
        }
    }

    class SimulationState
    {
        public List<SimulationObject> Objects { get; set; }
        public int ElevatorFloorNumber { get; set; }

        public static SimulationState FromFloorDescriptions(List<FloorDescription> floorDescriptions, int elevatorFloorNumber)
        {
            var objects = new List<SimulationObject>();
            foreach (FloorDescription floor in floorDescriptions)
            {
                foreach (ObjectDescription obj in floor.Objects)
                {
                    objects.Add(new SimulationObject { Type = obj.Type, Affinity = obj.Affinity, FloorNumber = floor.Number });
                }
            }

            return new SimulationState { Objects = objects, ElevatorFloorNumber = elevatorFloorNumber };
        }

        public SimulationState Clone()
        {
            return new SimulationState { Objects = new List<SimulationObject>(Objects.Select(obj => obj.Clone())),
                                         ElevatorFloorNumber = ElevatorFloorNumber };
        }

        public override string ToString()
        {
            var chars = new char[Objects.Count + 1];
            chars[0] = (char)((int)'0' + ElevatorFloorNumber);
            for (int i = 0; i < Objects.Count; i++)
            {
                chars[i+1] = (char)((int)'0' + Objects[i].FloorNumber);
            }
            return new String(chars);
        }
    }

    class Move
    {
        public int FirstObjectIndex { get; set; }
        public int SecondObjectIndex { get; set; }
        public int ElevatorFloorOffset { get; set; }
    }

    class SavedSimulationState
    {
        public SimulationState State { get; set; }
        public List<Move> Moves { get; set; }
    }

    class FloorDescriptionParser
    {
        private Regex lineRegex;
        private Regex elementSeparatorRegex;
        private Regex generatorRegex;
        private Regex chipRegex;
        private Regex nothingRegex;

        public FloorDescriptionParser()
        {
            lineRegex = new Regex(@"^The (\w+) floor contains (.*)\.$");
            elementSeparatorRegex = new Regex(@",? and |, ");
            generatorRegex = new Regex(@"^a (\w+) generator$");
            chipRegex = new Regex(@"^a (\w+)-compatible microchip$");
            nothingRegex = new Regex(@"^nothing relevant$");
        }

        public FloorDescription Parse(string line)
        {
            Match match = lineRegex.Match(line);
            if (!match.Success)
            {
                throw new Exception("no parse");
            }

            Capture numberText = match.Groups[1];
            Capture objectsText = match.Groups[2];

            return new FloorDescription { Number = ParseNumber(numberText.Value),
                                          Objects = ParseObjects(objectsText.Value) };
        }

        private int ParseNumber(string numberText)
        {
            if (numberText == "first")
            {
                return 1;
            }
            else if (numberText == "second")
            {
                return 2;
            }
            else if (numberText == "third")
            {
                return 3;
            }
            else if (numberText == "fourth")
            {
                return 4;
            }
            else
            {
                throw new Exception("no parse");
            }
        }

        private List<ObjectDescription> ParseObjects(string objectsText)
        {
            var objects = new List<ObjectDescription>();

            foreach (string objectText in elementSeparatorRegex.Split(objectsText))
            {
                if (IsNothing(objectText))
                {
                    continue;
                }

                ObjectDescription obj = ParseGenerator(objectText) ?? ParseChip(objectText);
                if (obj == null)
                {
                    throw new Exception("no parse");
                }
                objects.Add(obj);
            }

            return objects;
        }

        private bool IsNothing(string objectText)
        {
            return nothingRegex.Match(objectText).Success;
        }

        private ObjectDescription ParseGenerator(string objectText)
        {
            Match match = generatorRegex.Match(objectText);
            if (!match.Success)
            {
                return null;
            }

            Capture affinity = match.Groups[1];

            return new ObjectDescription { Type = ObjectType.Generator,
                                           Affinity = affinity.Value };
        }

        private ObjectDescription ParseChip(string objectText)
        {
            Match match = chipRegex.Match(objectText);
            if (!match.Success)
            {
                return null;
            }

            Capture affinity = match.Groups[1];

            return new ObjectDescription { Type = ObjectType.Chip,
                                           Affinity = affinity.Value };
        }
    }

    static List<FloorDescription> ReadFloorDescriptions(FloorDescriptionParser floorDescriptionParser)
    {
        var floorDescriptions = new List<FloorDescription>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            FloorDescription floorDescription = floorDescriptionParser.Parse(line);
            floorDescriptions.Add(floorDescription);
        }

        return floorDescriptions;
    }

    static bool IsValid(SimulationState state)
    {
        var chips = state.Objects.Where(obj => obj.Type == ObjectType.Chip);
        foreach (SimulationObject chip in chips)
        {
            var shieldingGenerator = state.Objects.Find(obj => obj.Type == ObjectType.Generator &&
                                                               obj.FloorNumber == chip.FloorNumber &&
                                                               obj.Affinity == chip.Affinity);
            if (shieldingGenerator == null)
            {
                var areThereGeneratorsOnTheSameFloor = state.Objects.Any(obj => obj.Type == ObjectType.Generator &&
                                                                                obj.FloorNumber == chip.FloorNumber);
                if (areThereGeneratorsOnTheSameFloor)
                {
                    return false;
                } 
            }
        }

        return true;
    }

    static bool IsSolved(SimulationState simulation)
    {
        return simulation.Objects.All(obj => obj.FloorNumber == 4);
    }

    static List<Move> PossibleMoves(SimulationState state)
    {
        var moves = new List<Move>();

        var offsets = new int[] { 1, -1 };

        foreach (int offset in offsets)
        {
            if (state.ElevatorFloorNumber + offset < 1 || state.ElevatorFloorNumber + offset > 4)
            {
                continue;
            }

            var elevatorFloorObjects = state.Objects.Where(obj => obj.FloorNumber == state.ElevatorFloorNumber);
            foreach (var obj in state.Objects.Select((x,i) => new { Value = x, Index = i }))
            {
                if (obj.Value.FloorNumber != state.ElevatorFloorNumber)
                {
                    continue;
                }

                Move move = new Move { FirstObjectIndex = obj.Index,
                                       SecondObjectIndex = -1,
                                       ElevatorFloorOffset = offset };
                moves.Add(move);

                foreach (var secondObj in state.Objects.Select((x,i) => new { Value = x, Index = i }))
                {
                    if (secondObj.Index == obj.Index || secondObj.Value.FloorNumber != state.ElevatorFloorNumber)
                    {
                        continue;
                    }

                    Move secondMove = new Move { FirstObjectIndex = obj.Index,
                                                 SecondObjectIndex = secondObj.Index,
                                                 ElevatorFloorOffset = offset };
                    moves.Add(secondMove); 
                }
            }
        }

        return moves;
    }

    static void PerformMove(SimulationState state, Move move)
    {
        state.ElevatorFloorNumber += move.ElevatorFloorOffset;

        if (move.FirstObjectIndex != -1)
        {
            state.Objects[move.FirstObjectIndex].FloorNumber += move.ElevatorFloorOffset;
        }

        if (move.SecondObjectIndex != -1)
        {
            state.Objects[move.SecondObjectIndex].FloorNumber += move.ElevatorFloorOffset;
        }
    }

    static void UndoMove(SimulationState state, Move move)
    {
        state.ElevatorFloorNumber -= move.ElevatorFloorOffset;

        if (move.FirstObjectIndex != -1)
        {
            state.Objects[move.FirstObjectIndex].FloorNumber -= move.ElevatorFloorOffset;
        }

        if (move.SecondObjectIndex != -1)
        {
            state.Objects[move.SecondObjectIndex].FloorNumber -= move.ElevatorFloorOffset;
        }
    }

    static List<Move> Solve(SimulationState initialState)
    {
        List<Move> shortestSolution = null;

        var visitedStates = new HashSet<string>();
        var statesToVisit = new Queue<SavedSimulationState>();
        statesToVisit.Enqueue(new SavedSimulationState { State = initialState,
                                                         Moves = new List<Move>() });

        while (statesToVisit.Count > 0)
        {
            SavedSimulationState savedState = statesToVisit.Dequeue();
            SimulationState currentState = savedState.State;
            List<Move> currentMoves = savedState.Moves;

            var currentStateString = currentState.ToString();
            if (visitedStates.Contains(currentStateString))
            {
                continue;
            }
            visitedStates.Add(currentStateString);

            int currentBestSolutionLength = shortestSolution?.Count ?? Int32.MaxValue;

            if (currentMoves.Count >= currentBestSolutionLength)
            {
                continue;
            }

            if (IsSolved(currentState))
            {
                if (currentMoves.Count < currentBestSolutionLength)
                {
                    shortestSolution = currentMoves;
                }
                continue;
            }

            foreach (Move move in PossibleMoves(currentState))
            {
                PerformMove(currentState, move);
                if (IsValid(currentState))
                {
                    var newState = currentState.Clone();
                    var newMoves = new List<Move>(currentMoves);
                    newMoves.Add(move);
        
                    statesToVisit.Enqueue(new SavedSimulationState { State = newState, Moves = newMoves });
                }
                UndoMove(currentState, move);
            }
        }

        return shortestSolution;
    }

    static void AddAdditionalObjects(List<FloorDescription> floorDescriptions)
    {
        FloorDescription firstFloor = floorDescriptions.Find(floor => floor.Number == 1);
        firstFloor.Objects.Add(new ObjectDescription { Type = ObjectType.Generator, Affinity = "elerium" });
        firstFloor.Objects.Add(new ObjectDescription { Type = ObjectType.Chip, Affinity = "elerium" });
        firstFloor.Objects.Add(new ObjectDescription { Type = ObjectType.Generator, Affinity = "dilithium" });
        firstFloor.Objects.Add(new ObjectDescription { Type = ObjectType.Chip, Affinity = "dilithium" });
    }

    public static void Main()
    {
        var floorDescriptionParser = new FloorDescriptionParser();
        List<FloorDescription> floorDescriptions = ReadFloorDescriptions(floorDescriptionParser);
        AddAdditionalObjects(floorDescriptions);
        
        var initialState = SimulationState.FromFloorDescriptions(floorDescriptions: floorDescriptions, elevatorFloorNumber: 1);
        List<Move> shortestSolution = Solve(initialState);
        if (shortestSolution == null)
        {
            throw new Exception("no solution?");
        }

        Console.WriteLine(shortestSolution.Count.ToString());
    }
}
