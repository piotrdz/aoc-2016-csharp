using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System;

using FrequencyMap = System.Collections.Generic.Dictionary<char, int>;

public class AOC06_1
{
    static List<string> ReadInputLines()
    {
        var lines = new List<string>();

        string line;
        while (!String.IsNullOrEmpty(line = Console.ReadLine()))
        {
            lines.Add(line);            
        }

        return lines;
    }

    static string DecodeMessage(List<string> lines)
    {
        string decodedMessage = "";

        var frequencyMapForPosition = new Dictionary<int, FrequencyMap>();
        foreach (string line in lines)
        {
            foreach (var it in line.Select((x,i) => new { Value = x, Index = i }))
            {
                if (!frequencyMapForPosition.ContainsKey(it.Index))
                {
                    frequencyMapForPosition[it.Index] = new FrequencyMap();
                }

                var positionFreqMap = frequencyMapForPosition[it.Index];
                if (!positionFreqMap.ContainsKey(it.Value))
                {
                    positionFreqMap[it.Value] = 1;
                }
                else
                {
                    ++positionFreqMap[it.Value];
                }
            }
        }

        int maxPosition = frequencyMapForPosition.Keys.Max();
        for (int position = 0; position <= maxPosition; ++position)
        {
            var positionFreqMap = frequencyMapForPosition[position];
            int maxFreqCount = positionFreqMap.Values.Max();
            char mostCommonCh = positionFreqMap.FirstOrDefault(x => x.Value == maxFreqCount).Key;
            decodedMessage += mostCommonCh;
        }

        return decodedMessage;
    }

    public static void Main()
    {
        List<string> lines = ReadInputLines();

        string decodedMessage = DecodeMessage(lines);
        Console.WriteLine(decodedMessage);
    }
}
