using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC04_1
{
    struct Room
    {
        public string Name;
        public int SectorId;
        public string Checksum;
    
        public Room(string name, int sectorId, string checksum)
        {
            Name = name;
            SectorId = sectorId;
            Checksum = checksum;
        }

        public bool IsReal()
        {
            return Checksum == FiveMostCommonCharactersInName();
        }

        public string FiveMostCommonCharactersInName()
        {
            var frequencies = new Dictionary<char, int>();
            foreach (char ch in Name)
            {
                if (ch != '-')
                {
                    int freq = 0;
                    frequencies.TryGetValue(ch, out freq);
                    freq++;
                    frequencies[ch] = freq;
                }
            }

            List<char> sortedChars = frequencies.Keys.ToList();
            sortedChars.Sort(delegate(char x, char y)
            {
                int frequencyCompare = frequencies[y].CompareTo(frequencies[x]);
                return (frequencyCompare != 0) ? frequencyCompare : x.CompareTo(y);
            });

            return String.Concat(sortedChars.Take(5));
        }
    }

    static List<Room> ParseRooms()
    {
        var rooms = new List<Room>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine())) {
            var room = ParseRoom(line);
            rooms.Add(room);
        }

        return rooms;
    }

    static Room ParseRoom(string line)
    {
        Regex roomRegex = new Regex(@"^(\w+(?:-\w+)*)-(\d+)\[(\w+)\]$");

        Match match = roomRegex.Match(line);
        if (!match.Success)
        {
            throw new Exception("no parse");
        }

        Capture name = match.Groups[1];
        Capture sectorId = match.Groups[2];
        Capture checksum = match.Groups[3];

        return new Room(name: name.Value, sectorId: Int32.Parse(sectorId.Value), checksum: checksum.Value);
    }

    public static void Main()
    {
        List<Room> rooms = ParseRooms();

        int realRoomsSectorIdSum = rooms.Where(room => room.IsReal()).Sum(room => room.SectorId);
        Console.WriteLine(realRoomsSectorIdSum.ToString());
    }
}
