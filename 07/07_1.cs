using System.Collections.Generic;
using System.Linq;
using System;

public class AOC07_1
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

    static bool AddressSupportsTls(string address)
    {
        var recentChars = new Queue<char>();
        bool insideHypernetSequence = false;
        bool abbaOutsideHypernet = false;
        bool abbaInsideHypernet = false;
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
                while (recentChars.Count > 4)
                {
                    recentChars.Dequeue();
                }

                if (insideHypernetSequence)
                {
                    if (IsAbba(recentChars))
                    {
                        abbaInsideHypernet = true;
                    }
                }
                else
                {
                    if (IsAbba(recentChars))
                    {
                        abbaOutsideHypernet = true;
                    }
                }
            }
        }

        return abbaOutsideHypernet && !abbaInsideHypernet;
    }

    static bool IsAbba(Queue<char> recentChars)
    {
        if (recentChars.Count != 4)
        {
            return false;
        }

        var array = recentChars.ToArray();
        return array[0] == array[3] && array[1] == array[2] && array[0] != array[1];
    }

    public static void Main()
    {
        List<string> addresses = ReadIpAddresses();

        int addressesSupportingTlsCount = addresses.Count(address => AddressSupportsTls(address));
        Console.WriteLine(addressesSupportingTlsCount.ToString());
    }
}
