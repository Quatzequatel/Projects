using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Fibonacci
    {
        public static void TestIt()
        {
            FibonacciSequenceUpTo(1);
        }

        public static void FibonacciSequenceUpTo(int lastNumber)
        {
            List<int> sequence = new List<int>();
            for (int f1 = 1, f2 = 0, fvalue = 0; fvalue < lastNumber; fvalue = f1 + f2, f1 = f2, f2 = fvalue )
            {
                sequence.Add(fvalue);
            }

            Console.WriteLine(string.Join(", ", sequence.ToArray()));
        }

        public static void FibonacciSequenceo(int count)
        {
            List<int> sequence = new List<int>();
            /* this is to explain the actual for loop
             int f1 = 1;
             int f2 = 0;
             int fvalue = 0
             for(int i=0; i<=count-1; i++)
             {
                sequence.Add(fvalue);
                fvalue = f1 + f2;
                f1 = f2;
                f2 = fvalue;
             }
             */
            for (int i=0, f1 = 1, f2 = 0, fvalue = 0; i <= count-1; i++, fvalue = f1 + f2, f1 = f2, f2 = fvalue)
            {
                sequence.Add(fvalue);
            }

            Console.WriteLine(string.Join(", ", sequence.ToArray()));
        }
    }
}
