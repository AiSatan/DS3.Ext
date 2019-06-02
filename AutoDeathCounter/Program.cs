using System;
using System.Threading.Tasks;

namespace AutoDeathCounter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(MainThread.Start);

            Console.WriteLine("Press 'any' to quit");
            Console.ReadKey();
        }
    }
}
