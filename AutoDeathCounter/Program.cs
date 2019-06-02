using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace AutoDeathCounter
{
    internal class Program
    {
        internal static bool IsRunnings;

        private static void Main(string[] args)
        {
            Console.WriteLine("Starting..");
            IsRunnings = true;

            if (!IsAdmin())
            {
                Console.WriteLine("Run as admin is required");
                throw new AccessViolationException("Run as admin is required");
            }

            Console.WriteLine("Running main..");

            Task.Run(MainThread.Start);

            Console.WriteLine("Press 'any' to quit");
            Console.ReadKey();

            IsRunnings = false;
        }

        private static bool IsAdmin()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
