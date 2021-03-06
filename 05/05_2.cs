using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System;

using Hash = System.Collections.Generic.List<byte>;

public class AOC05_2
{
    class HashFinder
    {
        private MD5 md5;
        private string doorId;
        private int counter;
    
        public HashFinder(string doorId)
        {
            this.doorId = doorId;
            this.md5 = MD5.Create();
            this.counter = -1;
        }

        public Hash NextInterestingHash()
        {
            Hash hash;
            do
            {
                ++counter;
            }
            while (!IsInteresting(hash = CurrentHash()));

            return hash;
        }

        private Hash CurrentHash()
        {
            string counterStr = counter.ToString();

            var bytes = new byte[doorId.Length + counterStr.Length];

            int index = 0;
            foreach (char ch in doorId)
            {
                bytes[index++] = (byte)ch;
            }
            foreach (char ch in counterStr)
            {
                bytes[index++] = (byte)ch;
            }

            return md5.ComputeHash(bytes).ToList();
        }

        private bool IsInteresting(Hash hash)
        {
            return hash[0] == 0 && hash[1] == 0 && hash[2] / 16 == 0;
        }
    }

    static string FindPassword(HashFinder hashFinder)
    {
        var password = new char?[8];
        for (int i = 0; i < 8; i++)
        {
            password[i] = null;
        }

        while (password.Any(c => c == null))
        {
            Hash hash = hashFinder.NextInterestingHash();
            int position = hash[2] % 16;
            if (position >= 0 && position <= 7 && password[position] == null)
            {
                password[position] = hash[3].ToString("x2")[0];
            }
        }

        return String.Concat(password);
    }

    static string ReadRoomId()
    {
        string line;
        if (String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            throw new Exception("i/o error");
        }

        return line;
    }

    public static void Main()
    {
        string roomId = ReadRoomId();
        HashFinder hashFinder = new HashFinder(roomId);
        string password = FindPassword(hashFinder);
        Console.WriteLine(password);
    }
}
