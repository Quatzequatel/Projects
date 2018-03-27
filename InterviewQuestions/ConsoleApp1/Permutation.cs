using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Permutation
    {
        public static void IsPermutationTest()
        {
            string[] s1 = { "apple","bar" };
            string[] s2 = { "elppa","foo" };
            for (int i = 0; i < s1.Length; i++)
            {
                Console.WriteLine(string.Format("isPermutation {0}, of {1} = {2}", s1[i], s2[i], IsPermutation(s1[i], s2[i])));
            }
        }

        public static void IsPalindromeTest()
        {
            string[] s1 = { "aba", "bab", "taco cat" };
            for (int i = 0; i < s1.Length; i++)
            {
                Console.WriteLine(string.Format("IsPalindrome {0}? {1}.", s1[i], IsPalindrome(s1[i])));
            }
        }

        private static bool IsPermutation(string s1, string s2)
        {
            if (s1.Length != s2.Length) return false;

            return s1.ToCharArray().OrderBy(c => c).ToArray()
                .Except(s2.ToCharArray().OrderBy(c => c).ToArray()).Count() == 0;
        }

        private static bool IsPalindrome(string s1)
        {
            char[] s1Char = s1.ToCharArray();
            int tailLength = s1.Length - 1;
            int count = (s1.Length % 2 == 0 ? (s1.Length / 2) : (s1.Length / 2) + 1);

            for (int i = 0; i < count;  i++)
            {
                if(s1Char[i] != s1Char[tailLength - i])
                {
                    return false;
                }
            }

            return true;

        }
    }
}
