using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Codility
    {
        public static void TestIt()
        {
            //binaryGapSolution(561892);

            //Console.WriteLine( solution(2, "1A 3C 2B 40G 5A"));

            //Console.WriteLine(OddOccurrencessInArray(new int[] { 9, 3, 9, 3, 9, 7, 9, 7, 8, 11, 0 }));  3, 8, 9, 7, 6

            int[] A = new int[] { 1, 2, 3, 4 };
            //int rotations = 4;
            //Console.WriteLine(string.Format("input {1}, Rotations {2}, CyclicRotation {0}", CyclicRotation(A, rotations).Print(), A.Print(), rotations.ToString()));
            //A = new int[] { 3, 8, 9, 7, 6 };
            //rotations = 3;
            //Console.WriteLine(string.Format("input {1}, Rotations {2}, CyclicRotation {0}", CyclicRotation(A, rotations).Print(), A.Print(), rotations.ToString()));
            //A = new int[] { 0, 0, 0 };
            //rotations = 1;
            //Console.WriteLine(string.Format("input {1}, Rotations {2}, CyclicRotation {0}", CyclicRotation(A, rotations).Print(), A.Print(), rotations.ToString()));

            //A = new int[] { 1, 3, 6, 4, 1, 2 };
            //Console.WriteLine(string.Format("input {1}, MissingInteger {0}", MissingInteger(A).ToString(), A.Print()));
            //A = new int[] {1,2,3};
            //Console.WriteLine(string.Format("input {1}, MissingInteger {0}", MissingInteger(A).ToString(), A.Print()));
            A = new int[] { 9, 3, 9, 3, 9, 7, 9, 7, 8, 11, 0 };
            Console.WriteLine(string.Format("input {1}, MissingInteger {0}", Distinct(A).ToString(), A.Print()));
        }
        /*
         * given a positive integer N, returns the length of its longest binary gap. 
         * The function should return 0 if N doesn't contain a binary gap
         * */
        public static int binaryGapSolution(int N)
        {
            string binary = Convert.ToString(N, 2);
            Console.WriteLine("Binary: " + binary);
            int gap = 0;
            int gapCount = 0;
            for (int i = 0; i < binary.Length; i++)
            {
                if (binary[i] == '0')
                {
                    gapCount += 1;
                }
                else if (binary[i] == '1')
                {
                    if (gapCount > gap)
                    {
                        gap = gapCount;
                    }
                    gapCount = 0;
                }
            }
            Console.WriteLine("gap value: " + gap.ToString());
            return gap;
        }

        public static int solution(int N, string S)
        {
            // write your code in C# 6.0 with .NET 4.5 (Mono)

            //intialize plane
            Row[] rows = new Row[51];
            for (int row = 1; row < 51; row++)
            {
                rows[row] = new Row(row);
                rows[row].AssignSeat(String.Format("{0}{1}", row, "A"));
            }

            //assign seats
            string[] seats = S.Split(' ');
            foreach (var seat in seats)
            {
                string row = seat.Substring(0, seat.Length - 1);
                int rowId = Convert.ToInt32(row);
                rows[rowId].AssignSeat(seat);
            }

            int availableRows = 0;
            for (int row = 1; row <= N; row++)
            {
                if (rows[row].ConsecutiveSeatsAvailable() > 4)
                {
                    availableRows += 1;
                }
            }

            return availableRows;
        }

        public static int MissingInteger(int[] A)
        {
            int[] B = A.OrderBy(i => i).ToArray();
            int diff = 0;
            for (int i = 0; i < A.Length - 1; i++)
            {
                if(B[i] > 0)
                {
                    diff = B[i + 1] - B[i];
                    if (diff > 1)
                    {
                        return B[i] + 1;
                    }
                }
            }

            return B.Last() < 0 ? 1 : B.Last() + 1;
        }

        public static int Distinct(int[] A)
        {
            return A.Distinct().Count();
        }

        public static int OddOccurrencessInArray(int[] A)
        {
            if (A.Length % 2 != 1)
            {
                throw new IndexOutOfRangeException("not odd elements");
            }

            if (A.Length > 1000000)
            {
                throw new IndexOutOfRangeException("to many elements");
            }

            Dictionary<int, List<int>> pairs = new Dictionary<int, List<int>>();
            int key = 0;
            for (int index = 0; index < A.Length; index++)
            {
                key = A[index];
                if (!pairs.ContainsKey(key))
                {
                    pairs.Add(key, new List<int>());
                }
                pairs[key].Add(index);
            }

            if (pairs.Where(i => i.Value.Count == 1).Select(i => i).Count() > 1)
            {
                throw new IndexOutOfRangeException("to many odd elements");
            }
            return (int)pairs.Where(i => i.Value.Count == 1).Select(i => i).First().Key;
        }

        public static int[] CyclicRotation(int[] A, int K)
        {
            int[] B = new int[A.Length];
            int length = A.Length;
            for (int i = 0; i < length; i++)
            {
                B[(i + K) % length] = A[i];
            }

            return B;
        }
    }

    public class Row
    {
        public enum Seats
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            J,
            K
        }
        int rowId = 0;
        Dictionary<Enum, bool> seats = new Dictionary<Enum, bool>();

        public int RowId { get => rowId; private set => rowId = value; }

        public Row(int rowId)
        {
            RowId = rowId;
            initSeats();
        }

        void initSeats()
        {
            foreach (var item in Enum.GetValues(typeof(Seats)).Cast<Seats>())
            {
                seats.Add(item, false);
            }
        }

        public void AssignSeat(string seat)
        {
            //string newSeat = seat[seat.Length - 1].ToString();
            Enum newSeat = (Seats)Enum.Parse(typeof(Seats), seat[seat.Length - 1].ToString());
            seats[newSeat] = true;
        }

        public int ConsecutiveSeatsAvailable()
        {
            int available = 0;
            int availableCount = 0;
            foreach (var seat in Enum.GetValues(typeof(Seats)).Cast<Seats>())
            {
                if (seats[seat] == false)
                {
                    availableCount += 1;
                }
                else if (seats[seat] == true)
                {
                    if (available < availableCount)
                    {
                        available = availableCount;
                    }
                    availableCount = 0;
                }
            }

            return available < availableCount ? availableCount : available;
        }
    }
}
