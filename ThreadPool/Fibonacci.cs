using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool
{
    public class Fibonacci
    {
        private int index { get; }
        private int FibNumber { get; set; }
        public Fibonacci(int index)
        {
            this.index = index;
        }
        public int Calculator(int index)
        {

            if (index <= 1)
            {
                return index;
            }
            return (Calculator(index - 2) + Calculator(index - 1));



        }
        public void ResultFunc()
        {
            Console.WriteLine(" {0} is working on Fibonacci number {1}", Thread.CurrentThread.Name, this.index);
            FibNumber = Calculator(this.index);
            Thread.Sleep(2500);
            Console.WriteLine(" {0} calculated Fibonacci number {1}, result is {2}", Thread.CurrentThread.Name, this.index, this.FibNumber);

        }
    }
}



