using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class EasyArray1
    {
        public static void TestIt()
        {
            string uniqueInput = "the quickbrownfxjmpd";
            Console.WriteLine(string.Format("input was '{0}'; result is '{1}'", uniqueInput, IsUniqueBest(uniqueInput).ToString()));
            uniqueInput += "over the cow";
            Console.WriteLine(string.Format("input was '{0}'; result is '{1}'", uniqueInput, IsUniqueBest(uniqueInput).ToString()));

            //string permutationInput1 = "abcdeedcba";
            //string permutationInput2 = "edcba";
            //Console.WriteLine(string.Format("input was '{0}, {1}'; result is '{2}'", permutationInput1, permutationInput2, IsPermutation0(permutationInput1, permutationInput2).ToString()));

            //permutationInput1 = "sumit ";
            //permutationInput2 = "mtisu ";
            //Console.WriteLine(string.Format("input was '{0}, {1}'; result is '{2}'", permutationInput1, permutationInput2, IsPermutation0(permutationInput1, permutationInput2).ToString()));

            //permutationInput1 = "xyzabb ";
            //permutationInput2 = "xbbayz ";
            //Console.WriteLine(string.Format("input was '{0}, {1}'; result is '{2}'", permutationInput1, permutationInput2, IsPermutation0(permutationInput1, permutationInput2).ToString()));

            string OneEditAwayInput1 = "pale";
            string OneEditAwayInput2 = "ale";
            Console.WriteLine(string.Format("input was '{0}, {1}'; is OneEditAway?  '{2}'", OneEditAwayInput1, OneEditAwayInput2, OneEditAway0(OneEditAwayInput1, OneEditAwayInput2).ToString()));

            OneEditAwayInput1 = "pale";
            OneEditAwayInput2 = "bake";
            Console.WriteLine(string.Format("input was '{0}, {1}'; is OneEditAway?  '{2}'", OneEditAwayInput1, OneEditAwayInput2, OneEditAway0(OneEditAwayInput1, OneEditAwayInput2).ToString()));

            OneEditAwayInput1 = "pale";
            OneEditAwayInput2 = "pales";
            Console.WriteLine(string.Format("input was '{0}, {1}'; is OneEditAway?  '{2}'", OneEditAwayInput1, OneEditAwayInput2, OneEditAway0(OneEditAwayInput1, OneEditAwayInput2).ToString()));

            OneEditAwayInput1 = "pale";
            OneEditAwayInput2 = "palss";
            Console.WriteLine(string.Format("input was '{0}, {1}'; is OneEditAway?  '{2}'", OneEditAwayInput1, OneEditAwayInput2, OneEditAway0(OneEditAwayInput1, OneEditAwayInput2).ToString()));

            //string permutationInput = "abc";
            //Console.WriteLine(string.Format("input was '{0}', factorial is {1}; result is: \n{2}\n\n", permutationInput, Factorial(permutationInput.Length), GetPermutations(permutationInput).PrintLines()));
            //Console.WriteLine(string.Format("input was '{0}', factorial is {1}; result is:", permutationInput, Factorial(permutationInput.Length)));
            //foreach (var item in permutationInput.ToCharArray().GetPermutations())
            //{
            //    Console.WriteLine(item.ToArray());
            //}
            ////GetPermutations(permutationInput.ToCharArray(), 0, permutationInput.Length -1);
        }

        #region One edit away
        static Boolean OneEditAway0(string s1, string s2)
        {
            if (Math.Abs(s1.Length - s2.Length) > 1) return false;
            int edits = s1.Length < s2.Length ? 1 : 0; //start with an edit.

            for (int i = 0; i < s1.Length && i < s2.Length; i++)
            {
                if (s1[i] == s2[i])
                {
                }
                else if (edits < 1 && s1[i] != s2[i])
                {
                    edits += 1;
                }
                else if (edits > 0 && s1[i] != s2[i - 1])
                {
                    return false;
                }

            }
            return true;
        }
        #endregion

        #region All permutations

        static void GetPermutations(char[] input, int recusionDepth, int maxDepth)
        {
            if (recusionDepth == maxDepth)
            {
                Console.Write(input);
                Console.Write("\n");
            }
            else
            {
                for (int i = recusionDepth; i <= maxDepth; i++)
                {
                    Swap(ref input[recusionDepth], ref input[i]);
                    GetPermutations(input, recusionDepth + 1, maxDepth);
                }
            }
        }

        static string[] GetPermutations(string input)
        {
            int maxDepth = input.Length;
            List<string> result = new List<string>();
            result.Add(new string(input.ToArray()));

            if (maxDepth < 2) return result.ToArray();

            char[] inputCopy = input.ToArray();
            //long factorial = Factorial(maxDepth);
            var factorials = Enumerable.Range(1, maxDepth).Select(Factorial).ToArray();

            //int last = 2;
            //int digit = 0;

            //result.Add(new string(inputCopy));
            //for (int i = 1; i < factorial; i++)
            //{
            //    //if( i % Factorial())
            //    //for factorial of 2
            //    if (i % 2 == 1)
            //    {
            //        Swap(ref inputCopy[last - 1], ref inputCopy[last]);
            //        result.Add(new string(inputCopy));
            //    }
            //    //for factorial of 3
            //    if (i % 2 == 0)
            //    {
            //        Swap(ref inputCopy[digit], ref inputCopy[last]);
            //        result.Add(new string(inputCopy));
            //    }
            //}

            //return result.OrderBy(a => a);
            return result.ToArray();
        }

        static long Factorial(int n)
        {
            long result = n;
            for (int i = 1; i < n; i++)
            {
                result *= i;
            }
            return result;
        }

        static string[] AllPermutations(string input)
        {
            char[] inputCopy = input.ToArray();
            int last = input.Length - 1;
            List<string> result = new List<string>();

            result.Add(new string(inputCopy));
            //for (int k = 0; k < length; k++)
            //{
            for (int i = 0, j = 1; i < input.Length; i++, j++)
            {
                //swap
                char temp = inputCopy[i];
                inputCopy[i] = inputCopy[j % last];
                inputCopy[j % last] = temp;

                result.Add(new string(inputCopy));
            }
            //}
            //return result.ToArray().OrderBy(a=>a).ToArray();
            return result.ToArray();
        }

        static void Swap(ref char c1, ref char c2)
        {
            if (c1 == c2) { return; }

            char temp = c1;
            c1 = c2;
            c2 = temp;
        }

        #endregion
        #region Check Permutation
        /*
         * must be the same charaters just not the same order.
         */
        static bool IsPermutation0(string s1, string s2)
        {
            if (s1.Length != s2.Length) return false;
            int length = s1.Length;
            int sum1 = 0, sum2 = 0;

            for (int i = 0; i < length; i++)
            {
                sum1 += s1[i].GetHashCode(); //sum check to validate permutation.
            }

            for (int j = 0; j < length; j++)
            {
                sum2 += s2[j].GetHashCode();
            }

            return sum1 == sum2;
        }

        static bool IsPermutation1(string s1, string s2)
        {
            if (s1.Length != s2.Length) return false;
            int length = s1.Length;
            //int i1 = 0; //string 1 accessor 
            //int j2 = 1; //string 2 accessor
            int sum1 = 0, sum2 = 0;
            //int[,] charCounts = new int[s1.Length,2];
            for (int i = 0; i < length; i++)
            {
                //charCounts[i,i1] = i;
                sum1 += i; //sum check to validate permutation.
                for (int j = 0; j < length; j++)
                {
                    if (s1[i] == s2[j])
                    {
                        //charCounts[i, j2] = j;
                        sum2 += j;
                        //break;
                    }
                }
            }
            return sum1 == sum2;
        }

        static Boolean IsPermutation2(string s1, string s2)
        {
            if (s1.Length != s2.Length) return false;
            HashSet<char> hastmap = new HashSet<char>(s1);
            for (int i = 0; i < s2.Length; i++)
            {
                if (!hastmap.Contains(s2[i])) return false;
            }

            return true;
        }

        /*
         * this fails on "abcdeedcba" and "edcba"
         */
        static Boolean IsPermutationWithSort(string s1, string s2)
        {
            string sort1 = s1.Length > s2.Length ? s1.OrderBy(a => a).CharToString() : s2.OrderBy(a => a).CharToString();
            string sort2 = s1.Length > s2.Length ? s2.OrderBy(a => a).CharToString() : s1.OrderBy(a => a).CharToString();

            int match = 0;
            int matchMax = sort2.Length - 1;

            for (int i = 0; i < sort1.Length; i++)
            {
                if (sort1[i] == sort2[match])
                {
                    if (match >= matchMax) break;
                    match++;

                    for (int j = i + 1; j < sort1.Length; j++)
                    {
                        if (sort1[j] != sort2[match])
                        {
                            return false;
                        }
                        else
                        {
                            i = j;
                            match++;
                        }
                    }
                }

            }

            return match >= matchMax;
        }
        #endregion

        #region IsUnique
        /*
         * implement algorith to determine if a string is all unique character.
         */
        static bool IsUniqueBruteForce(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    if (input[i] == input[j]) return false;
                }
            }

            return true;
        }


        /*
         this is better but ugly because of the size of the duplicates array.
             */
        static bool IsUniqueBetter(string input)
        {
            bool[] duplicates = new bool[char.MaxValue + 1];
            for (int i = 0; i < input.Length; i++)
            {
                if (!duplicates[input[i]])
                {
                    duplicates[input[i]] = true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        static bool IsUniqueBest(string input)
        {
            HashSet<char> uniqueChars = new HashSet<char>(input);

            return uniqueChars.Count == input.Length;
        }

    }

    #endregion
}
