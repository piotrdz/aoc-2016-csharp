using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class AOC04_2
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

        public string DecryptedName()
        {
            string decryptedName = "";
            foreach (char ch in Name)
            {
                char decryptedCh = ch;
                if (ch != '-')
                {
                    int decryptedChInt = (int)decryptedCh;
                    decryptedChInt -= (int)'a';
                    decryptedChInt += SectorId;
                    decryptedChInt %= 1 + (int)('z') - (int)('a');
                    decryptedChInt += (int)'a';
                    decryptedCh = (char)decryptedChInt;
                }
                decryptedName += decryptedCh;
            }

            return decryptedName;
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

        Room secretRoom = rooms.Where(room =>
        {
            string decryptedName = room.DecryptedName();
            return decryptedName.Contains("north") && decryptedName.Contains("pole");
        }).First();
        Console.WriteLine(secretRoom.SectorId.ToString());
    }
}
