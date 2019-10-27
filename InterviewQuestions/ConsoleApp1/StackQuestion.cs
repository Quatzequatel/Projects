using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class StackQuestion
    {
        public static void TestIt()
        {
            Stack<int> s1 = new Stack<int> ();
            for (int i = 4; i > 0; i--)
            {
                s1.Push(i);
            }
            Stack<int> s2 = new Stack<int>();
            for (int i = 8; i > 4; i--)
            {
                s2.Push(i);
            }
            Stack<int> s3 = new Stack<int>();
            for (int i = 8; i > 4; i--)
            {
                s3.Push(i);
            }

            Stack<int> result = SortTheseStacks(s1,s2,s3);
            StringBuilder stringBuilder = new StringBuilder();
            int first = 0;
            while(result.Count > 0)
            {
                if(first == 0)
                {
                    stringBuilder.Append(result.Pop().ToString());
                    first++;
                }
                else
                {
                    stringBuilder.Append(", " + result.Pop().ToString());
                }
                
            }

            Console.WriteLine(stringBuilder.ToString());

        }
        public static Stack<int> SortTheseStacks(Stack<int> s1, Stack<int> s2, Stack<int> s3)
        {
            Stack<int>[] stacks = new Stack<int>[] { s1,s2,s3};
            Stack<int> consolidated = new Stack<int>();

            for (int i = 0; i < 3; i++)
            {
                foreach (var item in stacks[i])
                {
                    consolidated.Push(item);
                }
            }
            return SortStack(consolidated);
        }

        public static Stack<int> SortStackLinq(Stack<int> input)
        {
            Stack<int> sorted = new Stack<int>();
            foreach (var item in input.OrderByDescending(a=>a).ToArray())
            {
                sorted.Push(item);
            }
            return sorted;
        }

        public static Stack<int> SortStack(Stack<int> input)
        {
            Stack<int> sorted = new Stack<int>();
            while(input.Count > 0)
            {
                //pop element to test.
                int tmp = input.Pop();

                if(sorted.Count > 0 && sorted.Peek() > tmp)
                {
                    input.Push(sorted.Pop());
                }

                sorted.Push(tmp);
            }
            return sorted;
        }
    }
}
