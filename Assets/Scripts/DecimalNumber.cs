using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// A DecimalNumber represents a certain decimal number. It cannot be negative.
/// DecimalNumber can be selected by giving the number of fractional digits it is supposed to have, as well as an int or a string representing its full value.
/// </summary>
/// <invariant>_NUMBER_OF_FRACTIONAL_DIGITS >= 0</invariant>
/// <invariant>_value >= 0</invariant>
public class DecimalNumber : IComparable<DecimalNumber>
{
    private static IDictionary<KeyValuePair<int, int>, DecimalNumber> _values = new Dictionary<KeyValuePair<int, int>, DecimalNumber>();

    /// <param name="numberOfFractionalDigits">How many fractional digits the decimal number is supposed to have.</param>
    /// <param name="value">The full value of the desired DecimalNumber.
    /// For example, if the desired DecimalNumber should represent 5.01, value would be 501.</param>
    /// <returns>The value as a DecimalNumber with the specified number of fractional digits.</returns>
    /// <require>numberOfFractionalDigits >= 0</require>
    /// <require>value >= 0</require>
    public static DecimalNumber getValue(int numberOfFractionalDigits, int value)
    {
        KeyValuePair<int, int> numberPair = new KeyValuePair<int, int>(numberOfFractionalDigits, value);
        DecimalNumber dictionaryResult;

        if (_values.TryGetValue(numberPair, out dictionaryResult))
        {
            return dictionaryResult;
        }

        DecimalNumber result = new DecimalNumber(numberPair.Key, numberPair.Value);
        _values.Add(numberPair, result);
        return result;
    }

    /// <param name="numberOfFractionalDigits">The maximum of how many fractional digits the decimal number is supposed to have.</param>
    /// <param name="input">A string representation of the desired DecimalNumber.</param>
    /// <returns>A DecimalNumber that matches the input string.</returns>
    /// <require>isValidInput(numberOfFractionalDigits, input)</require>
    public static DecimalNumber getValue(int numberOfFractionalDigits, string input)
    {
        return getValue(numberOfFractionalDigits, InputParser.parseInput(numberOfFractionalDigits, input));
    }

    /// <param name="numberOfFractionalDigits">The maximum of how many fractional digits the decimal number is supposed to have.</param>
    /// <param name="input">A string representation of the desired DecimalNumber.</param>
    /// <returns>Wether the input has the proper format.</returns>
    public static bool isValidInput(int numberOfFractionalDigits, string input)
    {
        return InputParser.isValidInput(numberOfFractionalDigits, input);
    }

    private readonly int _NUMBER_OF_FRACTIONAL_DIGITS;

    private int _value;

    /// <param name="summand">The DecimalNumber that is supposed to be added to this one.</param>
    /// <returns>Wether the summand can be added to this DecimalNumber.</returns>
    public bool canAdd(DecimalNumber summand)
    {
        if (summand == null)
        {
            return false;
        }

        KeyValuePair<string, string> valueStrings = getSameLengthStrings(_value, summand._value);
        if (InputParser.isNumberTooLargeForInt32(valueStrings.Key) || InputParser.isNumberTooLargeForInt32(valueStrings.Value))
        {
            return false;
        }

        KeyValuePair<int, int> additionValues = getCompatibleValues(this, summand);
        return ((long)additionValues.Key + (long)additionValues.Value) <= int.MaxValue;
    }

    /// <param name="summand">The DecimalNumber that is to be added to this DecimalNumber.</param>
    /// <returns>The sum of this DecimalNumber with the summand.</returns>
    public DecimalNumber plus(DecimalNumber summand)
    {
        KeyValuePair<int, int> additionValues = getCompatibleValues(this, summand);
        int sumValue = additionValues.Key + additionValues.Value;
        int sumNumberOfFractionalDigits = Math.Max(_NUMBER_OF_FRACTIONAL_DIGITS, summand._NUMBER_OF_FRACTIONAL_DIGITS);

        return getValue(sumNumberOfFractionalDigits, sumValue);
    }

    /// <param name="other">The DecimalNumber this one should be compared to.</param>
    /// <returns>A negative number if this DecimalNumber is smaller than other.
    /// 0 if they are equal.
    /// A positive number if it is larger.</returns>
    public int CompareTo(DecimalNumber other)
    {
        KeyValuePair<string, string> valueStrings = getSameLengthStrings(_value, other._value);

        return valueStrings.Key.CompareTo(valueStrings.Value);
    }

    private KeyValuePair<string, string> getSameLengthStrings(int first, int second)
    {
        string firstString = first.ToString();
        string secondString = second.ToString();
        int lengthDifference = firstString.Length - secondString.Length;

        if (lengthDifference < 0)
        {
            for (int i = 0; i < Math.Abs(lengthDifference); i++)
            {
                firstString += "0";
            }
        }
        else if (lengthDifference > 0)
        {
            for (int i = 0; i < Math.Abs(lengthDifference); i++)
            {
                secondString += "0";
            }
        }

        return new KeyValuePair<string, string>(firstString, secondString);
    }

