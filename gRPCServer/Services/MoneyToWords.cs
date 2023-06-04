using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gRPCServer.Services
{
    /// <summary>
    /// A class enacpsulate the functionality to convert numerical representation of money to words.
    /// </summary>
    public class MoneyToWords
    {
        #region attributes
        private string result = string.Empty;
        private string doll = " dollar"; // always there is a space before dollar.
        private string cen = " cent"; // always there is a space before cent.
        private string hundred = " hundred "; // always there is a space before and after.
        private static string thousand = " thousand "; // always there is a space before and after.
        private static string million = " million "; // always there is a space before and after.
        private string and = " and "; // always there is a space before and after.
        private List<string> designations = new() { "", thousand, million }; //can be extended to include billions...
        #endregion
        #region general Functions
        /// <summary>
        /// convert currency text to words.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string runConverter(string text)
        {
            try
            {
                return ConvertMoneyToWords(text);
            }
            catch (Exception error)
            {
                return error.Message;
            }
        }
        /// <summary>
        /// encapsulate the logic of conversion
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ConvertMoneyToWords(string text)
        {
            // check if input can be parsed to a number.
            var strippedText = text.Replace(" ", "");
            var check = double.TryParse(strippedText, out double textNumber);

            if (!check) return "input text is not representing a number.";
            if (text.Contains('.')) return "commas should be used for cents separation.";
  

            var decompose = strippedText.Split(",");

            if (decompose[0].Length > 9) return "maximum dollars numbers reached";

            var dollars = spaceString(decompose[0].Replace(" ", ""));

            //get only designations for the current range of numbers.
            designations = designations.GetRange(0, dollars.Split(" ").Length);

            //handles dollars portion by recursion.
            DollarsToWords(dollars);

            //handling cents after dollars finish recursion
            string? cents = null;
            if (decompose.Length > 1 && decompose[1].Length>2) cents = decompose[1].Substring(0,2).Replace(" ",""); //more than two characters.
            else if (decompose.Length > 1) cents = decompose[1]; // for only one character.

            // if there is no cents
            if (cents == null && !string.IsNullOrEmpty(result)) result = result.Equals("one") ? result + $"{doll}" : result + $"{doll}s";

            // if there are cents
            if (cents != null)
            {
                result = result != null ? result + $"{doll}s" : string.Empty;
                var resultCents = ConvertToWords(cents, true);

                if (resultCents != null && result.Length > 0)
                    result += resultCents.Equals("one") ? $"{and}{resultCents}{cen}" : $"{and}{resultCents}{cen}s";

                if (resultCents != null && result.Length == 0)
                    result += resultCents.Equals("one") ? $"{resultCents}{cen}" : $"{resultCents}{cen}";

            }
            return result;
        }
        /// <summary>
        /// convert the dollars portion.
        /// </summary>
        /// <param name="dollars">text represent the dollars portion</param>
        private void DollarsToWords(string dollars)
        {
            var decomposedDollars = dollars.Split(" ");
            // if we have one space or more.
            if (decomposedDollars.Length > 1)
            {
                foreach (var portion in decomposedDollars)
                {
                    DollarsToWords(portion);
                }
            }

            //if the number is without spaces.
            if (decomposedDollars.Length == 1)
            {
                if(!ConvertToWords(decomposedDollars[0]).Trim().Equals(string.Empty))
                    result += $"{ConvertToWords(decomposedDollars[0]).Trim()}{designations[designations.Count - 1]}";
                designations.RemoveAt(designations.Count - 1);
            }
        }
        private string ConvertToWords(string text, bool after = false)
        {
            var parsedText = text;

            if (text.Equals("0")) return "zero";
            //remove the trailing zeros if in cents
            if (after == true)
            {
                parsedText = int.Parse(text).ToString(); //parse it to remove leading zeros.
            }

            //handle 01 or 1 cent example.
            if (text.Length == 1 && after == true)
            {
                return TensToWord(parsedText);
            }
            if (text.Length == 2 && parsedText.Length == 1 && after == true)
            {
                return OneNumberToWord(parsedText);
            }
            else if (parsedText.Length == 1 && after == false)
            {
                return OneNumberToWord(parsedText);
            }

            // handle two digits
            if (parsedText.Length == 2)
            {
                if (parsedText[0].Equals('0') && after == true) return $"{and}{OneNumberToWord(parsedText[1].ToString())}";


                if (20 > int.Parse(parsedText) && int.Parse(parsedText) > 10) return $"{TeensToWord(parsedText[1].ToString())}";

                var secondNum = OneNumberToWord(parsedText[1].ToString()) != null ? $"-{OneNumberToWord(parsedText[1].ToString())}" : string.Empty;
                var combined = $"{TensToWord(parsedText[0].ToString())}{secondNum}";
                if (!combined.Equals(""))
                    combined = combined[0].Equals('-') ? combined.Substring(1) : combined;
                return combined.Trim();
            }

            //handle 3 digits
            if (parsedText.Length == 3)
            {
                if (parsedText[0].Equals('0'))
                {
                    return $"{ConvertToWords(parsedText.Substring(1, 2).ToString()).Trim()}";
                }

                return $"{OneNumberToWord(parsedText[0].ToString()).Trim()}{hundred}{ConvertToWords(parsedText.Substring(1, 2)).Trim()}";
            }
            return null;
        }
        /// <summary>
        /// introduce spaces whenever not provided.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string spaceString(string input)
        {
            string newInput = string.Empty;

            int count = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                newInput = input[i] + newInput;
                count++;
                // for each third place.
                if (count % 3 == 0 && i > 0)
                {
                    newInput = " " + newInput;
                }
            }
            return newInput;
        }
        #endregion
        #region charachters to word
        /// <summary>
        /// Convert text that represent values between 1 and 9 as well as first word of 3-values text-number
        /// </summary>
        /// <param name="text">first character of 3 characters numbers or 1 character numbers</param>
        /// <returns></returns>
        private string OneNumberToWord(string text)
        {
            switch (text)
            {
                case "1":
                    return "one";
                case "2":
                    return "two";
                case "3":
                    return "three";
                case "4":
                    return "four";
                case "5":
                    return "five";
                case "6":
                    return "six";
                case "7":
                    return "seven";
                case "8":
                    return "eight";
                case "9":
                    return "nine";
                default:
                    return null;
            }
        }
        /// <summary>
        /// Convert text that represent a value between 11 and 19 included.
        /// </summary>
        /// <param name="text">first character of two characters numbers</param>
        /// <returns></returns>
        private string TeensToWord(string text)
        {
            switch (text)
            {
                case "1":
                    return "eleven";
                case "2":
                    return "twelve";
                case "3":
                    return "thirteen";
                case "4":
                    return "fourteen";
                case "5":
                    return "fifteen";
                case "6":
                    return "sixteen";
                case "7":
                    return "seventeen";
                case "8":
                    return "eighteen";
                case "9":
                    return "ninteen";
                default:
                    return null;
            }
        }
        /// <summary>
        /// Convert discrete text that represent {10,20...90}
        /// </summary>
        /// <param name="text">first character of two characters numbers</param>
        /// <returns></returns>
        private string TensToWord(string text)
        {
            switch (text)
            {
                case "1":
                    return "ten";
                case "2":
                    return "twenty";
                case "3":
                    return "thirty";
                case "4":
                    return "forty";
                case "5":
                    return "fifty";
                case "6":
                    return "sixty";
                case "7":
                    return "seventy";
                case "8":
                    return "eighty";
                case "9":
                    return "ninety";
                default:
                    return null;
            }
        }
        #endregion
    }
}
