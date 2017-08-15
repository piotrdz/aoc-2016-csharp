using System;

public class AOC09_1
{
    static string ReadCompressedString()
    {
        string line = Console.ReadLine();
        if (line == null)
        {
            throw new Exception("i/o error");
        }
        return line;
    }

    static int CountDecompressedStringLength(string compressedString, int startIndex = 0)
    {
        if (startIndex >= compressedString.Length)
        {
            return 0;
        }

        int nextStartMarkerIndex = compressedString.IndexOf('(', startIndex);
        if (nextStartMarkerIndex == -1)
        {
            return compressedString.Length - startIndex;
        }

        int stringBeforeStartMarkerDecompressedLength = nextStartMarkerIndex - startIndex;

        int nextDelimiterIndex = compressedString.IndexOf('x', nextStartMarkerIndex);
        int nextStopMarkerIndex = compressedString.IndexOf(')', nextStartMarkerIndex);

        int repeatLength = Int32.Parse(compressedString.Substring(nextStartMarkerIndex + 1, nextDelimiterIndex - nextStartMarkerIndex - 1));
        int repeatCount = Int32.Parse(compressedString.Substring(nextDelimiterIndex + 1, nextStopMarkerIndex - nextDelimiterIndex - 1));
        int markerSequenceStringDecompressedLength = repeatLength * repeatCount;

        int remainingStringStartIndex = nextStopMarkerIndex + repeatLength + 1;
        int remainingStringDecompressedLength = CountDecompressedStringLength(compressedString, remainingStringStartIndex);

        return stringBeforeStartMarkerDecompressedLength + markerSequenceStringDecompressedLength + remainingStringDecompressedLength;
    }

    public static void Main()
    {
        string compressedString = ReadCompressedString();

        int decompressedStringLength = CountDecompressedStringLength(compressedString);

        Console.WriteLine(decompressedStringLength.ToString());
    }
}
