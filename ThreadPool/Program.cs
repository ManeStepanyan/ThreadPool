using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            Fibonacci[] numbers = new Fibonacci[10];
            using (var pool = new NewThreadPool(7))
            {
                for (int i = 0; i < 10; i++)
                {
                    int temp = i;

                    numbers[temp] = new Fibonacci(temp + 25);
                    try { pool.AddTask(() => numbers[temp].ResultFunc()); }
                    catch { Console.WriteLine("Disposed"); }
                }
            }
        }
    }
}







