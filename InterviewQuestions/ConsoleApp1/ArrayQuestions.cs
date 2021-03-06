﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp1
{
    public class ArrayQuestions
    {
        public ArrayQuestions()
        {

        }
        public static void TestIt()
        {
            TestItSudoku2();
            int[] numbers = new int[] { 2, 3, 1, 0, 2, 5, 3 };
            ArrayQuestions aq = new ArrayQuestions();
            Console.WriteLine(String.Format("has duplicates({1}) : {0}", aq.HasDuplicates3(numbers), numbers.Print<int>()));
        }
        /*
         This has duplicate question is lame. 
         yes there is a forumulic way to determine for very narrow set of circumstances.
         given those circumstances there is a faster way to apply the constraints
         and return the value. Book version is  HasDuplicates1(), IMHO HasDuplicates3()
         is better.

             */
        public int HasDuplicate1(int[] numbers)
        {
            int length = numbers.Length;
            int foo = numbers.Min();
            int sum1 = 0;
            for (int i = 0; i < length; i++)
            {
                if (numbers[i] < 0 || numbers[i] > length - 2)
                {
                    throw new ArgumentOutOfRangeException("Invalid numbers");
                }
                sum1 += numbers[i];
            }
            int sum2 = ((length - 1) * (length - 2)) >> 1;
            return sum1 - sum2;
        }

        public int HasDuplicates2(int[] numbers)
        {
            int length = numbers.Length;
            int firstValue = numbers[0];
            int difference = 1;
            int sum1 = 0;
            int sum2 = 0;
            //using formula
            sum2 = (length >> 1) * (2 * firstValue + (length - 1) * difference);

            for (int i = 0; i < length; i++)
            {
                sum1 += numbers[i];
            }

            return (sum1 - sum2) + difference;
        }

        public int HasDuplicates3(int[] numbers)
        {
            int length = numbers.Length;
            int firstValue = numbers[0];
            int difference = (numbers[numbers.Length - 1] - firstValue) / (length - 1);
            int sum1 = 0;

            for (int i = 0; i < length; i++)
            {
                sum1 += numbers[i];
                if (numbers[i] != firstValue + (i * difference) || i > 0 ? numbers[i] == numbers[i - 1] : false)
                {
                    return numbers[i];
                }

            }

            return -1;
        }

        public static int solution(int[] A)
        {
            int Length = A.Length;
            int low = A.Min();
            int[] sorted = A.OrderBy(x => x).ToArray();
            for (int i = 0; i < Length - 1; i++)
            {
                if (sorted[i] < 0 || sorted[i] == sorted[i + 1] || sorted[i] + 1 == sorted[i + 1]) { }
                else
                {
                    return sorted[i] + 1;
                }
            }
            return sorted[A.Length - 1] < 0 ? 1 : sorted[A.Length - 1] + 1;
        }



        public static void TestItFirstNotRepeatingCharacter()
        {
            if (FirstNotRepeatingCharacter("abacabad") != 'c') throw new Exception("firstNotRepeatingCharacter failed. expected '' for response");
            if (FirstNotRepeatingCharacter("abaabad") != 'd') throw new Exception("firstNotRepeatingCharacter failed. expected '' for response");
            if (FirstNotRepeatingCharacter("abaaba") != '_') throw new Exception("firstNotRepeatingCharacter failed. expected '' for response");

            //RotateImage(new int[][] { new int[] { 1, 2, 3, 0 }, new int[] { 4, 5, 6, 0 }, new int[] { 7, 8, 9, 0 } });
        }
        public static void TestItRotateImage()
        {
            int[][] input = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 }, new int[] { 7, 8, 9 } };
            if (RotateImage(input) == false) throw new Exception("RotateImage failed.");
        }

        public static void TestItSudoku2()
        {
            var grid = new char[][]{
            new char[] {'.', '.', '.', '1', '4', '.', '.', '2', '.' },
            new char[] {'.', '.', '6', '.', '.', '.', '.', '.', '.' },
            new char[] {'.', '.', '.', '.', '.', '.', '.', '.', '.' },
            new char[] {'.', '.', '1', '.', '.', '.', '.', '.', '.'},
            new char[] {'.', '6', '7', '.', '.', '.', '.', '.', '9'},
            new char[] {'.', '.', '.', '.', '.', '.', '8', '1', '.'},
            new char[] {'.', '3', '.', '.', '.', '.', '.', '.', '6' },
            new char[] {'.', '.', '.', '.', '.', '7', '.', '.', '.'},
            new char[] {'.', '.', '.', '5', '.', '.', '.', '7', '.'}};

            if (Sudoku2(grid) == false) throw new Exception("failed.");

            var grid2 = new char[][] {
                new char[] {'.', '.', '.', '.', '2', '.', '.', '9', '.'},
                new char[] {'.', '.', '.', '.', '6', '.', '.', '.', '.'},
                new char[] {'7', '1', '.', '.', '7', '5', '.', '.', '.'},
                new char[] {'.', '7', '.', '.', '.', '.', '.', '.', '.'},
                new char[] {'.', '.', '.', '.', '8', '3', '.', '.', '.'},
                new char[] {'.', '.', '8', '.', '.', '7', '.', '6', '.'},
                new char[] {'.', '.', '.', '.', '.', '2', '.', '.', '.'},
                new char[] {'.', '1', '.', '2', '.', '.', '.', '.', '.'},
                new char[] {'.', '2', '.', '.', '3', '.', '.', '.', '.'}
            };
            if (Sudoku2(grid2) == true) throw new Exception("failed.");
        }

        class TrackerItem
        {
            public int Index { get; set; }
            public int Count { get; set; }
        }

        public static char FirstNotRepeatingCharacter(string s)
        {
            if (String.IsNullOrEmpty(s)) return '_';
            if (s.Length == 1) return s[0];

            Dictionary<char, TrackerItem> tracker = new Dictionary<char, TrackerItem>();

            for (int i = 0; i < s.Length; i++)
            {
                if (tracker.ContainsKey(s[i]))
                {
                    ++tracker[s[i]].Count;
                }
                else
                {
                    tracker.Add(s[i], new TrackerItem() { Index = i, Count = 1 });
                }
            }

            KeyValuePair<char, TrackerItem> result = tracker.Where(i => i.Value.Count == 1).OrderBy(i => i.Value.Index).FirstOrDefault();
            return result.Value != null ? result.Key : '_';
        }

        public static bool RotateImage(int[][] matrix)
        {
            if (matrix.Length == 0 || matrix.Length != matrix[0].Length) return false;

            int n = matrix.Length;

            for (int layer = 0; layer < matrix.Length / 2; layer++)
            {
                int first = layer;
                int last = n - 1 - layer;
                for (int i = first; i < last; i++)
                {
                    int offset = i - first;

                    int top = matrix[first][i];
                    //left -> top
                    matrix[first][i] = matrix[last - offset][first];
                    //bottom -> left
                    matrix[last - offset][first] = matrix[last][last - offset];
                    //right -> bottom
                    matrix[last][last - offset] = matrix[i][last];
                    // top -> right
                    matrix[i][last] = top;
                }
            }
            return true;
        }



        public static bool Sudoku2(char[][] grid)
        {
            //Check rows.
            foreach (char[] row in grid)
            {
                //Debug.WriteLine(row.Print());
                if (IsValid(CharToInt(row)) == false) return false;
            }

            //Check columns.
            for (int c = 0; c < 9; c++)
            {
                char[] column = new char[9];
                for (int r = 0; r < 9; r++)
                {
                    column[r] = grid[r][c];
                }

                if (IsValid(CharToInt(column)) == false) return false;
            }

            //check panels

            char[][] panels = new char[9][];
            for (int p = 0; p < 9; p++)
            {
                panels[p] = new char[9];
            }

            for (int row = 0, panel = 0, rowcolumn = 0; panel < 9; row += 3, panel++)
            {
                if (panel % 3 == 0) { row = 0; rowcolumn = (panel / 3) * 3; }
                //keep brain from melting, being obvious what is being done.
                panels[panel][0] = grid[row][rowcolumn + 0];
                panels[panel][1] = grid[row][rowcolumn + 1];
                panels[panel][2] = grid[row][rowcolumn + 2];

                panels[panel][3] = grid[row + 1][rowcolumn + 0];
                panels[panel][4] = grid[row + 1][rowcolumn + 1];
                panels[panel][5] = grid[row + 1][rowcolumn + 2];

                panels[panel][6] = grid[row + 2][rowcolumn + 0];
                panels[panel][7] = grid[row + 2][rowcolumn + 1];
                panels[panel][8] = grid[row + 2][rowcolumn + 2];

            }


            for (int panel = 0; panel < 9; panel++)
            {
                if (IsValid(CharToInt(panels[panel])) == false) return false;
            }

            return true;
        }

        public static int[] CharToInt(char[] source)
        {
            int[] result = new int[source.Count()];
            for (int i = 0; i < source.Count(); i++)
            {
                if (source[i] != '.' && source[i] != '-')
                {
                    result[i] = int.Parse(source[i].ToString());
                }
            }
            return result;
        }

        public static bool IsValid(int[] values)
        {
            int flag = 0;
            foreach (int value in values)
            {
                if (value != 0)
                {
                    int bit = 1 << value;
                    if ((flag & bit) != 0) return false;
                    flag |= bit;
                }
            }
            return true;
        }

    }

    public static class Extensions
    {
        public static int[] CharToInt(this char[] source)
        {
            int[] result = new int[source.Count()];
            for (int i = 0; i < source.Count(); i++)
            {
                if (source[i] != '.' && source[i] != '-')
                {
                    result[i] = int.Parse(source[i].ToString());
                }
            }
            return result;
        }

        public static string CharToString(this IOrderedEnumerable<char> source)
        {
            StringBuilder sb = new StringBuilder(source.Count());
            foreach (var item in source)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }

        public static bool IsValid(this int[] values)
        {
            int flag = 0;
            foreach (int value in values)
            {
                if (value != 0)
                {
                    int bit = 1 << value;
                    if ((flag & bit) != 0) return false;
                    flag |= bit;
                }
            }
            return true;
        }

        public static string Print<T>(this T[] values)
        {
            return String.Join(", ", values);
        }

        public static string PrintLines<T>(this T[] values)
        {
            return String.Join("\n", values);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }

        public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
  where U : IComparable<U>
        {
            if (source == null) throw new ArgumentNullException("source");
            bool first = true;
            T maxObj = default(T);
            U maxKey = default(U);
            foreach (var item in source)
            {
                if (first)
                {
                    maxObj = item;
                    maxKey = selector(maxObj);
                    first = false;
                }
                else
                {
                    U currentKey = selector(item);
                    if (currentKey.CompareTo(maxKey) > 0)
                    {
                        maxKey = currentKey;
                        maxObj = item;
                    }
                }
            }
            if (first) throw new InvalidOperationException("Sequence is empty.");
            return maxObj;
        }

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            var factorials = Enumerable.Range(0, array.Length + 1)
                .Select(Factorial)
                .ToArray();

            for (var i = 0L; i < factorials[array.Length]; i++)
            {
                var sequence = GenerateSequence(i, array.Length - 1, factorials);

                yield return GeneratePermutation(array, sequence);
            }
        }

        private static IEnumerable<T> GeneratePermutation<T>(T[] array, IReadOnlyList<int> sequence)
        {
            var clone = (T[])array.Clone();

            for (int i = 0; i < clone.Length - 1; i++)
            {
                Swap(ref clone[i], ref clone[i + sequence[i]]);
            }

            return clone;
        }

        private static int[] GenerateSequence(long number, int size, IReadOnlyList<long> factorials)
        {
            var sequence = new int[size];

            for (var j = 0; j < sequence.Length; j++)
            {
                var facto = factorials[sequence.Length - j];

                sequence[j] = (int)(number / facto);
                number = (int)(number % facto);
            }

            return sequence;
        }

        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private static long Factorial(int n)
        {
            long result = n;

            for (int i = 1; i < n; i++)
            {
                result = result * i;
            }

            return result;
        }
    }
}

