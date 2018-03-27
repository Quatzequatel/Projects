using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class LinkListBook
    {
        class Node
        {
            public Node(int data)
            {
                this.data = data;
                head = this;
            }

            private Node head = null;
            public Node Head
            {
                get => head;
                set
                {
                    if (value == null) { throw new ArgumentNullException("Head cannot be null."); }
                    Node n = this.Head;
                    while (n?.Next != null)
                    {
                        n = n.Next;
                        n.head = value;
                    }
                }
            }
            private Node next = null;
            private int data;
            public Node Next { get => next; set => next = value; }
            public int Data { get => data; set => data = value; }


            public Node Clone(Node node)
            {
                if (node == null) return null;

                return new Node(node.data) { Next = node.Next, head = node.Head };
            }


            public void AppendToTail(int data)
            {
                Node end = new Node(data);
                Node n = this.Head;
                while (n?.Next != null)
                {
                    n = n.Next;
                }
                n.Next = end;
            }

            public Node DeleteNode(int data)
            {
                Node n = this.Head;
                if (n.Data == data)
                {
                    n.Head = n.Next;
                    return n;
                }
                while (n?.Next?.Next != null && n?.Next.Data != data)
                {
                    n = n.next;
                }
                if (n?.Next.data == data)
                {
                    Node x = new Node(data);
                    n.next = n?.Next?.Next;
                    return x;
                }
                return null;
            }

            public Node NthToThelast(int n)
            {
                if (head == null) throw new ArgumentOutOfRangeException("head is null.");

                Node p1 = head;
                Node p2 = head;

                //move p1 k nodes into the list.
                for (int i = 0; i < n; i++)
                {
                    if (p1 == null) { throw new ArgumentOutOfRangeException("List is smaller than nth index."); }

                    p1 = p1.next;
                }

                while (p1 != null)
                {
                    p1 = p1.next;
                    p2 = p2.next;
                }

                return p2;
            }

            public static int PrintKthToTheLast(Node head, int k)
            {
                if (head == null) return 0;

                int index = PrintKthToTheLast(head.Next, k);
                if (index == k)
                {
                    Console.WriteLine(String.Format("{0}th to the last node is {1}", k, head.data));
                }

                return ++index;
            }

            public Node KthToTheLast(int k)
            {
                return KthToTheLast(Head, k).Item2;
            }

            private Tuple<int, Node> KthToTheLast(Node node, int k)
            {
                if (node == null) return new Tuple<int, Node>(0, null);
                Tuple<int, Node> result = KthToTheLast(node.next, k);
                int index = result.Item1;
                if (result.Item1 == k)
                {
                    return result.Item2 != null ? result : new Tuple<int, Node>(k, node);
                }

                return new Tuple<int, Node>(++index, null);
            }


            public int Count()
            {
                if (head == null) return 0;

                Node p1 = this.Head;
                int result = 0;

                while (p1 != null)
                {
                    result++;
                    p1 = p1.next;
                }
                return result;
            }
        }
        public static void LinkListUnitTest()
        {
            LinkListBook.Node head = new LinkListBook.Node(1);
            foreach (var n in new int[] { 2, 3, 4, 5, 6, 7 })
            {
                head.AppendToTail(n);
            }
            head.Head = head;


            Console.WriteLine("LinkList Contains:");
            var item = head;
            while (item != null)
            {
                Console.WriteLine(item.Data.ToString());
                item = item.Next;
            }

            Console.WriteLine(string.Format("list contains {0} nodes", head.Count()));

            Console.WriteLine(String.Format("{0}, is in the middle of the list.", head.NthToThelast(4).Data));
            int onlyOnce = 6;
            Node x = head.DeleteNode(onlyOnce);
            Console.WriteLine(string.Format("{0}, has been removed from the list", x != null ? x.Data.ToString() : String.Format("{0} was not found", onlyOnce)));
            x = head.DeleteNode(onlyOnce);
            Console.WriteLine(string.Format("{0}", x != null ? x.Data.ToString()+", has been removed from the list" : String.Format("{0} was not found", onlyOnce)));
            Console.WriteLine(String.Format("{0}, 2nd to last node.", head.NthToThelast(2).Data));

            Node.PrintKthToTheLast(head, 2);
            int k = 1;
            Node kth = head.KthToTheLast(k);
            Console.WriteLine(String.Format("{0}th to the last node is {1}", k, kth.Data));

        }
    }
}
