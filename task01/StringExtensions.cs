using System.Linq;

namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            string cleanString = "";
            foreach (char c in input.ToLower())
            {
                if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                {
                    cleanString += c;
                }
            }

            string reversedString = "";
            for (int i = cleanString.Length - 1; i >= 0; i--)
            {
                reversedString += cleanString[i];
            }

            return cleanString == reversedString;
        }
    }
}