    private KeyValuePair<int, int> getCompatibleValues(DecimalNumber first, DecimalNumber second)
    {
        int fractionalDigitDifference = first._NUMBER_OF_FRACTIONAL_DIGITS - second._NUMBER_OF_FRACTIONAL_DIGITS;

        if (fractionalDigitDifference == 0)
        {
            return new KeyValuePair<int, int>(first._value, second._value);
        }

        if (fractionalDigitDifference < 0)
        {
            int modifiedValue = first._value;

            for (int i = 0; i < Math.Abs(fractionalDigitDifference); i++)
            {
                modifiedValue *= 10;
            }

            return new KeyValuePair<int, int>(modifiedValue, second._value);
        }
        else
        {
            int modifiedValue = second._value;

            for (int i = 0; i < fractionalDigitDifference; i++)
            {
                modifiedValue *= 10;
            }

            return new KeyValuePair<int, int>(first._value, modifiedValue);
        }
    }

    public override bool Equals(Object other)
    {
        DecimalNumber otherDecimalNumber = (DecimalNumber)other;
        if (otherDecimalNumber == null)
        {
            return false;
        }
        return CompareTo(otherDecimalNumber).Equals(0);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override string ToString()
    {
        string result = _value.ToString();
        if (_NUMBER_OF_FRACTIONAL_DIGITS.Equals(0))
        {
            return result;
        }

        if (result.Length <= _NUMBER_OF_FRACTIONAL_DIGITS)
        {
            for (int i = result.Length; i <= _NUMBER_OF_FRACTIONAL_DIGITS; i++)
            {
                result = "0" + result;
            }
        }

        return result.Insert(result.Length - _NUMBER_OF_FRACTIONAL_DIGITS, ",");
    }

    private DecimalNumber(int numberOfFractionalDigits, int value)
    {
        _NUMBER_OF_FRACTIONAL_DIGITS = numberOfFractionalDigits;
        _value = value;
    }

    /// <summary>
    /// Can parse different types of string inputs and returns int representations.
    /// </summary>
    private class InputParser
    {
        private static Dictionary<int, Regex> DecimalNumberRegexes;

        /// <param name="input">The user's input.</param>
        /// <param name="numberOfFractionalDigits">The maximum number of fractional digits the input number is supposed to have.</param>
        /// 
        /// <returns>Wether the input has the proper format to be parsed.</returns>
        public static bool isValidInput(int numberOfFractionalDigits, string input)
        {
            if (input == null || numberOfFractionalDigits < 0 || !getDecimalRegex(numberOfFractionalDigits).IsMatch(input))
            {
                return false;
            }

            input = formatInput(numberOfFractionalDigits, input);
            input = input.Remove(input.IndexOf(","), 1);
            return isNumberTooLargeForInt32(input);
        }

        /// <param name="input">The user's input.</param>
        /// <param name="numberOfFractionalDigits">The maximum number of fractional digits the input number is supposed to have.</param>
        /// 
        /// <returns>An integer representing the input.</returns>
        /// 
        /// <require>InputParser.isValidInput(input)</require>
        public static int parseInput(int numberOfFractionalDigits, string input)
        {
            Regex inputRegex = getDecimalRegex(numberOfFractionalDigits);

            Match inputMatch = inputRegex.Match(formatInput(numberOfFractionalDigits, input));
            string[] inputGroups = new string[3];
            inputMatch.Groups.CopyTo(inputGroups, 0);

            return int.Parse(inputGroups[0] + inputGroups[2]);
        }

        /// <param name="number">Der String, der überprüft werden soll.</param>
        /// 
        /// <returns>Ob der Wert von zahl größer ist, als Integer.MAX_VALUE.</returns>
        /// 
        /// <require>new System.Text.RegularExpressions.Regex(@"(?&lt;!.)\d*(?!.)", RegexOptions.ECMAScript).IsMatch(number)</require>
        public static bool isNumberTooLargeForInt32(string number)
        {
            if (number.Length != 10)
            {
                return number.Length > 10;
            }

            char[] digits = number.ToCharArray();
            char[] integerMaxDigits = Convert.ToString(int.MaxValue)
                .ToCharArray();
            for (int i = 0; i < 10; i++)
            {
                if (digits[i] != integerMaxDigits[i])
                {
                    return digits[i] > integerMaxDigits[i];
                }
            }
            return false;
        }

        private static Regex getDecimalRegex(int numberOfFractionalDigits)
        {
            Regex result;
            if (!DecimalNumberRegexes.ContainsKey(numberOfFractionalDigits))
            {
                DecimalNumberRegexes.Add(numberOfFractionalDigits, new Regex(@"(?<!.)(\d*)(,|\.)?(\d{0," + numberOfFractionalDigits + "})(?!.)", RegexOptions.ECMAScript));
            }
            DecimalNumberRegexes.TryGetValue(numberOfFractionalDigits, out result);
            return result;
        }

        private static string formatInput(int numberOfFractionalDigits, string input)
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

            string resultFractionalDigits = formatFractionalDigits(numberOfFractionalDigits, inputFractionalDigits);

            return resultIntegerDigits + "," + resultFractionalDigits;
        }

        private static string formatFractionalDigits(int numberOfFractionalDigits, string inputFractionalDigits)
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
}
