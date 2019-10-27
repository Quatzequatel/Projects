using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class foo<T>
    {
        T Data;
        foo<T> Next;
        foo<T> First;

        public foo(foo<T> head)
        {
            if (head == null)
            {
                head = this;
                return;
            }
                


        }
    }
}
