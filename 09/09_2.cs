using System;

public class AOC09_2
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

    static long CountDecompressedStringLength(string compressedString, int startIndex = 0, int length = -1)
    {
        if (startIndex >= compressedString.Length)
        {
            return 0;
        }

        if (length == -1)
        {
            return CountDecompressedStringLength(compressedString, startIndex, compressedString.Length - startIndex);
        }

        int nextStartMarkerIndex = compressedString.IndexOf('(', startIndex, length);
        if (nextStartMarkerIndex == -1)
        {
            return length;
        }

        int lengthAfterStartMarker = length - (nextStartMarkerIndex - startIndex);
        int nextDelimiterIndex = compressedString.IndexOf('x', nextStartMarkerIndex, lengthAfterStartMarker);
        int nextStopMarkerIndex = compressedString.IndexOf(')', nextStartMarkerIndex, lengthAfterStartMarker);
        if (nextDelimiterIndex == -1 || nextStopMarkerIndex == -1)
        {
            return length;
        }

        long stringBeforeStartMarkerDecompressedLength = nextStartMarkerIndex - startIndex;

        int repeatLength = Int32.Parse(compressedString.Substring(nextStartMarkerIndex + 1, nextDelimiterIndex - nextStartMarkerIndex - 1));
        int repeatCount = Int32.Parse(compressedString.Substring(nextDelimiterIndex + 1, nextStopMarkerIndex - nextDelimiterIndex - 1));

        int repeatingStringStartIndex = nextStopMarkerIndex + 1;
        long decompressedRepeatingStringLength = CountDecompressedStringLength(compressedString, repeatingStringStartIndex, repeatLength);
        long markerSequenceStringDecompressedLength = decompressedRepeatingStringLength * repeatCount;

        int remainingStringStartIndex = nextStopMarkerIndex + repeatLength + 1;
        int remainingStringLength = length - (remainingStringStartIndex - startIndex);
        long remainingStringDecompressedLength = CountDecompressedStringLength(compressedString, remainingStringStartIndex, remainingStringLength);

        return stringBeforeStartMarkerDecompressedLength + markerSequenceStringDecompressedLength + remainingStringDecompressedLength;
    }

    public static void Main()
    {
        string compressedString = ReadCompressedString();

        long decompressedStringLength = CountDecompressedStringLength(compressedString);

        Console.WriteLine(decompressedStringLength.ToString());
    }
}
