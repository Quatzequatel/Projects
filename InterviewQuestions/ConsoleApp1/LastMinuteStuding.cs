using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class LastMinuteStuding
    {
        public class Node
        {
            public int Data = default;
            public Node Next = null;
            public Node(int data)
            {
                Data = data;
            }

            public void Append(int data)
            {
                Node last = new Node(data);
                Node n = this;

                while( !this.IsLast())
                {
                    n = n.Next;
                }

                n.Next = last;
            }

            public bool IsLast()
            {
                return this.Next == null;
            }

            public void RemoveDuplicateNodes(Node head, int data)
            {
                Node n = head;
                if (n.Data == data) throw new Exception("head node needs to be previous node.");
                // need first case to be valid
                if (n.Next == null) return;
                //iterate over each node.
                while(n.Next.Next != null)
                {
                    if(n.Next.Data == data) //when duplicate
                    {
                        n.Next = n.Next.Next; //remove node
                    }
                    n = n.Next; //move to next node.
                }
            }

            public Node jumbXNodes(int x, Node start)
            {
                Node n = start;
                for (int i = 0; i < x; i++)
                {
                    if(n.Next == null) { return null; }
                    n = n.Next;
                }
                return n;
            }

            public int Count(Node start)
            {
                Node n = start;
                int count = 0;
                while(!n.IsLast()) { count++; n = n.Next; }
                return count;
            }
        } //class

        public static void TestStackSort()
        {
            Stack<int> unsorted = new Stack<int>(new int[]{5,3,4,1,2});
            //Console.WriteLine(String.Format("{0} ; result is {1}", unsorted.ToArray().PrintLines(), () => stackSort(unsorted);  unsorted.ToArray().PrintLines())); ;
            stackSort(unsorted);
        }

        static void stackSort(Stack<int> unsorted)
        {
            Stack<int> tmp = new Stack<int>();
            while(unsorted.Count() > 0)
            {
                int item = unsorted.Pop();
                while(tmp.Count()>0 && tmp.Peek() < item)
                {
                    unsorted.Push(tmp.Pop());
                }
                tmp.Push(item);
            }

            while(tmp.Count() > 0)
            {
                unsorted.Push(tmp.Pop());
            }
        }

    }
}
