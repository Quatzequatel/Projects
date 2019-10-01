using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Lists
{
    public class Stack<T>
    {
        /*
         Implement a stack using a linked list.
         Stack is LIFO
         Pop() returns value of head
         Push(item) adds value to stack
         IsEmpty() returns true when no items in the stack.
         Size() returns number of items in the stack
         */

        private Node<T> _head;
        public Node<T> Head { get => _head; private set => _head = value; }
        public bool IsEmpty() => Head == null;

        public int Size() => Nodes().Count();

        public void Push(T item) => Head = IsEmpty() ? new Node<T>(item) : new Node<T>(item) { Next = Head };

        public T Peek() => IsEmpty() ? default(T) : Head.Data;

        public T Pop()
        {
            if (IsEmpty()) throw new IndexOutOfRangeException("Stack is empty.");
            T result = Head.Data;
            Head = Head.Next;
            return result;
        }

        public IEnumerable<Node<T>> Nodes()
        {
            Node<T> node = Head;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }

        public static void CanPushOnStackTest()
        {
            Stack<int> list = new Stack<int>();
            foreach (var item in new int[] { 1, 2, 3, 4, 5 })
            {
                list.Push(item);
            }

            Console.WriteLine("Stack contains the following items");
            list.Nodes().ForEach(x => Console.WriteLine(x.Data.ToString()));

            list.Nodes().ForEach(x => Console.WriteLine(String.Format("NodeTest Pop() Size before={0}, value poped {1} Size after={2}", list.Size(), list.Pop(), list.Size())));


        }
    }

    public class Queue<T>
    {
        /*
         * implement a queue using a link list
         * Queue is FIFO and has the following methods
         * Add(item)
         * Remove()
         * Peek()
         * isEmpty()
         * Size() returns number of items in the 
         */

        Node<T> _head;
        public Node<T> Head { get => _head; private set => _head = value; }

        public bool IsEmpty { get => _head == null; }

        public int Size { get => Nodes().Count(); }

        public void Add(T item)
        {
            if (IsEmpty)
                Head = new Node<T>(item);
            else
                Nodes().Last().Next = new Node<T>(item);
        }

        public T Peek() => Nodes().Last().Data;

        public T Remove()
        {
            Node<T> node = Nodes().FirstOrDefault(x => x?.Next?.Next == null);
            //no nodes.
            if (node == null)
            {
                return default(T);
            }
            //node is head.
            T result;
            if (node.Next == null)
            {
                result = node.Data;
                Head = null;
                return result;
            }
            //all other cases.
            result = node.Next.Data;
            node.Next = null;
            return result;
        }

        private IEnumerable<Node<T>> Nodes()
        {
            Node<T> node = Head;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }



        public static void QueueUnitTest()
        {
            Queue<int> list = new Queue<int>();
            foreach (var item in new int[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                list.Add(item);
            }

            Console.WriteLine("Queue Contains:");
            list.Nodes().ForEach(item => Console.WriteLine(item.Data.ToString()));

            Console.WriteLine("Test Remove() on Queue:");
            while (!list.IsEmpty)
            {
                Console.WriteLine(String.Format("Queue Count = {0} Removed = {1}, Now Count is {2}", list.Size, list.Remove(), list.Size));
            }
        }
    }

    public class LinkList<T>
    {
        /*
         * A singly linked list is a simple set of nodes 
         * that a single pointer to another node and store a value
         * methods are
         * AddFirst(item) & AddLast(item) adds new node to begining and end of list respectivly.
         * AddAfter() inserts new node between 2 existing nodes
         * RemoveFirst() & RemoveLast() removes first and last node of list respectivly.
         * implement a First and Last property.
         * 
         */
        private Node<T> _first;
        public Node<T> First { get => _first; private set => _first = value; }
        public bool IsEmpty { get => First == null; }

        public Node<T> Last
        {
            get => Nodes().SingleOrDefault(x => x.Next == null) ?? First;
            private set
            {
                if (IsEmpty) { First = value; }
                else
                {
                    Nodes().SingleOrDefault(x => x.Next == null).Next = value;
                }
            }
        }
        private Node<T> NextToLast { get => Nodes().FirstOrDefault(n => n?.Next?.Next == null) ?? First; }

        public int Size { get => Nodes().Count(); }

        public void AddFirst(T item) => First = IsEmpty ? new Node<T>(item) : new Node<T>(item) { Next = First };

        public void AddLast(T item) => Last = new Node<T>(item);

        public T RemoveFirst()
        {
            if (IsEmpty) throw new IndexOutOfRangeException("list is empty.");
            T result = First.Data;
            First = First.Next;
            return result;
        }

        public T RemoveLast()
        {
            if (IsEmpty) throw new IndexOutOfRangeException("list is empty.");
            T result = Last.Data;
            if (Object.ReferenceEquals(First, Last))
            {
                First = null;
            }
            else
            {
                NextToLast.Next = null;
            }
            return result;
        }

        private IEnumerable<Node<T>> Nodes()
        {
            Node<T> node = First;
            while (node != null)
            {
                yield return node;
                node = node.Next;
            }
        }

        public static void LinkListUnitTest()
        {
            LinkList<int> list = new LinkList<int>();
            foreach (var item in new int[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                list.AddLast(item);
            }

            foreach (var item in new int[] { 1, 2, 3, 4, 5, 6, 7 })
            {
                list.AddFirst(item);
            }

            Console.WriteLine("LinkList Contains:");
            list.Nodes().ForEach(item => Console.WriteLine(item.Data.ToString()));

            while (!list.IsEmpty)
            {
                Console.WriteLine(String.Format("List Count = {0} RemovedLast = {1}, Now Count is {2}", list.Size, list.RemoveLast(), list.Size));
            }
        }
    }
}
