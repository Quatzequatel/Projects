using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class BinaryTree<T> where T : IComparable
    {
        public enum State
        {
            Unvisited,
            Visited,
            Visiting,
        }
        public static void testMe()
        {
            int itemsCount = 20;
            List<int> list = new List<int>(itemsCount);
            Random rand = new Random(9001);
            for (int i = 0; i < itemsCount; i++)
            {
                list.Add(rand.Next(0, 1000));
            }
            Console.WriteLine(list.ToArray().Print());
            BinaryTree<int> testTree = new BinaryTree<int>(list);
            testTree.Print();
            testTree.LeafPrint(testTree.root, false, false, '*');
        }
        public Node root;

        public BinaryTree(IList<T> values)
        {
            root = new Node(values[0]);
            for (int i = 1; i < values.Count; i++)
            {
                Node node = new Node(values[i]);
                root.Add(node);

            }
        }

        public Add(Node node)
        {

        }




        public class Node
        {
            public T Value = default;

            public Node Left { get; private set; }
            public Node Right { get; private set; }

            public Node()
            {
            }

            public Node(T value)
            {
                Value = value;
            }
            public void Add(Node node)
            {
                //case -1 less than, store on left
                //case 0 equal (throw error)
                //case 1 greater than, store on right
                switch (this.Value.CompareTo(node.Value))
                {
                    case 0:
                        throw new ArgumentException("no duplicate values.");
                    case 1:
                        if (Left != null)
                        {
                            Left.Add(node);
                        }
                        else
                        {
                            Left = node;
                        }
                        break;
                    case -1:
                        if (Right != null)
                        {
                            Right.Add(node);
                        }
                        else
                        {
                            Right = node;
                        }
                        break;
                    default:
                        break;
                }
            }

            public Boolean HasChildren()
            {
                return (Left != null || Right != null);
            }


        }
        public void Print()
        {
            String output = "";
            Node node = root;
            while (node != null)
            {
                output += node.Value.ToString() + " ";
                node = node.Left;
            }
            Console.WriteLine(output);
        }

        public void LeafPrint(Node node, Boolean goneL, Boolean goneR, Char direction)
        {
            //String output = "";
            if (!(goneL & goneR))
            {
                Console.Write(node.Value.ToString() + direction + " ");
                //output += node.Value.ToString() + " ";
            }
            else if (!node.HasChildren())
            {
                Console.Write(node.Value.ToString() + direction + " ");
                //output += node.Value.ToString() + " ";
            }
            if (node.Left != null)
            {
                LeafPrint(node.Left, true, goneR, 'L');
            }
            if (node.Right != null)
            {
                LeafPrint(node.Right, goneL, true, 'R');
            }
            
        }
    }
}
