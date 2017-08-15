using System.Collections.Generic;
using System.Linq;
using System;

public class AOC07_2
{
    static List<string> ReadIpAddresses()
    {
        var lines = new List<string>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            lines.Add(line);            
        }

        return lines;
    }

    static bool AddressSupportsSsl(string address)
    {
        var recentChars = new Queue<char>();
        bool insideHypernetSequence = false;
        var supernetAbas = new List<string>();
        var hypernetAbas = new List<string>();

        foreach (char ch in address)
        {
            if (ch == '[')
            {
                insideHypernetSequence = true;
                recentChars.Clear();
            }
            else if (ch == ']')
            {
                insideHypernetSequence = false;
                recentChars.Clear();
            }
            else
            {
                recentChars.Enqueue(ch);
                while (recentChars.Count > 3)
                {
                    recentChars.Dequeue();
                }

                if (insideHypernetSequence)
                {
                    if (IsAba(recentChars))
                    {
                        hypernetAbas.Add(String.Concat(recentChars.ToArray()));
                    }
                }
                else
                {
                    if (IsAba(recentChars))
                    {
                        supernetAbas.Add(String.Concat(recentChars.ToArray()));
                    }
                }
            }
        }

        return supernetAbas.Any(aba => hypernetAbas.Contains(AbaToBab(aba)));
    }

    static bool IsAba(Queue<char> recentChars)
    {
        if (recentChars.Count != 3)
        {
            return false;
        }

        var array = recentChars.ToArray();
        return array[0] == array[2] && array[0] != array[1];
    }
    
    static string AbaToBab(string aba)
    {
        return $"{aba[1]}{aba[0]}{aba[1]}";
    }

    public static void Main()
    {
        List<string> addresses = ReadIpAddresses();

        int addressesSupportingSslCount = addresses.Count(address => AddressSupportsSsl(address));
        Console.WriteLine(addressesSupportingSslCount.ToString());
    }
}
