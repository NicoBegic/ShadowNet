using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Can parse different types of string inputs and returns int representations.
/// </summary>
class InputParser
{
    private static Dictionary<int, Regex> DecimalNumberRegexes;

    /// <param name="input">The user's input.</param>
    /// <param name="numberOfFractionalDigits">The maximum number of fractional digits the input number is supposed to have.</param>
    /// <returns>Wether the input has the proper format to be parsed.</returns>
    public static bool isValidInput(string input, int numberOfFractionalDigits)
    {
        return getDecimalRegex(numberOfFractionalDigits).IsMatch(input);
    }

    /// <param name="input">The user's input.</param>
    /// <param name="numberOfFractionalDigits">The maximum number of fractional digits the input number is supposed to have.</param>
    /// <returns>An integer representing the input.</returns>
    /// <require>InputParser.isValidInput(input)</require>
    public static int parseInput(string input, int numberOfFractionalDigits)
    {
        Regex inputRegex = getDecimalRegex(numberOfFractionalDigits);

        Match inputMatch = inputRegex.Match(formatInput(input, numberOfFractionalDigits));
        string[] inputGroups = new string[3];
        inputMatch.Groups.CopyTo(inputGroups, 0);

        return int.Parse(inputGroups[0] + inputGroups[2]);
    }

    private static Regex getDecimalRegex(int numberOfFractionalDigits)
    {
        Regex result;
        if (!DecimalNumberRegexes.ContainsKey(numberOfFractionalDigits))
        {
            DecimalNumberRegexes.Add(numberOfFractionalDigits, new Regex("(?<!.)(\\d*+)(,|\\.)?+(\\d{0," + numberOfFractionalDigits + "}+)(?!.)"));
        }
        DecimalNumberRegexes.TryGetValue(numberOfFractionalDigits, out result);
        return result;
    }

    private static string formatInput(string input, int numberOfFractionalDigits)
    {
        Match inputMatch = getDecimalRegex(numberOfFractionalDigits).Match(input);

        string[] groups = new string[3];
        inputMatch.Groups.CopyTo(groups, 0);

        string inputIntegerDigits = (string)groups.GetValue(0);
        string inputFractionalDigits = (string)groups.GetValue(2);

        string resultIntegerDigits = "0";

        if (!inputIntegerDigits.Equals(""))
        {
            resultIntegerDigits = inputIntegerDigits;
        }

        string resultFractionalDigits = formatFractionalDigits(inputFractionalDigits, numberOfFractionalDigits);

        return resultIntegerDigits + "." + resultFractionalDigits;
    }

    private static string formatFractionalDigits(string inputFractionalDigits, int numberOfFractionalDigits)
    {
        string resultFractionalDigits = inputFractionalDigits;
        int missingDigits = numberOfFractionalDigits - inputFractionalDigits.Count();

        if (missingDigits < 0)
        {
            throw new ArgumentException("Input digits can not be more than desired number of digits!", "inputFractionalDigits, numberOfFractionalDigits");
        }
        else if (missingDigits > 0)
        {
            for (int i = 0; i < missingDigits; i++)
            {
                resultFractionalDigits = resultFractionalDigits + "0";
            }
        }

        return resultFractionalDigits;
    }
}
