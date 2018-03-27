using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ConsoleApp1
{
    public class ArrayQuestions
    {
        public static void TestIt()
        {
            TestItSudoku2();
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
                Debug.WriteLine(row.Print());
                if (row.CharToInt().IsValid() == false) return false;
            }

            //Check columns.
            for (int c = 0; c < 9; c++)
            {
                char[] column = new char[9];
                for (int r = 0; r < 9; r++)
                {
                    column[r] = grid[r][c];
                }

                if (column.CharToInt().IsValid() == false) return false;
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
                if (panels[panel].CharToInt().IsValid() == false) return false;
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
        public static string Print(this char[] values)
        {
            return String.Join(", ", values);
        }
    }
}
