using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Codility.TestIt();

            //Solution solution = new Solution();
            //int[] A = new int[] { 1,3,6,4,1,2 };
            //int[] sorted = A.OrderBy(x => x).ToArray();
            //Console.WriteLine(String.Format("{0} ; result is {1}", sorted.Print(), ArrayQuestions.solution(A)));

            ////Fibonacci.FibonacciSequenceo(10);
            ////Fibonacci.FibonacciSequenceUpTo(10000);
            //ArrayQuestions.TestIt();
            ////Permutation.IsPermutationTest();
            ////Permutation.IsPalindromeTest();
            ////Lists.Stack<int>.CanPushOnStackTest();
            ////Lists.Queue<int>.QueueUnitTest();
            ////Lists.LinkList<int>.LinkListUnitTest();
            ////LinkListBook.LinkListUnitTest();
            ///StackQuestion
            //StackQuestion.TestIt();
            //LastMinuteStuding.TestStackSort();
            //Sort.TestIt2( Sort.SortType.MergeSort);
            //Sort.TestIt(Sort.SortType.MergeSort);
            Sort.TestIt2(Sort.SortType.QuickSort);
            //Sort.TestIt2(Sort.SortType.MergeSort);
            //Sort.TestIt2(Sort.SortType.InsertionSort);
            //Sort.TestIt2(Sort.SortType.HeapSort);
            //Sort.TestIt(Sort.SortType.HeapSort);
            Sort.TestIt(Sort.SortType.QuickSort);

            //EasyArray1.TestIt();

            Console.ReadKey();
        }

        static void TestOneEditAway()
        {
            string[] test = { "pale", "ple", "pales", "pale", "pale", "bale", "pale", "bake", "pale", "ppale" };

            for (int i = 0; i < test.Length; i += 2)
            {
                Console.WriteLine(String.Format("{0}, {1} -> {2}", test[i], test[i + 1], OneEditAwayV2(test[i], test[i + 1])));
            }

        }

        static int OneEditAwayV2(string s1, string s2)
        {
            if (Math.Abs(s1.Length - s2.Length) > 1) return -1;
            char[] s1CharArray = (s1.Length > s2.Length) ? s1.ToCharArray() : s2.ToCharArray();
            char[] s2CharArray = (s1.Length > s2.Length) ? s2.ToCharArray() : s1.ToCharArray();
            int length = (s1.Length > s2.Length) ? s2.Length : s1.Length;

            bool isSameLength = (s1.Length == s2.Length);


            int edits = 0;

            int s2i = 0;
            int differenceIndex = int.MinValue;

            for (int i = 0; i < length; i++)
            {
                if (s1CharArray[i] != s2CharArray[s2i])
                {
                    differenceIndex = (differenceIndex == int.MinValue && edits < 1) ? s2i : -1;
                    s2i = isSameLength ? s2i : --s2i;
                    if (++edits > 1)
                    {
                        return differenceIndex;
                    }
                }
                s2i++;
            }
            return differenceIndex == int.MinValue ? length + 1 : differenceIndex;
        }

        static bool OneEditAway(string s1, string s2)
        {
            char[] s1CharArray = s1.ToCharArray();
            char[] s2CharArray = s2.ToCharArray();
            int edits = 0;

            if (s1.Length == s2.Length)
            {
                for (int i = 0; i < s1CharArray.Length; i++)
                {
                    if (s1CharArray[i] != s2CharArray[i] && (++edits) > 1)
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (s2.Length - 1 == s1.Length || s2.Length + 1 == s1.Length)
            {
                int j = 0;
                for (int i = 0; i < s2.Length; i++)
                {
                    if (s1CharArray[i] != s2CharArray[j])
                    {
                        j -= 1;
                        if (++edits > 1)
                        {
                            return false;
                        }
                    }
                    j++;
                }
                return true;
            }
            return false;
        }

        static Boolean OneEditAwayold(string input, string check)
        {
            if (input.Length != check.Length && input.Length == check.Length + 1)
            {
                int inputSum = AsciiSum(input.ToUpper());
                int checkSum = AsciiSum(check.ToUpper());
                int difference = inputSum - checkSum;
                //Char letter = (Char)difference;

                if (difference <= (int)'Z')
                {
                    return true;
                }
            }
            else
            {
                Char[] inputChar = input.ToUpper().ToCharArray();
                Char[] checkChar = check.ToUpper().ToCharArray();
                int edits = 0;
                for (int i = 0; i < checkChar.Length; i++)
                {
                    if (inputChar[i] != checkChar[i])
                    {
                        if ((++edits) > 1)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        static int AsciiSum(string input)
        {
            return input.ToCharArray().Sum(a => (int)a);
        }
    }
}
