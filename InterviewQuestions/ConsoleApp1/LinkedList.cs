using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Lists
{
    public class LinkedList<T>
    {
        public Node<T> Head { get => head; private set => head = value; }
        Node<T> head = null;

        public LinkedList()
        {
        }

        public Node<T> Append(Node<T> end)
        {
            if (IsEmpty())
            {
                head = end;
                return end;
            }

            Node<T> n = head;
            while (n.Next != null)
            {
                n = n.Next;
            }

            n.Next = end;
            return end;
        }

        public LinkedList<T> Append(T[] values)
        {
            foreach (T item in values)
            {
                this.Append(new Node<T>(item));
            }

            return this;
        }

        public int Count()
        {
            Node<T> n = head;
            int count = IsEmpty() ? 0:1;

            while (n?.Next != null)
            {
                n = n.Next;
                count++;
            }
            return count;
        }

        //public Node<T> Pop()
        //{
        //    if (IsEmpty()) throw new IndexOutOfRangeException("List is empty."); //or throw exception
        //    Node<T> result = null;
        //    Node<T> n = head;
        //    if (n.Next == null)
        //    {
        //        result = n;
        //        head = null;
        //        return result;
        //    }

        //    while (n.Next.Next != null)
        //    {
        //        n = n.Next;
        //    }

        //    result = n.Next;
        //    n.Next = null;
        //    return result;
        //}

        public T Pop()
        {
            if (IsEmpty()) throw new IndexOutOfRangeException("List is empty.");
            Node<T> n = head;
            if(n.Next == null)
            {
                head = null;
                return n.Data;
            }

            head = n.Next;
            return n.Data;
        }

        public Node<T> Remove(T data)
        {
            Node<T> n = head;
            Node<T> result = null;

            if (n.Data.Equals(data))
            {
                head = n.Next;
                result = n;
                result.Next = null;
                return result;
            }

            while (n.Next != null)
            {
                if (n.Next.Data.Equals(data))
                {
                    result = n.Next;
                    result.Next = null;
                    n.Next = n.Next.Next;
                    return result;
                }
            }

            //not found
            return result;
        }

        public bool IsEmpty()
        {
            return head == null;
        }

        public static void NodeTest()
        {
            LinkedList<int> testlist = new LinkedList<int>();
            testlist.Append(new int[] { 12, 14, 15, 16, 17, 20 });

            Console.WriteLine(string.Format("testlist.Count {0}", testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));
            Console.WriteLine(string.Format("NodeTest Pop() Before={0}, value poped {1} After={2}", testlist.Count(), testlist.Pop(), testlist.Count()));

        }

    }

    public class Node<T>
    {
        public Node<T> Next = null;
        public T Data;
        public Node(T data)
        {
            this.Data = data;
        }
    }

}

